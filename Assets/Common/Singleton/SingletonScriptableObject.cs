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
                T[] instances = Resources.FindObjectsOfTypeAll<T>();
                int count = instances.Length;
                if (count > 0)
                {
                    if (count == 1)
                        return instance = instances[0];

                    Debug.LogWarning($"[{nameof(SingletonScriptableObject<T>)}] There should never be " +
                        $"more than one {nameof(SingletonScriptableObject<T>)} in the project, " +
                        $"but {count} were found. The first instance found will be used, and all others will be destroyed.");

                    for (int i = 1; i < instances.Length; i++)
                        DestroyImmediate(instances[i], true);

                    return instance = instances[0];
                }

                Debug.LogError($"[{nameof(SingletonScriptableObject<T>)}] An instance is needed in the project but no existing instances were found!");
                return null;
            }
        }
    }
}