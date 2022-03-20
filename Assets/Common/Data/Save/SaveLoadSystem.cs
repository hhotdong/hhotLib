//데이터 리셋 기능

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

        private static SaveData s_SaveDataContainer;
        public static SaveData SaveDataContainer => s_SaveDataContainer;

        private static readonly List<ISavable> s_Savables = new List<ISavable>();

        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// BeforeSceneLoad attribute makes sure that this method is called before Awake().
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            CreateSaveDataContainer();
            s_Savables.Clear();
            IsInitialized = true;
        }

        private static void CreateSaveDataContainer()
        {
            if (s_SaveDataContainer != null)
            {
                UnityEngine.Object.DestroyImmediate(s_SaveDataContainer, true);
                s_SaveDataContainer = null;
            }

#if !UNITY_EDITOR
            saveDataContainer = ScriptableObject.CreateInstance<SaveData>();
#else
            var saveData = Resources.FindObjectsOfTypeAll<SaveData>();
            if (saveData.Length > 0)
            {
                var debugData = saveData.Where((data, b) => data.IsDebug);
                if (debugData.Count<SaveData>() > 0)
                {
                    var enumerator = debugData.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        s_SaveDataContainer = enumerator.Current;
                        UnityEngine.Debug.Log("Found saveData for debugging.");
                        return;
                    }
                }
                else
                    Debug.LogError($"SaveData for debug not found!", DebugTagConstant.SaveLoad);
            }
            else
                Debug.LogError($"SaveData not found!", DebugTagConstant.SaveLoad);

            s_SaveDataContainer = ScriptableObject.CreateInstance<SaveData>();

            const string ROOT_NAME           = "Assets";
            const string ASSET_PATH          = "Resources/Save";
            const string FOLDER_PARENT_NAME  = "Assets/Resources";
            const string FOLDER_NAME         = "Save";
            const string ASSET_NAME          = "SaveDataContainer";
            const string ASSET_EXTENSION     = ".asset";

            s_SaveDataContainer.name = ASSET_NAME;
            var temp = Resources.Load(ASSET_NAME) as SaveData;
            if (temp != null)
            {
                UnityEngine.Object.DestroyImmediate(temp, true);
                temp = null;
            }

            string path = Path.Combine(Application.dataPath, ASSET_PATH);
            if (!Directory.Exists(path))
                AssetDatabase.CreateFolder(FOLDER_PARENT_NAME, FOLDER_NAME);
            string fullPath = Path.Combine(Path.Combine(ROOT_NAME, ASSET_PATH), ASSET_NAME + ASSET_EXTENSION);
            AssetDatabase.CreateAsset(s_SaveDataContainer, fullPath);
#endif
        }

        public static void Load()
        {
            if (!IsInitialized)
                return;

            UnityEngine.Debug.Log($"Load");
#if UNITY_EDITOR
            if (!s_SaveDataContainer.IsDebug)  // 에디터에서는 디버그용 세이브 데이터 SO가 있다면 해당 에셋에 설정한 값으로 게임을 시작함.
#endif
            {
                string saveDataJSON = EncryptedPlayerPrefs.GetString(KEY_SAVE_DATA, "");
                if (string.IsNullOrEmpty(saveDataJSON))
                    UnityEngine.Debug.LogWarning("JSON saveData not found!");
                else
                {
                    Debug.Log($"JSON saveData found. App initializes with the following data.\n\n{saveDataJSON}", DebugTagConstant.SaveLoad);
                    s_SaveDataContainer.LoadFromJson(saveDataJSON);
                }
            }

            for (int i = 0; i < s_Savables.Count; i++)
                s_Savables[i].OnLoad();
        }

        public static void Save()
        {
            if (IsInitialized)
            {
                UnityEngine.Debug.Log("Save");
                for (int i = 0; i < s_Savables.Count; i++)
                    s_Savables[i].OnSave();
                EncryptedPlayerPrefs.SetString(KEY_SAVE_DATA, s_SaveDataContainer.ToJson());
                PlayerPrefs.Save();
            }
            else
                UnityEngine.Debug.LogError($"Failed to save! SaveLoadSystem isn't initialized!");
        }

        public static void Register(ISavable savable)
        {
            if (s_Savables.Contains(savable))
                Debug.LogWarning("This savable already registered!", DebugTagConstant.SaveLoad);
            else
                s_Savables.Add(savable);
        }

        public static void Unregister(ISavable savable)
        {
            if (s_Savables.Contains(savable))
                s_Savables.Remove(savable);
            else
                Debug.LogWarning("This savable is not registered!", DebugTagConstant.SaveLoad);
        }

        public static bool CheckIfSavable(object client)
        {
            return client is ISavable && s_Savables.Contains(client);
        }

        public static void Reset()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("Reset all save data", DebugTagConstant.SaveLoad);
            PlayerPrefs.DeleteAll();
            for (int i = 0; i < s_Savables.Count; i++)
                s_Savables[i].OnReset();
            Initialize();
#else
            UnityEngine.Debug.LogWarning("You tried to remove save data but it's not possible on editor or development build!");
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
                    s_Savables.Clear();
                    Debug.Log($"Enter Edit Mode : Savables({s_Savables.Count}) , IsInitialized({IsInitialized})", DebugTagConstant.SaveLoad);
                    break;

                case PlayModeStateChange.ExitingEditMode:
                    IsInitialized = false;
                    s_Savables.Clear();
                    Debug.Log($"Exit Edit Mode : Savables({s_Savables.Count}) , IsInitialized({IsInitialized})", DebugTagConstant.SaveLoad);
                    break;

                case PlayModeStateChange.EnteredPlayMode:
                    Debug.Log($"Enter Play Mode : Savables({s_Savables.Count}) , IsInitialized({IsInitialized})", DebugTagConstant.SaveLoad);
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    Save();
                    Debug.Log($"Exit Play Mode : Savables({s_Savables.Count}) , IsInitialized({IsInitialized})", DebugTagConstant.SaveLoad);
                    break;

                default:
                    break;
            }
        }
#endif
    }
}