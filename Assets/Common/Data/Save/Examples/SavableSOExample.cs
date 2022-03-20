using UnityEngine;

namespace hhotLib.Save.Example
{
    [CreateAssetMenu]
    public class SavableSOExample : SavableSO
    {
        public string userId = "";

        public override void OnLoad()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out SaveData.User user))
            {
                Debug.Log($"SO OnLoad : {userId = user.USER_ID}");
            }
        }

        public override void OnSave()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out SaveData.User user))
            {
                Debug.Log($"SO OnSave : {user.USER_ID = userId}");
            }
        }

        public override void OnReset()
        {
            userId = "";
        }
    }
}