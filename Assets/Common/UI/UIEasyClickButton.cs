//using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;
//using deVoid.Utils;
//using TMPro;
//using I2.Loc;
//using ScriptableObjectArchitecture;

//public enum EasyClickButtonState
//{
//    NONE = -1,
//    UNLOCKABLE,
//    LEVEL_UPGRADABLE
//}

//public class ClickMilestoneButtonSignal : ASignal { }

//public class UIEasyClickButton : MonoBehaviour
//{
//    private bool m_IsInitialized = false;
//    private bool m_IsToggledOn = false;
//    private EasyClickButtonState m_ButtonState = EasyClickButtonState.NONE;
//    private LandmarkData m_SelectedData;

//    private Image m_ToggleButtonImg;
//    private CanvasGroup m_DisplayCG;
//    private CanvasGroup m_ToggleOnPanelCG;
//    private CanvasGroup m_ToggleOffPanelCG;
//    [SerializeField] private RectTransform m_ToggleOnPanel;
//    [SerializeField] private RectTransform m_ToggleOffPanel;
//    [SerializeField] private RectTransform m_NotificationsPanel;
//    [SerializeField] private RectTransform m_Display;
//    [SerializeField] private Image m_SelectedLandmarkImg;
//    [SerializeField] private RectTransform m_ToggleButtonTr;
//    [SerializeField] private TextMeshProUGUI m_LandmarkDesc0, m_LandmarkDesc1, m_LandmarkDesc2;
//    [SerializeField] private LocalizationParamsManager m_LandmarkLevel;
//    [SerializeField] private TextMeshProUGUI m_ButtonDesc0, m_ButtonDesc1;
//    [SerializeField] private TextMeshProUGUI m_Cost;
//    [SerializeField] private Image m_Coin, m_BenefitCoin_Heart;
//    [SerializeField] private UIButtonPressed m_UpgradeButton;

//    private float[] m_ToggleOnWidth = new float[2];
//    private float[] m_ToggleOffWidth = new float[2];
//    private float[] m_NotificationPanelWidth = new float[2] { 700.0F, 207.0F };
//    private float m_DisplayToggleOnPos_X;
//    private Vector3 m_InitScale;
//    [SerializeField] private float m_Duration_Panel;
//    [SerializeField] private float m_Duration_Display;
//    [SerializeField] private Ease m_Ease_Panel;
//    [SerializeField] private Ease m_Ease_Display;
//    private const float COIN_TEXT_OFFSET = 4.0F;
//    private readonly float[] BENEFIT_DESC_OFFSET = new float[2] { -10.0F, 10.0F };

//    [SerializeField] private TextMeshProUGUI m_ShowOrderReachedToMax;
//    [SerializeField] private RectTransform m_NextLookChangeProgressbarBg;
//    [SerializeField] private RectTransform m_NextLookChangeProgressbar;
//    private static float PROGRESS_MAX;

//    private static int OneButtonClickCount = 0;
//    private static readonly string TWEEN_ID_TOGGLE_EASY_BUTTON = "TOGGLE_EASY_BUTTON";


//    [Header("SO Events")]
//    [SerializeField] private IntGameEvent _OnUpgradeLandmarkLevel;

//    [Header("SO Managers")]
//    [SerializeField] private LandmarkManager _LandmarkManager;
//    [SerializeField] private AssetReferenceManager _AssetReferenceManager;


//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    private void Awake()
//    {
//        m_ToggleOnWidth[0] = m_ToggleOnPanel.sizeDelta.x;
//        m_ToggleOnWidth[1] = m_ToggleOffPanel.sizeDelta.x;
//        float toggleOffSize = m_ToggleOffPanel.GetComponent<Image>().mainTexture.width;
//        m_ToggleOffWidth[0]
//            = m_ToggleOffWidth[1]
//            = toggleOffSize;
//        m_DisplayToggleOnPos_X = m_Display.localPosition.x;
//        m_InitScale = m_ToggleButtonTr.localScale;
//        PROGRESS_MAX = m_NextLookChangeProgressbarBg.sizeDelta.x;

//        m_ToggleButtonImg = m_ToggleButtonTr.GetComponent<Image>();

//        if (!m_DisplayCG)
//        {
//            CanvasGroup cg = m_Display.GetComponent<CanvasGroup>();
//            if (!cg)
//                m_DisplayCG = m_Display.gameObject.AddComponent<CanvasGroup>();
//            else
//                m_DisplayCG = cg;
//        }

