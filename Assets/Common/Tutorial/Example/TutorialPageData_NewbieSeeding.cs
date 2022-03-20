using UnityEngine;

namespace hhotLib.Tutorial
{
    [CreateAssetMenu(fileName = "TutorialPage_NewbieSeeding", menuName ="TutorialPage/Newbie/Seeding")]
    public class TutorialPageData_NewbieSeeding : TutorialPageData
    {
        public override void OnBegin()
        {
            base.OnBegin();
            TutorialTester.OnFinishFirstTutorial += OnComplete;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            TutorialTester.OnFinishFirstTutorial -= OnComplete;
        }

        public override void OnCancel()
        {
            base.OnCancel();
            TutorialTester.OnFinishFirstTutorial -= OnComplete;
        }

        private void OnComplete()
        {
            TutorialTester.OnFinishFirstTutorial -= OnComplete;
            TutorialManager.Instance.BroadcastTutorialAction($"{tutorialType.ToString()}_COMPLETE");
        }
    }
}