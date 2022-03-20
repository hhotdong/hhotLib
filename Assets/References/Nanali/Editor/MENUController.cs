//using UnityEngine;
//using UnityEditor;

//public class MENUController : ScriptableObject
//{
//    [MenuItem("Nanali/IAP/Settings")]
//    public static void OpenIAPSettings()
//    {
//        var obj = AssetDatabase.LoadAssetAtPath<IAPAsset>("Assets/Nanali/2. IAP/Resources/IAPSetting.asset");
//        if (obj == null)
//        {
//            var variable = CreateInstance<IAPAsset>();
//            AssetDatabase.CreateAsset(variable, "Assets/Nanali/2. IAP/Resources/IAPSetting.asset");
//        }

//        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Nanali/2. IAP/Resources/IAPSetting.asset");
//        AssetDatabase.Refresh();
//    }

//    [MenuItem("Nanali/Admob/Settings")]
//    public static void OpenAdmobSettings()
//    {
//        var obj = AssetDatabase.LoadAssetAtPath<AdmobAsset>("Assets/Nanali/1. Admob/Resources/ADSetting.asset");
//        if (obj == null)
//        {
//            var variable = CreateInstance<AdmobAsset>();
//            AssetDatabase.CreateAsset(variable, "Assets/Nanali/1. Admob/Resources/ADSetting.asset");
//        }

//        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Nanali/1. Admob/Resources/ADSetting.asset");
//        AssetDatabase.Refresh();
//    }

//    [MenuItem("Nanali/Backend/Settings")]
//    public static void OpenBackendSettings()
//    {
//        var obj = AssetDatabase.LoadAssetAtPath<BackendAsset>("Assets/Nanali/6. Backend/Resources/BackendAsset.asset");
//        if (obj == null)
//        {
//            var variable = CreateInstance<BackendAsset>();
//            AssetDatabase.CreateAsset(variable, "Assets/Nanali/6. Backend/Resources/BackendAsset.asset");
//        }

//        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Nanali/6. Backend/Resources/BackendAsset.asset");
//        AssetDatabase.Refresh();
//    }
//}
