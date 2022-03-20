using UnityEngine;
using hhotLib.Save;

namespace hhotLib.Common
{
    //[CreateAssetMenu]
    public class GameResourcesManager : SingletonScriptableObject<GameResourcesManager>, ISavable
    {
        [Header("Initial Values")]
        [SerializeField] private int goldInitValue = 0;
        [SerializeField] private int jewelInitValue = 0;

        [Header("Resources")]
        [SerializeField] private int goldRetained;
        [SerializeField] private int jewelRetained;

        public int GoldRetained => goldRetained;
        public int JewelRetained => jewelRetained;

        public int goldRetainedProxy;
        public int jewelRetainedProxy;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            var inst = Instance;
        }

        public void AddGold(int additive, bool deferredProxy)
        {
            goldRetained += additive;

            if (deferredProxy)
            {
                EventManager.Instance.RegisterQueuedEvent("AddGoldProxy", () => goldRetainedProxy += additive);
            }
            else
            {
                goldRetainedProxy += additive;
            }
        }

        public bool TrySpendGold(int sink, bool deferredProxy)
        {
            if (goldRetained >= sink)
            {
                goldRetained -= sink;

                if (deferredProxy)
                {
                    EventManager.Instance.RegisterQueuedEvent("SpendGoldProxy", () => goldRetainedProxy -= sink);
                }
                else
                {
                    goldRetainedProxy -= sink;
                }
                return true;
            }
            else
            {
                Debug.LogWarning("Error : Tried to spend gold greater than you have!");
                return false;
            }
        }

        public void Register() => SaveLoadSystem.Register(this);
        public void Unregister() => SaveLoadSystem.Unregister(this);

        public void OnLoad()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out SaveData.GameResources resources))
            {
                goldRetained = resources.RESOURCE_GOLD;
            }
        }

        public void OnSave()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out SaveData.GameResources resources))
            {
                resources.RESOURCE_GOLD = goldRetained;
            }
        }

        public void OnReset()
        {
            goldRetained = goldInitValue;
            jewelRetained = jewelInitValue;
        }
    }
}