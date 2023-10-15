using UnityEngine;

namespace deVoid.UIFramework {
    /// <summary>
    /// Properties common to all panels
    /// </summary>
    [System.Serializable] 
    public class PanelProperties : IPanelProperties {
        [SerializeField] 
        [Tooltip("Panels go to different para-layers depending on their priority. You can set up para-layers in the Panel Layer.")]
        private PanelPriority priority;

        [SerializeField] 
        [Tooltip("Can reopen the panel while visible.")]
        private bool canReopenWhileVisible;

        public PanelPriority Priority {
            get { return priority; }
            set { priority = value; }
        }

        public bool CanReopenWhileVisible {
            get { return canReopenWhileVisible; }
            set { canReopenWhileVisible = value; }
        }

        /// <summary>
        /// When properties are passed in the Open() call, should the ones
        /// configured in the viewPrefab be overwritten?
        /// </summary>
        /// <value><c>true</c> if suppress viewPrefab properties; otherwise, <c>false</c>.</value>
        public bool SuppressPrefabProperties { get; set; }

        public PanelProperties()
        {
            priority = PanelPriority.None;
            canReopenWhileVisible = false;
            SuppressPrefabProperties = false;
        }
    }
}
