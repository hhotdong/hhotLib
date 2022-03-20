//using System.IO;
//using UnityEditor;
//using UnityEngine;

//public class IAPAsset : ScriptableObject
//{
//    const string settingAssetsName = "IAPSetting";
//    const string settingPath = "Nanali/IAP/Resources";
//    const string settingExtension = ".asset";

//    public bool IsDevelop = true;
//    public IAPInformation[] IAPInformations;

//    private static IAPAsset instance;

//    public static IAPAsset Instance
//    {
//        get
//        {
//            if (instance == null)
//            {
//                instance = Resources.Load(settingAssetsName) as IAPAsset;
//                if (instance == null)
//                {
//                    // If not found, autocreate the asset object.
//                    instance = CreateInstance<IAPAsset>();

//#if UNITY_EDITOR
//                    string properPath = Path.Combine(Application.dataPath, settingPath);
//                    if (!Directory.Exists(properPath))
//                    {
//                        AssetDatabase.CreateFolder("Nanali/IAP", "Resources");
//                    }

//                    string fullPath = Path.Combine(Path.Combine("Assets", settingPath),
//                        settingAssetsName + settingExtension
//                    );
//                    AssetDatabase.CreateAsset(instance, fullPath);
//#endif
//                }
//            }
//            return instance;
//        }
//    }
//}
