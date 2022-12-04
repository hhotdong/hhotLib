using System;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace hhotLib.Common
{
    [Serializable]
    public class DebugTag
    {
        [Disable] public string TagName;
        [Disable] public bool   ShouldDisplay;
    }

    [Serializable]
    public class DebugTag_Temporary
    {
        public string TagName;
        public bool   ShouldDisplay;
    }

    public static class DebugTagConstant
    {
        public const string Default = "Default";
        public const string Save    = "Save";
        public const string Test    = "Test";
        public const string Debug   = "Debug";
    }

    public class DebugSettings : ScriptableObject
    {
        [SerializeField] private DebugTag[]           constantTags;
        [SerializeField] private DebugTag_Temporary[] temporaryTags;

        private static DebugSettings instance;
        public static DebugSettings Instance
        {
            get
            {
                if (instance != null)
                    return instance;
            #if UNITY_EDITOR
                const string ASSET_PATH       = "Assets/Editor Default Resources";
                const string ASSET_NAME       = "DebugSettings";
                const string ASSET_EXT        = ".asset";
                string assetPath              = Path.Combine(ASSET_PATH, ASSET_NAME);
                string assetPathWithExtension = Path.ChangeExtension(assetPath, ASSET_EXT);
                instance = AssetDatabase.LoadAssetAtPath<DebugSettings>(assetPathWithExtension);

                if (instance != null)
                    return instance;

                Directory.CreateDirectory(ASSET_PATH);
                instance = ScriptableObject.CreateInstance<DebugSettings>();
                AssetDatabase.CreateAsset(instance, assetPathWithExtension);
                AssetDatabase.SaveAssets();
                return instance;
            #else
                instance = ScriptableObject.CreateInstance<DebugSettings>();
                return instance;
            #endif
            }
        }

        public bool CheckIfDebugTagValid(string tag)
        {
            for (int i = 0; i < constantTags.Length; i++)
            {
                if (constantTags[i].TagName == tag && constantTags[i].ShouldDisplay)
                    return true;
            }

            for (int i = 0; i < temporaryTags.Length; i++)
            {
                if (temporaryTags[i].TagName == tag && temporaryTags[i].ShouldDisplay)
                    return true;
            }
            return false;
        }

        [MenuItem("DebugSettings", menuItem = "Debug/Open DebugSettings #l")]
        public static void SelectAsset()
        {
    #if UNITY_EDITOR
            Selection.activeObject = Instance;
    #endif
        }

        private void OnValidate()
        {
            if (constantTags == null || temporaryTags == null)
                return;

            var groupByTag  = constantTags.GroupBy(x => x.TagName.ToUpper());
            var isDuplicate = groupByTag.Any(g => g.Count() > 1);
            if (isDuplicate)
            {
                var duplicateElement = groupByTag.FirstOrDefault(g => g.Count() > 1);
                Debug.LogError($"Debug tag({duplicateElement.Key}) is duplicated in constant tags!", this, DebugTagConstant.Default);
            }

            var constants = Utils.GetConstants(typeof(DebugTagConstant));
            if (constantTags.Length != constants.Length)
            {
                Array.Resize(ref constantTags, constants.Length);
                for (int i = 0; i < constantTags.Length; i++)
                {
                    if (constantTags[i] == null)
                        constantTags[i] = new DebugTag();
                }
            }

            for (int i = 0; i < constantTags.Length; i++)
            {
                constantTags[i].TagName       = constants[i].Name;
                constantTags[i].ShouldDisplay = true;
            }

            var groupByTag_temp = temporaryTags.GroupBy(x => x.TagName.ToUpper());
            isDuplicate = groupByTag_temp.Any(g => g.Count() > 1);
            if (isDuplicate)
            {
                var duplicateElement = groupByTag_temp.FirstOrDefault(g => g.Count() > 1);
                Debug.LogError($"Debug tag({duplicateElement.Key}) is duplicated in temporary tags!", this, DebugTagConstant.Default);
            }
        }
    }
}