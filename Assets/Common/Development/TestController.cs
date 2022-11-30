using UnityEngine;
using hhotLib.Common;

public class TestController : DebugOnlyMonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
        }
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
