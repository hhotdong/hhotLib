using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace hhotLib.Common
{
    public class GameSettings : ScriptableObject
    {
        public Sound sound;

        [Serializable]
        public class Sound
        {
            public float volume = 1.0f;
        }

        private static GameSettings instance;
        public static GameSettings Instance
        {
            get
            {
                if (instance != null)
                    return instance;
            #if UNITY_EDITOR
                const string ASSET_PATH       = "Assets/Resources/Configuration";
                const string ASSET_NAME       = "GameSettings";
                const string ASSET_EXT        = ".asset";
                string assetPath              = Path.Combine(ASSET_PATH, ASSET_NAME);
                string assetPathWithExtension = Path.ChangeExtension(assetPath, ASSET_EXT);
                instance = AssetDatabase.LoadAssetAtPath<GameSettings>(assetPathWithExtension);

                if (instance != null)
                    return instance;

                Directory.CreateDirectory(ASSET_PATH);
                instance = ScriptableObject.CreateInstance<GameSettings>();
                AssetDatabase.CreateAsset(instance, assetPathWithExtension);
                AssetDatabase.SaveAssets();
                return instance;
            #else
                instance = ScriptableObject.CreateInstance<GameSettings>();
                return instance;
            #endif
            }
        }
    }
}