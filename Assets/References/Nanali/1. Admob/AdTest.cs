//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AdTest : MonoBehaviour
//{
//    ADManager manager;

//    public GameObject Go_RewardedTestBtn;
//    public GameObject Go_RewardedTreasureboxBtn;
//    public GameObject Go_RewardedHeartBtn;
//    public GameObject Go_InterstitialBtn;

//    private void Awake()
//    {
//        manager = ADManager.Instance;

//        Go_RewardedTestBtn.SetActive(false);
//        Go_RewardedTreasureboxBtn.SetActive(false);
//        Go_RewardedHeartBtn.SetActive(false);
//        Go_InterstitialBtn.SetActive(false);
//    }

//    private void OnEnable()
//    {
//        manager.OnLoadRewardedAdCallback += OnLoadRewardedAdCallback;
//        manager.OnPlayRewardedAdCallback += OnPlayRewardedAdCallback;
//        manager.OnLoadInterstitialAdCallback += OnLoadInterstitialAdCallback;
//        manager.OnPlayInterstitialAdCallback += OnPlayInterstitialAdCallback;
//    }

//    private void OnDisable()
//    {
//        manager.OnLoadRewardedAdCallback -= OnLoadRewardedAdCallback;
//        manager.OnPlayRewardedAdCallback -= OnPlayRewardedAdCallback;
//        manager.OnLoadInterstitialAdCallback -= OnLoadInterstitialAdCallback;
//        manager.OnPlayInterstitialAdCallback -= OnPlayInterstitialAdCallback;
//    }

//    //보상형 광고 시청.
//    public void ShowRewardedAd(int _type)
//    {
//        AdPointType type = (AdPointType)_type;
//        //manager.ShowRewardedAd(type, OnCompleteRewardedAdCallback);
//    }

//    //전면 광고 시청.
//    public void ShowInterstitialAd()
//    {
//        manager.ShowInterstitialAd(OnCompleteInterstitialAdCallback);
//    }


//    //rewarded ad callbacks.
//    void OnLoadRewardedAdCallback(AdPointType type)
//    {
//        //보상형광고 시청 가능. UI를 활성화 해 줍니다.
//        switch(type)
//        {
//            case AdPointType.Test: Go_RewardedTestBtn.SetActive(true); break;
//            case AdPointType.Treasurebox: Go_RewardedTreasureboxBtn.SetActive(true); break;
//            case AdPointType.Heart: Go_RewardedHeartBtn.SetActive(true); break;
//        }
//    }

//    void OnPlayRewardedAdCallback(AdPointType type)
//    {
//        //보상형광고 시작. UI를 제거해줍니다.
//        switch (type)
//        {
//            case AdPointType.Test: Go_RewardedTestBtn.SetActive(false); break;
//            case AdPointType.Treasurebox: Go_RewardedTreasureboxBtn.SetActive(false); break;
//            case AdPointType.Heart: Go_RewardedHeartBtn.SetActive(false); break;
//        }
//    }
    
//    void OnCompleteRewardedAdCallback(AdPointType type, bool success)
//    {
//        //리워드 광고 완료. 보상 지급.
//        if (success)
//        {
//            switch (type)
//            {
//                case AdPointType.Test: break;
//                case AdPointType.Treasurebox: break;
//                case AdPointType.Heart: break;
//            }
//        }
//    }

//    //interstitial ad callbacks.
//    void OnLoadInterstitialAdCallback()
//    {
//        //전면광고 시청 가능. UI를 활성화 해 줍니다.
//        Go_InterstitialBtn.SetActive(true);
//    }

//    void OnPlayInterstitialAdCallback()
//    {
//        //전면광고 시작. UI를 제거해줍니다.
//        Go_InterstitialBtn.SetActive(false);
//    }

//    void OnCompleteInterstitialAdCallback(bool success)
//    {
//        //전면 광고 완료. 보상 지급.
//        if (success)
//        {

//        }
//    }
//}
