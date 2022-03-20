//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using ScriptableObjectArchitecture;
//using deVoid.UIFramework;
//using deVoid.Utils;
//using DG.Tweening;

//public class ShowRewardedAdSignal : ASignal<AdPointType> { }
//public class ShowRewardedAdWithCouponSignal : ASignal<AdPointType>{ }
//public class SkipRewardedAdBeforeComplete : ASignal { }
//public class CheckAdvAccumulationSignal : ASignal{ }

//[CreateAssetMenu]
//public partial class RewardManager : ScriptableObject, IInitializable, IRemoteInitializable
//{
//    public const int ADV_ACCUMULATION_MAX = 6;
//    public bool IsInitialized { get; private set; }

//    // partial 쪽도 초기화 필요
//    [Header("Common Infos")]
//    private bool isFetchingServerTimeToResetOfflineReward = false;

//    [Header("Landmark Rewards")]
//    [SerializeField] private List<RewardData_LandmarkLookChange> m_LandmarkLookChangeThresholds;
//    [SerializeField] private List<RewardData_AnimalLevelup> m_LandmarkPhaseRewards;
//    [SerializeField] private List<RewardData_NewLandmark> m_NewLandmarkRewards;
//    private Dictionary<int, List<int>> m_LandmarkLookChangeThresholds_Internal;
//    private Dictionary<AnimalMaxCountType, List<int>> m_LandmarkPhaseRewardsThresholds_Internal;
//    [SerializeField] private List<PhaseRewardsThreshold> m_LandmarkPhaseRewardsThresholdsData;

//    [SerializeField] private int m_AdvAccumulationCount;

//    public bool IsReadyAdvAccumulation => TutorialEventTracker.SubTutorial_Clear[4];
//    public bool IsMaxAdvAccumulation => m_AdvAccumulationCount >= ADV_ACCUMULATION_MAX;
//    public bool IsOfflineRewardReady;
//    public int AdvAccumulationCount => m_AdvAccumulationCount;
//    public int MaxPhaseLevel => m_LandmarkPhaseRewards.Count;

//    [Header("SO Managers")]
//    [SerializeField] private IdleManager _IdleManager;
//    [SerializeField] private LandmarkManager _LandmarkManager;
//    [SerializeField] private ShopManager _ShopManager;
//    [SerializeField] private AnimalManager _AnimalManager;
//    [SerializeField] private AssetReferenceManager _AssetReferenceManager;

//    [Header("SO Events")]
//    [SerializeField] private IntGameEvent _OnAddAnimal = default(IntGameEvent);
//    [SerializeField] private IntGameEvent _OnAddRareAnimal = default(IntGameEvent);
//    [SerializeField] private IntGameEvent _OnGrantReward = default(IntGameEvent);
//    [SerializeField] private IntGameEvent _OnPlayTimeline = default(IntGameEvent);
//    [SerializeField] private BigNumberGameEvent _OnAddJewel = default(BigNumberGameEvent);
//    [SerializeField] private IntGameEvent _OnAddUtility = default(IntGameEvent);
//    [SerializeField] private IntGameEvent _OnProgressQuest = default(IntGameEvent);
//    [SerializeField] private IntGameEvent _OnProgressAchievement = default(IntGameEvent);

//    public event Action<int> UpdateClamProgressbar;

//    //////////////////////////////////////////
//    // Initialize & Reset
//    //////////////////////////////////////////

//    public void Initialize(SaveData saveData)
//    {
//        IsInitialized = false;

//        SaveData.MultiplierData _data = saveData.GetMultiplierData;
//        _Mult_Common_AnimalCount.Value = _data.ANIMAL_MULTIPLIER;
//        _Mult_Common_LandmarkShowUpgrade.Value = _data.EVO_MULTIPLIER;

//        m_AdvAccumulationCount = saveData.GetCommonData.REWARDED_ADV_ACCUMUATION_COUNT;

//        isFetchingServerTimeToResetOfflineReward = false;

//        //m_DeferredRewards = new Queue<DeferredReward>();

//        BindLookChangeData();
//        BindPhaseRewardsData();
//        BindNewLandmarkRewardsData();

//        LandmarkData.OnSucceedLevelUpgrade += DoUpgradeLandmarkLevel;
//        LandmarkData.OnSucceedPhaseUpgrade += DoUpgradeLandmarkPhase;
//        SaveLoadManager.SaveEvent += OnSave;
//        _OnGrantReward.AddListener(DoGrantReward);
//        ADManager.Instance.OnPlayRewardedAdCallback += OnPlayRewardedAdCallback;

//        Signals.Get<CheckAdvAccumulationSignal>().AddListener(CheckAdvAccumulationCount);
//        Signals.Get<ShowRewardedAdSignal>().AddListener(OnShowRewardedAd);
//        Signals.Get<ShowRewardedAdWithCouponSignal>().AddListener(OnShowRewardedAdWithCoupon);
        
//        IsInitialized = true;
//    }

//    public void DoReset()
//    {
//        LandmarkData.OnSucceedLevelUpgrade -= DoUpgradeLandmarkLevel;
//        LandmarkData.OnSucceedPhaseUpgrade -= DoUpgradeLandmarkPhase;
//        SaveLoadManager.SaveEvent -= OnSave;
//        _OnGrantReward.RemoveListener(DoGrantReward);
//        ADManager.Instance.OnPlayRewardedAdCallback -= OnPlayRewardedAdCallback;

//        Signals.Get<CheckAdvAccumulationSignal>().RemoveListener(CheckAdvAccumulationCount);
//        Signals.Get<ShowRewardedAdSignal>().RemoveListener(OnShowRewardedAd);
//        Signals.Get<ShowRewardedAdWithCouponSignal>().RemoveListener(OnShowRewardedAdWithCoupon);

//        IsInitialized = false;
//    }

//    public void OnSave(SaveData data)
//    {
//        SaveData.MultiplierData dataToSave = data.GetMultiplierData;
//        if (dataToSave == null)
//            Debug.LogError("Failed to save RewardManager because MultiplierData is null!");

//        dataToSave.ANIMAL_MULTIPLIER = _Mult_Common_AnimalCount.Value;
//        dataToSave.EVO_MULTIPLIER = _Mult_Common_LandmarkShowUpgrade.Value;

//        data.GetCommonData.REWARDED_ADV_ACCUMUATION_COUNT = m_AdvAccumulationCount;
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void DoUpgradeLandmarkLevel(LandmarkData data, bool isChangeLook)
//    {
//        if (isChangeLook)
//        {
//            int currShowOrder = data.CurrentShowOrder;
//            int nextShowOrder = currShowOrder + 1;
//            if (LandmarkManager.IsShowOrderValid(nextShowOrder))
//            {
//                // 랜드마크 진화 시
//                if (currShowOrder >= (int)ShowOrder.ONE)
//                {
//                    // 진화 횟수 배수 Multiplier가 증가
//                    _Mult_Common_LandmarkShowUpgrade.Value = System.Math.Floor(_Mult_Common_LandmarkShowUpgrade.Value * LandmarkManager.CommonIncreaseRate);

//                    // 진화 업적 누적
//                    _OnProgressAchievement.Raise((int)AchievementType.CHANGE_LANDMARK_LOOK);

//                    string convertedType = LandmarkData.ConvertLandmarkTypeToAlphabet(data.DataTypeIdx);
//                    GAManager.Instance.GA_DesignEvent(string.Format(StatisticEventInfo.Progression.LANDMARK_RESTORE_, convertedType, currShowOrder));
//                }
                
