using System;
using System.Collections.Generic;
using UnityEngine;

namespace hhotLib.Common
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey>   keys;
        [SerializeField] private List<TValue> values;

        private Dictionary<TKey, TValue> target = new Dictionary<TKey, TValue>();

        public Dictionary<TKey, TValue> ToDictionary()
        {
            return target;
        }

        public void OnBeforeSerialize()
        {
            keys   = new List<TKey>  (target.Keys);
            values = new List<TValue>(target.Values);
        }

        public void OnAfterDeserialize()
        {
            int count = Math.Min(keys.Count, values.Count);

            target = new Dictionary<TKey, TValue>(count);
            for (var i = 0; i < count; ++i)
                target.Add(keys[i], values[i]);
        }
    }
}