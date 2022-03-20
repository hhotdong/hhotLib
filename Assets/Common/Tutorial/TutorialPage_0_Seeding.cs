//namespace hhotLib
//{
//    using UnityEngine;

//    public class TutorialPage_0_Seeding : TutorialPage
//    {
//        protected override void OnBegin()
//        {
//            base.OnBegin();

//            // 튜토리얼 텍스트 활성화
//            //Signals.Get<ToggleTutorialPanelSignal>().Dispatch(true, new TutorialPanelProperty(MoveNext, true));
//        }

//        protected override void OnEnd()
//        {
//            base.OnEnd();

//            if (tutorialType == TutorialType.Newbie && moveNextPage)
//            {
//                TutorialManager.Instance.NewbieTutorialPhase = nextTutorialPhase;
//            }

//            // 튜토리얼 텍스트 비활성화
//            //Signals.Get<ToggleTutorialPanelSignal>().Dispatch(false, null);
//        }

//        private bool m_IsClicked = false;
//        private void Update()
//        {
//            if (!m_IsClicked && Input.GetMouseButtonDown(0))
//            {
//                m_IsClicked = true;

//                SoundManager.PlaySoundEffect(SoundType.BUTTON_DEFAULT);
//                SoundManager.PlaySoundEffectDelayed(SoundType.MUSICAL_LANDMARK_UPGRADE, 1.0F, false, 1.0F);

//                // 첫 번째 랜드마크 언락 이벤트 호출
//                if (_TutorialEventTracker.firstLandmarkData.IsLocked && _TutorialEventTracker.firstLandmarkData.UpgradeLevel == 0)
//                {
//                    _TutorialEventTracker.onUpgradeLandmarkLevel.Raise((int)LandmarkType.LANDMARK_0);
//                    TutorialManager.Instance.BroadcastTutorialAction(awaitingAction);
//                }
//            }
//        }
//    }
//}