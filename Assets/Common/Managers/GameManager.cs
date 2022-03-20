using UnityEngine;
using hhotLib.Save;
using hhotLib.Common;

namespace hhotLib
{
    public partial class GameManager : Singleton<GameManager>, ISavable
    {
        [Header("Common Savables"), Space(5)]
        public string userID = "";

        public void OnLoad()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out SaveData.User user))
            {
                userID = user.USER_ID;
            }
        }

        public void OnSave()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out SaveData.User user))
            {
                user.USER_ID = userID;
            }
        }

        public void OnReset()
        {
            userID = "";
        }

        public void Register() => SaveLoadSystem.Register(this);
        public void Unregister() => SaveLoadSystem.Unregister(this);
    }
}