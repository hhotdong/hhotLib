using UnityEngine;

namespace hhotLib.Common
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                string assetName = typeof(T).Name;
                T[] assets = Resources.LoadAll<T>(assetName);
                if (assets.Length > 1)
                {
                    Debug.LogError("Found multiple " + assetName + "s on the resources folder. It is a Singleton ScriptableObject, there should only be one.");
                    return null;
                }

                if (assets.Length == 0)
                {
                    Debug.LogError("Could not find a " + assetName + " on the resources folder. It was created at runtime, therefore it will not be visible on the assets folder and it will not persist.");
                    return null;
                }

                instance = assets[0];
                return instance;
            }
        }
    }
}