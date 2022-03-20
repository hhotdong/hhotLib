namespace hhotLib.Save.Example
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu]
    public class SavableSOExampleDictionary : SavableSO
    {
        [SerializeField] private string key = "";

        public int typeIdx = -1;
        public int subTypeIdx = -1;
        public bool complete = false;

        public override void OnLoad()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out Dictionary<string, SaveData.Tutorial> tutorials))
            {
                if(tutorials.TryGetValue(key, out SaveData.Tutorial value))
                {
                    complete = value.COMPLETE;
                    Debug.Log($"SO OnLoad(dictionary) : {key} , {typeIdx} , {subTypeIdx} , {complete}");
                }
            }
        }

        public override void OnSave()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out Dictionary<string, SaveData.Tutorial> tutorials))
            {
                var newData = new SaveData.Tutorial();
                newData.COMPLETE = complete;

                if (tutorials.ContainsKey(key))
                {
                    tutorials[key] = newData;
                    Debug.Log($"SO OnSave(dictionary) : {key} , {typeIdx} , {subTypeIdx} , {complete}");
                }
                else
                {
                    tutorials.Add(key, newData);
                    Debug.Log($"SO OnSave(dictionary) : {key} , {typeIdx} , {subTypeIdx} , {complete}");
                }
            }
        }

        public override void OnReset()
        {
            typeIdx = -1;
            subTypeIdx = -1;
            complete = false;
        }
    }
}