//        if (!m_ToggleOnPanelCG)
//        {
//            CanvasGroup cg = m_ToggleOnPanel.GetComponent<CanvasGroup>();
//            if (!cg)
//                m_ToggleOnPanelCG = m_ToggleOnPanel.gameObject.AddComponent<CanvasGroup>();
//            else
//                m_ToggleOnPanelCG = cg;
//        }

//        if (!m_ToggleOffPanelCG)
//        {
//            CanvasGroup cg = m_ToggleOffPanel.GetComponent<CanvasGroup>();
//            if (!cg)
//                m_ToggleOffPanelCG = m_ToggleOffPanel.gameObject.AddComponent<CanvasGroup>();
//            else
//                m_ToggleOffPanelCG = cg;
//        }

//        OnUpdateEasyButton();

//        Signals.Get<UpdateEasyButtonSignal>().AddListener(OnUpdateEasyButton);
//        m_UpgradeButton.onClick.AddListener(OnClickUpgradeButton);

//        OneButtonClickCount = EncryptedPlayerPrefs.GetInt(PlayerPrefsKeys.ONE_BUTTON_CLICK_COUNT, 0);
//    }

//    private void OnDestroy()
//    {
//        EncryptedPlayerPrefs.SetInt(PlayerPrefsKeys.ONE_BUTTON_CLICK_COUNT, OneButtonClickCount);
//        Signals.Get<UpdateEasyButtonSignal>().RemoveListener(OnUpdateEasyButton);
//        m_UpgradeButton.onClick.RemoveListener(OnClickUpgradeButton);
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void OnUpdateEasyButton()
//    {
//        _LandmarkManager.GetEasyButtonItem(out m_SelectedData, out m_ButtonState, out bool isUpgradable);
//        SetButtonUI(isUpgradable);
//        ToggleButton(isUpgradable);

//        Signals.Get<UpdateNotificationSignal>().Dispatch((int)NotificationType.ANIMAL_NORMAL_LEVELUP);
//        Signals.Get<UpdateNotificationSignal>().Dispatch((int)NotificationType.ZENMODE);
//        Signals.Get<UpdateNotificationSignal>().Dispatch((int)NotificationType.CAMERA);
//        Signals.Get<UpdateNotificationSignal>().Dispatch((int)NotificationType.LANDMARK_LEVELUP);
//        Signals.Get<UpdateNotificationSignal>().Dispatch((int)NotificationType.GROWTH_LEVELUP);


//        void SetButtonUI(bool isToggleOn)
//        {
//            if (isToggleOn)
//            {
//                if (!m_SelectedData)
//                    return;

//                bool isLookChange = false;
//                switch (m_ButtonState)
//                {
//                    case EasyClickButtonState.UNLOCKABLE:
//                        m_Cost.text = IncrementHelper.GetFormattedNumber(m_SelectedData.FinalUpgradeCost, false);
//                        m_Coin.sprite = _AssetReferenceManager.m_GoldImg;
//                        m_Coin.SetNativeSize();
//                        break;

//                    case EasyClickButtonState.LEVEL_UPGRADABLE:
//                        isLookChange = m_SelectedData.IsReadyToChangeLook;
//                        m_Cost.text = IncrementHelper.GetFormattedNumber(m_SelectedData.FinalUpgradeCost, false);
//                        m_Coin.sprite = _AssetReferenceManager.m_GoldImg;
//                        m_Coin.SetNativeSize();
//                        break;

//                    case EasyClickButtonState.NONE:
//                    default:
//                        Debug.Log("EasyClickButtonState is not appropriate!");
//                        return;
//                }

//                m_Cost.gameObject.SetActive(!isLookChange);
//                m_Coin.gameObject.SetActive(!isLookChange);
//                m_ButtonDesc0.gameObject.SetActive(!isLookChange);
//                m_ButtonDesc1.gameObject.SetActive(isLookChange);
//                m_BenefitCoin_Heart.gameObject.SetActive(isLookChange);
//                if (isLookChange)
//                {
//                    m_LandmarkDesc1.text = LocalizationManager.GetTermTranslation(LocTerm.COMMON_BENEFIT_GOLD_AND_HEART);
//                    m_LandmarkDesc2.text = string.Format(HelperManager.STRING_FORMAT_PERCENTAGE, LandmarkManager.CommonIncreaseRate_MultipliedBy100);
//                    m_LandmarkDesc2.color = HelperManager.BUTTON_COLOR_BLUE;
//                    m_ToggleButtonImg.color = HelperManager.BUTTON_COLOR_BLUE;
//                }
//                else
//                {
//                    m_LandmarkDesc1.text = LocalizationManager.GetTermTranslation(LocTerm.COMMON_BENEFIT_GOLD);
//                    m_LandmarkDesc2.text = string.Format(HelperManager.STRING_FORMAT_PLUS_SIGN, IncrementHelper.GetFormattedNumber(m_SelectedData.FinalGoldOutputUpgradeBenefit, false));
//                    m_LandmarkDesc2.color = HelperManager.BUTTON_COLOR_RED;
//                    m_ToggleButtonImg.color = HelperManager.BUTTON_COLOR_RED;
//                }

