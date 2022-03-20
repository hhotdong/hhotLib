using UnityEngine;

namespace hhotLib.Save.Example
{
    public class SavableMonoExample : MonoBehaviour, ISavable
    {
        public string userPlayTime = "";

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
#endif
            {
                Register();
                if (SaveLoadSystem.IsInitialized)
                {
                    OnLoad();
                }
            }
        }

        private void OnDisable()
        {
            if (SaveLoadSystem.IsInitialized)
            {
                OnSave();
            }
            Unregister();
        }

        public void OnLoad()
        {
            if(SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out SaveData.User user))
            {
                Debug.Log($"MONO OnLoad : {userPlayTime = user.USER_PLAY_TIME}");
            }
        }

        public void OnSave()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out SaveData.User user))
            {
                Debug.Log($"MONO OnSave : {user.USER_PLAY_TIME = userPlayTime}");
            }
        }

        public void OnReset()
        {
            userPlayTime = "";
        }

        public void Register() => SaveLoadSystem.Register(this);
        public void Unregister() => SaveLoadSystem.Unregister(this);
    }
}