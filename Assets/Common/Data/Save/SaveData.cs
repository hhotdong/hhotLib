using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hhotLib.Save
{
    public interface ISaveDataElement { }
    public interface ISaveDataDictionary { }

    // Make sure that all data have initial value with which users start for the first time.
    public sealed class SaveData : ScriptableObject
    {
#if UNITY_EDITOR
        public bool IsDebug = false;
#endif

        #region Data Formats

        [Serializable]
        public class User : ISaveDataElement
        {
            public string USER_ID         = string.Empty;
            public int USER_COUNTRY       = -1;
            public string USER_PLAY_TIME  = string.Empty;
            public string USER_EXIT_TIME  = string.Empty;
        }

        [Serializable]
        public class Flag : ISaveDataElement
        {
            public bool FIRST_LAUNCH;
            public bool STAR_RATING_COMPLETE;
            public bool EVENT_PROGRESS_0;
        }

        [Serializable]
        public class GameResources : ISaveDataElement
        {
            public int RESOURCE_GOLD;
            public int RESOURCE_JEWEL;
        }

        [Serializable]
        public class Store : ISaveDataElement
        {
            public bool AD_COUPON_SUBSCRIBE_PROGRESS;
            public bool PACKAGE_0_BUY_PROGRESS;
            public int AD_COUPON_COUNT;
            public int NORMAL_BOX_COUNT;
            public int RARE_BOX_COUNT;
            public int GOLD_1H_TICKET_COUNT;
            public int GOLD_6H_TICKET_COUNT;
            public int GOLD_12H_TICKET_COUNT;
            public int HEART_1H_TICKET_COUNT;
            public int HEART_6H_TICKET_COUNT;
            public int HEART_12H_TICKET_COUNT;
        }

        [Serializable]
        public class Quest : ISaveDataElement
        {
            public int QUEST_NUMBER;
            public int QUEST_PROGRESS_VALUE;
            public bool QUEST_ALERT_STATE;
        }

        [Serializable]
        public class Achievement : ISaveDataElement
        {
            public int UPG_LNDMRK_0_LEVEL;
            public int UPG_LNDMRK_0_VALUE;
            public int GET_NORMAL_ANIMAL_LEVEL;
            public int GET_NORMAL_ANIMAL_VALUE;
            public int CALL_ALBATROSS_LEVEL;
            public int CALL_ALBATROSS_VALUE;
            public int PHOTO_MISSION_LEVEL;
            public int PHOTO_MISSION_VALUE;
            public int GET_TOUCH_REWARD_LEVEL;
            public int GET_TOUCH_REWARD_VALUE;
            public int USE_TIME_TICKET_LEVEL;
            public int USE_TIME_TICKET_VALUE;
            public int LNDMRK_EVOLUTION_LEVEL;
            public int LNDMRK_EVOLUTION__VALUE;
            public int OPEN_RARE_ANIMAL_BOX_LEVEL;
            public int OPEN_RARE_ANIMAL_BOX_VALUE;
        }

        [Serializable]
        public class Tutorial : ISaveDataElement
        {
            public bool COMPLETE;
            public bool TRIGGERED;
        }

        [Serializable]
        public class Animal : ISaveDataElement
        {
            public int TYPE_IDX;
            public bool LOCKED;
            public int UPG_LEVEL;
        }

        #endregion


        #region Data Instances

        [SerializeField] private User user = new User();
        [SerializeField] private Flag flag = new Flag();
        [SerializeField] private Store store = new Store();
        [SerializeField] private Quest quest = new Quest();
        [SerializeField] private Achievement achievement = new Achievement();
        [SerializeField] private List<Animal> animals = new List<Animal>();
        [SerializeField] private SerializableDictionary<string, Tutorial> tutorials = new SerializableDictionary<string, Tutorial>();

        private readonly Dictionary<Type, ISaveDataElement> dataElements = new Dictionary<Type, ISaveDataElement>();
        private readonly Dictionary<Type, IEnumerable<ISaveDataElement>> dataElementsList = new Dictionary<Type, IEnumerable<ISaveDataElement>>();
        private readonly Dictionary<Type, ISaveDataDictionary> dataElementsDictionary = new Dictionary<Type, ISaveDataDictionary>();

        #endregion


        #region Functions

        private void Awake()
        {
            dataElements.Clear();
            dataElements.Add(typeof(User), user);
            dataElements.Add(typeof(Flag), flag);
            dataElements.Add(typeof(Store), store);
            dataElements.Add(typeof(Quest), quest);
            dataElements.Add(typeof(Achievement), achievement);
            
            dataElementsList.Clear();
            dataElementsList.Add(typeof(List<Animal>), animals);

            dataElementsDictionary.Clear();
            dataElementsDictionary.Add(typeof(SerializableDictionary<string, Tutorial>), tutorials);
        }

        private void OnDestroy()
        {
            dataElements.Clear();
            dataElementsList.Clear();
            dataElementsDictionary.Clear();
        }

        private void OnDisable()
        {
            dataElements.Clear();
            dataElementsList.Clear();
            dataElementsDictionary.Clear();
        }

        public bool TryGetSaveData<T>(object client, out T service) where T : class, ISaveDataElement
        {
            Type dataElementType = typeof(T);
            if (SaveLoadSystem.CheckIfSavable(client) && dataElements.TryGetValue(dataElementType, out ISaveDataElement data))
            {
                service = data as T;
                if (service != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                service = null;
                return false;
            }
        }

        public bool TryGetSaveData<T>(object client, out List<T> service) where T : class, ISaveDataElement
        {
            Type dataElementType = typeof(List<T>);
            if (SaveLoadSystem.CheckIfSavable(client) && dataElementsList.TryGetValue(dataElementType, out IEnumerable<ISaveDataElement> data))
            {
                //Debug.Log($"Check types : " +
                //    $"{typeof(T)}\n" +
                //    $"{typeof(List<T>)}\n" +
                //    $"{data.GetType()}");

                service = data as List<T>;
                if (service != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                service = null;
                return false;
            }
        }

        public bool TryGetSaveData<TKey,TValue>(object client, out Dictionary<TKey,TValue> service) where TValue : class, ISaveDataElement
        {
            Type dataElementType = typeof(SerializableDictionary<TKey, TValue>);
            if (SaveLoadSystem.CheckIfSavable(client) && dataElementsDictionary.TryGetValue(dataElementType, out ISaveDataDictionary data))
            {
                //Debug.Log($"Check types : " +
                //    $"{typeof(T)}\n" +
                //    $"{typeof(List<T>)}\n" +
                //    $"{data.GetType()}");

                var temp = data as SerializableDictionary<TKey, TValue>;
                if (temp != null)
                {
                    service = temp.ToDictionary();
                    return true;
                }
                else
                {
                    service = null;
                    return false;
                }
            }
            else
            {
                service = null;
                return false;
            }
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void LoadFromJson(string a_Json)
        {
            JsonUtility.FromJsonOverwrite(a_Json, this);
        }

        #endregion
    }
}