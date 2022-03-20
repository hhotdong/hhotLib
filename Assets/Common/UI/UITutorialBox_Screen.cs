//using System;
//using UnityEngine;
//using TMPro;
//using I2.Loc;
//using DG.Tweening;
//using deVoid.Utils;

//public enum TutorialBoxScreen { GET_RARE_ANIMAL_RANDOM_BOX, GET_UTILITY_POINT }

//public class ShowTutorialBoxScreenSignal : ASignal<int, string, Action> { }

//public class UITutorialBox_Screen : MonoBehaviour
//{
//    private Sequence seq;

//    [SerializeField] private TutorialBoxScreen m_TutorialBoxType;
//    [SerializeField] private CanvasGroup cg;
//    [SerializeField] private TextMeshProUGUI m_TutorialText;
//    [SerializeField] private Localize m_TutorialTextLocTerm;
//    [SerializeField] private RectTransform m_TutorialTextRt;
//    [SerializeField] private RectTransform m_TutorialTextBgRt;


//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    private void Awake()
//    {
//        Signals.Get<ShowTutorialBoxScreenSignal>().AddListener(OnShowTutorialBoxScreen);
//        cg.alpha = 0.0F;
//        this.gameObject.SetActive(false);
//    }

//    private void OnDestroy()
//    {
//        Signals.Get<ShowTutorialBoxScreenSignal>().RemoveListener(OnShowTutorialBoxScreen);
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void OnShowTutorialBoxScreen(int typeIdx, string locTerm, Action callback)
//    {
//        TutorialBoxScreen type = (TutorialBoxScreen)typeIdx;
//        if(m_TutorialBoxType == type)
//        {
//            const float FADE_DURATION = 0.75F;
//            const float WAIT_DURATION = 3.0F;
//            const float TUTORIAL_BOX_PADDING_W = 75.0F;
//            const float TUTORIAL_BOX_PADDING_H = 35.0F;

//            if (seq != null && seq.IsPlaying())
//                seq.Kill();

//            seq = DOTween.Sequence()
//                    .Append(cg.DOFade(1.0F, FADE_DURATION).SetEase(Ease.InOutSine))
//                    .AppendInterval(WAIT_DURATION)
//                    .Append(cg.DOFade(0.0F, FADE_DURATION).SetEase(Ease.InOutSine))
//                    .OnStart(DoStart)
//                    .OnComplete(DoComplete)
//                    .Play();


//            void DoStart()
//            {
//                if (!this.gameObject.activeSelf)
//                    this.gameObject.SetActive(true);

//                m_TutorialTextLocTerm.SetTerm(locTerm);
//                m_TutorialTextRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_TutorialText.preferredWidth);
//                m_TutorialTextRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_TutorialText.preferredHeight);
//                m_TutorialTextBgRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_TutorialText.preferredWidth + TUTORIAL_BOX_PADDING_W);
//                m_TutorialTextBgRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_TutorialText.preferredHeight + TUTORIAL_BOX_PADDING_H);
//            }

//            void DoComplete()
//            {
//                callback?.Invoke();
//                seq = null;
//                this.gameObject.SetActive(false);
//            }
//        }
//    }
//}
