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

        public virtual void UI_Close() {
            CloseRequest(this);
        }

        protected sealed override void SetProperties(T props) {
            if (props != null) {
                // If the Properties set on the prefab should not be overwritten,
                // copy the default values to the passed in properties
                if (!props.SuppressPrefabProperties) {
                    props.Priority = Properties.Priority;
                }

                Properties = props;
            }
        }
    }
}
