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
        public bool animate;

        private void Start()
        {
            deVoid.UIFramework.UINavigation.Instance.Initialize();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.ShowPanelSignal>().Dispatch("TestPanel", new deVoid.UIFramework.Examples.TestPanelProperties("TestPanel"));
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.HidePanelSignal>().Dispatch("TestPanel");
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.PushWindowSignal>().Dispatch("TestWindow", new deVoid.UIFramework.Examples.TestWindowProperties("TestWindow"));
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.PushWindowSignal>().Dispatch("TestWindow2", new deVoid.UIFramework.Examples.TestWindowProperties("TestWindow2"));
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.PushWindowSignal>().Dispatch("TestWindow3", new deVoid.UIFramework.Examples.TestWindowProperties("TestWindow3"));
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.PushWindowSignal>().Dispatch("TestPopupWindow", new deVoid.UIFramework.Examples.TestWindowProperties("TestPopupWindow"));
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.PopWindowSignal>().Dispatch();
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.ShowPanelSignal>().Dispatch("AlertPanel", new AlertPanelProperties("TestText"));
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
            }
        }

        private void OnGUI()
        {
            
        }
    }
}