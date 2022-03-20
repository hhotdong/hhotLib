//using System;
//using System.Collections;
//using System.Threading.Tasks;
//using Threading;
//using UnityEngine;
//using deVoid.Utils;
//using TMPro;

//public class UIToggleButton_WatchRewardedAd : UIToggleButton
//{
//    [SerializeField] private GameObject m_LoadingIndicator;
//    [SerializeField] private GameObject m_RewardedImg;
//    [SerializeField] private TextMeshProUGUI m_AdCouponCountText;
//    [SerializeField] private TextMeshProUGUI m_DescText;

//    [Header("Button Events")]
//    [SerializeField] private AdPointType m_AdPointType; 
//    [SerializeField] protected RewardButtonType m_RewardButtonType = RewardButtonType.NONE;

//    public override event Action<int> ButtonClicked;

//    [SerializeField] private ProductData adCouponData;


//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    protected override void OnEnable()
//    {
//        base.OnEnable();

//        ADManager adManager = ADManager.Instance;
//        adManager.OnLoadRewardedAdCallback += OnLoadRewardedAdCallback;
//        adManager.OnPlayRewardedAdCallback += OnPlayRewardedAdCallback;

//        Signals.Get<SkipRewardedAdBeforeComplete>().AddListener(OnSkipRewardedAdBeforeComplete);
//    }

//    protected override void OnDisable()
//    {
//        base.OnDisable();

//        ADManager adManager = ADManager.Instance;
//        adManager.OnLoadRewardedAdCallback -= OnLoadRewardedAdCallback;
//        adManager.OnPlayRewardedAdCallback -= OnPlayRewardedAdCallback;

//        Signals.Get<SkipRewardedAdBeforeComplete>().RemoveListener(OnSkipRewardedAdBeforeComplete);
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    protected override void OnClickButton()
//    {
//        base.OnClickButton();
//        m_ToggleButton.interactable = false;
//        ButtonClicked?.Invoke((int)m_RewardButtonType);
//        if (adCouponData.GetProductData() > 0)
//        {
//            Signals.Get<SpendAdCouponSignal>().Dispatch(1);
//            Signals.Get<ShowRewardedAdWithCouponSignal>().Dispatch(m_AdPointType);
//            if (adCouponData.GetProductData() > 0)
//                m_AdCouponCountText.text = $"x{adCouponData.GetProductData()}";
//        }
//        else
//            Signals.Get<ShowRewardedAdSignal>().Dispatch(m_AdPointType);
//    }

//    private void OnLoadRewardedAdCallback(AdPointType type)
//    {
//        if (type == m_AdPointType)
//            Task.Run(DoToggleButton);
//    }

//    private void OnPlayRewardedAdCallback(AdPointType type)
//    {
//        if (type == m_AdPointType)
//            m_CurrentButtonState = ToggleButtonState.NONE;
//    }

//    private void OnSkipRewardedAdBeforeComplete()
//    {
//        if(m_ToggleButton)
//            m_ToggleButton.interactable = true;
//    }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public override void Toggle(ToggleButtonState state)
//    {
//        if (CheckIfRewardedAdReady())
//        {
//            m_DescText.gameObject.SetActive(true);
//            m_LoadingIndicator.SetActive(false);
//        }
//        else
//        {
//            UnityEngine.Debug.Log($"RewardedAd({m_AdPointType}) is now being loaded.)");
//            m_LoadingIndicator.SetActive(true);
//            m_ToggleButton.interactable = false;
//            ToggleButtonImgs(false);
//            ToggleByLockState(false);
//            m_AdCouponCountText.gameObject.SetActive(false);
//            m_RewardedImg.SetActive(false);
//            m_DescText.gameObject.SetActive(false);
//            m_CurrentButtonState = ToggleButtonState.NONE;
//            Task.Run(DoCheckLoadingComplete);
//            return;
//        }

//        if (state <= ToggleButtonState.NONE || state > ToggleButtonState.FREE_AD || m_CurrentButtonState == state)
//            return;

//        m_CurrentButtonState = state;

//        switch (state)
//        {
//            case ToggleButtonState.NOT_CLICKABLE:
//                m_ToggleButton.interactable = false;
//                ToggleButtonImgs(false);
//                ToggleByLockState(false);
//                m_AdCouponCountText.gameObject.SetActive(false);
//                m_RewardedImg.SetActive(true);
//                break;

//            case ToggleButtonState.CLICKABLE_ENABLED:
//                m_ToggleButton.interactable = true;
//                ToggleButtonImgs(true);
//                ToggleByLockState(false);
//                m_AdCouponCountText.gameObject.SetActive(false);
//                m_RewardedImg.SetActive(true);
//                break;

//            case ToggleButtonState.FREE_AD:
//                m_ToggleButton.interactable = true;
//                ToggleButtonImgs(true);
//                m_AdCouponCountText.gameObject.SetActive(true);
//                m_RewardedImg.SetActive(false);
//                SetAdCouponCountText();
//                break;

//            default:
//                UnityEngine.Debug.LogError($"UpgradeButtonState({state}) is not appropriate!");
//                return;
//        }

//        OnToggle();


//        bool CheckIfRewardedAdReady()
//        {
//            if (ADManager.Instance.IsRewardedAd(m_AdPointType))
//            {
//                Debug.Log($"RewardedAd({m_AdPointType}) is ready.");
//                return true;
//            }
//            else
//            {
//                Debug.Log($"RewardedAd({m_AdPointType}) is not ready.");
//                return false;
//            }
//        }
//    }

//    public void SetAdCouponCountText()
//    {
//        m_AdCouponCountText.text = $"x{adCouponData.GetProductData()}";
//    }

//    private async Task DoCheckLoadingComplete()
//    {
//        await ThreadSwitcher.ResumeUnityAsync();
//        await Task.Delay(100);
//        StartCoroutine(CheckLoadingComplete());

//        IEnumerator CheckLoadingComplete()
//        {
//            ADManager adManager = ADManager.Instance;
//            if (adManager == null)
//            {
//                UnityEngine.Debug.LogError("AdManager is null!");
//                yield break;
//            }

//            int waitTime = 0;
//            const int REFRESH_LOAD_INTERVAL = 5;

//            while (!adManager.IsRewardedAd(m_AdPointType))
//            {
//                Debug.Log("Still loading rewarded ad...");

//                if (waitTime % REFRESH_LOAD_INTERVAL == 0)
//                {
//                    Debug.Log("Refresh loading!");
//                    adManager.LoadRewardedAd(m_AdPointType);
//                }

//                yield return HelperManager.WS_ONE_SEC;
//                waitTime++;
//            }

//            UnityEngine.Debug.Log("Loading rewarded ad complete.");
//        }
//    }

//    private async Task DoToggleButton()
//    {
//        await ThreadSwitcher.ResumeUnityAsync();
//        await Task.Delay(100);

//        if (adCouponData.GetProductData() > 0)
//            Toggle(ToggleButtonState.FREE_AD);
//        else
//            Toggle(ToggleButtonState.CLICKABLE_ENABLED);
//    }
//}