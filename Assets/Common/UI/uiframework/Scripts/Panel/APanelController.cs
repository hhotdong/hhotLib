namespace deVoid.UIFramework {
    /// <summary>
    /// Base class for panels that need no special Properties
    /// </summary>
    public abstract class APanelController : APanelController<PanelProperties> { }

    /// <summary>
    /// Base class for Panels
    /// </summary>
    public abstract class APanelController<T> : AUIScreenController<T>, IPanelController where T : IPanelProperties {
        public PanelPriority Priority {
            get {
                if (Properties != null) {
                    return Properties.Priority;
                }
                else {
                    return PanelPriority.None;
                }
            }
        }

        public bool CanReopenWhileVisible {
            get {
                if (Properties != null) {
                    return Properties.CanReopenWhileVisible;
                }
                else {
                    return false;
                }
            }
        }

        public virtual void UI_Close() {
            RequestClosing();
        }

        protected sealed override void SetProperties(T props) {
            if (props != null) {
                // If the Properties set on the prefab should not be overwritten,
                // copy the default values to the passed in properties
                if (!props.SuppressPrefabProperties) {
                    props.Priority = Properties.Priority;
                    props.CanReopenWhileVisible = Properties.CanReopenWhileVisible;
                }

                Properties = props;
            }
        }
    }
}
