using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace hhotLib.Build
{
    public class PreprocessScene : IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        public void OnProcessScene(UnityEngine.SceneManagement.Scene scene, BuildReport report)
        {
            if (report == null)
                return;

            if (!EditorUserBuildSettings.development)
            {
                var debugObjs = Object.FindObjectsOfType<DebugOnlyMonoBehaviour>();
                for (int i = 0; i < debugObjs.Length; i++)
                {
                    if (debugObjs[i].gameObject && debugObjs[i].DestroyGOInRelease)
                        Object.DestroyImmediate(debugObjs[i].gameObject);
                    else
                        Object.DestroyImmediate(debugObjs[i]);
                }
            }

            //foreach (var root in scene.GetRootGameObjects())
            //    ProcessHierarchy(root.transform, report);
        }

        //private void ProcessHierarchy(Transform root, BuildReport report)
        //{
        //    var allTransforms = root.GetComponentsInChildren<Transform>(true);
        //    var mobileGameObjects = allTransforms.Where(t => t.CompareTag("Mobile")).Select(t => t.gameObject);
        //    BuildTarget bt = report.summary.platform;
        //    bool isMobile = bt == BuildTarget.Android || bt == BuildTarget.iOS;
        //    foreach (var mobileGameObject in mobileGameObjects)
        //    {
        //        mobileGameObject.SetActive(isMobile);
        //    }
        //}
    }
}