//using UnityEngine;
//using DG.Tweening;

//public abstract class UIGen : MonoBehaviour, IWorldUI
//{
//    protected RectTransform tr;
//    protected Transform cam;
//    protected CanvasGroup m_CanvasGroup;
//    protected Vector3 m_InitScale;
//    protected float m_MinScale;

//    [SerializeField] protected bool m_IsScailable = false;
//    [SerializeField] protected float m_ToggleDuration = 0.25F;


//    //////////////////////////////////////////
//    // Initialize & Reset
//    //////////////////////////////////////////

//    protected virtual void Awake()
//    {
//        tr = GetComponent<RectTransform>();
//        cam = Camera.main.transform;
//        m_CanvasGroup = GetComponent<CanvasGroup>();
//        if (!m_CanvasGroup)
//            m_CanvasGroup = this.gameObject.AddComponent<CanvasGroup>();

//        m_InitScale = tr.localScale;
//        m_MinScale = m_IsScailable ? 0.45F : 1.0F;

//        // 카메라 빌보드 효과를 위한 X축 회전
//        tr.Rotate(CameraManager.CamInitRotation);
//    }

//    protected virtual void OnEnable()
//    {
//        if (m_IsScailable && !UIManager.WorldUIs.Contains(this))
//            UIManager.WorldUIs.Add(this);
//    }

//    protected virtual void OnDisable()
//    {
//        if (UIManager.WorldUIs.Contains(this))
//            UIManager.WorldUIs.Remove(this);
//    }

//    public virtual void Initialize(Vector3 initPos, bool showNow = true)
//    {
//        tr.position = initPos;
//        m_CanvasGroup.alpha = 0.0F;
//        if (showNow)
//        {
//            float duration = 1.0F;
//            m_CanvasGroup
//                .DOFade(1.0F, duration)
//                .SetEase(Ease.InOutSine)
//                .Play();
//        }
//    }

//    public virtual void Initialize(Vector3 initPos, BigNumber num) { }

//    protected virtual void OnDestroy()
//    {
//        if (UIManager.WorldUIs.Contains(this))
//            UIManager.WorldUIs.Remove(this);
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    public virtual void ButtonClicked()
//    {

//    }

//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public virtual void SquashAndStretch()
//    {

//    }

//    public virtual void Toggle(bool isOn)
//    {
//        if (gameObject.activeSelf == isOn)
//            return;

//        float startVal = isOn ? 0.0F : 1.0F;
//        float endVal = 1.0F - startVal;

//        gameObject.SetActive(true);

//        m_CanvasGroup.alpha = startVal;
//        m_CanvasGroup.interactable = false;

//        m_CanvasGroup
//            .DOFade(endVal, m_ToggleDuration)
//            .SetEase(Ease.OutQuad)
//            .OnComplete(DoComplete)
//            .Play();


//        void DoComplete()
//        {
//            m_CanvasGroup.alpha = endVal;
//            m_CanvasGroup.interactable = true;
//            gameObject.SetActive(isOn);
//        }
//    }

//    public virtual void UpdateScaleAndRotation()
//    {
//        //카메라 줌에 따른 스케일 조절
//        if (m_IsScailable)
//            tr.localScale = m_InitScale * Mathf.Clamp(CameraHandler.ZoomScaler, m_MinScale, 1.0F);

//        //빌보드 효과
//        //tr.LookAt(cam.transform);

//        //Vector3 viewDir = tr.position - cam.position;
//        //Vector3 backward = -tr.forward;
//        //viewDir.y = backward.y = 0.0F;
//        //tr.Rotate(Vector3.Cross(backward, -viewDir));
//    }
//}
