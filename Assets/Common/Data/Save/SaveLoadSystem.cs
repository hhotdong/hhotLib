﻿//데이터 리셋 기능

// Execution order of callbacks where ScriptableObject(SO) is referenced(used) on the first scene.
// Before playMode starts : PlayModeStateChange.ExitingEditMode
// 0 frame : (SO)OnEnable -> SubsystemRegistration -> AfterAssembliesLoaded -> BeforeSplashScreen -> BeforeSceneLoad
//             -> (Mono)Awake -> (Mono)OnEnable -> AfterSceneLoad -> RuntimeInitializeOnLoadMethod(without parameter)
// 1 frame : (Mono)Start -> (Mono)Update
// 2 frame : (Mono)Update -> PlayModeStateChange.EnteredPlayMode
// ...
// 1 frame before quit : (Mono)Update -> PlayModeStateChange.ExitingPlayMode
// 0 frame before quit : (Mono)Update -> (Mono) OnDisable -> (Mono) OnDestroy -> PlayModeStateChange.EnteredEditMode

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace hhotLib.Save
{
#if UNITY_EDITOR
    using System.Linq;
    [InitializeOnLoad]
#endif
    public static class SaveLoadSystem
    {
        private const string KEY_SAVE_DATA = "SAVE_DATA";

        private static SaveData saveDataContainer;
        public static SaveData SaveDataContainer => saveDataContainer;

        private static readonly List<ISavable> savables = new List<ISavable>();

        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// BeforeSceneLoad attribute makes sure that this method is called before Awake().
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            IsInitialized = false;
            CreateSaveDataContainer();
            Load();
            IsInitialized = true;
        }

        private static void CreateSaveDataContainer()
        {
            if (saveDataContainer != null)
            {
                UnityEngine.Object.DestroyImmediate(saveDataContainer, true);
                saveDataContainer = null;
            }

#if !UNITY_EDITOR
            saveDataContainer = ScriptableObject.CreateInstance<SaveData>();
#else
            var saveData = Resources.FindObjectsOfTypeAll<SaveData>();
            if(saveData.Length > 0)
            {
                var debugData = saveData.Where((data, b) => data.IsDebug);
                if(debugData.Count<SaveData>() > 0)
                {
                    var enumerator = debugData.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        saveDataContainer = enumerator.Current;
                        UnityEngine.Debug.Log("Found saveData for debugging.");
                        return;
                    }
                }
            }

            saveDataContainer = ScriptableObject.CreateInstance<SaveData>();

            const string ROOT_NAME           = "Assets";
            const string ASSET_PATH          = "Resources/Save";
            const string FOLDER_PARENT_NAME  = "Assets/Resources";
            const string FOLDER_NAME         = "Save";
            const string ASSET_NAME          = "SaveDataContainer";
            const string ASSET_EXTENSION     = ".asset";

            saveDataContainer.name = ASSET_NAME;
            var temp = Resources.Load(ASSET_NAME) as SaveData;
            if (temp != null)
            {
                UnityEngine.Object.DestroyImmediate(temp, true);
                temp = null;
            }

            string path = Path.Combine(Application.dataPath, ASSET_PATH);
            if (!Directory.Exists(path))
            {
                AssetDatabase.CreateFolder(FOLDER_PARENT_NAME, FOLDER_NAME);
            }

            string fullPath = Path.Combine(Path.Combine(ROOT_NAME, ASSET_PATH), ASSET_NAME + ASSET_EXTENSION);
            AssetDatabase.CreateAsset(saveDataContainer, fullPath);
#endif
        }

        private static void Load()
        {
            UnityEngine.Debug.Log($"Load");

#if UNITY_EDITOR
            if (!saveDataContainer.IsDebug)
#endif
            {
                string saveDataJSON = EncryptedPlayerPrefs.GetString(KEY_SAVE_DATA, string.Empty);

                // Generate new save data if there is no one. Otherwise, load the save data.
                if (string.IsNullOrEmpty(saveDataJSON))
                {
                    UnityEngine.Debug.LogWarning("JSON saveData not found!");
                }
                else
                {
                    UnityEngine.Debug.Log($"JSON saveData found. App initializes with the following data.\n\n{saveDataJSON}");
                    saveDataContainer.LoadFromJson(saveDataJSON);
                }
            }

            for (int i = 0; i < savables.Count; i++)
            {
                savables[i].OnLoad();
            }
        }

        public static void Save()
        {
            if (IsInitialized && saveDataContainer)
            {
                UnityEngine.Debug.Log("Save");

                for (int i = 0; i < savables.Count; i++)
                {
                    savables[i].OnSave();
                }

                EncryptedPlayerPrefs.SetString(KEY_SAVE_DATA, saveDataContainer.ToJson());
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Failed to save : IsSavable({IsInitialized}) , SaveDataContainer{saveDataContainer != null}");
            }
        }

        public static void Register(ISavable savable)
        {
            if (savables.Contains(savable))
            {
                Debug.LogWarning("This savable already registered!");
            }
            else
            {
                savables.Add(savable);
            }
        }

        public static void Unregister(ISavable savable)
        {
            if (savables.Contains(savable))
            {
                savables.Remove(savable);
            }
            else
            {
                Debug.LogWarning("This savable is not registered!");
            }
        }

        public static bool CheckIfSavable(object client)
        {
            return client is ISavable && savables.Contains(client);
        }

        public static void Reset()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("Reset all save data");
            PlayerPrefs.DeleteAll();
            for (int i = 0; i < savables.Count; i++)
            {
                savables[i].OnReset();
            }
            Initialize();
#else
            Debug.LogWarning("You tried to remove save data but it's not possible on editor or development build!");
#endif
        }

#if UNITY_EDITOR
        static SaveLoadSystem()
        {
            EditorApplication.playModeStateChanged -= PlaymodeStateChanged;
            EditorApplication.playModeStateChanged += PlaymodeStateChanged;
        }

        private static void PlaymodeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                    IsInitialized = false;
                    savables.Clear();
                    UnityEngine.Debug.Log($"Enter Edit Mode : Savables({savables.Count}) , IsInitialized({IsInitialized})");
                    break;

                case PlayModeStateChange.ExitingEditMode:
                    IsInitialized = false;
                    savables.Clear();
                    UnityEngine.Debug.Log($"Exit Edit Mode : Savables({savables.Count}) , IsInitialized({IsInitialized})");
                    break;

                case PlayModeStateChange.EnteredPlayMode:
                    UnityEngine.Debug.Log($"Enter Play Mode : Savables({savables.Count}) , IsInitialized({IsInitialized})");
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    Save();
                    UnityEngine.Debug.Log($"Exit Play Mode : Savables({savables.Count}) , IsInitialized({IsInitialized})");
                    break;

                default:
                    break;
            }
        }
#endif
    }
}