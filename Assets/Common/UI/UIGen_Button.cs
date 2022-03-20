//using UnityEngine;
//using UnityEngine.UI;
//using deVoid.Utils;
//using DG.Tweening;

//public class UIGen_Button : UIGen
//{
//    protected Button m_Button;
//    protected Tween m_ButtonTween;
//    protected Vector3 m_ContentTrInitScale;
//    [SerializeField] protected Transform m_ContentTr;
//    [SerializeField] protected Image m_ContentImage;


//    //////////////////////////////////////////
//    // Initialize & Reset
//    //////////////////////////////////////////

//    protected override void Awake()
//    {
//        base.Awake();
//        m_Button = GetComponent<Button>();
//        m_ContentTrInitScale = m_ContentTr.localScale;

//        m_Button.onClick.AddListener(ButtonClicked);
//        Signals.Get<ChangeTouchContextSignal>().AddListener(OnChangeTouchContext);
//    }

//    protected override void OnEnable()
//    {
//        m_Button.interactable = TouchManager.CurrentTouchContext == TouchContext.CAM_HANDLING ? true : false;

//        m_ContentTr.localScale = Vector3.zero;

//        if (m_ButtonTween == null)
//        {
//            m_ButtonTween = UIManager.GetSquashAndStretchSequence(m_ContentTr, m_ContentTrInitScale.x)
//                            .OnComplete(FloatingInTheAir)
//                            .SetAutoKill(false)
//                            .SetRecyclable(true)
//                            .Play();
//        }
//        else
//            m_ButtonTween.Restart();
//    }

//    protected override void OnDisable()
//    {
//        base.OnDisable();
//        m_ButtonTween?.Pause();
//    }

//    public virtual void Initialize(Sprite image, Vector3 pos)
//    {
//        if (image != null)
//        {
//            m_ContentImage.sprite = image;
//            m_ContentImage.SetNativeSize();
//        }

//        base.Initialize(pos, true);
//    }

//    public virtual void UpdateContentsAndPosition(Sprite image, Vector3 pos)
//    {
//        if (image != null)
//        {
//            m_ContentImage.sprite = image;
//            m_ContentImage.SetNativeSize();
//        }

//        m_ContentTr.localScale = Vector3.zero;

//        if (m_ButtonTween == null)
//        {
//            m_ButtonTween = UIManager.GetSquashAndStretchSequence(m_ContentTr, m_ContentTrInitScale.x)
//                            .OnComplete(FloatingInTheAir)
//                            .SetAutoKill(false)
//                            .SetRecyclable(true)
//                            .Play();
//        }
//        else
//            m_ButtonTween?.Restart();

//        tr.position = pos;
//    }

//    protected void FloatingInTheAir()
//    {
//        m_ContentTr.DOLocalMoveY(0.1F, 5.0F)
//                .SetRelative(true)
//                .SetLoops(-1, LoopType.Yoyo)
//                .SetEase(Ease.InOutQuad);
//    }

//    protected override void OnDestroy()
//    {
//        base.OnDestroy();
//        Signals.Get<ChangeTouchContextSignal>().RemoveListener(OnChangeTouchContext);
//        m_Button.onClick.RemoveListener(ButtonClicked);
//        m_Button = null;
//        m_ButtonTween = null;
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void OnChangeTouchContext(TouchContext context)
//    {
//        m_Button.interactable = context == TouchContext.CAM_HANDLING;
//    }

//    public override void ButtonClicked()
//    {
//        SoundManager.PlaySoundEffect(SoundType.BUTTON_DEFAULT);
//        m_Button.interactable = false;
//        m_CanvasGroup.alpha = 0.0F;
//    }

//    public virtual void RefreshButton()
//    {
//        m_Button.interactable = TouchManager.CurrentTouchContext == TouchContext.CAM_HANDLING ? true : false;
//        m_CanvasGroup.alpha = 1.0F;
//    }
//}
