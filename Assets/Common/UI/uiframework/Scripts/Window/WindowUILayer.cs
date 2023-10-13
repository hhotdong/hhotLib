using System;
using System.Collections.Generic;
using UnityEngine;

namespace deVoid.UIFramework
{
    /// <summary>
    /// This layer controls all Windows.
    /// Windows are Screens that follow a history and a queue, and are displayed
    /// one at a time (and may or may not be modals). This also includes pop-ups.
    /// </summary>
    public class WindowUILayer : AUILayer<IWindowController>
    {
        [SerializeField] private WindowParaLayer priorityParaLayer = null;

        public IWindowController CurrentWindow { get; private set; }
        
        private Queue<WindowHistoryEntry> windowQueue;
        private Stack<WindowHistoryEntry> windowHistory;

        public event Action RequestScreenBlock;
        public event Action RequestScreenUnblock;

        private bool IsScreenTransitionInProgress {
            get { return screensTransitioning.Count > 0; }
        }

        private HashSet<IUIScreenController> screensTransitioning;

        public override void Initialize() {
            base.Initialize();
            registeredScreens = new Dictionary<string, IWindowController>();
            windowQueue = new Queue<WindowHistoryEntry>();
            windowHistory = new Stack<WindowHistoryEntry>();
            screensTransitioning = new HashSet<IUIScreenController>();
        }

        protected override void ProcessScreenRegister(string screenId, IWindowController controller) {
            base.ProcessScreenRegister(screenId, controller);
            controller.InTransitionFinished += OnInAnimationFinished;
            controller.OutTransitionFinished += OnOutAnimationFinished;
            controller.CloseRequest += OnCloseRequestedByWindow;
        }

        protected override void ProcessScreenUnregister(string screenId, IWindowController controller) {
            base.ProcessScreenUnregister(screenId, controller);
            controller.InTransitionFinished -= OnInAnimationFinished;
            controller.OutTransitionFinished -= OnOutAnimationFinished;
            controller.CloseRequest -= OnCloseRequestedByWindow;
        }

        public override void ShowScreen(IWindowController screen) {
            ShowScreen<IWindowProperties>(screen, null);
        }

        public override void ShowScreen<TProp>(IWindowController screen, TProp properties) {
            IWindowProperties windowProp = properties as IWindowProperties;

            if (CanShowScreen(screen) == false) {
                return;
            }

            if (ShouldEnqueue(screen, windowProp)) {
                EnqueueWindow(screen, properties);
            }
            else {
                DoShow(screen, windowProp);
            }
        }

        public override void HideScreen(IWindowController screen) {
            if (screen == CurrentWindow) {
                if (CanHideScreen(screen) == false) {
                    return;
                }

                windowHistory.Pop();
                AddTransition(screen);
                screen.Hide();

                CurrentWindow = null;

                if (windowQueue.Count > 0) {
                    ShowNextInQueue();
                }
                else if (windowHistory.Count > 0) {
                    ShowPreviousInHistory();
                }
            }
            else {
                Debug.LogError(
                    string.Format(
                        "[WindowUILayer] Hide requested on WindowId {0} but that's not the currently open one ({1})! Ignoring request.",
                        screen.ScreenId, CurrentWindow != null ? CurrentWindow.ScreenId : "current is null"));
            }
        }

        public override void HideAll(bool animate = true) {
            base.HideAll(animate);
            CurrentWindow = null;
            priorityParaLayer.DarkenBG(false);
            windowHistory.Clear();
            windowQueue.Clear();
        }

        public override void ReparentScreen(IUIScreenController controller, Transform screenTransform) {
            IWindowController window = controller as IWindowController;

            if (window == null) {
                Debug.LogError("[WindowUILayer] Screen " + screenTransform.name + " is not a Window!");
            }
            else {
                if (window.IsPopup) {
                    priorityParaLayer.AddScreen(screenTransform);
                    return;
                }
            }

            base.ReparentScreen(controller, screenTransform);
        }