//                m_SelectedLandmarkImg.sprite = m_SelectedData.m_DataImage;
//                m_SelectedLandmarkImg.SetNativeSize();
//                m_LandmarkDesc0.text = LocalizationManager.GetTermTranslation(string.Format(HelperManager.STRING_FORMAT_UNIT_B, LocTerm.LANDMARK_NAME, m_SelectedData.DataTypeIdx));
//                m_LandmarkLevel.SetParameterValue(LocTerm.PARAM_LEVEL, m_SelectedData.UpgradeLevel.ToString());
//                UpdateProgress();
//            }
//            else
//            {
//                // 아무런 업그레이드를 할 수 없을 때는 버튼을 비활성화하고 레벨 업그레이드 가격이 가장 낮은 랜드마크의 정보를 표시한다
//                if (m_SelectedData && m_ButtonState == EasyClickButtonState.LEVEL_UPGRADABLE)
//                {
//                    m_SelectedLandmarkImg.sprite = m_SelectedData.m_DataImage;
//                    m_SelectedLandmarkImg.SetNativeSize();
//                    m_LandmarkLevel.SetParameterValue(LocTerm.PARAM_LEVEL, m_SelectedData.UpgradeLevel.ToString());
//                    m_NextLookChangeProgressbarBg.gameObject.SetActive(false);
//                    m_NextLookChangeProgressbar.gameObject.SetActive(false);
//                    m_ShowOrderReachedToMax.gameObject.SetActive(false);
//                }
//                else
//                    Debug.Log($"Upgrade using EasyButton is impossible and button state is {m_ButtonState}!");
//            }
//        }

//        void UpdateProgress()
//        {
//            bool shouldDisplayProgress = m_SelectedData.LookChangeProgress >= 0.0F;
//            if (shouldDisplayProgress)
//            {
//                Vector3 currentProgressSize = m_NextLookChangeProgressbar.sizeDelta;
//                currentProgressSize.x = m_SelectedData.LookChangeProgress * PROGRESS_MAX;
//                m_NextLookChangeProgressbar.sizeDelta = currentProgressSize;
//            }
//            m_NextLookChangeProgressbarBg.gameObject.SetActive(shouldDisplayProgress);
//            m_NextLookChangeProgressbar.gameObject.SetActive(shouldDisplayProgress);
//            m_ShowOrderReachedToMax.gameObject.SetActive(!shouldDisplayProgress);
//        }
//    }

//    private void ToggleButton(bool isOn)
//    {
//        if (m_IsInitialized && isOn == m_IsToggledOn)
//        {
//            Debug.Log($"EasyClickButton is already on that state({isOn})!");
//            return;
//        }
//        m_IsInitialized = true;
//        m_IsToggledOn = isOn;

//        Vector3 parentEndScale = isOn ? Vector3.one : Vector3.one * 0.9F;
//        Vector2 toggleOnPanelEndSize = new Vector2(isOn ? m_ToggleOnWidth[0] : m_ToggleOffWidth[0], m_ToggleOnPanel.sizeDelta.y);
//        Vector2 toggleOffPanelEndSize = new Vector2(isOn ? m_ToggleOnWidth[1] : m_ToggleOffWidth[1], m_ToggleOffPanel.sizeDelta.y);
//        Vector2 notiPanelEndSize = new Vector2(isOn ? m_NotificationPanelWidth[0] : m_NotificationPanelWidth[1], m_NotificationsPanel.sizeDelta.y);
//        float toggleOnPanelEndOpacity = isOn ? 1.0F : 0.0F;
//        float toggleOnDisplayXPos = isOn ? m_DisplayToggleOnPos_X : 0.0F;

//        if (DOTween.IsTweening(TWEEN_ID_TOGGLE_EASY_BUTTON))
//            DOTween.Kill(TWEEN_ID_TOGGLE_EASY_BUTTON);

