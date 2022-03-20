//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using GoogleMobileAds.Api;
//using System;

//public class ADManager : MonoBehaviour
//{
//    private static ADManager _instance;
//    public static ADManager Instance
//    {
//        get
//        {
//            if (_instance == null)
//                _instance = FindObjectOfType(typeof(ADManager)) as ADManager;
//            return _instance;
//        }
//    }

//    //informations.
//    private AdmobAsset Asset; //광고 관련 ID 등 정보.
//    private List<RewardedAdData> rewardedAdList = new List<RewardedAdData>(); //보상형 광고 리스트.
//    private InterstitialAd interstitialAd; //전면 광고.

//    //callback.
//    public Action<AdPointType> OnLoadRewardedAdCallback;
//    public Action<AdPointType> OnPlayRewardedAdCallback;
//    public Action OnLoadInterstitialAdCallback;
//    public Action OnPlayInterstitialAdCallback;
//    //public Action<AdPointType, bool> OnRewardedAdCallback;
//    public Action<AdPointType> OnCloseRewardedAdCallback;
//    public Action<AdPointType, bool> OnGrantRewardsForRewardedAdCallback;
//    public Action<bool> OnInterstitialAdCallback;

//    private bool m_IsInitialized = false;
    
//    //debug.
//    private void DebugLog(object msg)
//    {
//        if (Asset.IsShowLog)
//            Debug.Log("(Admob, Nanali) " + msg);
//    }




//    //-------------------------------------Initialize------------------------------------//
//    private void Start()
//    {
//        _instance = FindObjectOfType(typeof(ADManager)) as ADManager;

//        Asset = AdmobAsset.Instance;
//        if (Asset.IsDevelop)
//            _instance = Instance;

//        DebugLog("Initialize : 광고 SDK 초기화 시작.");
//        // Initialize the Google Mobile Ads SDK.
//        if (!m_IsInitialized)
//            Initialize();
//    }

//    private void Initialize()
//    {
//        MobileAds.Initialize(initStatus =>
//        {
//            DebugLog("Initialize : 광고 SDK 초기화 성공.");

//            DebugLog("Initialize : 미디에이션 어댑터 초기화 상태 체크 시작.");
//            Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
//            foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
//            {
//                string className = keyValuePair.Key;
//                AdapterStatus status = keyValuePair.Value;
//                switch (status.InitializationState)
//                {
//                    case AdapterState.NotReady:
//                        // The adapter initialization did not complete.
//                        DebugLog("Initialize : Adapter: " + className + " not ready.");
//                        break;
//                    case AdapterState.Ready:
//                        // The adapter was successfully initialized.
//                        DebugLog("Initialize : Adapter: " + className + " is initialized.");
//                        break;
//                }
//            }
//            DebugLog("Initialize : 미디에이션 어댑터 초기화 상태 체크 종료.");

//            //광고 객체 생성.
//            DebugLog("Initialize : 광고 객체 생성 프로세스 시작.");

//            //create rewardedAd.
//            for (int i = 0; i < Asset.RewardedAdInfo.Length; i++)
//            {
//                //bool createFlag;
//                AdmobInformation info = Asset.RewardedAdInfo[i];

//                if (Asset.IsDevelop)
//                    rewardedAdList.Add(new RewardedAdData(info.adPointType, Asset.TestAdId));
//                else
//                    rewardedAdList.Add(new RewardedAdData(info.adPointType, info.AdvertiseID));

//                //if (Asset.IsDevelop) //테스트 환경.
//                //{
//                //    createFlag = info.adPointType == AdPointType.Test;
//                //}
//                //else //실제 환경.
//                //{
//                //    createFlag = info.adPointType != AdPointType.Test;
//                //}

//                //if (createFlag)
//                //{
//                //    //객체 생성 및 리스트에 등록.
//                //    rewardedAdList.Add(new RewardedAdData(info.adPointType, info.AdvertiseID));
//                //}
//            }

//            //create interstitialAd.
//            //CreateAndLoadInterstitialAd(Asset.IsDevelop ? Asset.InterstitialAdInfo_Test.AdvertiseID : Asset.InterstitialAdInfo_Live.AdvertiseID);