//                Signals.Get<ToggleCameraActingSignal>().Dispatch(true, new CameraActingInfos(CameraActingType.SPECTATE, data.DataTypeIdx, nextShowOrder, null));
//            }
//            else
//                Debug.Log($"ShowOrder({((ShowOrder)currShowOrder).ToString()}) is not appropriate!");

//            _IdleManager.UpdateLandmarkFinalGoldOutputs();
//            _IdleManager.UpdateFinalHeartOutput();
//        }
//        else
//        {
//            int currShowOrder = data.CurrentShowOrder;
//            string convertedType = LandmarkData.ConvertLandmarkTypeToAlphabet(data.DataTypeIdx);

//            // 랜드마크 첫 생성 시 보상 지급
//            if (currShowOrder == (int)ShowOrder.NONE)
//            {
//                if (TryGetNewLandmarkReward(data.DataTypeIdx, out RewardData_NewLandmark rewardData, out List<RewardWidgetData> widgetData))
//                    rewardData?.GrantRewards();
//                else
//                    Debug.LogError($"Failed to grant new landmark reward because there is no rewardData for this landmark({data.DataType})!");

//                GAManager.Instance.GA_DesignEvent(string.Format(StatisticEventInfo.Progression.LANDMARK_LEVEL_, convertedType, 1));
//                GAManager.Instance.GA_DesignEvent(string.Format(StatisticEventInfo.Progression.LANDMARK_OPEN_, convertedType));
//            }
//            else
//                GAManager.Instance.GA_DesignEvent(string.Format(StatisticEventInfo.Progression.LANDMARK_LEVEL_, convertedType, data.UpgradeLevel));

//            _IdleManager.UpdateLandmarkFinalGoldOutputs(data.DataTypeIdx);

//            // 첫 번째 랜드마크는 업적 프로그레스에 반영
//            if (data.DataType == LandmarkType.LANDMARK_0)
//                _OnProgressAchievement?.Raise((int)AchievementType.UPG_LNDMRK_0);
//        }
//    }

//    private void DoUpgradeLandmarkPhase(LandmarkData data, Action<double> callback, bool shouldGrantAnimal)
//    {
//        if (shouldGrantAnimal)
//        {
//            // 동물 보상여부 체크
//            Debug.Log("Grant animal.");

//            // 일반동물 생성 이벤트(동물 인덱스와 랜드마크 인덱스가 동일하다는 전제)
//            _OnAddAnimal.Raise(data.DataTypeIdx);

//            // 첫 번째 동물 생성 시에는 Multiplier 효과 적용하지 않도록 예외 처리
//            if(data.UpgradePhase > 1)
//                _Mult_Common_AnimalCount.Value = System.Math.Floor(_Mult_Common_AnimalCount.Value * LandmarkManager.CommonIncreaseRate);
//        }
//        else
//        {
//            if (TryGetPhaseLevelupReward(data.UpgradePhase, out RewardData_AnimalLevelup reward))
//            {
//                int animalTypeIdx = data.DataTypeIdx;
//                if (_AnimalManager.TryGetRandomActivatedAnimal(out Transform animalTr, animalTypeIdx))
//                {
//                    Signals.Get<ToggleCameraActingSignal>().Dispatch(true, new CameraActingInfos(CameraActingType.ORBIT, (int)CameraOrbitType.ANIMAL_LEVELUP, -1, animalTr, PlayLevelupEffect));
//                    DOTween.Sequence().AppendInterval(0.1F).AppendCallback(AllowUserInput).Play();
//                }

//                callback?.Invoke(reward.increaseRate);


//                void AllowUserInput()
//                {
//                    Signals.Get<ChangeTouchContextSignal>().Dispatch(TouchContext.CAM_HANDLING);
//                    Signals.Get<ToggleTouchDetectingSignal>().Dispatch(true);
//                }

//                void PlayLevelupEffect()
//                {
//                    Signals.Get<PlayAnimalLevelupEffectSignal>().Dispatch(animalTypeIdx);
//                    Signals.Get<AddDeferredEventSignal>().Dispatch((int)DeferredEventType.LEVELUP_INFOS, ShowLevelupInfos);

//                    void ShowLevelupInfos()
//                    {
//                        Signals.Get<ShowNamePopupSignal>().Dispatch
//                        (
//                            new NamePopupRewardData
//                            (
//                                NamePopupType.LEVELUP_ANIMAL,
//                                animalTypeIdx,
//                                new List<RewardWidgetData>() { new RewardWidgetData(RewardWidgetType.MULTIPLIER, null, System.Math.Floor(reward.increaseRate * 100.0).ToString()) }
//                            )
//                            , () => Signals.Get<ToggleCameraActingSignal>().Dispatch(false, new CameraActingInfos(CameraActingType.NONE, -1, -1, null))
//                            //, OpenCameraModeWindow
//                        );

//                        //void OpenCameraModeWindow() => Signals.Get<NavigateToWindowSignal>().Dispatch(ScreenIds.CameraModeWindow, new CameraModeWindowProperties(animalTr.GetComponent<Collider>(), CameraModeEntry.LEVELUP_ANIMAL));
//                    }
//                }
//            }
//            else
//                Debug.Log($"There is no phase-reward of {data.DataType.ToString()} in level {data.UpgradePhase}!");
//        }

//        _IdleManager.UpdateLandmarkFinalGoldOutputs();
//        _IdleManager.UpdateFinalHeartOutput();
//    }

//    private void DoGrantReward(int typeIdx)
//    {
//        double rewardAmount = 1.0;
//        GrantRewardType rewardType = (GrantRewardType)typeIdx;

//        switch (rewardType)
//        {
//            case GrantRewardType.REWARDED_AD_GOLD:
//                BigNumber rewardAmount_RewardedAdGold = IncrementHelper.Multiply(_LandmarkManager.GoldOutputPerSecond, FinalMult_RewardedAdGold);
//                Signals.Get<AddGoldSignal>().Dispatch(rewardAmount_RewardedAdGold, (int)DeferredEventType.REWARDED_AD_GOLD);
//                _OnProgressAchievement.Raise((int)AchievementType.CALL_ALBATROSS);
//                _OnProgressQuest.Raise((int)QuestType.CALL_ALBATROSS);
//                _OnPlayTimeline.Raise((int)TimelineEvent.REWARDED_AD_GOLD_PLAY);
//                break;

//            case GrantRewardType.REWARDED_AD_HEART:
//                BigNumber rewardAmount_RewardedAdHeart = IncrementHelper.Multiply(_IdleManager.FinalHeartOutput, m_RewardedAdHeartMultiplier_Default / (double)_IdleManager.m_HeartOutputInterval);
//                Signals.Get<AddHeartSignal>().Dispatch(rewardAmount_RewardedAdHeart, (int)DeferredEventType.NONE);
//                _OnPlayTimeline.Raise((int)TimelineEvent.REWARDED_AD_HEART_PLAY);
//                break;

//            case GrantRewardType.REWARDED_AD_JEWEL:
//                _OnAddJewel.Raise(REWARDED_AD_JEWEL_AMOUNT_BIGNUM);
//                Signals.Get<UpdateProductBoxSignal>().Dispatch();
//                Signals.Get<PlayCoinEffectSignal>().Dispatch((int)CoinEffectType.JEWEL, (int)UIReferencePointType.NONE);
//                GAManager.Instance.GA_ResourceEvent(false, StatisticEventInfo.CurrencyType.JEWEL, REWARDED_AD_JEWEL_AMOUNT, StatisticEventInfo.ResourceType.AD_REWARD, StatisticEventInfo.Jewel.JEWEL_BUNCH_1);
//                Signals.Get<ResetRewardedAdSettingsSignal>().Dispatch((int)rewardType, false);
//                break;

