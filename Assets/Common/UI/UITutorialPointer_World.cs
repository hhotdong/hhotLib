//using UnityEngine;
//using DG.Tweening;
//using deVoid.Utils;

//public class UITutorialPointer_World : UITutorialPointer, IWorldUI
//{
//    private Vector3 m_InitScale;
//    private bool m_ToggleState;
//    private bool m_IsTransitioning;
//    private Transform effectTr;
//    private float m_MinScale;

//    [SerializeField] private bool m_IsScailable = false;
//    [SerializeField] private Transform m_PointerTr;
//    [SerializeField] private Transform m_ContentTr;
//    [SerializeField] private ParticleSystem ps;

//    private static readonly string TWEEN_ID_TUTORIAL_POINTER_WORLD = "TWEEN_ID_TUTORIAL_POINTER_WORLD";


//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    protected override void Start()
//    {
//        m_InitScale = m_ContentTr.localScale;
//        m_MinScale = m_IsScailable ? 0.2F : 1.0F;
//        effectTr = ps.transform;
//        this.gameObject.SetActive(false);
//    }

//    private void OnEnable()
//    {
//        Signals.Get<ToggleWorldUIVisible>().AddListener(OnToggleWorldUIVisible);

//        if (!UIManager.WorldUIs.Contains(this))
//            UIManager.WorldUIs.Add(this);
//    }

//    private void OnDisable()
//    {
//        Signals.Get<ToggleWorldUIVisible>().RemoveListener(OnToggleWorldUIVisible);

//        if (UIManager.WorldUIs.Contains(this))
//            UIManager.WorldUIs.Remove(this);

//        if (DOTween.IsTweening(TWEEN_ID_TUTORIAL_POINTER_WORLD))
//            DOTween.Kill(TWEEN_ID_TUTORIAL_POINTER_WORLD);
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    public virtual void OnToggleWorldUIVisible(bool isOn, bool forceNow)
//    {
//        if (m_IsTransitioning)
//            return;

//        float duration = forceNow ? 0.01F : 0.5F;

//        if (DOTween.IsTweening(TWEEN_ID_TUTORIAL_POINTER_WORLD))
//            DOTween.Kill(TWEEN_ID_TUTORIAL_POINTER_WORLD);

//        GetFadeTweener(isOn, duration).Play();
//    }

//    //protected override void OnFadeTutorialObjects(bool fade)
//    //{
//    //    if (!gameObject.activeSelf || !m_ToggleState || m_IsTransitioning)
//    //        return;

//    //    if (DOTween.IsTweening(TWEEN_ID_TUTORIAL_POINTER_WORLD))
//    //        DOTween.Kill(TWEEN_ID_TUTORIAL_POINTER_WORLD);

//    //    GetFadeTweener(fade).Play();
//    //}


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    private Tweener GetFadeTweener(bool toggle, float duration = 0.5F)
//    {
//        var psCol = ps.main;
//        var tempCol = psCol.startColor.color;
//        float endVal = toggle ? 1.0F : 0.0F;

//        return DOTween.ToAlpha(() => tempCol, x => tempCol = x, endVal, duration)
//            .SetEase(Ease.InOutSine)
//            .SetId(TWEEN_ID_TUTORIAL_POINTER_WORLD);
//    }

//    protected override void TogglePointer(bool toggle, Vector3 worldPoint)
//    {
//        if (m_ToggleState == toggle)
//        {
//            if (toggle && !worldPoint.Equals(m_PointerTr.position))
//                m_PointerTr.position = worldPoint;
//            return;
//        }

//        m_ToggleState = toggle;
//        m_IsTransitioning = true;

//        if (DOTween.IsTweening(TWEEN_ID_TUTORIAL_POINTER_WORLD))
//            DOTween.Kill(TWEEN_ID_TUTORIAL_POINTER_WORLD);

//        var psCol = ps.main;
//        var tempCol = psCol.startColor.color;
//        float endVal = toggle ? 1.0F : 0.0F;

//        Tweener tw = GetFadeTweener(toggle)
//                        .OnStart(DoStart)
//                        .OnUpdate(DoUpdate)
//                        .OnComplete(DoComplete)
//                        .Play();


//        void DoStart()
//        {
//            if (toggle)
//            {
//                this.gameObject.SetActive(true);
//                m_PointerTr.position = worldPoint;
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

//    public void UpdateScaleAndRotation()
//    {
//        effectTr.localScale = m_InitScale * Mathf.Clamp(CameraHandler.ZoomScaler, m_MinScale, 1.0F);

//        // 빌보드 효과
//        m_PointerTr.rotation = UIManager.WORLD_UI_BILLBOARD_ROTATION;
//    }

//    public void SquashAndStretch()
//    {
//        throw new System.NotImplementedException();
//    }

//    public void ButtonClicked()
//    {
//        throw new System.NotImplementedException();
//    }
//}
