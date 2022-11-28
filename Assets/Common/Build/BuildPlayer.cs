using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace hhotLib.Build
{
    public class BuildPlayer
    {
        public static void Build()
        {
#if UNITY_ANDROID
        string AOS_FILE_NAME = EditorUserBuildSettings.buildAppBundle
            ? $"Build/AOS/APPNAME_{PlayerSettings.bundleVersion}_{System.DateTime.Now.Date.ToString("yyMMdd")}.aab"
            : $"Build/AOS/APPNAME_{PlayerSettings.bundleVersion}_{System.DateTime.Now.Date.ToString("yyMMdd")}.apk";

        var opts              = new BuildPlayerOptions();
        opts.scenes           = FindEnabledEditorScenes();
        opts.locationPathName = AOS_FILE_NAME;
        opts.target           = BuildTarget.Android;
        opts.options          = BuildOptions.None;
        if (EditorUserBuildSettings.development)
            opts.options |= BuildOptions.Development;

        BuildReport  report  = BuildPipeline.BuildPlayer(opts);
        BuildSummary summary = report.summary;

        if      (summary.result == BuildResult.Succeeded) Debug.Log($"Build succeeded: {summary.totalSize} bytes");
        else if (summary.result == BuildResult.Failed)    Debug.LogError("Build failed");
        else if (summary.result == BuildResult.Cancelled) Debug.LogError("Build cancelled");
        else                                              Debug.LogError("Build failed(Unknown issues)");
#elif UNITY_IOS
            var opts              = new BuildPlayerOptions();
		opts.scenes           = FindEnabledEditorScenes();
		opts.locationPathName = "Build/iOS";
		opts.target           = BuildTarget.iOS;
		opts.options          = BuildOptions.None;
		if (EditorUserBuildSettings.development)
			opts.options |= BuildOptions.Development;

		BuildReport  report  = BuildPipeline.BuildPlayer(opts);
		BuildSummary summary = report.summary;

		if      (summary.result == BuildResult.Succeeded) Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
		else if (summary.result == BuildResult.Failed)    Debug.LogError("Build failed");
        else if (summary.result == BuildResult.Cancelled) Debug.LogError("Build cancelled");
        else                                              Debug.LogError("Build failed(Unknown issues)");
#else
            Debug.LogError($"Invalid platform({Application.platform}) to build player.");
        return;
#endif
        }

		private static string[] FindEnabledEditorScenes()
        {
            var editorScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                    editorScenes.Add(scene.path);
            }
            return editorScenes.ToArray();
        }
    }
}