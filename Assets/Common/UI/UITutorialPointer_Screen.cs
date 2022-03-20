//using UnityEngine;
//using DG.Tweening;

//public class UITutorialPointer_Screen : UITutorialPointer
//{
//    private bool m_ToggleState;
//    private bool m_IsTransitioning;

//    [SerializeField] private RectTransform m_ContentRt;
//    [SerializeField] private ParticleSystem ps;
//    [SerializeField] private CanvasGroup cg;

//    private static readonly string TWEEN_ID_TUTORIAL_POINTER_SCREEN = "TWEEN_ID_TUTORIAL_POINTER_SCREEN";


//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    protected override void Start()
//    {
//        this.gameObject.SetActive(false);
//    }

//    private void OnDisable()
//    {
//        if (DOTween.IsTweening(cg))
//            cg.DOKill();

//        if (DOTween.IsTweening(TWEEN_ID_TUTORIAL_POINTER_SCREEN))
//            DOTween.Kill(TWEEN_ID_TUTORIAL_POINTER_SCREEN);
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    protected override void OnFadeTutorialObjects(bool fade)
//    {
//        if (!gameObject.activeSelf || !m_ToggleState || m_IsTransitioning)
//            return;

//        float endValue = fade ? 0.0F : 1.0F;

//        if (DOTween.IsTweening(cg))
//            cg.DOKill();

//        cg.DOFade(endValue, 1.0F).Play();
//    }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    protected override void TogglePointer(bool toggle, Vector3 worldPoint)
//    {
//        if (m_ToggleState == toggle)
//        {
//            if (toggle)
//                UpdatePosition();

//            return;
//        }

//        m_ToggleState = toggle;
//        m_IsTransitioning = true;

//        if (DOTween.IsTweening(TWEEN_ID_TUTORIAL_POINTER_SCREEN))
//            DOTween.Kill(TWEEN_ID_TUTORIAL_POINTER_SCREEN);

//        var psCol = ps.main;
//        var tempCol = psCol.startColor.color;
//        float endVal = toggle ? 1.0F : 0.0F;

//        DOTween.ToAlpha(() => tempCol, x => tempCol = x, endVal, 0.5F)
//            .SetEase(Ease.InOutSine)
//            .OnStart(DoStart)
//            .OnUpdate(DoUpdate)
//            .OnComplete(DoComplete)
//            .SetId(TWEEN_ID_TUTORIAL_POINTER_SCREEN)
//            .Play();


//        void UpdatePosition()
//        {
//            Vector3 screenPoint = CameraManager.UICam.WorldToScreenPoint(worldPoint);

//            var parentCanvas = GetComponentInParent<Canvas>();
//            if (!parentCanvas)
//                return;

//            var parentRt = parentCanvas.GetComponent<RectTransform>();
//            if (!parentRt)
//                return;

//            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRt, screenPoint, CameraManager.UICam, out Vector2 localPoint);

//            m_ContentRt.anchorMin = Vector2.zero;
//            m_ContentRt.anchorMax = Vector3.one;

//            m_ContentRt.localPosition = localPoint;
//        }

//        void DoStart()
//        {
//            if (toggle)
//            {
//                this.gameObject.SetActive(true);
//                UpdatePosition();
//            }
//        }

//        void DoUpdate()
//        {
//            psCol.startColor = tempCol;
//        }

//        void DoComplete()
//        {
//            if (!toggle)
//                this.gameObject.SetActive(false);
//            m_IsTransitioning = false;
//        }
//    }
//}
