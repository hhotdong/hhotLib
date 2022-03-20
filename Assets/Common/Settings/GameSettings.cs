using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace hhotLib
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
                if (instance == null)
                {
                    const string ROOT_NAME = "Assets";
                    const string ASSET_PATH = "Resources/Configuration";
                    const string FOLDER_PARENT_NAME = "Assets/Resources";
                    const string FOLDER_NAME = "Configuration";
                    const string ASSET_NAME = "GameSettings";
                    const string ASSET_EXTENSION = ".asset";

                    instance = Resources.Load(ASSET_NAME) as GameSettings;
                    if (instance == null)
                    {
                        instance = CreateInstance<GameSettings>();
#if UNITY_EDITOR
                        string path = Path.Combine(Application.dataPath, ASSET_PATH);
                        if (!Directory.Exists(path))
                            AssetDatabase.CreateFolder(FOLDER_PARENT_NAME, FOLDER_NAME);

                        string fullPath = Path.Combine(Path.Combine(ROOT_NAME, ASSET_PATH), ASSET_NAME + ASSET_EXTENSION);
                        AssetDatabase.CreateAsset(instance, fullPath);
#endif
                    }
                }
                return instance;
            }
        }
    }
}