//            case GrantRewardType.REWARDED_AD_RARE_ANIMAL_RANDOM_BOX:
//                Signals.Get<TogglePurchasePopupSignal>().Dispatch(true, new PurchasePopupWindowProperties(_ShopManager.GetRandomBoxData(ShopManager.PRODUCT_DATA_NORMAL_RANDOMBOX), true));
//                Signals.Get<ResetRewardedAdSettingsSignal>().Dispatch((int)rewardType, false);
//                break;

//            case GrantRewardType.OFFLINE_REWARD:
//                double offlineTime = _IdleManager.OfflineRewardTimer;
//                BigNumber rewardAmount_Offline_Gold = IncrementHelper.Multiply(_LandmarkManager.GoldOutputPerSecond, offlineTime);
//                BigNumber rewardAmount_Offline_Heart = IncrementHelper.Multiply(_IdleManager.FinalHeartOutput, _IdleManager.OfflineRewardHeartMultiplier);
//                Signals.Get<AddGoldSignal>().Dispatch(rewardAmount_Offline_Gold, (int)DeferredEventType.NONE);
//                Signals.Get<AddHeartSignal>().Dispatch(rewardAmount_Offline_Heart, (int)DeferredEventType.NONE);

//                Signals.Get<ResetRewardedAdSettingsSignal>().Dispatch((int)rewardType, false);
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.Balance.NORMAL_OFFLINE_REWARD);
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.Balance.OFFLINE_REWARD_TIME, (float)System.Math.Floor(offlineTime));
//                _OnPlayTimeline.Raise((int)TimelineEvent.OFFLINE_REWARD_PLAY);
//                break;

//            case GrantRewardType.OFFLINE_REWARD_DOUBLE:
//                double _offlineTime = _IdleManager.PrevOfflineRewardTimer;
//                BigNumber rewardAmount_Offline_Gold_Double = IncrementHelper.Multiply(_LandmarkManager.GoldOutputPerSecond, _offlineTime * 2.0);
//                BigNumber rewardAmount_Offline_Heart_Double = IncrementHelper.Multiply(_IdleManager.FinalHeartOutput, _IdleManager.OfflineRewardHeartMultiplierDouble * 2.0);
//                Signals.Get<AddGoldSignal>().Dispatch(rewardAmount_Offline_Gold_Double, (int)DeferredEventType.NONE);
//                Signals.Get<AddHeartSignal>().Dispatch(rewardAmount_Offline_Heart_Double, (int)DeferredEventType.NONE);

//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.Balance.OFFLINE_REWARD_TIME, (float)System.Math.Floor(_offlineTime));
//                _OnPlayTimeline.Raise((int)TimelineEvent.OFFLINE_REWARD_DOUBLE);
//                break;

//            case GrantRewardType.REWARDED_TOUCH_GOLD:
//                BigNumber rewardAmount_Touch_Gold = IncrementHelper.Multiply(_LandmarkManager.GoldOutputPerSecond, FinalMult_GoldBox);
//                Signals.Get<AddGoldSignal>().Dispatch(rewardAmount_Touch_Gold, (int)DeferredEventType.NONE);

//                Signals.Get<ResetRewardedAdSettingsSignal>().Dispatch((int)rewardType, false);
//                _OnProgressQuest.Raise((int)QuestType.GET_TOUCH_REWARD);
//                _OnProgressAchievement.Raise((int)AchievementType.GET_TOUCH_REWARD);
//                Signals.Get<PlayCoinEffectSignal>().Dispatch((int)CoinEffectType.GOLD, -1);
//                break;

//            case GrantRewardType.REWARDED_PICTURE_GOLD:
//                BigNumber pictureRewardAmount_Gold = IncrementHelper.Multiply(_LandmarkManager.GoldOutputPerSecond, FinalMult_TakeAPicture);
//                Signals.Get<AddGoldSignal>().Dispatch(pictureRewardAmount_Gold, (int)DeferredEventType.NONE);

//                Signals.Get<ResetRewardedAdSettingsSignal>().Dispatch((int)rewardType, false);
//                _OnProgressQuest.Raise((int)QuestType.PHOTO_MISSION);
//                _OnProgressAchievement.Raise((int)AchievementType.PHOTO_MISSION);
//                Signals.Get<PlayCoinEffectSignal>().Dispatch((int)CoinEffectType.GOLD, -1);
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.Balance.PHOTO_MISSION);
//                break;

//            case GrantRewardType.GROWTH_TIER_LEVELUP:
//                _OnAddJewel.Raise(GROWTH_REWARD_TIER_LEVELUP_JEWEL_AMOUNT_BIGNUM);
//                Signals.Get<PlayCoinEffectSignal>().Dispatch((int)CoinEffectType.JEWEL, (int)UIReferencePointType.GROWTH_TIER_LEVELUP);
//                break;

//            case GrantRewardType.GROWTH_DECREASE_UPGRADE_COST:
//                rewardAmount = _Mult_Growth_LandmarkUpgradeCost.Value * m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_DECREASE_UPGRADE_COST;
//                _Mult_Growth_LandmarkUpgradeCost.Value = Math.Floor(rewardAmount * 1000.0) / 1000.0;
//                _IdleManager.UpdateLandmarkFinalUpgradeCosts();
//                Signals.Get<UpdateMilestoneStateSignal>().Dispatch();
//                Signals.Get<UpdateNotificationSignal>().Dispatch((int)NotificationType.LANDMARK_LEVELUP);
//                break;

//            case GrantRewardType.GROWTH_INCREASE_GOLD_OUTPUT:
//                rewardAmount = _Mult_Growth_LandmarkGoldOutput.Value * m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_LANDMARK_GOLD_OUTPUT;
//                _Mult_Growth_LandmarkGoldOutput.Value = Math.Floor(rewardAmount);
//                _IdleManager.UpdateLandmarkFinalGoldOutputs();
//                break;

//            case GrantRewardType.GROWTH_INCREASE_REWARD_REWARDED_AD_GOLD:
//                rewardAmount = _Mult_Growth_RewardedAdGold.Value * m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_REWARDED_AD_GOLD;
//                _Mult_Growth_RewardedAdGold.Value = Math.Floor(rewardAmount);
//                break;

//            case GrantRewardType.GROWTH_INCREASE_REWARD_TOUCH:
//                rewardAmount = _Mult_Growth_GoldBox.Value * m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_GOLDBOX;
//                _Mult_Growth_GoldBox.Value = Math.Floor(rewardAmount);
//                break;

//            case GrantRewardType.GROWTH_INCREASE_ANIMAL_HEART_OUTPUT:
//                rewardAmount = _Mult_Growth_HeartOutput.Value * m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_ANIMAL_HEART_OUTPUT;
//                _Mult_Growth_HeartOutput.Value = Math.Floor(rewardAmount);
//                _IdleManager.UpdateFinalHeartOutput();
//                break;

//            case GrantRewardType.GROWTH_INCREASE_CHANCE_DOUBLEHEART:
//                _Chance_Growth_DoubleHeart.Value += m_GrowthRewardsAmount.GROWTH_REWARD_ADDITIVE_RATE_INCREASE_CHANCE_DOUBLEHEART;
//                break;

