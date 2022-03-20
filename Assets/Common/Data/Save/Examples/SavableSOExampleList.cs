using System.Collections.Generic;
using UnityEngine;

namespace hhotLib.Save.Example
{
    [CreateAssetMenu]
    public class SavableSOExampleList : SavableSO
    {
        public int typeIdx = -1;
        public bool locked = false;
        public int upgradeLevel = -1;

        public override void OnLoad()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out List<SaveData.Animal> animals))
            {
                for (int i = 0; i < animals.Count; i++)
                {
                    if (animals[i].TYPE_IDX == typeIdx)
                    {
                        locked = animals[i].LOCKED;
                        upgradeLevel = animals[i].UPG_LEVEL;
                        Debug.Log($"SO OnLoad(list) : {locked} , {upgradeLevel}");
                        break;
                    }
                }
            }
        }

        public override void OnSave()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out List<SaveData.Animal> animals))
            {
                bool addNewData = true;
                for (int i = 0; i < animals.Count; i++)
                {
                    if (animals[i].TYPE_IDX == typeIdx)
                    {
                        animals[i].LOCKED = locked;
                        animals[i].UPG_LEVEL = upgradeLevel;
                        addNewData = false;
                        Debug.Log($"SO OnSave(list) : {locked} , {upgradeLevel}");
                        break;
                    }
                }

                if (addNewData)
                {
                    var animal = new SaveData.Animal();
                    animal.TYPE_IDX = typeIdx;
                    animal.LOCKED = locked;
                    animal.UPG_LEVEL = upgradeLevel;
                    animals.Add(animal);
                    Debug.Log($"SO OnSave(list) : {locked} , {upgradeLevel}");
                }
            }
        }

        public override void OnReset()
        {
            locked = true;
            upgradeLevel = -1;
        }
    }
}