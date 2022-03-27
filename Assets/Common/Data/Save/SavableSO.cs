using UnityEngine;

namespace hhotLib.Save
{
    public abstract class SavableSO : ScriptableObject, ISavable
    {
        /// <summary>
        /// If SaveLoadSystem is already initialized, SavableSOData calls OnLoad() method by itself.
        /// Otherwise, OnLoad() method is called by SaveLoadSystem after it becomes initialized.
        /// Important is that the data derived from this class must be dependencies of other objects.
        /// In other words, that data "must be referenced" by any objects(e.g. Any Object which is placed on your scenes where the data used).
        /// If not, OnEnable() method will not always be called so that this data cannot be updated.
        /// </summary>
        private void OnEnable()
        {
            Debug.Log($"SO OnEnable : {this.name}", this, DebugTagConstant.Save);
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
#endif
            {
                Register();
                if (SaveLoadSystem.IsInitialized)
                    OnLoad();
            }
        }

        /// <summary>
        /// At least on Unity editor, ScriptableObject's OnDisable() method is called only if
        /// there exist no dependencies to this object when the editor application is about to
        /// quit or load other scene. In other words, OnDisable() method won't be called if this
        /// ScriptableObject instance is being referenced by others even though editor application
        /// goes from play mode to editor mode. Therefore, it's not guaranteed that OnDisable() method
        /// is always called whenever app quits or loads other scenes so that the following functions(Unregister, OnSave)
        /// should be performed in other scripts(e.g. SaveLoadSystem).
        /// </summary>
        private void OnDisable()
        {
            Debug.Log($"SO OnDisable : {this.name}", this, DebugTagConstant.Save);
            if (SaveLoadSystem.IsInitialized)
                OnSave();
            Unregister();
        }

        public void Register() => SaveLoadSystem.Register(this);
        public void Unregister() => SaveLoadSystem.Unregister(this);
        public abstract void OnLoad();
        public abstract void OnSave();
        public abstract void OnReset();
    }
}