//            case GrantRewardType.GROWTH_INCREASE_REWARD_OFFLINE:
//                _Add_Growth_MaxOfflineTime.Value += m_GrowthRewardsAmount.GROWTH_REWARD_ADDITIVE_RATE_INCREASE_REWARD_OFFLINE;
//                break;

//            case GrantRewardType.GROWTH_INCREASE_REWARD_PHOTO_MISSION:
//                rewardAmount = _Mult_Growth_TakeAPicture.Value * m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_TAKE_A_PICTURE;
//                _Mult_Growth_TakeAPicture.Value = Math.Floor(rewardAmount);
//                break;

//            case GrantRewardType.QUEST_UPG_ANIMAL_0:
//            case GrantRewardType.QUEST_UPG_ANIMAL_1:
//            case GrantRewardType.QUEST_UPG_ANIMAL_2:
//            case GrantRewardType.QUEST_UPG_ANIMAL_3:
//            case GrantRewardType.QUEST_UPG_ANIMAL_4:
//            case GrantRewardType.QUEST_UPG_ANIMAL_5:
//            case GrantRewardType.QUEST_UPG_ANIMAL_6:
//            case GrantRewardType.QUEST_UPG_ANIMAL_7:
//            case GrantRewardType.QUEST_UPG_ANIMAL_8:
//            case GrantRewardType.QUEST_UPG_ANIMAL_9:
//            case GrantRewardType.QUEST_UPG_ANIMAL_10:
//            case GrantRewardType.QUEST_UPG_ANIMAL_11:
//            case GrantRewardType.QUEST_UPG_ANIMAL_12:
//            case GrantRewardType.QUEST_UPG_ANIMAL_13:
//            case GrantRewardType.QUEST_UPG_ANIMAL_14:
//            case GrantRewardType.QUEST_UPG_LNDMRK_0:
//            case GrantRewardType.QUEST_GET_NORMAL_ANIMAL:
//            case GrantRewardType.QUEST_CALL_ALBATROSS:
//            case GrantRewardType.QUEST_PHOTO_MISSION:
//            case GrantRewardType.QUEST_GET_TOUCH_REWARD:
//            case GrantRewardType.QUEST_USE_TIME_TICKET:
//            case GrantRewardType.QUEST_OPEN_RARE_ANIMAL_BOX:
//            case GrantRewardType.QUEST_FEED_ANIMAL:
//            case GrantRewardType.QUEST_ANIMAL_HEART:
//                _OnAddUtility.Raise(QUEST_REWARD_UTILITY_AMOUNT);
//                break;

//            default:
//                Debug.Log($"Failed to grant reward. GrantRewardType({typeIdx.ToString()}) is not appropriate!");
//                break;
//        }
//    }

//    private void OnPlayRewardedAdCallback(AdPointType type)
//    {
//        GrantRewardType rewardType;
//        switch (type)
//        {
//            case AdPointType.Coin:
//                rewardType = GrantRewardType.REWARDED_AD_GOLD;
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.ALBATROSS_SHOWN);
//                Signals.Get<ResetRewardedAdSettingsSignal>().Dispatch((int)GrantRewardType.REWARDED_AD_GOLD, false);
//                break;

//            case AdPointType.Heart:
//                rewardType = GrantRewardType.REWARDED_AD_HEART;
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.HEART_DOLPHIN_SHOWN);
//                Signals.Get<ResetRewardedAdSettingsSignal>().Dispatch((int)GrantRewardType.REWARDED_AD_HEART, false);
//                break;

//            case AdPointType.Offline:
//                rewardType = GrantRewardType.OFFLINE_REWARD_DOUBLE;
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.OFFLINE_REWARD_SHOWN);
//                Signals.Get<ResetRewardedAdSettingsSignal>().Dispatch((int)GrantRewardType.OFFLINE_REWARD_DOUBLE, false);
//                break;

//            case AdPointType.Gem:
//                rewardType = GrantRewardType.REWARDED_AD_JEWEL;
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.JEWEL_SHOWN);
//                break;

//            case AdPointType.Treasurebox:
//                rewardType = GrantRewardType.REWARDED_AD_RARE_ANIMAL_RANDOM_BOX;
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.TREASURE_CHEST_SHOWN);
//                break;

//            default:
//                return;
//        }

//        Signals.Get<UpdateOfflineTimeSignal>().Dispatch(false);
//        Signals.Get<AddDeferredEventSignal>().Dispatch((int)DeferredEventType.SKIP_REWARDED_AD, () =>
//        {
//            Signals.Get<SkipRewardedAdBeforeComplete>().Dispatch();
//            Signals.Get<ResetRewardedAdSettingsSignal>().Dispatch((int)rewardType, true);
//        });
//    }

//    private void OnShowRewardedAdWithCoupon(AdPointType _type)
//    {
//        int rewardTypeIdx;
//        switch (_type)
//        {
//            case AdPointType.Coin:
//                rewardTypeIdx = (int)GrantRewardType.REWARDED_AD_GOLD;
//                Signals.Get<DeactivateRewardedAdWorldUIButtonSignal>().Dispatch((int)RewardButtonType.REWARDED_AD_GOLD);
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.ALBATROSS_AD_COUPON);
//                break;

//            case AdPointType.Heart:
//                rewardTypeIdx = (int)GrantRewardType.REWARDED_AD_HEART;
//                Signals.Get<DeactivateRewardedAdWorldUIButtonSignal>().Dispatch((int)RewardButtonType.REWARDED_AD_HEART);
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.HEART_DOLPHIN_AD_COUPON);
//                break;

//            case AdPointType.Offline:
//                rewardTypeIdx = (int)GrantRewardType.OFFLINE_REWARD_DOUBLE;
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.OFFLINE_REWARD_AD_COUPON);
//                break;

//            case AdPointType.Gem:
//                rewardTypeIdx = (int)GrantRewardType.REWARDED_AD_JEWEL;
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.JEWEL_AD_COUPON);
//                break;

//            case AdPointType.Treasurebox:
//                rewardTypeIdx = (int)GrantRewardType.REWARDED_AD_RARE_ANIMAL_RANDOM_BOX;
//                GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.TREASURE_CHEST_AD_COUPON);
//                break;

//            default: return;
//        }

//        Signals.Get<ResetRewardedAdSettingsSignal>().Dispatch(rewardTypeIdx, false);
//        DoGrantReward(rewardTypeIdx);

//        m_AdvAccumulationCount++;
//        UpdateClamProgressbar?.Invoke(m_AdvAccumulationCount);
//        CheckAdvAccumulationCount();

//        if (_type != AdPointType.Gem && _type != AdPointType.Treasurebox)
//            Signals.Get<NavigateToWindowSignal>().Dispatch(null, null);

//    }

//    private void OnShowRewardedAd(AdPointType _type)
//    {
//        switch (_type)
//        {
//            case AdPointType.Coin: case AdPointType.Heart: case AdPointType.Gem: case AdPointType.Treasurebox: case AdPointType.Offline:
//                ADManager.Instance.ShowRewardedAd(_type, OnCloseRewardedAd, GrantRewards);
//                return;
//        }

