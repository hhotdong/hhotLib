using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;

namespace hhotLib.Build
{
    public class BuildPlayer
    {
        private static string[] SCENES = FindEnabledEditorScenes();
        private static string[] FindEnabledEditorScenes()
        {
            List<string> EditorScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                    EditorScenes.Add(scene.path);
            }

            return EditorScenes.ToArray();
        }

        public static void Build()
        {
#if UNITY_ANDROID
		string AOS_FILE_NAME = EditorUserBuildSettings.buildAppBundle
			? $"Build/AOS/APPNAME_{PlayerSettings.bundleVersion}_{System.DateTime.Now.Date.ToString("yyMMdd")}.aab"
			: $"Build/AOS/APPNAME_{PlayerSettings.bundleVersion}_{System.DateTime.Now.Date.ToString("yyMMdd")}.apk";

		BuildPlayerOptions opts = new BuildPlayerOptions();
		opts.scenes = SCENES;
		opts.locationPathName = AOS_FILE_NAME;
		opts.target = BuildTarget.Android;
		opts.options = BuildOptions.None;
		if (EditorUserBuildSettings.development)
			opts.options |= BuildOptions.Development;

		BuildReport report = BuildPipeline.BuildPlayer(opts);
		BuildSummary summary = report.summary;
		if (summary.result == BuildResult.Succeeded)
			Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
		else if (summary.result == BuildResult.Failed)
			Debug.LogError("Build failed");
		else if (summary.result == BuildResult.Cancelled)
			Debug.LogError("Build cancelled");
		else
			Debug.LogError("Build failed(Unknown issues)");
#elif UNITY_IOS
		BuildPlayerOptions opts = new BuildPlayerOptions();
		opts.scenes = SCENES;
		opts.locationPathName = "Build/iOS";
		opts.target = BuildTarget.iOS;
		opts.options = BuildOptions.None;
		if (EditorUserBuildSettings.development)
			opts.options |= BuildOptions.Development;

		BuildReport report = BuildPipeline.BuildPlayer(opts);
		BuildSummary summary = report.summary;
		if (summary.result == BuildResult.Succeeded)
			Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
		else if (summary.result == BuildResult.Failed)
			Debug.LogError("Build failed");
        else if (summary.result == BuildResult.Cancelled)
			Debug.LogError("Build cancelled");
        else
			Debug.LogError("Build failed(Unknown issues)");
#else
            Debug.LogError("Invalid platform to build player.");
            return;
#endif
        }
    }
}