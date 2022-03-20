using UnityEngine;
using hhotLib.Common;

namespace hhotLib.Build
{
    public class TestController : DebugOnlyMonoBehaviour
    {
        private void Start()
        {

        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 100), "Load test scene"))
            {
                Debug.Log("Load Battle Scene");
                SceneLoader.Instance.Load("Battle", true, true, "Base");
            }
        }
    }
}