//using System.IO;
//using UnityEditor;
//using UnityEngine;

//public class AdmobAsset : ScriptableObject
//{
//    const string settingAssetsName = "ADSetting";
//    const string settingPath = "Nanali/Admob/Resources";
//    const string settingExtension = ".asset";

//    public string TestAdId
//    {
//        get
//        {
//#if UNITY_ANDROID
//            return "ca-app-pub-3940256099942544/5224354917";
//#elif UNITY_IOS
//            return "ca-app-pub-3940256099942544/1712485313";
//#else
//            return string.Empty;
//#endif
//        }
//    }

//    public bool IsDevelop = true;
//    public bool IsShowLog = true;
//    [Header("보상형 광고")]
//    public AdmobInformation[] RewardedAdInfo;
//    [Header("전면 광고")]
//    public AdmobInformation InterstitialAdInfo_Test;
//    public AdmobInformation InterstitialAdInfo_Live;

//    private static AdmobAsset instance;

//    public static AdmobAsset Instance
//    {
//        get
//        {
//            if (instance == null)
//            {
//                instance = Resources.Load(settingAssetsName) as AdmobAsset;
//                if (instance == null)
//                {
//                    // If not found, autocreate the asset object.
//                    instance = CreateInstance<AdmobAsset>();

//#if UNITY_EDITOR
//                    string properPath = Path.Combine(Application.dataPath, settingPath);
//                    if (!Directory.Exists(properPath))
//                    {
//                        AssetDatabase.CreateFolder("Assets/Nanali/Admob", "Resources");
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
