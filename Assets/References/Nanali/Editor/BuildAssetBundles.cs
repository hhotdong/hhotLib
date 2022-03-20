using UnityEngine;
using UnityEditor;

public class BuildAsssetBundles : MonoBehaviour
{

	[MenuItem("Nanali/Bundles/에셋 번들 캐시 삭제")]
	static void DeleteAssetBundleCache()
	{
		if(Caching.ClearCache())
        {
            EditorUtility.DisplayDialog("알림", "캐시가 삭제되었습니다.", "확인");
        }
        else
        {
            EditorUtility.DisplayDialog("오류", "캐시 삭제에 실패했습니다.", "확인");
        }
	}

	[MenuItem("Nanali/PlayerPrefs 전체 삭제")]
	static void PlayerPrefsDeleteAl()
	{
		PlayerPrefs.DeleteAll();
		EditorUtility.DisplayDialog("알림", "초기화 완료.", "확인");
	}

	/***********************************************************************
	 *
	 * 용도 : MenuItem을 사용하면 메뉴창에 새로운 메뉴를 추가할 수 있습니다.
	 * (아래의 코드에서는 Bundles 항목에 하위 항목으로 Build AssetBundles 항목을 추가.)
	***********************************************************************/
	//[MenuItem("Nanali/Bundles/LevelDesign - Android")]
	//static void BuildAllAssetBundles_Android()
	//{
	//	BuildAssetBundle(BuildTarget.Android, AssetBundleType.leveldesign);
	//}

	//[MenuItem("Nanali/Bundles/LevelDesign - iOS")]
	//static void BuildAllAssetBundles_iOS()
	//{
	//	BuildAssetBundle(BuildTarget.iOS, AssetBundleType.leveldesign);
	//}

	static void BuildAssetBundle(BuildTarget platform, params AssetBundleType[] type)
	{
		/***********************************************************************
		* 이름 : BuildPipeLine.BuildAssetBundles()
		* 용도 : BuildPipeLine 클래스의 함수 BuildAssetBundles()는 에셋번들을 만들어줍니다.
		* 매개변수에는 String 값을 넘기게 되며, 빌드된 에셋 번들을 저장할 경로입니다.
		* 예를 들어 Assets 하위 폴더에 저장하려면 "Assets/AssetBundles"로 입력해야합니다.
		***********************************************************************/

		string savedPath = "";
		switch (platform) 
		{
			case BuildTarget.Android: savedPath = "Assets/AssetBundles/Android"; break;
			case BuildTarget.iOS: savedPath = "Assets/AssetBundles/iOS"; break;
		}

		if (type.Length == 1 && type[0] == AssetBundleType.all) //all bundle build.
		{
			BuildPipeline.BuildAssetBundles(savedPath, BuildAssetBundleOptions.None, platform);
		}
		else
		{
			AssetBundleBuild[] buildBundles = new AssetBundleBuild[type.Length];

			for (int i = 0; i < type.Length; i++)
			{
				string bundleName = GetBundleName(type[i]);

				buildBundles[i].assetBundleName = bundleName;
				buildBundles[i].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
			}

			BuildPipeline.BuildAssetBundles(savedPath, buildBundles, BuildAssetBundleOptions.None, platform);
		}

	}

	static string GetBundleName(AssetBundleType type)
	{
		string bundleName = "";
		switch (type)
		{
			case AssetBundleType.leveldesign: bundleName = "leveldesign"; break;
		}

		return bundleName;
	}
}

public enum AssetBundleType
{
	all,
	leveldesign,
}