//        void OnCloseRewardedAd(AdPointType type)
//        {
//#if UNITY_EDITOR
//            switch (_type)
//            {
//                case AdPointType.Coin: case AdPointType.Heart: case AdPointType.Offline:
//                    Signals.Get<NavigateToWindowSignal>().Dispatch(null, null);
//                    return;
//            }
//#elif UNITY_IOS
//            // iOS에서는 광고종료 시 백그라운드에서 돌아오는 게 아니어서 OnApplicationPause() 함수가 호출되지 않으므로 이 부분에서 오프라인 시간체크하도록 예외 처리
//            Signals.Get<UpdateOfflineTimeSignal>().Dispatch(true);
//            switch (_type)
//            {
//                case AdPointType.Coin: case AdPointType.Heart: case AdPointType.Offline:
//                    Signals.Get<NavigateToWindowSignal>().Dispatch(null, null);
//                    break;

//                case AdPointType.Gem: case AdPointType.Treasurebox:
//                    break;

//                default:
//                    return;
//            }

//            Signals.Get<InvokeDeferredEventSignal>().Dispatch((int)DeferredEventType.REWARDED_AD_GRANT_REWARDS);
//            Signals.Get<InvokeDeferredEventSignal>().Dispatch((int)DeferredEventType.SKIP_REWARDED_AD);
//#elif UNITY_ANDROID
//            Signals.Get<AddDeferredEventSignal>().Dispatch((int)DeferredEventType.REWARDED_AD_CLOSED, () =>
//            {
//                switch (_type)
//                {
//                    case AdPointType.Coin: case AdPointType.Heart: case AdPointType.Offline:
//                        Signals.Get<NavigateToWindowSignal>().Dispatch(null, null);
//                        break;

//                    case AdPointType.Gem: case AdPointType.Treasurebox:
//                        break;

//                    default:
//                        return;
//                }
//                Signals.Get<InvokeDeferredEventSignal>().Dispatch((int)DeferredEventType.SKIP_REWARDED_AD);
//            });
//#endif
//        }

//        void GrantRewards(AdPointType type, bool shouldGrant)
//        {
//#if UNITY_EDITOR
//            if (shouldGrant)
//            {
//                switch (type)
//                {
//                    case AdPointType.Coin:
//                        DoGrantReward((int)GrantRewardType.REWARDED_AD_GOLD);
//                        GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.ALBATROSS_WATCHED);
//                        Signals.Get<DeactivateRewardedAdWorldUIButtonSignal>().Dispatch((int)RewardButtonType.REWARDED_AD_GOLD);
//                        break;

//                    case AdPointType.Heart:
//                        DoGrantReward((int)GrantRewardType.REWARDED_AD_HEART);
//                        GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.HEART_DOLPHIN_WATCHED);
//                        Signals.Get<DeactivateRewardedAdWorldUIButtonSignal>().Dispatch((int)RewardButtonType.REWARDED_AD_HEART);
//                        break;

//                    case AdPointType.Gem:
//                        DoGrantReward((int)GrantRewardType.REWARDED_AD_JEWEL);
//                        GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.JEWEL_WATCHED);
//                        break;

//                    case AdPointType.Treasurebox:
//                        DoGrantReward((int)GrantRewardType.REWARDED_AD_RARE_ANIMAL_RANDOM_BOX);
//                        GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.TREASURE_CHEST_WATCHED);
//                        break;

//                    case AdPointType.Offline:
//                        DoGrantReward((int)GrantRewardType.OFFLINE_REWARD_DOUBLE);
//                        GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.OFFLINE_REWARD_WATCHED);
//                        break;

//                    default: UnityEngine.Debug.LogError($"Failed to grant rewards because AdPointType({type}) is not valid!"); break;
//                }
//                m_AdvAccumulationCount++;
//                UpdateClamProgressbar?.Invoke(m_AdvAccumulationCount);
//                CheckAdvAccumulationCount();
//            }

//#elif UNITY_ANDROID || UNITY_IOS
//            Signals.Get<AddDeferredEventSignal>().Dispatch((int)DeferredEventType.REWARDED_AD_GRANT_REWARDS, () =>
//            {
//                if (shouldGrant)
//                {
//                    switch (type)
//                    {
//                        case AdPointType.Coin:
//                            DoGrantReward((int)GrantRewardType.REWARDED_AD_GOLD);
//                            GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.ALBATROSS_WATCHED);
//                            Signals.Get<DeactivateRewardedAdWorldUIButtonSignal>().Dispatch((int)RewardButtonType.REWARDED_AD_GOLD);
//                            break;

//                        case AdPointType.Heart:
//                            DoGrantReward((int)GrantRewardType.REWARDED_AD_HEART);
//                            GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.HEART_DOLPHIN_WATCHED);
//                            Signals.Get<DeactivateRewardedAdWorldUIButtonSignal>().Dispatch((int)RewardButtonType.REWARDED_AD_HEART);
//                            break;

//                        case AdPointType.Gem:
//                            DoGrantReward((int)GrantRewardType.REWARDED_AD_JEWEL);
//                            GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.JEWEL_WATCHED);
//                            break;

//                        case AdPointType.Treasurebox:
//                            DoGrantReward((int)GrantRewardType.REWARDED_AD_RARE_ANIMAL_RANDOM_BOX);
//                            GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.TREASURE_CHEST_WATCHED);
//                            break;

//                        case AdPointType.Offline:
//                            DoGrantReward((int)GrantRewardType.OFFLINE_REWARD_DOUBLE);
//                            GAManager.Instance.GA_DesignEvent(StatisticEventInfo.AD.OFFLINE_REWARD_WATCHED);
//                            break;

//                        default: UnityEngine.Debug.LogError($"Failed to grant rewards because AdPointType({type}) is not valid!"); break;
//                    }
//                    m_AdvAccumulationCount++;
//                    UpdateClamProgressbar?.Invoke(m_AdvAccumulationCount);
//                    CheckAdvAccumulationCount();
//                    Signals.Get<RemoveDeferredEventSignal>().Dispatch((int)DeferredEventType.SKIP_REWARDED_AD);
//                }
//            });
//#endif
//        }
//    }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    private bool TryGetPhaseLevelupReward(int currLevel, out RewardData_AnimalLevelup reward)
//    {
//        if (m_LandmarkPhaseRewards == null || m_LandmarkPhaseRewards.Count == 0)
//        {
//            reward = null;
//            return false;
//        }

//        if (currLevel < 1)
//        {
//            Debug.Log("No rewards under level 1!");
//            reward = null;
//            return false;
//        }

//        int rewardIdx = currLevel - 1;
//        if (rewardIdx < m_LandmarkPhaseRewards.Count)
//        {
//            if (m_LandmarkPhaseRewards[rewardIdx].grantLevel == currLevel)
//            {
//                reward = m_LandmarkPhaseRewards[rewardIdx];
//                Debug.Log($"Found Reward\n" +
//                        $"GrantLevel : {reward.grantLevel}\n" +
//                        $"RewardAnimalType : {reward.animalType}\n" +
//                        $"IncreaseRate : {reward.increaseRate}\n");
//                return true;
//            }
//            else
//            {
//                Debug.Log($"Found animalLevelup reward but grant level({m_LandmarkPhaseRewards[rewardIdx].grantLevel}) is different with currLevel({currLevel})!");
//                reward = null;
//                return false;
//            }
//        }
//        else
//        {
//            Debug.Log("PhaseLevel has reached to max so that rewardsMult is returned to  -1.0!");
//            reward = null;
//            return false;
//        }
//    }

//    public bool TryGetNewLandmarkReward(int typeIdx, out RewardData_NewLandmark rewardData, out List<RewardWidgetData> widgetData)
//    {
//        if (!_LandmarkManager.IsValidLandmarkType(typeIdx))
//        {
//            rewardData = null;
//            widgetData = null;
//            return false;
//        }

