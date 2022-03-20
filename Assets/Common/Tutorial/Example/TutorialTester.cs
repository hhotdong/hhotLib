using System;
using UnityEngine;
using hhotLib.Save;

namespace hhotLib.Tutorial
{
    public class TutorialTester : MonoBehaviour
    {
        public static event Action OnFinishFirstTutorial;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                OnFinishFirstTutorial?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TutorialManager.Instance.BroadcastTutorialAction($"{TutorialType.NEWBIE_SEEDING.ToString()}_COMPLETE");
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TutorialManager.Instance.BroadcastTutorialAction($"{TutorialType.NEWBIE_PURIFY.ToString()}_COMPLETE");
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TutorialManager.Instance.BroadcastTutorialAction($"{TutorialType.NEWBIE_GET_ENERGY.ToString()}_COMPLETE");
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                TutorialManager.Instance.BroadcastTutorialAction($"{TutorialType.NEWBIE_AUTO_GEN_ENERGY.ToString()}_COMPLETE");
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                TutorialManager.Instance.BroadcastTutorialAction($"{TutorialType.NEWBIE_ACHIEVE_LEVEL_TEN.ToString()}_COMPLETE");
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                TutorialManager.Instance.ShowTutorial(TutorialType.MAIN_GET_NEW_VEHICLE);
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                TutorialManager.Instance.BroadcastTutorialAction($"{TutorialType.MAIN_GET_NEW_VEHICLE.ToString()}_COMPLETE");
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                TutorialManager.Instance.ShowTutorial(TutorialType.MAIN_GET_HEART);
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                TutorialManager.Instance.ShowTutorial(TutorialType.NEWBIE_SEEDING);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TutorialManager.Instance.RestoreTriggeredTutorials();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                TutorialManager.Instance.CancelAllTutorials();
                SaveLoadSystem.Reset();
            }
        }
    }
}