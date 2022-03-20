//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;
//using TMPro;

//public enum ToggleButtonState
//{
//    NONE                = -1,
//    LOCKED              = 0,
//    NOT_CLICKABLE       = 1,
//    CLICKABLE_ENABLED   = 2,
//    CLICKABLE_DISABLED  = 3,
//    COMPLETE            = 4,
//    FREE                = 5,
//    TIMER               = 6,
//    FREE_AD             = 7,
//}

//[RequireComponent(typeof(Button)), DisallowMultipleComponent]
//public class UIToggleButton : MonoBehaviour
//{
//    protected ToggleButtonState m_CurrentButtonState = ToggleButtonState.NONE;
//    protected Transform tr;
//    protected Vector3 m_InitScale;

//    [SerializeField] protected Button m_ToggleButton;
//    [SerializeField] protected GameObject m_ToggleOnImg;
//    [SerializeField] protected GameObject m_ToggleOffImg;

//    [Header("Toggle Button Infos")]
//    [SerializeField] protected Image m_CoinImg;
//    [SerializeField] protected TextMeshProUGUI m_CostText;

//    // Events
//    public virtual event Action<int> ButtonClicked;


//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    protected virtual void Awake()
//    {
//        tr = GetComponent<Transform>();
//        m_InitScale = tr.localScale;
//    }

//    protected virtual void OnEnable() { /*HelperManager.AlignWithSameDistanceFromCenter(m_CostText, m_CoinImg);*/ }
//    protected virtual void OnDisable() { }

//    private void Start()
//    {
//        m_ToggleButton.onClick.AddListener(OnClickButton);
//        OnStart();
//    }

//    protected virtual void OnDestroy() { m_ToggleButton.onClick.RemoveListener(OnClickButton); }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////
    
//    protected virtual void OnStart() { }

//    protected virtual void OnClickButton()
//    {
//        SoundManager.PlaySoundEffect(SoundType.BUTTON_DEFAULT);

//        if (DOTween.IsTweening(tr))
//            tr.DOKill();

//        tr.localScale = m_InitScale;
//        tr.DOPunchScale(Vector3.one * 0.2F, 0.25F, 7, 1).SetRecyclable(true).Play();
//        m_CurrentButtonState = ToggleButtonState.NONE;
//    }

//    protected virtual void OnToggle() { }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public virtual void Toggle(ToggleButtonState state)
//    {
//        if ((state <= ToggleButtonState.NONE && state > ToggleButtonState.CLICKABLE_DISABLED) || m_CurrentButtonState == state)
//            return;

//        m_CurrentButtonState = state;

//        switch (state)
//        {
//            case ToggleButtonState.LOCKED:
//                m_ToggleButton.interactable = false;
//                m_ToggleOnImg.SetActive(false);
//                m_ToggleOffImg.SetActive(false);
//                ToggleByLockState(true);
//                break;

//            case ToggleButtonState.NOT_CLICKABLE:
//                m_ToggleButton.interactable = false;
//                ToggleButtonImgs(false);
//                ToggleByLockState(false);
//                break;

//            case ToggleButtonState.CLICKABLE_ENABLED:
//                m_ToggleButton.interactable = true;
//                ToggleButtonImgs(true);
//                ToggleByLockState(false);
//                break;

//            case ToggleButtonState.CLICKABLE_DISABLED:
//                m_ToggleButton.interactable = true;
//                ToggleButtonImgs(false);
//                ToggleByLockState(false);
//                break;

//            default:
//                Debug.Log("UpgradeButtonState is not appropriate!");
//                return;
//        }

//        OnToggle();
//    }

//    public virtual void Toggle(ToggleButtonState state, ToggleButtonColor btnCol) { }

//    protected virtual void ToggleButtonImgs(bool isOn)
//    {
//        m_ToggleOnImg.SetActive(isOn);
//        m_ToggleOffImg.SetActive(!isOn);
//    }

//    protected virtual void ToggleByLockState(bool isLocked)
//    {
//        m_CoinImg.gameObject.SetActive(!isLocked);
//        m_CostText.gameObject.SetActive(!isLocked);
//    }

//    public void UpdateCost(string cost)
//    {
//        if (!string.IsNullOrEmpty(cost))
//        {
//            m_CostText.text = cost;
//            //HelperManager.AlignWithSameDistanceFromCenter(m_CostText, m_CoinImg);
//        }
//    }

//    public bool GetToggleButtonEventState()
//    {
//        switch(m_CurrentButtonState)
//        {
//            case ToggleButtonState.CLICKABLE_DISABLED: return false;
//            case ToggleButtonState.CLICKABLE_ENABLED: return true;
//            default: return false;
//        }
//    }
//}