//        if (typeIdx < m_NewLandmarkRewards.Count && m_NewLandmarkRewards[typeIdx] != null)
//        {
//            rewardData = m_NewLandmarkRewards[typeIdx];
//            widgetData = GetRewardWidgetData(m_NewLandmarkRewards[typeIdx]);
//            return true;
//        }

//        rewardData = null;
//        widgetData = null;
//        return false;


//        List<RewardWidgetData> GetRewardWidgetData(RewardData_NewLandmark newLandmarkRewardData)
//        {
//            var result = new List<RewardWidgetData>();
//            result.Add(
//                  new RewardWidgetData
//                  (
//                      RewardWidgetType.ANIMAL,
//                      _AnimalManager.GetAnimalDisplayImage(typeIdx),
//                      I2.Loc.LocalizationManager.GetTermTranslation(LocTerm.COMMON_UNLOCK)
//                  ));

//            BigNumber jewelAmount = newLandmarkRewardData.rewards_Jewel;
//            if (jewelAmount.HasValue)
//            {
//                result.Add(
//                    new RewardWidgetData
//                    (
//                        RewardWidgetType.JEWEL,
//                        _AssetReferenceManager.m_JewelImg,
//                        string.Format("x{0}", IncrementHelper.GetFormattedNumber(jewelAmount, true))
//                    ));
//            }

//            int randomBoxAmount = newLandmarkRewardData.rewards_RandomBox;
//            if (randomBoxAmount > 0)
//            {
//                result.Add(
//                    new RewardWidgetData
//                    (
//                        RewardWidgetType.RARE_ANIMAL_RANDOM_BOX_B,
//                        _AssetReferenceManager.m_RandomBox_BType_Img,
//                        string.Format("x{0}", randomBoxAmount)
//                    ));
//            }

//            BigNumber heartAmount = newLandmarkRewardData.rewards_Heart;
//            if (heartAmount.HasValue)
//            {
//                result.Add(
//                    new RewardWidgetData
//                    (
//                        RewardWidgetType.HEART,
//                        _AssetReferenceManager.m_HeartImg,
//                        string.Format("x{0}", IncrementHelper.GetFormattedNumber(heartAmount, true))
//                    ));
//            }

//            return result;
//        }
//    }

//    public bool CheckIfGrantAnimal(int level, AnimalMaxCountType type, out double rewardsMult)
//    {
//        if(TryGetPhaseLevelupReward(level, out RewardData_AnimalLevelup reward))
//        {
//            rewardsMult = reward.increaseRate;
//            return ((int)reward.animalType & (int)type) != 0;
//        }

//        rewardsMult = -1.0;
//        return false;
//    }

//    public bool TryGetRewardMultiplier(int level, out double rewardsMult)
//    {
//        if (TryGetPhaseLevelupReward(level, out RewardData_AnimalLevelup reward))
//        {
//            rewardsMult = reward.increaseRate;
//            return true;
//        }
//        else
//        {
//            rewardsMult = -1.0;
//            return false;
//        }
//    }

//    public bool TryGetLandmarkProgressForNextReward(LandmarkData data, out float progress, out bool isComplete)
//    {
//        int currShowOrder = data.CurrentShowOrder;
//        int nextShowOrder = currShowOrder + 1;
//        int currLevel = data.UpgradeLevel;

//        if (!m_LandmarkLookChangeThresholds_Internal.TryGetValue(data.DataTypeIdx, out List<int> thresholds))
//        {
//            Debug.Log($"Failed to get landmark level thresholds because there is no key for {data.DataTypeIdx} in m_LandmarkLookChangeThresholds_Internal!");
//            progress = -1.0F;
//            isComplete = false;
//            return false;
//        }

//        // 랜드마크를 이미 획득한 경우(=진화 가능)
//        if (LandmarkManager.IsShowOrderUpgradable(currShowOrder))
//        {
//            // 첫 번째 랜드마크는 회복 단계가 다르므로 예외 처리
//            if (data.DataType == LandmarkType.LANDMARK_0)
//            {
//                if (currShowOrder == (int)ShowOrder.ONE)
//                {
//                    int destLevel = thresholds[0];
//                    progress = currLevel / (float)destLevel;
//                    isComplete = currLevel >= destLevel;
//                    return true;
//                }
//                else if (currShowOrder == (int)ShowOrder.THREE)
//                {
//                    int destLevel = thresholds[1];
//                    int prevDestLevel = thresholds[0];
//                    progress = (currLevel - prevDestLevel) / (float)(destLevel - prevDestLevel);
//                    isComplete = currLevel >= destLevel;
//                    return true;
//                }
//                else
//                {
//                    Debug.LogError($"Landmark({data.DataType}) ShowOrder({currShowOrder}) is not valid!");
//                    progress = -1.0F;
//                    isComplete = false;
//                    return false;
//                }
//            }
//            else
//            {
//                int destLevel = thresholds[currShowOrder - 1];
//                if (currShowOrder == (int)ShowOrder.ONE)
//                {
//                    progress = currLevel / (float)destLevel;
//                }
//                else
//                {
//                    int prevDestLevel = thresholds[currShowOrder - 2];
//                    progress = (currLevel - prevDestLevel) / (float)(destLevel - prevDestLevel);
//                }

//                isComplete = currLevel >= destLevel;
//                return true;
//            }
//        }
//        // 랜드마크를 아직 획득하지 않은 경우
//        else if (nextShowOrder <= (int)ShowOrder.ONE)
//        {
//            Debug.Log("This landmark is not yet achieved.");
//            progress = 0.0F;
//            isComplete = false;
//            return false;
//        }
//        //  랜드마크가 외형변화 최종단계에 도달한 경우
//        else
//        {
//            Debug.Log($"Failed to get Landmark progress because it's not yet achieved or showOrder({currShowOrder}) already reached to MAX!");
//            progress = -1.0F;
//            isComplete = false;
//            return false;
//        }
//    }

//    public bool TryGetAnimalProgress(int currAnimalCount, int currPhaseLevel, AnimalMaxCountType maxCountType, out float progress, out bool shouldGrantNow)
//    {
//        if (m_LandmarkPhaseRewardsThresholds_Internal == null || m_LandmarkPhaseRewardsThresholds_Internal.Count == 0)
//        {
//            Debug.Log("Reward dictionary is null or count is zero!");
//            progress = -1.0F;
//            shouldGrantNow = false;
//            return false;
//        }

//        if (currAnimalCount < 0)
//        {
//            Debug.Log($"Failed to get AnimalProgress because currAnimalCount({currAnimalCount}) is less than zero!");
//            progress = -1.0F;
//            shouldGrantNow = false;
//            return false;
//        }

//        List<int> thresholdList = m_LandmarkPhaseRewardsThresholds_Internal[maxCountType];
//        int lastThreshold = thresholdList[thresholdList.Count - 1];

//        // 동물 개체수 또는 동물 레벨이 이미 최대치에 도달한 경우
//        if (currAnimalCount >= thresholdList.Count)
//        {
//            Debug.Log("Animal Count or phaseLevel reached to max!");
//            progress = -100.0F;
//            shouldGrantNow = false;
//            return false;
//        }

