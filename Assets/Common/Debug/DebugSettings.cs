using System;
using System.IO;
using System.Linq;
using UnityEngine;
using hhotLib.Common;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebugSettings : ScriptableObject
{
    private const string DebugSettingsDir = "Assets/Editor Default Resources";
    private const string DebugSettingsFile = "DebugSettings";
    private const string DebugSettingsFileExtension = ".asset";

    private static DebugSettings instance;
    public static DebugSettings Instance
    {
        get
        {
            if (instance != null)
                return instance;

#if UNITY_EDITOR
            string assetPath = Path.Combine(DebugSettingsDir, DebugSettingsFile);
            string assetPathWithExtension = Path.ChangeExtension(assetPath, DebugSettingsFileExtension);
            instance = AssetDatabase.LoadAssetAtPath<DebugSettings>(assetPathWithExtension);

            if (instance != null)
                return instance;

            Directory.CreateDirectory(DebugSettingsDir);
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

    [SerializeField] private DebugTag[] constantTags;
    [SerializeField] private DebugTag_Temporary[] temporaryTags;

    public bool CheckIfDebugTagValid(string tag)
    {
        for (int i = 0; i < constantTags.Length; i++)
            if (constantTags[i].TagName == tag && constantTags[i].ShouldDisplay)
                return true;
        for (int i = 0; i < temporaryTags.Length; i++)
            if (temporaryTags[i].TagName == tag && temporaryTags[i].ShouldDisplay)
                return true;
        return false;
    }

    [MenuItem("DebugSettings", menuItem = "Debug/Open DebugSettings #l")]
    public static void SelectAsset()
    {
#if UNITY_EDITOR
        //string assetPath = Path.Combine(DebugSettingsDir, DebugSettingsFile);
        //string assetPathWithExtension = Path.ChangeExtension(assetPath, DebugSettingsFileExtension);
        Selection.activeObject = Instance;
#endif
    }

    private void OnValidate()
    {
        if (constantTags == null || temporaryTags == null)
            return;

        var groupByTag = constantTags.GroupBy(x => x.TagName.ToUpper());
        var isDuplicate = groupByTag.Any(g => g.Count() > 1);
        if (isDuplicate)
        {
            var duplicateElement = groupByTag.FirstOrDefault(g => g.Count() > 1);
            Debug.LogError($"Debug tag({duplicateElement.Key}) is duplicated in constantTags!", this, DebugTagConstant.Default);
        }

        var constants = Utils.GetConstants(typeof(DebugTagConstant));
        if (constantTags.Length != constants.Length)
            Array.Resize(ref constantTags, constants.Length);
        for (int i = 0; i < constants.Length; i++)
            constantTags[i].TagName = constants[i].Name;

        var _groupByTag = temporaryTags.GroupBy(x => x.TagName.ToUpper());
        isDuplicate = _groupByTag.Any(g => g.Count() > 1);
        if (isDuplicate)
        {
            var duplicateElement = _groupByTag.FirstOrDefault(g => g.Count() > 1);
            Debug.LogError($"Debug tag({duplicateElement.Key}) is duplicated in temporaryTags!", this, DebugTagConstant.Default);
        }
    }
}

[Serializable]
public class DebugTag
{
    [Disable] public string TagName;
    public bool ShouldDisplay;
}

[Serializable]
public class DebugTag_Temporary
{
    public string TagName;
    public bool ShouldDisplay;
}

public static class DebugTagConstant
{
    public const string Default = "Default";
    public const string SaveLoad = "SaveLoad";
    public const string Debug = "Debug";
    public const string Test = "Test";
}