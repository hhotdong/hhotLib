using System.Collections.Generic;
using UnityEngine;

namespace hhotLib.Common
{
    public class UICoinEffectDestination : MonoBehaviour
    {
        public CoinEffectType GetDestinationType => coinEffectType;

        [SerializeField] private CoinEffectType coinEffectType;

        private static readonly Dictionary<CoinEffectType, UICoinEffectDestination> destinations = new Dictionary<CoinEffectType, UICoinEffectDestination>();

        public static Transform GetDestination(CoinEffectType type)
        {
            if (destinations.ContainsKey(type) == false || destinations[type] == null)
                return null;
            return destinations[type].transform;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            destinations.Clear();
        }

        private void OnEnable()
        {
            destinations[coinEffectType] = this;
        }

        private void OnDisable()
        {
            destinations.Remove(coinEffectType);
        }
    }
}