//        int prevThreshold = 0;
//        int currThreshold = thresholdList[currAnimalCount];
//        if (currAnimalCount < 1)
//        {
//            progress = currPhaseLevel / (float)currThreshold;
//        }
//        // 마지막 동물을 획득할 차례인 경우
//        else if (currPhaseLevel == lastThreshold)
//        {
//            Debug.Log("Last Animal!");
//            progress = 1.0F;
//            shouldGrantNow = true;
//            return true;
//        }
//        else
//        {
//            prevThreshold = thresholdList[currAnimalCount - 1];
//            progress = (currPhaseLevel - prevThreshold) / (float)(currThreshold - prevThreshold);
//        }

//        shouldGrantNow = currPhaseLevel == currThreshold;
//        return true;
//    }

//    private void BindLookChangeData()
//    {
//        if (m_LandmarkLookChangeThresholds == null || m_LandmarkLookChangeThresholds.Count < 1)
//        {
//            Debug.Log("Failed to bind lookChange data because m_LandmarkLookChangeThresholds is null or count is zero!");
//            return;
//        }

//        if (m_LandmarkLookChangeThresholds_Internal == null)
//            m_LandmarkLookChangeThresholds_Internal = new Dictionary<int, List<int>>();
//        else
//            m_LandmarkLookChangeThresholds_Internal.Clear();

//        for (int i = 0; i < m_LandmarkLookChangeThresholds.Count; i++)
//        {
//            int key = (int)m_LandmarkLookChangeThresholds[i].type;
//            if (!m_LandmarkLookChangeThresholds_Internal.ContainsKey(key))
//                m_LandmarkLookChangeThresholds_Internal.Add(key, new List<int>());

//            m_LandmarkLookChangeThresholds_Internal[key].Add(m_LandmarkLookChangeThresholds[i].grantLevel);
//        }
//    }

//    private void BindPhaseRewardsData()
//    {
//        if (m_LandmarkPhaseRewardsThresholdsData == null || m_LandmarkPhaseRewardsThresholdsData.Count < 1)
//        {
//            Debug.Log("Failed to bind phase reward data because m_LandmarkPhaseRewardsThresholdsData is null or count is zero!");
//            return;
//        }

//        if (m_LandmarkPhaseRewardsThresholds_Internal == null)
//            m_LandmarkPhaseRewardsThresholds_Internal = new Dictionary<AnimalMaxCountType, List<int>>();
//        else
//            m_LandmarkPhaseRewardsThresholds_Internal.Clear();

//        m_LandmarkPhaseRewardsThresholds_Internal.Add(AnimalMaxCountType.MAX_COUNT_2, new List<int>());
//        m_LandmarkPhaseRewardsThresholds_Internal.Add(AnimalMaxCountType.MAX_COUNT_3, new List<int>());
//        m_LandmarkPhaseRewardsThresholds_Internal.Add(AnimalMaxCountType.MAX_COUNT_5, new List<int>());
//        m_LandmarkPhaseRewardsThresholds_Internal.Add(AnimalMaxCountType.MAX_COUNT_7, new List<int>());

//        for (int i = 0; i < m_LandmarkPhaseRewardsThresholdsData.Count; i++)
//        {
//            switch (i)
//            {
//                case 0: m_LandmarkPhaseRewardsThresholds_Internal[AnimalMaxCountType.MAX_COUNT_7].AddRange(m_LandmarkPhaseRewardsThresholdsData[i].thresholds); break;
//                case 1: m_LandmarkPhaseRewardsThresholds_Internal[AnimalMaxCountType.MAX_COUNT_5].AddRange(m_LandmarkPhaseRewardsThresholdsData[i].thresholds); break;
//                case 2: m_LandmarkPhaseRewardsThresholds_Internal[AnimalMaxCountType.MAX_COUNT_3].AddRange(m_LandmarkPhaseRewardsThresholdsData[i].thresholds); break;
//                case 3: m_LandmarkPhaseRewardsThresholds_Internal[AnimalMaxCountType.MAX_COUNT_2].AddRange(m_LandmarkPhaseRewardsThresholdsData[i].thresholds); break;
//                default: Debug.Log($"Index({i}) is not appropriate!"); break;
//            }
//        }

//        foreach (var thresholdsByType in m_LandmarkPhaseRewardsThresholds_Internal)
//            thresholdsByType.Value.Sort();
//    }

//    private void BindNewLandmarkRewardsData()
//    {
//        if(m_NewLandmarkRewards != null && m_NewLandmarkRewards.Count > 0)
//        {
//            for (int i = 0; i < m_NewLandmarkRewards.Count; i++)
//            {
//                var reward = m_NewLandmarkRewards[i];
//                List<Action> callbacks = new List<Action>();

//                bool grantJewel = reward.rewards_Jewel.HasValue;
//                bool grantHeart = reward.rewards_Heart.HasValue;
//                bool grantRandomBox = reward.rewards_RandomBox > 0;

//                if (grantJewel)
//                    callbacks.Add(() =>
//                    {
//                        _OnAddJewel.Raise(reward.rewards_Jewel);
//                        int grantAmount = IncrementHelper.ConvertBigNumberToInteger(reward.rewards_Jewel);
//                        if (grantAmount > 0)
//                        GAManager.Instance.GA_ResourceEvent(false, StatisticEventInfo.CurrencyType.JEWEL, grantAmount, StatisticEventInfo.ResourceType.UPGRADE, StatisticEventInfo.Jewel.LANDMARK_CREATE);
//                    });

//                if (grantHeart)
//                    callbacks.Add(() =>
//                    {
//                        Signals.Get<AddHeartSignal>().Dispatch(reward.rewards_Heart, (int)DeferredEventType.NONE);
//                    });

//                if (grantRandomBox)
//                {
//                    callbacks.Add(() => Signals.Get<AddRandomBoxSignal>().Dispatch(1, reward.rewards_RandomBox));
//                }

//                if(grantJewel || grantHeart || grantRandomBox)
//                {
//                    callbacks.Add(DoGrantRewardEffect);

//                    void DoGrantRewardEffect()
//                    {
//                        Signals.Get<AddDeferredEventSignal>().Dispatch((int)DeferredEventType.NEW_LANDMARK, () =>
//                        {
//                            if (grantJewel) { Signals.Get<PlayCoinEffectSignal>().Dispatch((int)CoinEffectType.JEWEL, (int)UIReferencePointType.NONE); }
//                            if (grantHeart) { Signals.Get<PlayCoinEffectSignal>().Dispatch((int)CoinEffectType.HEART, (int)UIReferencePointType.NONE); }
//                            if (grantRandomBox) { Signals.Get<PlayCoinEffectSignal>().Dispatch((int)CoinEffectType.RARE_BOX, (int)UIReferencePointType.NONE); }
//                        });
//                    }
//                }

//                reward.callbacks = callbacks;
//            }
//        }
//    }

//    public void CheckAdvAccumulationCount()
//    {
//        if (IsMaxAdvAccumulation)
//        {
//            if (IsReadyAdvAccumulation)
//            {
//                _OnPlayTimeline.Raise((int)TimelineEvent.AD_ACCUMULATION_OPEN);
//            }
//            else
//            {
//                _OnPlayTimeline.Raise((int)TimelineEvent.AD_ACCUMULATION_FIRST_READY);
//                Signals.Get<NavigateToWindowSignal>().Dispatch(null, null);
//            }
//        }

//        else
//        {
//            if (IsReadyAdvAccumulation)
//                _OnPlayTimeline.Raise((int)TimelineEvent.AD_ACCUMULATION_IDLE_1);
//        }
//    }

