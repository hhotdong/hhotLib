using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace hhotLib.Build
{
    public class PreprocessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (EditorPrefs.GetBool("development", false))
            {
                // do something before build
            }
        }
    }
}