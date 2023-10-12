using UnityEngine;
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

        private readonly HashSet<IPanelController> savedScreenContext = new HashSet<IPanelController>();

        public override void ReparentScreen(IUIScreenController controller, Transform screenTransform) {
            var ctl = controller as IPanelController;
            if (ctl != null) {
                ReparentToParaLayer(ctl.Priority, screenTransform);
            }
            else {
                base.ReparentScreen(controller, screenTransform);
            }
        }

        public override void ShowScreen(IPanelController screen) {
            if (CanShowScreen(screen)) {
                screen.Show();
            }
        }

        public override void ShowScreen<TProps>(IPanelController screen, TProps properties) {
            if (CanShowScreen(screen)) {
                screen.Show(properties);
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

        public override void SaveScreenContext(bool animate)
        {
            foreach (var item in registeredScreens) {
                IPanelController panel = item.Value;
                if (panel.IsVisible) {
                    savedScreenContext.Add(panel);
                    panel.Hide(animate);
                }
            }
        }

        public override void RestoreScreenContext()
        {
            foreach (var item in savedScreenContext) {
                item.Show();
            }
            savedScreenContext.Clear();
        }
        
        private void ReparentToParaLayer(PanelPriority priority, Transform screenTransform) {
            Transform trans;
            if (!priorityLayers.ParaLayerLookup.TryGetValue(priority, out trans)) {
                trans = transform;
            }
            
            screenTransform.SetParent(trans, false);
        }
    }
}