//    public void ResetAdvAccumulationCount()
//    {
//        m_AdvAccumulationCount = 0;
//        UpdateClamProgressbar?.Invoke(m_AdvAccumulationCount);
//    }


//    //////////////////////////////////////////
//    // Remote Initialize
//    //////////////////////////////////////////

//    public void OnRemoteInitialize(RemoteInitType type, params object[] args)
//    {
//#if UNITY_EDITOR || DEVELOPMENT_BUILD
//        if (type != RemoteInitType.REWARD_DATA)
//            return;

//        const int paramsCount = 19;
//        if (args.Length != paramsCount)
//        {
//            Debug.Log($"Arguments count is different with paramsCount({paramsCount})! Remote initialization process failed!");
//            return;
//        }

//        List<int> grantLevels = (List<int>)args[0];
//        List<string> rewardSubTypes = (List<string>)args[1];
//        List<double> increaseRates = (List<double>)args[2];
//        List<List<int>> lookChangeThresholds = (List<List<int>>)args[15];
//        List<BigNumber> newLandmarkRewards_Jewel = (List<BigNumber>)args[16];
//        List<int> newLandmarkRewards_RandomBox = (List<int>)args[17];
//        List<BigNumber> newLandmarkRewards_Heart = (List<BigNumber>)args[18];

//        if ((grantLevels == null || grantLevels.Count == 0)
//            || (rewardSubTypes == null || rewardSubTypes.Count == 0)
//            || (increaseRates == null || increaseRates.Count == 0)
//            || (grantLevels.Count != rewardSubTypes.Count)
//            || (rewardSubTypes.Count != increaseRates.Count)
//            || (newLandmarkRewards_Jewel == null || newLandmarkRewards_Jewel.Count == 0)
//            || (newLandmarkRewards_RandomBox == null || newLandmarkRewards_RandomBox.Count == 0)
//            || (newLandmarkRewards_Heart == null || newLandmarkRewards_Heart.Count == 0)
//            || (newLandmarkRewards_Jewel.Count != newLandmarkRewards_RandomBox.Count)
//            || (newLandmarkRewards_RandomBox.Count != newLandmarkRewards_Heart.Count)
//            || (newLandmarkRewards_Jewel.Count != newLandmarkRewards_Heart.Count))
//        {
//            Debug.LogError("UpdateData cannot be done!");
//            return;
//        }

//        // Initialize Landmark Rewards
//        if (m_LandmarkLookChangeThresholds == null)
//            m_LandmarkLookChangeThresholds = new List<RewardData_LandmarkLookChange>();
//        else
//            m_LandmarkLookChangeThresholds.Clear();

//        if (m_LandmarkPhaseRewards == null)
//            m_LandmarkPhaseRewards = new List<RewardData_AnimalLevelup>();
//        else
//            m_LandmarkPhaseRewards.Clear();

//        if (m_LandmarkPhaseRewardsThresholdsData == null)
//            m_LandmarkPhaseRewardsThresholdsData = new List<PhaseRewardsThreshold>();
//        else
//            m_LandmarkPhaseRewardsThresholdsData.Clear();

//        if (m_NewLandmarkRewards == null)
//            m_NewLandmarkRewards = new List<RewardData_NewLandmark>();
//        else
//            m_NewLandmarkRewards.Clear();

//        m_LandmarkPhaseRewardsThresholdsData.Add(new PhaseRewardsThreshold(AnimalMaxCountType.MAX_COUNT_7));
//        m_LandmarkPhaseRewardsThresholdsData.Add(new PhaseRewardsThreshold(AnimalMaxCountType.MAX_COUNT_5));
//        m_LandmarkPhaseRewardsThresholdsData.Add(new PhaseRewardsThreshold(AnimalMaxCountType.MAX_COUNT_3));
//        m_LandmarkPhaseRewardsThresholdsData.Add(new PhaseRewardsThreshold(AnimalMaxCountType.MAX_COUNT_2));

//        for (int i = 0; i < lookChangeThresholds.Count; i++)
//            for (int j = 0; j < lookChangeThresholds[i].Count; j++)
//                m_LandmarkLookChangeThresholds.Add(new RewardData_LandmarkLookChange((LandmarkType)i, lookChangeThresholds[i][j]));

//        for (int i = 0; i < grantLevels.Count; i++)
//        {
//            m_LandmarkPhaseRewards.Add(
//                new RewardData_AnimalLevelup(
//                    grantLevels[i],
//                    GetRewardAnimalType(rewardSubTypes[i], grantLevels[i]),
//                    increaseRates[i])
//                );
//        }

//        for (int i = 0; i < newLandmarkRewards_Jewel.Count; i++)
//        {
//            m_NewLandmarkRewards.Add(
//                new RewardData_NewLandmark(
//                    (LandmarkType)i,
//                    newLandmarkRewards_Jewel[i],
//                    newLandmarkRewards_RandomBox[i],
//                    newLandmarkRewards_Heart[i]
//                ));
//        }

//        BindLookChangeData();
//        BindPhaseRewardsData();

//        // Initialize Rewards Multipliers
//        m_RewardedAdGoldMultiplier_Default = (double)args[3];
//        m_RewardedAdHeartMultiplier_Default = (double)args[4];
//        m_RewardedTouchGoldMultiplier_Default = (double)args[5];
//        m_RewardedPictureGoldMultiplier_Default = (double)args[6];

//        m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_DECREASE_UPGRADE_COST = (double)args[7];
//        m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_LANDMARK_GOLD_OUTPUT = (double)args[8];
//        m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_REWARDED_AD_GOLD = (double)args[9];
//        m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_GOLDBOX = (double)args[10];
//        m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_ANIMAL_HEART_OUTPUT= (double)args[11];
//        m_GrowthRewardsAmount.GROWTH_REWARD_ADDITIVE_RATE_INCREASE_CHANCE_DOUBLEHEART = (double)args[12];
//        m_GrowthRewardsAmount.GROWTH_REWARD_ADDITIVE_RATE_INCREASE_REWARD_OFFLINE = (double)args[13];
//        m_GrowthRewardsAmount.GROWTH_REWARD_MULTIPLIER_RATE_INCREASE_REWARD_TAKE_A_PICTURE = (double)args[14];


//        AnimalMaxCountType GetRewardAnimalType(string rewardAnimalType, int grantLevel)
//        {
//            int sum = 0;
//            if (rewardAnimalType.Contains("A"))
//            {
//                sum += (int)AnimalMaxCountType.MAX_COUNT_7;
//                m_LandmarkPhaseRewardsThresholdsData[0].thresholds.Add(grantLevel);
//            }

//            if (rewardAnimalType.Contains("B"))
//            {
//                sum += (int)AnimalMaxCountType.MAX_COUNT_5;
//                m_LandmarkPhaseRewardsThresholdsData[1].thresholds.Add(grantLevel);
//            }

//            if (rewardAnimalType.Contains("C"))
//            {
//                sum += (int)AnimalMaxCountType.MAX_COUNT_3;
//                m_LandmarkPhaseRewardsThresholdsData[2].thresholds.Add(grantLevel);
//            }

//            if (rewardAnimalType.Contains("D"))
//            {
//                sum += (int)AnimalMaxCountType.MAX_COUNT_2;
//                m_LandmarkPhaseRewardsThresholdsData[3].thresholds.Add(grantLevel);
//            }

//            return (AnimalMaxCountType)sum;
//        }
//#else
//        Debug.Log("RemoteInitialize only works on UNITY_EDITOR or DEVELOPMENT_BUILD!");
//#endif
//    }
//}