//#if !UNITY_EDITOR && DEVELOPMENT_BUILD
//            List<string> deviceIds = new List<string>();
//                deviceIds.Add("A558E307F204B1E59DED9C7EF4DCD834");
//                RequestConfiguration requestConfiguration = new RequestConfiguration
//                    .Builder()
//                    .SetTestDeviceIds(deviceIds)
//                    .build();

//                for (int i = 0; i < deviceIds.Count; i++)
//                    DebugLog($"Initialize : 테스트 아이디 등록({deviceIds[i]})");
//#endif

//            DebugLog("Initialize : 광고 객체 생성 프로세스 종료. 보상형 광고 갯수 : " + rewardedAdList.Count + "개, 전면광고 갯수 : " + (interstitialAd != null ? 1 : 0) + "개, 환경 : " + (Asset.IsDevelop ? "테스트" : "라이브"));

//            m_IsInitialized = true;
//            UnityEngine.Debug.Log("ADManager initialized");
//        });
//    }



//    //-------------------------------------RewardedAd------------------------------------//
//    private string GetRewardedAdID(AdPointType type)
//    {
//        string id = "";
//        for (int i = 0; i < rewardedAdList.Count; i++)
//        {
//            if (type == rewardedAdList[i].type)
//            {
//                id = rewardedAdList[i].AdvertiseID;
//                break;
//            }
//        }

//        if(Asset.IsDevelop)
//            id = Asset.TestAdId;

//        DebugLog("GetRewardedAdID : 아이디 추출. ID : " + id);
//        return id;
//    }

//    private RewardedAdData GetRewardedAd(AdPointType type)
//    {
//        RewardedAdData target = null;
//        for (int i = 0; i < rewardedAdList.Count; i++)
//        {
//            if (type == rewardedAdList[i].type)
//            {
//                target = rewardedAdList[i];
//                break;
//            }
//        }

//        return target;
//    }




//    public void HandleRewardedAdClosed(AdPointType type)
//    {
//        //광고가 닫히면 호출됨.
//        DebugLog("HandleRewardedAdClosed : 보상형 광고 종료. type = " + type);

//        RewardedAdData data = GetRewardedAd(type);
//        //콜백 전달.
//        //OnRewardedAdCallback?.Invoke(type, false);
//        OnCloseRewardedAdCallback?.Invoke(type);
//        OnCloseRewardedAdCallback = null;
//        //인스턴스 재할당.
//        data.CreateAndLoadRewardedAd(GetRewardedAdID(type));
//    }

//    public void HandleUserEarnedReward(AdPointType type, Reward args)
//    {
//        //보상을 받을 수 있는 시점까지 시청했을 때 호출됨.
//        DebugLog("HandleUserEarnedReward : 보상형 광고를 보상을 받을 수 있는 시점까지 시청하였음. type = " + type + ", 광고에 규정된 보상입니다. (type : " + args.Type + ", amount : " + args.Amount + ")");

//        RewardedAdData data = GetRewardedAd(type);
//        //콜백 전달.
//        OnGrantRewardsForRewardedAdCallback?.Invoke(type, data.IsValidToReceiveReward);
//        OnGrantRewardsForRewardedAdCallback = null;
//    }

//    public void HandleRewardedAdFailedToShow(AdPointType type, AdErrorEventArgs args)
//    {
//        //광고를 표시 할 수 없을 때 호출됨.
//        DebugLog("HandleRewardedAdFailedToShow : 보상형 광고 표시 불가. type = " + type + ", Reason : " + args.Message);
//        //OnRewardedAdCallback?.Invoke(type, false);
//        OnCloseRewardedAdCallback?.Invoke(type);
//        OnCloseRewardedAdCallback = null;
//    }

//    public void HandleRewardedAdOpening(AdPointType type)
//    {
//        //광고가 표시될 때 호출됨.
//        DebugLog("HandleRewardedAdOpening : 보상형 광고 표시 성공. type = " + type);
//        OnPlayRewardedAdCallback?.Invoke(type);
//    }

//    public void HandleRewardedAdFailedToLoad(AdPointType type, AdErrorEventArgs args)
//    {
//        //광고 로드에 실패하면 호출됨.
//        DebugLog("HandleRewardedAdFailedToLoad : 보상형 광고 가져오기 실패. type = " + type + ", Reason : " + args.Message);
//    }

//    public void HandleRewardedAdLoaded(AdPointType type)
//    {
//        //광고 로드에 성공하면 호출됨.
//        DebugLog("HandleRewardedAdLoaded : 보상형 광고 가져오기 성공. type = " + type);