//        DOTween.Sequence()
//            .Append(m_ToggleOnPanel.DOSizeDelta(toggleOnPanelEndSize, m_Duration_Panel).SetEase(m_Ease_Panel))
//            .Join(m_ToggleOffPanel.DOSizeDelta(toggleOffPanelEndSize, m_Duration_Panel).SetEase(m_Ease_Panel))
//            .Join(m_NotificationsPanel.DOSizeDelta(notiPanelEndSize, m_Duration_Panel).SetEase(m_Ease_Panel))
//            .Join(m_Display.DOLocalMoveX(toggleOnDisplayXPos, m_Duration_Display).SetEase(m_Ease_Display))
//            .Join(m_ToggleOffPanelCG.DOFade(1.0F - toggleOnPanelEndOpacity, m_Duration_Panel).SetEase(m_Ease_Panel))
//            .Join(m_ToggleOnPanelCG.DOFade(toggleOnPanelEndOpacity, m_Duration_Panel).SetEase(m_Ease_Panel))
//            .OnStart(DoStart)
//            .OnComplete(DoComplete)
//            .SetRecyclable(true)
//            .SetId(TWEEN_ID_TOGGLE_EASY_BUTTON)
//            .Play();


//        void DoStart()
//        {
//            m_ToggleOffPanelCG.gameObject.SetActive(true);
//            m_ToggleOnPanelCG.gameObject.SetActive(true);

//            m_ToggleOffPanelCG.interactable
//                = m_ToggleOnPanelCG.interactable
//                = m_UpgradeButton.interactable
//                = false;

//            if (!isOn)
//                m_DisplayCG.alpha = 0.0F;
//        }

//        void DoComplete()
//        {
//            m_ToggleOffPanelCG.interactable = !isOn;
//            m_ToggleOnPanelCG.interactable
//                = m_UpgradeButton.interactable
//                = isOn;

//            m_ToggleOffPanelCG.alpha = 1.0F - toggleOnPanelEndOpacity;
//            m_ToggleOnPanelCG.alpha = toggleOnPanelEndOpacity;

//            m_ToggleOffPanelCG.gameObject.SetActive(!isOn);
//            m_ToggleOnPanelCG.gameObject.SetActive(isOn);

//            if (m_DisplayCG.alpha < 0.1F)
//            {
//                m_DisplayCG.alpha = 1.0F;
//                m_Display.localScale = Vector3.zero;
//                UIManager.GetSquashAndStretchSequence(m_Display).Play();
//            }
//        }
//    }

//    public void OnClickUpgradeButton()
//    {
//        if (DOTween.IsTweening(m_ToggleButtonTr))
//            m_ToggleButtonTr.DOKill();

//        m_ToggleButtonTr.localScale = m_InitScale;
//        m_ToggleButtonTr.DOPunchScale(Vector3.one * 0.2F, 0.25F, 7, 1).SetRecyclable(true);

//        if (!m_SelectedData)
//        {
//            Debug.Log("EasyClickButton is clicked but the current selected data is null!");
//            return;
//        }

//        switch (m_ButtonState)
//        {
//            case EasyClickButtonState.UNLOCKABLE:
//                Signals.Get<ClickMilestoneButtonSignal>().Dispatch();
//                break;

//            case EasyClickButtonState.LEVEL_UPGRADABLE:
//                _OnUpgradeLandmarkLevel.Raise(m_SelectedData.DataTypeIdx);
//                if (!m_UpgradeButton.IsAutoClickPlaying)
//                    m_UpgradeButton.StartAutoClick(CheckAutoClickCondition);

//                bool CheckAutoClickCondition() => m_ButtonState == EasyClickButtonState.LEVEL_UPGRADABLE;
//                break;

//            case EasyClickButtonState.NONE:

//            default:
//                Debug.Log($"EasyClickButton is clicked but the current button state({m_ButtonState.ToString()}) is not appropriate!");
//                return;
//        }

//        OnUpdateEasyButton();

//        if (++OneButtonClickCount >= StatisticEventInfo.BUTTON_CLICK_THRESHOLD)
//        {
//            OneButtonClickCount = 0;
//            GAManager.Instance.GA_DesignEvent(StatisticEventInfo.UX.UPGRADE_ONE_BUTTON);
//        }
//    }

//    public void ToggleButtonTemporarily(bool isOn)
//    {
//        Signals.Get<UpdateEasyButtonSignal>().RemoveListener(OnUpdateEasyButton);

//        if (isOn)
//        {
//            OnUpdateEasyButton();
//            Signals.Get<UpdateEasyButtonSignal>().AddListener(OnUpdateEasyButton);
//        }
//        else
//            ToggleButton(false);
//    }
//}
