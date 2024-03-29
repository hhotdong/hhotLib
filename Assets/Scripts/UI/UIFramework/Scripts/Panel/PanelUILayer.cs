﻿using UnityEngine;
using System.Collections.Generic;

namespace deVoid.UIFramework {
    /// <summary>
    /// This Layer controls Panels.
    /// Panels are Screens that have no history or queuing,
    /// they are simply shown and hidden in the Frame
    /// eg: a HUD, an energy bar, a mini map etc.
    /// </summary>
    public class PanelUILayer : AUILayer<IPanelController> {
        [SerializeField]
        [Tooltip("Settings for the priority para-layers. A Panel registered to this layer will be reparented to a different para-layer object depending on its Priority.")]
        private PanelPriorityLayerList priorityLayers = null;

        private readonly HashSet<IPanelController> stashedScreenContext = new HashSet<IPanelController>();

        public override void ReparentScreen(IUIScreenController controller, Transform screenTransform) {
            var ctl = controller as IPanelController;
            if (ctl != null) {
                ReparentToParaLayer(ctl.Priority, screenTransform);
            }
            else {
                base.ReparentScreen(controller, screenTransform);
            }
        }

        protected override void ProcessScreenRegister(string screenId, IPanelController controller) {
            base.ProcessScreenRegister(screenId, controller);
            controller.InTransitionStarted += OnInAnimationStarted;
            controller.InTransitionFinished += OnInAnimationFinished;
            controller.OutTransitionStarted += OnOutAnimationStarted;
            controller.OutTransitionFinished += OnOutAnimationFinished;
            controller.CloseRequest += OnCloseRequestedByPanel;
        }

        protected override void ProcessScreenUnregister(string screenId, IPanelController controller) {
            base.ProcessScreenUnregister(screenId, controller);
            controller.InTransitionStarted -= OnInAnimationStarted;
            controller.InTransitionFinished -= OnInAnimationFinished;
            controller.OutTransitionStarted -= OnOutAnimationStarted;
            controller.OutTransitionFinished -= OnOutAnimationFinished;
            controller.CloseRequest -= OnCloseRequestedByPanel;
        }

        public override void ShowScreen(IPanelController screen) {
            ShowScreen<IPanelProperties>(screen, null);
        }

        public override void ShowScreen<TProps>(IPanelController screen, TProps properties) {
            if (CanShowScreen(screen)) {
                screen.Show(properties);
            } else if (screen.CanReopenWhileVisible) {
                if (screen.IsVisible) {
                    if (screen.IsTransitioning) {
                        screen.StopTransition();
                    }
                    screen.Hide(false);
                }
                screen.Show(properties);
            } else {
                Debug.LogWarning($"Cannot show {screen.ScreenId}! IsVisible({screen.IsVisible}), IsTransitioning({screen.IsTransitioning})");
            }
        }

        public override void HideScreen(IPanelController screen) {
            if (CanHideScreen(screen)) {
                screen.Hide();
            }
        }

        public bool IsPanelVisible(string panelId) {
            IPanelController panel;
            if (registeredScreens.TryGetValue(panelId, out panel)) {
                return panel.IsVisible;
            }

            return false;
        }

        public override void StashScreenContext(bool animate)
        {
            foreach (var item in registeredScreens) {
                IPanelController panel = item.Value;
                if (panel.IsVisible) {
                    stashedScreenContext.Add(panel);
                    panel.Hide(animate);
                }
            }
        }

        public override void RestoreScreenContext()
        {
            foreach (var item in stashedScreenContext) {
                item.Show();
            }
            stashedScreenContext.Clear();
        }
        
        private void ReparentToParaLayer(PanelPriority priority, Transform screenTransform) {
            Transform trans;
            if (!priorityLayers.ParaLayerLookup.TryGetValue(priority, out trans)) {
                trans = transform;
            }
            
            screenTransform.SetParent(trans, false);
        }

        private void OnInAnimationStarted(IUIScreenController screen) {
            InvokePendingEvents(screen.ScreenId, VisibleState.IsAppearing);
        }

        private void OnInAnimationFinished(IUIScreenController screen) {
            InvokePendingEvents(screen.ScreenId, VisibleState.IsAppeared);
        }

        private void OnOutAnimationStarted(IUIScreenController screen) {
            InvokePendingEvents(screen.ScreenId, VisibleState.IsDisappearing);
        }

        private void OnOutAnimationFinished(IUIScreenController screen) {
            InvokePendingEvents(screen.ScreenId, VisibleState.IsDisappeared);
        }

        private void OnCloseRequestedByPanel(IUIScreenController screen) {
            HideScreen(screen as IPanelController);
        }
    }
}
