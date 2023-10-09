using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace hhotLib.Common
{
    public class TestController : DebugOnlyMonoBehaviour
    {
        public TextMeshProUGUI text;
        public Image img;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log($"Make UINavigation");
                deVoid.UIFramework.UINavigation.Instance.Initialize();
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
}