//        OnLoadRewardedAdCallback?.Invoke(type);
//    }



//    public bool IsRewardedAd(AdPointType type)
//    {
//        RewardedAdData target = GetRewardedAd(type);
//        return target != null && target.IsLoaded();
//    }

//    public void ShowRewardedAd(AdPointType type, Action<AdPointType> closeCallback, Action<AdPointType, bool> grantRewardsCallback)
//    {
//        //OnRewardedAdCallback = callback;
//        OnCloseRewardedAdCallback = closeCallback;
//        OnGrantRewardsForRewardedAdCallback = grantRewardsCallback;

//        RewardedAdData target = GetRewardedAd(type);

//        if (target.IsPlaying)
//        {
//            DebugLog("ShowRewardedAd : 재생중입니다.");
//            //callback(type, false);
//            OnCloseRewardedAdCallback?.Invoke(type);
//            return;
//        }

//        if (target == null || !target.IsLoaded())
//        {
//            DebugLog("ShowRewardedAd : 보상형 광고 표시 불가. Reason : 가져오기가 진행되지 않았습니다.");
//            target.LoadRewardedAd();
//            //callback(type, false);
//            OnCloseRewardedAdCallback?.Invoke(type);
//            return;
//        }

//        DebugLog("ShowRewardedAd : 보상형 광고 표시 시작.");
//        target.Show();
//    }

//    public void LoadRewardedAd(AdPointType type)
//    {
//        if (m_IsInitialized)
//        {
//            RewardedAdData target = GetRewardedAd(type);

//            if (!target.IsPlaying && !target.IsLoaded())
//            {
//                UnityEngine.Debug.Log($"Start loading rewarded Ad({type.ToString()}).");
//                target.LoadRewardedAd();
//            }
//        }
//        else
//            Initialize();
//    }








//    //-------------------------------------Interstitial------------------------------------//
//    //광고 객체 생성.
//    private void CreateAndLoadInterstitialAd(string adUnitId)
//    {
//        interstitialAd = new InterstitialAd(adUnitId);

//        interstitialAd.OnAdLoaded += HandleOnAdLoaded;
//        interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
//        interstitialAd.OnAdOpening += HandleOnAdOpened;
//        interstitialAd.OnAdClosed += HandleOnAdClosed;
//        interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;

//        LoadInterstitialAd();
//    }

//    private void LoadInterstitialAd()
//    {
//        DebugLog("LoadInterstitialAd : 전면 광고 가져오기 시작.");

//        AdRequest request = new AdRequest.Builder().Build();
//        interstitialAd.LoadAd(request);
//    }

//    private void HandleOnAdLoaded(object sender, EventArgs args)
//    {
//        //전면 광고 로드에 성공하면 호출됨.
//        DebugLog("HandleOnAdLoaded : 전면 광고가 가져오기 성공.");

//        OnLoadInterstitialAdCallback?.Invoke();
//    }

//    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
//    {
//        //전면 광고 로드에 실패하면 호출됨.
//        DebugLog("HandleOnAdFailedToLoad : 전면 광고 가져오기 실패. Reason : " + args.Message);
//    }

//    private void HandleOnAdOpened(object sender, EventArgs args)
//    {
//        //전면 광고가 표시될 때 호출됨.
//        DebugLog("HandleOnAdOpened : 전면 광고 표시 성공.");
//        OnPlayInterstitialAdCallback?.Invoke();
//    }

//    private void HandleOnAdClosed(object sender, EventArgs args)
//    {
//        //전면 광고가 닫히면 호출됨.
//        DebugLog("HandleOnAdClosed : 전면 광고 종료.");
//        //콜백 전달.
//        OnInterstitialAdCallback?.Invoke(true);
//        //전면 광고 로드.
//        LoadInterstitialAd();
//    }

//    private void HandleOnAdLeavingApplication(object sender, EventArgs args)
//    {
//        //광고를 터치하여 앱에서 벗어나면 호출됨.
//        DebugLog("HandleOnAdLeavingApplication : 전면 광고를 터치하였음. 앱을 벗어납니다.");
//    }

//    public bool IsInterstitialAd()
//    {
//        return interstitialAd.IsLoaded();
//    }

