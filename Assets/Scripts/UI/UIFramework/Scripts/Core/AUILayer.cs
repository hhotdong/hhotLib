﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace deVoid.UIFramework {
    public class ScreenTransitionEvent {
        public readonly VisibleState visibleState;
        public readonly Action callback;

        public ScreenTransitionEvent(VisibleState visibleState, Action callback) {
            this.visibleState = visibleState;
            this.callback = callback;
        }
    }

    /// <summary>
    /// Base class for UI Layers. Layers implement custom logic
    /// for Screen types when opening, closing etc.
    /// </summary>
    public abstract class AUILayer<TScreen> : MonoBehaviour where TScreen : IUIScreenController {
        protected Dictionary<string, TScreen> registeredScreens;

        protected readonly Dictionary<string, List<ScreenTransitionEvent>> pendingTransitionEvents = new();

        /// <summary>
        /// Shows a screen
        /// </summary>
        /// <param name="screen">The ScreenController to show</param>
        public abstract void ShowScreen(TScreen screen);

        /// <summary>
        /// Shows a screen passing in properties
        /// </summary>
        /// <param name="screen">The ScreenController to show</param>
        /// <param name="properties">The data payload</param>
        /// <typeparam name="TProps">The type of the data payload</typeparam>
        public abstract void ShowScreen<TProps>(TScreen screen, TProps properties) where TProps : IScreenProperties;

        /// <summary>
        /// Hides a screen
        /// </summary>
        /// <param name="screen">The ScreenController to be hidden</param>
        public abstract void HideScreen(TScreen screen);

        /// <summary>
        /// Save visible screens as context.
        /// </summary>
        /// <param name="animate">Should the screen animate while hiding?</param>
        public abstract void StashScreenContext(bool animate);

        /// <summary>
        /// Restore screen context.
        /// </summary>
        public abstract void RestoreScreenContext();

        /// <summary>
        /// Initialize this layer
        /// </summary>
        public virtual void Initialize() {
            registeredScreens = new Dictionary<string, TScreen>();
        }

        /// <summary>
        /// Reparents the screen to this Layer's transform
        /// </summary>
        /// <param name="controller">The screen controller</param>
        /// <param name="screenTransform">The Screen Transform</param>
        public virtual void ReparentScreen(IUIScreenController controller, Transform screenTransform) {
            screenTransform.SetParent(transform, false);
        }

        /// <summary>
        /// Register a ScreenController to a specific ScreenId
        /// </summary>
        /// <param name="screenId">Target ScreenId</param>
        /// <param name="controller">Screen Controller to be registered</param>
        public void RegisterScreen(string screenId, TScreen controller) {
            if (!registeredScreens.ContainsKey(screenId)) {
                ProcessScreenRegister(screenId, controller);
            }
            else {
                Debug.LogError("[AUILayerController] Screen controller already registered for id: " + screenId);
            }
        }

        /// <summary>
        /// Unregisters a given controller from a ScreenId
        /// </summary>
        /// <param name="screenId">The ScreenId</param>
        /// <param name="controller">The controller to be unregistered</param>
        public void UnregisterScreen(string screenId, TScreen controller) {
            if (registeredScreens.ContainsKey(screenId)) {
                ProcessScreenUnregister(screenId, controller);
            }
            else {
                Debug.LogError("[AUILayerController] Screen controller not registered for id: " + screenId);
            }
        }

        /// <summary>
        /// Attempts to find a registered screen that matches the id
        /// and shows it.
        /// </summary>
        /// <param name="screenId">The desired ScreenId</param>
        public void ShowScreenById(string screenId) {
            TScreen ctl;
            if (registeredScreens.TryGetValue(screenId, out ctl)) {
                ShowScreen(ctl);
            }
            else {
                Debug.LogError("[AUILayerController] Screen ID " + screenId + " not registered to this layer!");
            }
        }

        /// <summary>
        /// Attempts to find a registered screen that matches the id
        /// and shows it, passing a data payload.
        /// </summary>
        /// <param name="screenId">The Screen Id (by default, it's the name of the Prefab)</param>
        /// <param name="properties">The data payload for this screen to use</param>
        /// <typeparam name="TProps">The type of the Properties class this screen uses</typeparam>
        public void ShowScreenById<TProps>(string screenId, TProps properties) where TProps : IScreenProperties {
            TScreen ctl;
            if (registeredScreens.TryGetValue(screenId, out ctl)) {
                ShowScreen(ctl, properties);
            }
            else {
                Debug.LogError("[AUILayerController] Screen ID " + screenId + " not registered!");
            }
        }

        /// <summary>
        /// Attempts to find a registered screen that matches the id
        /// and hides it
        /// </summary>
        /// <param name="screenId">The id for this screen (by default, it's the name of the Prefab)</param>
        public void HideScreenById(string screenId) {
            TScreen ctl;
            if (registeredScreens.TryGetValue(screenId, out ctl)) {
                HideScreen(ctl);
            }
            else {
                Debug.LogError("[AUILayerController] Could not hide Screen ID " + screenId + " as it is not registered to this layer!");
            }
        }

        /// <summary>
        /// Checks if a screen is registered to this UI Layer
        /// </summary>
        /// <param name="screenId">The Screen Id (by default, it's the name of the Prefab)</param>
        /// <returns>True if screen is registered, false if not</returns>
        public bool IsScreenRegistered(string screenId) {
            return registeredScreens.ContainsKey(screenId);
        }
        
        /// <summary>
        /// Hides all screens registered to this layer
        /// </summary>
        /// <param name="animate">Should the screen animate while hiding?</param>
        public virtual void HideAll(bool animate = true) {
            foreach (var kvp in registeredScreens) {
                TScreen screen = kvp.Value;
                screen.StopTransition();
                screen.Hide(animate);
            }
        }

        /// <summary>
        /// Append screen transition event.
        /// </summary>
        public void AppendScreenTransitionEvent(string screenId, ScreenTransitionEvent transitionEvent) {
            if (pendingTransitionEvents.TryGetValue(screenId, out List<ScreenTransitionEvent> pendingEvents)) {
                pendingEvents.Add(transitionEvent);
            } else {
                Debug.LogError($"ScreenId({screenId}) not registered for pending transition event!");
            }
        }

        /// <summary>
        /// Clear screen transition events.
        /// </summary>
        public void ClearScreenTransitionEvent(string screenId) {
            if (pendingTransitionEvents.TryGetValue(screenId, out List<ScreenTransitionEvent> pendingEvents)) {
                pendingEvents.Clear();
            } else {
                Debug.LogError($"ScreenId({screenId}) not registered for pending transition event!");
            }
        }

        protected virtual void ProcessScreenRegister(string screenId, TScreen controller) {
            controller.ScreenId = screenId;
            registeredScreens.Add(screenId, controller);
            if (pendingTransitionEvents.TryGetValue(screenId, out List<ScreenTransitionEvent> pendingEvents) == false) {
                pendingTransitionEvents.Add(screenId, new List<ScreenTransitionEvent>());
            }
            controller.ScreenDestroyed += OnScreenDestroyed;
        }

        protected virtual void ProcessScreenUnregister(string screenId, TScreen controller) {
            if (pendingTransitionEvents.TryGetValue(screenId, out List<ScreenTransitionEvent> pendingEvents)) {
                pendingEvents.Clear();
                pendingTransitionEvents.Remove(screenId);
            }
            controller.ScreenDestroyed -= OnScreenDestroyed;
            registeredScreens.Remove(screenId);
        }

        protected bool CanShowScreen(TScreen controller) {
            if (controller.IsTransitioning) {
                return false;
            }

            if (controller.IsVisible) {
                return false;
            }
            return true;
        }

        protected bool CanHideScreen(TScreen controller) {
            if (controller.IsTransitioning) {
                Debug.LogWarning($"Cannot hide {controller.ScreenId} because it's transitioning now!");
                return false;
            }

            if (controller.IsVisible == false) {
                Debug.LogWarning($"Cannot hide {controller.ScreenId} because it's already not visible!");
                return false;
            }
            return true;
        }

        protected void InvokePendingEvents(string screenId, VisibleState visibleState) {
            if (pendingTransitionEvents.TryGetValue(screenId, out List<ScreenTransitionEvent> pendingEvents)) {
                for (int i = 0; i < pendingEvents.Count; i++) {
                    if (pendingEvents[i].visibleState != visibleState)
                        continue;
                    pendingEvents[i].callback?.Invoke();
                    pendingEvents.RemoveAt(i--);
                }
            }
        }

        private void OnScreenDestroyed(IUIScreenController screen) {
            if (!string.IsNullOrEmpty(screen.ScreenId)
                && registeredScreens.ContainsKey(screen.ScreenId)) {
                UnregisterScreen(screen.ScreenId, (TScreen) screen);
            }
        }
    }
}
