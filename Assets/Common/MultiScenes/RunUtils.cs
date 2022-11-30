// Credit: https://coffeebraingames.wordpress.com/2017/11/12/script-to-play-the-main-scene-from-anywhere/
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class RunUtils
{
	static RunUtils()
	{
        EditorApplication.playModeStateChanged += LoadLastOpenedScene;
	}

	private static bool aboutToRun;

	private const string PREFS_KEY_LAST_OPENED_SCENE      = "Game.LastOpenedScene";
	private const string PREFS_KEY_PLAYED_USING_RUN_UTILS = "Game.PlayedUsingRunUtils";
	private const string ENTRY_SCENE_NAME                 = "Entry";

	[MenuItem("Game/Run Game %&z")]
	public static void Run()
	{
		SceneSetup[] setups = EditorSceneManager.GetSceneManagerSetup();
		if (setups.Length > 0)
			EditorPrefs.SetString(PREFS_KEY_LAST_OPENED_SCENE, setups[0].path);

		EditorPrefs.SetBool(PREFS_KEY_PLAYED_USING_RUN_UTILS, true);
		aboutToRun = true;

		// Refresh first to cause compilation and include new assets
		AssetDatabase.Refresh();

		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        var entryScene = Resources.Load<SceneAsset>(ENTRY_SCENE_NAME);
        Debug.Assert(entryScene);
        EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(entryScene));
		EditorApplication.isPlaying = true;
	}

	private static void LoadLastOpenedScene(PlayModeStateChange playModeStateChange)
	{
		if (EditorApplication.isPlaying || EditorApplication.isCompiling)
		{
			// changed to playing or compiling
			// no need to do anything
			return;
		}

		if (EditorPrefs.GetBool(PREFS_KEY_PLAYED_USING_RUN_UTILS) == false)
		{
			// this means that normal play mode might have been used
			return;
		}

		// We added this check because this method is still invoked while EditorApplication.isPlaying is false
		// We only load the last opened scene when the aboutToRun flag is "consumed"
		if (aboutToRun)
		{
			aboutToRun = false;
			return;
		}

		// at this point, the scene has stopped playing
		// so we load the last opened scene
		string lastScene = EditorPrefs.GetString(PREFS_KEY_LAST_OPENED_SCENE);
		if (string.IsNullOrEmpty(lastScene) == false)
			EditorSceneManager.OpenScene(lastScene);

		EditorPrefs.SetBool(PREFS_KEY_PLAYED_USING_RUN_UTILS, false);  // reset flag
	}

}