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
                deVoid.Utils.Signals.Get<deVoid.UIFramework.AppendScreenTransitionEventSignal>().Dispatch("TestPanel",
                    new deVoid.UIFramework.ScreenTransitionEvent(deVoid.UIFramework.VisibleState.IsAppearing, () => Debug.Log($"IsAppearing TestPanel", "TEST")));
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.AppendScreenTransitionEventSignal>().Dispatch("TestPanel",
                    new deVoid.UIFramework.ScreenTransitionEvent(deVoid.UIFramework.VisibleState.IsAppeared, () => Debug.Log($"IsAppeared TestPanel", "TEST")));
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.AppendScreenTransitionEventSignal>().Dispatch("TestPanel",
                    new deVoid.UIFramework.ScreenTransitionEvent(deVoid.UIFramework.VisibleState.IsDisappearing, () => Debug.Log($"IsDisappearing TestPanel", "TEST")));
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.AppendScreenTransitionEventSignal>().Dispatch("TestPanel",
                    new deVoid.UIFramework.ScreenTransitionEvent(deVoid.UIFramework.VisibleState.IsDisappeared, () => Debug.Log($"IsDisappeared TestPanel", "TEST")));
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.ShowPanelSignal>().Dispatch("AlertPanel", new AlertPanelProperties("TestText"));
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                deVoid.Utils.Signals.Get<deVoid.UIFramework.PopWindowSignal>().Dispatch();
            }
        }

        private void OnGUI()
        {
            
        }
    }
}