//    public void ShowInterstitialAd(Action<bool> callback)
//    {
//        OnInterstitialAdCallback = callback;
//        DebugLog("ShowInterstitialAd : 전면 광고 표시 시작.");
//        if (interstitialAd.IsLoaded())
//        {
//            interstitialAd.Show();
//        }
//        else
//        {
//            DebugLog("ShowInterstitialAd : 전면 광고 표시 불가. Reason : 가져오기가 진행되지 않았습니다.");
//            LoadInterstitialAd();
//            OnInterstitialAdCallback(false);
//        }
//    }
//}

////애드몹 scriptableObject 세팅.
//[Serializable]
//public class AdmobInformation
//{
//    public AdPointType adPointType;
//    public string AndroidID;
//    public string iOSID;

//    public string AdvertiseID
//    {
//        get
//        {
//            string _val = "";
//#if UNITY_ANDROID
//            _val = AndroidID;
//#elif UNITY_IOS
//            _val = iOSID;
//#endif
//            return _val;
//        }

//    }
//}

//public enum AdPointType : int
//{
//    Test = 0,
//    General,
//    Treasurebox,
//    Coin,
//    Gem,
//    Heart,
//    Offline,
//}

//public enum AdvertiseType
//{
//    Interstitial,
//    Rewarded
//}

////보상형 광고 로드에 사용될 클래스.
//public class RewardedAdData
//{
//    public AdPointType type;
//    public RewardedAd rewardedAd;
//    public bool IsPlaying; //재생중인지.
//    public bool IsValidToReceiveReward;//보상받기가 가능한지.

//    public RewardedAdData(AdPointType _type, string id)
//    {
//        type = _type;
//        CreateAndLoadRewardedAd(id);
//        IsPlaying = false;
//        IsValidToReceiveReward = false;
//    }

//    //광고 객체 생성.
//    public void CreateAndLoadRewardedAd(string adUnitId)
//    {
//        rewardedAd = new RewardedAd(adUnitId);

//        SetListeners();

//        LoadRewardedAd();
//    }

//    public void LoadRewardedAd()
//    {
//        AdRequest request = new AdRequest.Builder().Build();
//        rewardedAd.LoadAd(request);
//    }

//    public string AdvertiseID
//    {
//        get
//        {
//            string val = "";
//            for (int i = 0; i < AdmobAsset.Instance.RewardedAdInfo.Length; i++)
//            {
//                if (type == AdmobAsset.Instance.RewardedAdInfo[i].adPointType)
//                {
//                    val = AdmobAsset.Instance.RewardedAdInfo[i].AdvertiseID;
//                    break;
//                }
//            }

//            return val;
//        }
//    }

//    public bool IsLoaded()
//    {
//        return rewardedAd.IsLoaded();
//    }

//    public void SetListeners()
//    {
//        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
//        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
//        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
//        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
//        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
//        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
//    }

//    public void Show()
//    {
//        IsPlaying = true;
//        IsValidToReceiveReward = false;
//        rewardedAd.Show();
//    }

//    public void HandleRewardedAdClosed(object sender, EventArgs args)
//    {
//        //광고가 닫히면 호출됨.
//        IsPlaying = false;
//        IsValidToReceiveReward = false;
//        ADManager.Instance.HandleRewardedAdClosed(type);
//    }

//    public void HandleUserEarnedReward(object sender, Reward args)
//    {
//        //보상을 받을 수 있는 시점까지 시청했을 때 호출됨.
//        IsValidToReceiveReward = true;
//        ADManager.Instance.HandleUserEarnedReward(type, args);
//    }

//    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
//    {
//        //광고를 표시 할 수 없을 때 호출됨.
//        ADManager.Instance.HandleRewardedAdFailedToShow(type, args);
//    }

//    public void HandleRewardedAdOpening(object sender, EventArgs args)
//    {
//        //광고가 표시될 때 호출됨.
//        ADManager.Instance.HandleRewardedAdOpening(type);
//    }

//    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
//    {
//        //광고 로드에 실패하면 호출됨.
//        ADManager.Instance.HandleRewardedAdFailedToLoad(type, args);
//    }

//    public void HandleRewardedAdLoaded(object sender, EventArgs args)
//    {
//        //광고 로드에 성공하면 호출됨.
//        ADManager.Instance.HandleRewardedAdLoaded(type);
//    }
//}