        public void PopTo(string windowId, bool clearWindowQueue, bool animate)
        {
            if (registeredScreens.TryGetValue(windowId, out IWindowController controller) == false) {
                Debug.LogError("[WindowUILayer] Window ID " + windowId + " not registered to this layer!");
                return;
            }

            if (CurrentWindow == null) {
                Debug.LogWarning("[WindowUILayer] CurrentWindow is null!");
                return;
            }

            if (CurrentWindow.ScreenId == windowId) {
                Debug.LogWarning("[WindowUILayer] Window ID " + windowId + " is same as CurrentWindow!");
                return;
            }

            if (CanHideScreen(CurrentWindow) == false) {
                Debug.LogWarning("[WindowUILayer] Cannot hide CurrentWindow now!");
                return;
            }

            bool windowExistsInStack = false;
            foreach (WindowHistoryEntry entry in windowHistory) {
                if (entry.Screen.ScreenId == windowId) {
                    windowExistsInStack = true;
                    break;
                }
            }

            if (windowExistsInStack == false) {
                Debug.LogWarning("[WindowUILayer] Window ID " + windowId + " doesn't exist in stack!");
                return;
            }

            if (clearWindowQueue)
                windowQueue.Clear();

            windowHistory.Pop();
            AddTransition(CurrentWindow);
            CurrentWindow.Hide(animate);
            CurrentWindow = null;

            WindowHistoryEntry history;
            while (windowHistory.TryPeek(out history) && history.Screen.ScreenId != windowId) {
                IWindowController screen = windowHistory.Pop().Screen;
                if (screen.IsVisible) {
                    screen.StopTransition();
                    screen.Hide(false);
                }
            }

            if (windowHistory.TryPeek(out history)) {
                ShowPreviousInHistory();
            }
            else {
                Debug.LogError("[WindowUILayer] Window ID " + windowId + " was reported to exist but not found in stack!");
            }
        }

        public override void SaveScreenContext(bool animate)
        {
            if (CurrentWindow != null && CurrentWindow.IsVisible) {
                CurrentWindow.Hide(animate);
            }
        }

        public override void RestoreScreenContext()
        {
            if (CurrentWindow != null && CurrentWindow.IsVisible == false
                && windowHistory.TryPeek(out WindowHistoryEntry history) && history.Screen == CurrentWindow) {
                history.Show();
            }
        }

        private void EnqueueWindow<TProp>(IWindowController screen, TProp properties) where TProp : IScreenProperties {
            windowQueue.Enqueue(new WindowHistoryEntry(screen, (IWindowProperties) properties));
        }
        
        private bool ShouldEnqueue(IWindowController controller, IWindowProperties windowProp) {
            if (CurrentWindow == null && windowQueue.Count == 0) {
                return false;
            }

            if (windowProp != null && windowProp.SuppressPrefabProperties) {
                return windowProp.WindowQueuePriority != WindowPriority.ForceForeground;
            }

            if (controller.WindowPriority != WindowPriority.ForceForeground) {
                return true;
            }

            return false;
        }

        private void ShowPreviousInHistory() {
            if (windowHistory.Count > 0) {
                WindowHistoryEntry window = windowHistory.Pop();
                DoShow(window);
            }
        }

        private void ShowNextInQueue() {
            if (windowQueue.Count > 0) {
                WindowHistoryEntry window = windowQueue.Dequeue();
                DoShow(window);
            }
        }

        private void DoShow(IWindowController screen, IWindowProperties properties) {
            DoShow(new WindowHistoryEntry(screen, properties));
        }

        private void DoShow(WindowHistoryEntry windowEntry) {
            if (CurrentWindow == windowEntry.Screen) {
                Debug.LogWarning(
                    string.Format(
                        "[WindowUILayer] The requested WindowId ({0}) is already open! This will add a duplicate to the " +
                        "history and might cause inconsistent behaviour. It is recommended that if you need to open the same" +
                        "screen multiple times (eg: when implementing a warning message pop-up), it closes itself upon the player input" +
                        "that triggers the continuation of the flow."
                        , CurrentWindow.ScreenId));
            }
            else if (CurrentWindow != null
                     && CurrentWindow.HideOnForegroundLost
                     && !windowEntry.Screen.IsPopup) {
                CurrentWindow.Hide();
            }

            windowHistory.Push(windowEntry);
            AddTransition(windowEntry.Screen);

            windowEntry.Show();

            CurrentWindow = windowEntry.Screen;
        }
        
        private void OnInAnimationFinished(IUIScreenController screen) {
            RemoveTransition(screen);
        }

        private void OnOutAnimationFinished(IUIScreenController screen) {
            RemoveTransition(screen);
        }

        private void OnCloseRequestedByWindow(IUIScreenController screen) {
            HideScreen(screen as IWindowController);
        }

        private void AddTransition(IUIScreenController screen) {
            screensTransitioning.Add(screen);
            if (RequestScreenBlock != null) {
                RequestScreenBlock();
            }
        }

        private void RemoveTransition(IUIScreenController screen) {
            screensTransitioning.Remove(screen);
            if (!IsScreenTransitionInProgress) {
                if (RequestScreenUnblock != null) {
                    RequestScreenUnblock();
                }
            }
        }
    }
}
