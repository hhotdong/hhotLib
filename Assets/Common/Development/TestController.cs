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

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Load test scene"))
        {
        }
    }
}
