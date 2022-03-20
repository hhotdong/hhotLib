//https://coffeebraingames.wordpress.com/2017/11/12/script-to-play-the-main-scene-from-anywhere/

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

	// pref IDs
	private const string LAST_OPENED_SCENE = "Game.LastOpenedScene";
	private const string PLAYED_USING_RUN_UTILS = "Game.PlayedUsingRunUtils";

	// bool states
	private static bool aboutToRun = false;

	[MenuItem("Game/Run Game %&z")]
	public static void Run()
	{
		SceneSetup[] setups = EditorSceneManager.GetSceneManagerSetup();
		if (setups.Length > 0)
		{
			EditorPrefs.SetString(LAST_OPENED_SCENE, setups[0].path);
		}

		EditorPrefs.SetBool(PLAYED_USING_RUN_UTILS, true);
		aboutToRun = true;

		// Refresh first to cause compilation and include new assets
		AssetDatabase.Refresh();

		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        var entryScene = Resources.Load<SceneAsset>("Entry");
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

		if (!EditorPrefs.GetBool(PLAYED_USING_RUN_UTILS))
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
		string lastScene = EditorPrefs.GetString(LAST_OPENED_SCENE);
		if (!string.IsNullOrEmpty(lastScene))
		{
			EditorSceneManager.OpenScene(lastScene);
		}

		EditorPrefs.SetBool(PLAYED_USING_RUN_UTILS, false); // reset flag
	}

}