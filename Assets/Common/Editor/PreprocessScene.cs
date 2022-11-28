using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor;

public class PreprocessScene : IProcessSceneWithReport
{
    public int callbackOrder => 0;

    public void OnProcessScene(UnityEngine.SceneManagement.Scene scene, BuildReport report)
    {
        if (report == null)  // When this callback is invoked for Scene loading during Editor playmode, this parameter will be null.
            return;

        if (EditorUserBuildSettings.development == false)
        {
            var debugObjs = Object.FindObjectsOfType<DebugOnlyMonoBehaviour>();
            for (int i = 0; i < debugObjs.Length; i++)
            {
                if (debugObjs[i].gameObject)
                    Object.DestroyImmediate(debugObjs[i].gameObject);
                else
                    Object.DestroyImmediate(debugObjs[i]);
            }
        }
    }
}
