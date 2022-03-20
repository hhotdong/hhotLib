//using System;
//using UnityEngine;
//using I2.Loc;
//using TMPro;
//using DG.Tweening;
//using deVoid.Utils;

//public class UITutorialBox_World : MonoBehaviour, IWorldUI
//{
//    private Sequence seq;
//    private Vector3 m_InitScale;
//    private float m_MinScale;

//    [SerializeField] private Transform tr;
//    [SerializeField] private CanvasGroup cg;
//    [SerializeField] private TextMeshProUGUI m_TutorialText;
//    [SerializeField] private Localize m_TutorialTextLocTerm;
//    [SerializeField] private RectTransform m_m_TutorialTextRt;
//    [SerializeField] private RectTransform m_m_TutorialTextBgRt;


//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    private void Awake()
//    {
//        m_InitScale = tr.localScale;
//        cg.alpha = 0.0F;
//        m_MinScale = 0.45F;
//    }

//    private void OnEnable()
//    {
//        Signals.Get<ToggleWorldUIVisible>().AddListener(OnToggleWorldUIVisible);
//    }

//    private void OnDisable()
//    {
//        Signals.Get<ToggleWorldUIVisible>().RemoveListener(OnToggleWorldUIVisible);
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void OnToggleWorldUIVisible(bool isOn, bool forceNow)
//    {
//        // WorldUIVisible 비활성화 시그널에만 반응하도록 예외 처리
//        if (isOn)
//            return;

//        const float FADE_DURATION = 0.75F;

//        if (seq != null && seq.IsPlaying())
//            seq.Kill();

//        if (DOTween.IsTweening(cg))
//            cg.DOKill();

//        cg.DOFade(0.0F, FADE_DURATION).OnComplete(DoComplete).SetEase(Ease.InOutSine).Play();

//        void DoComplete()
//        {
//            seq = null;
//            UIObjectPoolManager.Free(this.gameObject);
//        }
//    }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public void SetText(string locTerm, Vector3 worldPoint, Action callback)
//    {
//        const float FADE_DURATION = 1.0F;
//        const float WAIT_DURATION = 3.0F;
//        const float TUTORIAL_BOX_PADDING = 0.3F;

//        if (seq != null && seq.IsPlaying())
//            seq.Kill();

//        seq = DOTween.Sequence()
//                .Append(cg.DOFade(1.0F, FADE_DURATION))
//                .AppendInterval(WAIT_DURATION)
//                .Append(cg.DOFade(0.0F, FADE_DURATION))
//                .SetUpdate(UpdateType.Late)
//                .OnStart(DoStart)
//                .OnComplete(DoComplete)
//                .OnUpdate(DoUpdate)
//                .Play();


//        void DoStart()
//        {
//            m_TutorialTextLocTerm.SetTerm(locTerm);
//            m_m_TutorialTextRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_TutorialText.preferredWidth);
//            m_m_TutorialTextRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_TutorialText.preferredHeight);
//            m_m_TutorialTextBgRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_TutorialText.preferredWidth + TUTORIAL_BOX_PADDING);
//            m_m_TutorialTextBgRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_TutorialText.preferredHeight + TUTORIAL_BOX_PADDING);

//            tr.position = worldPoint;
//        }

//        void DoUpdate()
//        {
//            UpdateScaleAndRotation();
//        }

//        void DoComplete()
//        {
//            callback?.Invoke();
//            seq = null;
//            UIObjectPoolManager.Free(this.gameObject);
//        }
//    }

//    public void UpdateScaleAndRotation()
//    {
//        // 카메라 줌에 따른 스케일 조정
//        tr.localScale = m_InitScale * Mathf.Clamp(CameraHandler.ZoomScaler, m_MinScale, 1.0F);

//        // 빌보드 효과
//        tr.rotation = UIManager.WORLD_UI_BILLBOARD_ROTATION;
//    }

//    public void SquashAndStretch()
//    {
//        throw new NotImplementedException();
//    }

//    public void ButtonClicked()
//    {
//        throw new NotImplementedException();
//    }
//}
