//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using ScriptableObjectArchitecture;
//using deVoid.Utils;

//public class ResetRewardedAdSettingsSignal : ASignal<int, bool> { }
//public class UpdateOfflineTimeSignal : ASignal<bool> { }
//public class TriggerRewardTimer : ASignal<int> { }

//public enum RewardTimer
//{
//    NONE = -1, REWARDED_AD_GOLD, REWARDED_AD_HEART, TOUCH_OBJECT, PHOTO_MISSION
//}

////[CreateAssetMenu]
//public class IdleManager : ScriptableObject, IInitializable, IRemoteInitializable
//{
//    public bool IsInitialized { get; private set; }

//    [Header("Heart Infos")]
//    private float m_HeartGenTimer;
//    private BigNumber m_FinalHeartOutput;
//    [SerializeField] private BigNumberData m_PureHeartOutput;
//    [SerializeField] private float[] m_HeartOutputIntervals;
//    [SerializeField] private int[] m_HeartOutputIntervalThresholds;
//    public float m_HeartOutputInterval;
//    public bool m_CheckHeartGenTimer = false;
//    public bool m_IsHeartAutoClick = false;

//    public double OfflineRewardHeartMultiplier => (double)m_OfflineRewardTimer / (double)m_HeartOutputInterval;
//    public double OfflineRewardHeartMultiplierDouble => (double)PrevOfflineRewardTimer / (double)m_HeartOutputInterval;
//    public BigNumber FinalHeartOutput => m_FinalHeartOutput;

//    [Header("Rewards Infos")]
//    public Action<TouchObjectPosition, bool> OnCreate; //생성 콜백.
//    public Action<int> OnSetJewelTimer; //생성 콜백.
//    public Action<int> OnSetRandomBoxTimer; //생성 콜백.

//    [SerializeField] private float m_RewardedAdGoldTimer;
//    [SerializeField] private float m_RewardedAdHeartTimer;
//    [SerializeField] private float m_RewardedTouchGoldTimer;
//    [SerializeField] private float m_RewardedCameraTimer;
//    public float m_RandomBoxFreeTimer;
//    public float m_JewelFreeTimer;

//    [SerializeField] private double m_OfflineRewardTimer;

//    [SerializeField] private float m_RewardedAdGoldInterval;
//    [SerializeField] private float m_RewardedAdHeartInterval;
//    [SerializeField] private float m_RewardedTouchGoldInterval;
//    [SerializeField] private float m_RewardedCameraInterval;
//    [SerializeField] private float m_RandomBoxFreeInterval;
//    [SerializeField] private float m_JewelFreeInterval;
//    [SerializeField] private double m_OfflineRewardTimer_Min;

//    private bool m_IsRewardAdGoldTimerTriggered = false;
//    private bool m_IsRewardAdHeartTimerTriggered = false;
//    private bool m_IsTouchObjectTimerTriggered = false;
//    private bool m_IsPhotoMissionTimerTriggered = false;

//    public bool m_CheckRewardedAdGoldTimer = false;
//    public bool m_CheckRewardedAdHeartTimer = false;
//    public bool m_CheckRewardedTouchGoldTimer = false;
//    public bool m_CheckOfflineRewardTimer = false;
//    public bool m_CheckRewardedCameraTimer = false;
//    public bool m_CheckRandomBoxFreeTimer = false;
//    public bool m_CheckJewelFreeTimer = false;

//    public bool m_IsRewardedAdGoldReady = false;
//    public bool m_IsRewardedAdHeartReady = false;
//    public bool m_IsRewardedTouchGoldReady = false;
//    public bool m_IsRewardedCameraReady = false;
//    public bool m_IsRandomBoxFreeReady = false;
//    public bool m_IsJewelFreeReady = false;
//    public bool IsOfflineRewardReady => m_OfflineRewardTimer >= m_OfflineRewardTimer_Min;
//    public bool IsTouchObjectTimerTriggered => m_IsTouchObjectTimerTriggered;

//    public double OfflineRewardTimer => m_OfflineRewardTimer;
//    public double PrevOfflineRewardTimer { get; private set; } = 0.0;

//    public static Action OnTouchObjectQuestCleared;


//    [Header("SO Managers")]
//    [SerializeField] private AnimalManager _AnimalManager;
//    [SerializeField] private LandmarkManager _LandmarkManager;
//    [SerializeField] private RewardManager _RewardManager;

//    [Header("SO Events")]
//    [SerializeField] private IntGameEvent _OnPlayTimeline = default(IntGameEvent);
//    [SerializeField] private IntGameEvent _OnProgressQuest = default(IntGameEvent);

//    // DEBUG
//    public string RemainingTime_TouchReward => string.Format("남은 시간 : {0:#0}초", Mathf.Max(m_RewardedTouchGoldInterval - m_RewardedTouchGoldTimer, 0.0F));
//    public string RemainingTime_PictureReward => string.Format("남은 시간 : {0:#0}초", Mathf.Max(m_RewardedCameraInterval - m_RewardedCameraTimer, 0.0F));


//    //////////////////////////////////////////
//    // Initialize & Reset
//    //////////////////////////////////////////

//    public void Initialize(SaveData saveData)
//    {
//        IsInitialized = false;

//        SaveData.CommonData _data = saveData.GetCommonData;

//        m_PureHeartOutput.Value = _data.PURE_HEART_OUTPUT;

//        m_IsRewardedAdGoldReady = _data.REWARDED_AD_GOLD_READY;
//        m_IsRewardedAdHeartReady = _data.REWARDED_AD_HEART_READY;
//        m_IsRewardedTouchGoldReady = _data.REWARDED_TOUCH_GOLD_READY;
//        m_IsRewardedCameraReady = _data.REWARDED_CAMERA_READY;
//        m_IsRandomBoxFreeReady = _data.RANDOM_BOX_READY;
//        m_IsJewelFreeReady = _data.JEWEL_READY;

//        m_RewardedAdGoldTimer = _data.REWARDED_AD_GOLD_TIMER;
//        m_RewardedAdHeartTimer = _data.REWARDED_AD_HEART_TIMER;
//        m_OfflineRewardTimer = _data.OFFLINE_REWARD_TIMER;
//        m_RewardedTouchGoldTimer = _data.REWARDED_TOUCH_GOLD_TIMER;
//        m_RewardedCameraTimer = _data.REWARDED_CAMERA_TIMER;
//        m_RandomBoxFreeTimer = _data.RANDOM_BOX_TIMER;
//        m_JewelFreeTimer = _data.JEWEL_TIMER;

//        m_IsRewardAdGoldTimerTriggered = _data.TIMER_TRIGGERED_REWARDED_AD_GOLD;
//        m_IsRewardAdHeartTimerTriggered = _data.TIMER_TRIGGERED_REWARDED_AD_HEART;
//        m_IsTouchObjectTimerTriggered = _data.TIMER_TRIGGERED_TOUCH_OBJECT;
//        m_IsPhotoMissionTimerTriggered = _data.TIMER_TRIGGERED_PHOTO_MISSION;

//        m_HeartGenTimer = 0.0F;
//        m_CheckHeartGenTimer
//            = m_IsHeartAutoClick
//            = true;

//        m_CheckRewardedAdGoldTimer = !m_IsRewardedAdGoldReady && m_IsRewardAdGoldTimerTriggered;
//        m_CheckRewardedAdHeartTimer = !m_IsRewardedAdHeartReady && m_IsRewardAdHeartTimerTriggered;
//        m_CheckOfflineRewardTimer = true;
//        m_CheckRewardedTouchGoldTimer = !m_IsRewardedTouchGoldReady;
//        m_CheckRewardedCameraTimer = !m_IsRewardedCameraReady && m_IsPhotoMissionTimerTriggered;
//        m_CheckRandomBoxFreeTimer = !m_IsRandomBoxFreeReady;
//        m_CheckJewelFreeTimer = !m_IsJewelFreeReady;

//        UpdateOfflineTimeOnConnection();

//        OverrideSettingsByTutorialState(TutorialEventTracker.NewbieTutorialPhase);

//        GameManager.WillCompleteInitialize += UpdateIdleValues;
//        AnimalData.AddAnimalCompleteEvent += AddAnimalCompleteEventHandler;
//        UIGen_Heart.GenerateHeartEvent += GenerateHeartEventHandler;
//        SaveLoadManager.SaveEvent += OnSave;

//        Signals.Get<ResetRewardedAdSettingsSignal>().AddListener(ResetRewardedAdSettings);
//        Signals.Get<UpdateOfflineTimeSignal>().AddListener(OnUpdateOfflineTime);
//        Signals.Get<UpdateNetworkStateSignal>().AddListener(OnUpdateNetworkState);
//        Signals.Get<TriggerRewardTimer>().AddListener(OnTriggerRewardTimer);

//        IsInitialized = true;


//        void OverrideSettingsByTutorialState(TutorialPhase tutorialPhase)
//        {
//            switch (tutorialPhase)
//            {
//                case TutorialPhase.NONE:
//                case TutorialPhase.SEEDING:
//                case TutorialPhase.PURIFY:
//                case TutorialPhase.GET_ENERGY:
//                case TutorialPhase.UPGRADE_LANDMARK:
//                case TutorialPhase.ACHIEVE_LEVEL_FIVE:
//                case TutorialPhase.UPGRADE_ANIMAL:
//                case TutorialPhase.AUTO_GEN_ENERGY:
//                case TutorialPhase.ACHIEVE_LEVEL_TEN:
//                case TutorialPhase.ZENMODE_ONE:
//                case TutorialPhase.ZENMODE_TWO:
//                case TutorialPhase.NEWBIE_TUTORIAL_END:
//                    m_IsRewardedAdGoldReady
//                        = m_IsRewardedAdHeartReady
//                        = m_IsRewardedTouchGoldReady
//                        = m_IsRewardedCameraReady
//                        = m_CheckRewardedAdGoldTimer
//                        = m_CheckRewardedAdHeartTimer
//                        = m_CheckOfflineRewardTimer
//                        = m_CheckRewardedTouchGoldTimer
//                        = m_CheckRewardedCameraTimer
//                        = m_CheckHeartGenTimer
//                        = m_IsHeartAutoClick
//                        = false;
//                    TutorialEventTracker.OnCompleteNewbieTutorial += DoCompleteNewbieTutorial;
//                    break;

//                case TutorialPhase.COMPLETE:
//                    Debug.Log("No Override because newbie tutorial is complete.");
//                    break;

//                default:
//                    Debug.Log($"Failed to OverrideSettingsByTutorialState because TutorialPhase({tutorialPhase}) is not appropriate!");
//                    break;
//            }
//        }
//    }
    
//    public void DoReset()
//    {
//        GameManager.WillCompleteInitialize -= UpdateIdleValues;
//        AnimalData.AddAnimalCompleteEvent -= AddAnimalCompleteEventHandler;
//        UIGen_Heart.GenerateHeartEvent -= GenerateHeartEventHandler;
//        TutorialEventTracker.OnCompleteNewbieTutorial -= DoCompleteNewbieTutorial;
//        SaveLoadManager.SaveEvent -= OnSave;

//        Signals.Get<ResetRewardedAdSettingsSignal>().RemoveListener(ResetRewardedAdSettings);
//        Signals.Get<UpdateOfflineTimeSignal>().RemoveListener(OnUpdateOfflineTime);
//        Signals.Get<UpdateNetworkStateSignal>().RemoveListener(OnUpdateNetworkState);
//        Signals.Get<TriggerRewardTimer>().RemoveListener(OnTriggerRewardTimer);

//        IsInitialized = false;
//    }

//    public void OnSave(SaveData data)
//    {
//        SaveData.CommonData dataToSave = data.GetCommonData;
//        if (dataToSave == null)
//        {
//            Debug.LogError("Failed to save IdleManager because CommonData is null!");
//            return;
//        }

//        dataToSave.PURE_HEART_OUTPUT = m_PureHeartOutput.Value;

//        dataToSave.REWARDED_AD_GOLD_READY = m_IsRewardedAdGoldReady;
//        dataToSave.REWARDED_AD_HEART_READY = m_IsRewardedAdHeartReady;
//        dataToSave.REWARDED_TOUCH_GOLD_READY = m_IsRewardedTouchGoldReady;
//        dataToSave.REWARDED_CAMERA_READY = m_IsRewardedCameraReady;
//        dataToSave.RANDOM_BOX_READY = m_IsRandomBoxFreeReady;
//        dataToSave.JEWEL_READY = m_IsJewelFreeReady;

//        dataToSave.REWARDED_AD_GOLD_TIMER = m_RewardedAdGoldTimer;
//        dataToSave.REWARDED_AD_HEART_TIMER = m_RewardedAdHeartTimer;
//        dataToSave.OFFLINE_REWARD_TIMER = m_OfflineRewardTimer;
//        dataToSave.REWARDED_TOUCH_GOLD_TIMER = m_RewardedTouchGoldTimer;
//        dataToSave.REWARDED_CAMERA_TIMER = m_RewardedCameraTimer;
//        dataToSave.RANDOM_BOX_TIMER = m_RandomBoxFreeTimer;
//        dataToSave.JEWEL_TIMER = m_JewelFreeTimer;

//        dataToSave.TIMER_TRIGGERED_REWARDED_AD_GOLD = m_IsRewardAdGoldTimerTriggered;
//        dataToSave.TIMER_TRIGGERED_REWARDED_AD_HEART = m_IsRewardAdHeartTimerTriggered;
//        dataToSave.TIMER_TRIGGERED_TOUCH_OBJECT = m_IsTouchObjectTimerTriggered;
//        dataToSave.TIMER_TRIGGERED_PHOTO_MISSION = m_IsPhotoMissionTimerTriggered;
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void UpdateIdleValues()
//    {
//        UpdateLandmarkFinalGoldOutputs();
//        UpdateLandmarkFinalUpgradeCosts();
//        UpdateFinalHeartOutput();
//        UpdateHeartOutputInterval();
//        Signals.Get<UpdateEasyButtonSignal>().Dispatch();

//        GameManager.WillCompleteInitialize -= UpdateIdleValues;
//    }

//    private void DoCompleteNewbieTutorial()
//    {
//        TutorialEventTracker.OnCompleteNewbieTutorial -= DoCompleteNewbieTutorial;

//        m_CheckOfflineRewardTimer
//            = m_CheckHeartGenTimer
//            = m_IsHeartAutoClick
//            = true;
//    }

//    private void ResetRewardedAdSettings(int typeIdx, bool isReady)
//    {
//        Debug.Log($"ResetRewardedAdSettings : {(GrantRewardType)typeIdx} , {isReady}");
//        switch (typeIdx)
//        {
//            case (int)GrantRewardType.REWARDED_AD_GOLD:
//                m_IsRewardedAdGoldReady = isReady;
//                m_CheckRewardedAdGoldTimer = !isReady;
//                m_RewardedAdGoldTimer = 0.0F;
//                break;

//            case (int)GrantRewardType.REWARDED_AD_HEART:
//                m_IsRewardedAdHeartReady = isReady;
//                m_CheckRewardedAdHeartTimer = !isReady;
//                m_RewardedAdHeartTimer = 0.0F;
//                break;

//            case (int)GrantRewardType.REWARDED_TOUCH_GOLD:
//                m_IsRewardedTouchGoldReady = isReady;
//                m_CheckRewardedTouchGoldTimer = !isReady;
//                m_RewardedTouchGoldTimer = 0.0F;
//                break;

//            case (int)GrantRewardType.REWARDED_PICTURE_GOLD:
//                m_IsRewardedCameraReady = isReady;
//                m_CheckRewardedCameraTimer = !isReady;
//                m_RewardedCameraTimer = 0.0F;
//                break;

//            case (int)GrantRewardType.OFFLINE_REWARD:
//                m_OfflineRewardTimer = 0.0;
//                break;

//            case (int)GrantRewardType.OFFLINE_REWARD_DOUBLE:
//                if (isReady)
//                    m_OfflineRewardTimer = PrevOfflineRewardTimer;
//                else
//                {
//                    PrevOfflineRewardTimer = m_OfflineRewardTimer;
//                    m_OfflineRewardTimer = 0.0;
//                }
//                break;

//            case (int)GrantRewardType.REWARDED_AD_JEWEL:
//                m_IsJewelFreeReady = isReady;
//                m_CheckJewelFreeTimer = !isReady;
//                Signals.Get<UpdateProductBoxSignal>().Dispatch();
//                break;

//            case (int)GrantRewardType.REWARDED_AD_RARE_ANIMAL_RANDOM_BOX:
//                Debug.Log("Box Ad Clear");
//                m_IsRandomBoxFreeReady = isReady;
//                m_CheckRandomBoxFreeTimer = !isReady;
//                Signals.Get<UpdateProductBoxSignal>().Dispatch();
//                break;

//            default:
//                return;
//        }
//    }

//    private void OnUpdateOfflineTime(bool connected)
//    {
//        if (connected)
//            UpdateOfflineTimeOnConnection();
//        else
//            UpdateOfflineTimeOnDisconnection();
//    }

//    private void OnUpdateNetworkState(bool connected)
//    {
//        if (m_CheckOfflineRewardTimer)
//        {
//            // 네트워크 연결이 복원된 경우
//            if (connected)
//            {
//                UpdateOfflineTimeOnConnection();

//                if (m_IsRewardedAdGoldReady) _OnPlayTimeline.Raise((int)TimelineEvent.REWARDED_AD_GOLD_READY);
//                if (m_IsRewardedAdHeartReady) _OnPlayTimeline.Raise((int)TimelineEvent.REWARDED_AD_HEART_READY);
//                if (IsOfflineRewardReady) _OnPlayTimeline.Raise((int)TimelineEvent.OFFLINE_REWARD_READY);
//            }
//            // 네트워크 연결이 소실된 경우
//            else
//                UpdateOfflineTimeOnDisconnection();
//        }
//    }

//    private void OnTriggerRewardTimer(int typeIdx)
//    {
//        switch (typeIdx)
//        {
//            case (int)RewardTimer.REWARDED_AD_GOLD: m_CheckRewardedAdGoldTimer = true; break;
//            case (int)RewardTimer.REWARDED_AD_HEART: m_CheckRewardedAdHeartTimer = true; break;
//            case (int)RewardTimer.TOUCH_OBJECT: OnTouchObjectQuestCleared?.Invoke(); break;
//            case (int)RewardTimer.PHOTO_MISSION: m_CheckRewardedCameraTimer = true; break;
//            default: Debug.Log($"Failed to trigger reward timer because typeIdx({typeIdx}) is not valid!"); break;
//        }
//    }


//    //////////////////////////////////////////
//    // Update
//    //////////////////////////////////////////

//    public void UpdateIdle(float dt)
//    {
//        #region Generate Gold

//        _LandmarkManager.CheckGoldGenTimer(dt);

//        #endregion


//        #region Generate Heart

//        if (m_CheckHeartGenTimer)
//        {
//            m_HeartGenTimer += dt;

//            // 하트생성 쿨타임 체크
//            if (m_HeartGenTimer >= m_HeartOutputInterval)
//            {
//                AnimalAI animal;
//                if (_AnimalManager.TryGetHeartlessAnimal(out animal))
//                    animal.ToggleHeartButton();

//                m_HeartGenTimer = 0.0F;
//            }
//        }

//        #endregion


//        #region Check Rewards Timer

//        if (m_CheckRewardedAdGoldTimer)
//        {
//            m_RewardedAdGoldTimer += dt;

//            // 리워드 광고(골드) 쿨타임 체크
//            if (m_RewardedAdGoldTimer >= m_RewardedAdGoldInterval)
//            {
//                m_CheckRewardedAdGoldTimer = false;
//                m_IsRewardedAdGoldReady = true;
//                _OnPlayTimeline.Raise((int)TimelineEvent.REWARDED_AD_GOLD_READY);
//                m_RewardedAdGoldTimer = 0.0F;
//            }
//        }

//        if (m_CheckRewardedAdHeartTimer)
//        {
//            m_RewardedAdHeartTimer += dt;

//            // 리워드 광고(하트) 쿨타임 체크
//            if (m_RewardedAdHeartTimer >= m_RewardedAdHeartInterval)
//            {
//                m_CheckRewardedAdHeartTimer = false;
//                m_IsRewardedAdHeartReady = true;
//                _OnPlayTimeline.Raise((int)TimelineEvent.REWARDED_AD_HEART_READY);
//                m_RewardedAdHeartTimer = 0.0F;
//            }
//        }

//        if (m_CheckRandomBoxFreeTimer)
//        {
//            m_RandomBoxFreeTimer -= dt;
//            OnSetRandomBoxTimer?.Invoke((int)m_RandomBoxFreeTimer);
//            if (m_RandomBoxFreeTimer <= 0f)
//            {
//                m_CheckRandomBoxFreeTimer = false;
//                m_IsRandomBoxFreeReady = true;
//                Signals.Get<UpdateProductBoxSignal>().Dispatch();
//                m_RandomBoxFreeTimer = m_RandomBoxFreeInterval;
//            }
//        }

//        if (m_CheckJewelFreeTimer)
//        {
//            m_JewelFreeTimer -= dt;

//            OnSetJewelTimer?.Invoke((int)m_JewelFreeTimer);
//            if (m_JewelFreeTimer <= 0f)
//            {
//                m_CheckJewelFreeTimer = false;
//                m_IsJewelFreeReady = true;
//                Signals.Get<UpdateProductBoxSignal>().Dispatch();
//                m_JewelFreeTimer = m_JewelFreeInterval;
//            }
//        }

//        // NEED_TO_REFACTOR : 매번 동물 마릿수 체크하지 않도록 리팩터링
//        if (m_CheckRewardedCameraTimer && _AnimalManager.NormalAnimalTotalCount > 0)
//        {
//            m_RewardedCameraTimer += dt;

//            // 사진찍기 보상(골드) 쿨타임 체크
//            if (m_RewardedCameraTimer >= m_RewardedCameraInterval)
//            {
//                m_CheckRewardedCameraTimer = false;
//                m_IsRewardedCameraReady = true;

//                _AnimalManager.TrySetRandomCameraAnimal();

//                m_RewardedCameraTimer = 0.0F;
//            }
//        }
//        #endregion
//    }

//    /// <summary>
//    /// 랜드마크 골드 생산량을 업데이트합니다.
//    /// </summary>
//    /// <param name="idx">-1이면 모든 랜드마크, 0보다 크거나 같으면 특정 랜드마크</param>
//    public void UpdateLandmarkFinalGoldOutputs(int idx = -1)
//    {
//        _LandmarkManager.UpdateLandmarkFinalGoldOutputs(_RewardManager.FinalMult_LandmarkGoldOutput, idx);
//        Signals.Get<UpdatePlayerStatsSignal>().Dispatch(PlayerStat.GOLD_OUTPUT);
//        Signals.Get<UpdateEasyButtonSignal>().Dispatch();
//    }

//    public void UpdateFinalHeartOutput()
//    {
//        m_FinalHeartOutput = IncrementHelper.Multiply(m_PureHeartOutput.Value, _RewardManager.FinalMult_HeartOutput);
//    }

//    /// <summary>
//    /// 언락 여부에 상관없이 최종 업그레이드 가격을 업데이트합니다.
//    /// </summary>
//    /// <param name="idx">-1이면 모든 랜드마크, 0보다 크거나 같으면 특정 랜드마크</param>
//    public void UpdateLandmarkFinalUpgradeCosts(int idx = -1)
//    {
//        _LandmarkManager.UpdateLandmarkFinalUpgradeCosts(_RewardManager.FinalMult_LandmarkUpgradeCost, idx);
//        Signals.Get<UpdateUnitBoxSignal>().Dispatch(UnitBox.LANDMARK);
//        Signals.Get<UpdateEasyButtonSignal>().Dispatch();
//    }

//    /// <summary>
//    /// 동물 마릿수에 따라 하트 생산주기를 업데이트합니다.
//    /// </summary>
//    public void UpdateHeartOutputInterval()
//    {
//        int animalTotalCount = _AnimalManager.NormalAnimalTotalCount;
//        for (int i = 0; i < m_HeartOutputIntervalThresholds.Length; i++)
//        {
//            if (animalTotalCount >= m_HeartOutputIntervalThresholds[i])
//                continue;
//            else
//            {
//                if (animalTotalCount == 0)
//                    m_HeartOutputInterval = m_HeartOutputIntervals[0];
//                else
//                    m_HeartOutputInterval = m_HeartOutputIntervals[i - 1];

//                return;
//            }
//        }

//        m_HeartOutputInterval = m_HeartOutputIntervals[m_HeartOutputIntervals.Length - 1];
//    }

//    /// <summary>
//    /// 아래 경우에 호출된다.
//    /// 앱을 실행한 경우, 광고시청 완료 후 인게임으로 돌아온 경우, 백그라운드에서 돌아온 경우, 네트워크가 다시 연결된 경우
//    /// </summary>
//    private void UpdateOfflineTimeOnConnection()
//    {
//        // 종료시간이 0보다 작거나 같은 경우, 오프라인 타이머 체크 플래그가 False인 경우, 네트워크가 끊어진 경우에는 수행하지 않도록 예외 처리
//        if (PlayerStateManager.USER_EXIT_TIME <= 0L
//            || !m_CheckOfflineRewardTimer
//            || Application.internetReachability == NetworkReachability.NotReachable)
//        {
//            Debug.Log($"Failed to UpdateOfflineTimeOnConnection()!" +
//                $"\nUSER_EXIT_TIME : {PlayerStateManager.USER_EXIT_TIME.ToString()}, " +
//                $"\nm_CheckOfflineRewardTimer : {m_CheckOfflineRewardTimer.ToString()}, " +
//                $"\nNetwork Connection : {Application.internetReachability != NetworkReachability.NotReachable}");
//            return;
//        }

//        // NEED_TO_REFACTOR : 서버 API 사용 전까지 임시로 디바이스 시간 사용
//        long currentServerTime = ServerTimer.ConvertDateTimeIntoTimeStamp(DateTime.Now);
//        int timeDiff = (int)(currentServerTime - PlayerStateManager.USER_EXIT_TIME);
//        double sum = m_OfflineRewardTimer + timeDiff;

//        if (m_OfflineRewardTimer_Min <= sum)
//        {
//            double maxTime = _RewardManager.FinalAdd_MaxOfflineTime;
//            if (maxTime <= sum)
//            {
//                Debug.Log($"OfflineRewardTimer({sum}) is over maxTime({maxTime}) : TimeDiff({timeDiff}) , Sum({sum})");
//                m_OfflineRewardTimer = maxTime;
//            }
//            else
//            {
//                Debug.Log($"OfflineRewardTimer({sum}) is between minTime({m_OfflineRewardTimer_Min}) and maxTime({maxTime}): TimeDiff({timeDiff}) , Sum({sum})");
//                m_OfflineRewardTimer = sum;
//            }

//            _OnPlayTimeline.Raise((int)TimelineEvent.OFFLINE_REWARD_READY);
//        }
//        else
//        {
//            Debug.Log($"OfflineRewardTimer({sum}) is lower than minTime({m_OfflineRewardTimer_Min}) : " +
//                    $"TimeDiff({timeDiff}) , Sum({sum})");
//            m_OfflineRewardTimer = 0L;
//        }

//        const float INTERVAL_OFFSET = 10.0F;

//        if (m_CheckRewardedAdGoldTimer)
//        {
//            m_RewardedAdGoldTimer += timeDiff;
//            m_RewardedAdGoldTimer = Mathf.Clamp(m_RewardedAdGoldTimer, 0.0F, m_RewardedAdGoldInterval - INTERVAL_OFFSET);
//        }

//        if (m_CheckRewardedAdHeartTimer)
//        {
//            m_RewardedAdHeartTimer += timeDiff;
//            m_RewardedAdHeartTimer = Mathf.Clamp(m_RewardedAdHeartTimer, 0.0F, m_RewardedAdHeartInterval - INTERVAL_OFFSET);
//        }

//        if (m_CheckRewardedTouchGoldTimer)
//        {
//            m_RewardedTouchGoldTimer += timeDiff;
//            m_RewardedTouchGoldTimer = Mathf.Clamp(m_RewardedTouchGoldTimer, 0.0F, m_RewardedTouchGoldInterval - INTERVAL_OFFSET);
//        }

//        if (m_CheckRewardedCameraTimer)
//        {
//            m_RewardedCameraTimer += timeDiff;
//            m_RewardedCameraTimer = Mathf.Clamp(m_RewardedCameraTimer, 0.0F, m_RewardedCameraInterval);
//        }

//        if (m_CheckJewelFreeTimer)
//        {
//            m_JewelFreeTimer -= timeDiff;
//            m_JewelFreeTimer = Mathf.Clamp(m_JewelFreeTimer, 0, m_JewelFreeInterval);
//        }

//        if (m_CheckRandomBoxFreeTimer)
//        {
//            m_RandomBoxFreeTimer -= timeDiff;
//            m_RandomBoxFreeTimer = Mathf.Clamp(m_RandomBoxFreeTimer, 0, m_RandomBoxFreeInterval);
//        }

//        PlayerStateManager.USER_EXIT_TIME = 0L;
//    }


//    /// <summary>
//    /// 아래 경우에 호출된다.
//    /// 앱을 종료한 경우, 광고시청을 시작하는 경우, 백그라운드로 전환한 경우
//    /// </summary>
//    private void UpdateOfflineTimeOnDisconnection()
//    {
//        if (m_CheckOfflineRewardTimer)
//        {
//            if (PlayerStateManager.USER_EXIT_TIME <= 0L
//                && Application.internetReachability != NetworkReachability.NotReachable)
//            {
//                // NEED_TO_REFACTOR : 서버 API 사용 전까지 임시로 디바이스 시간 사용
//                PlayerStateManager.USER_EXIT_TIME = ServerTimer.ConvertDateTimeIntoTimeStamp(DateTime.Now);
//            }
//        }
//    }

//    public class TouchObjectPosition
//    {
//        public LandType type;
//        public int Level;
//        public Transform transform;

//        //On live.
//        [HideInInspector]
//        public string Code;
//        [HideInInspector]
//        public UIGen_TouchObject UsingTarget; //사용중인 대상.

//        public bool IsUsed { get { return UsingTarget != null; } }

//        public void SetCode(int arrayIndex) //펫을 처음 생성할때, 사용되는 함수.
//        {
//            string guid = PlayerPrefs.GetString(string.Format("TOPCODE{0}", arrayIndex), Guid.NewGuid().ToString("N"));

//            Code = guid;
//            PlayerPrefs.SetString(string.Format("TOPCODE{0}", arrayIndex), guid);
//        }
//    }

//    public struct OfflineRewardInformation
//    {
//        public int CreatedCount;
//        public int RemainSeconds;

//        public OfflineRewardInformation(int _createdCount, int _remainSeconds)
//        {
//            CreatedCount = _createdCount;
//            RemainSeconds = _remainSeconds;
//        }
//    }

//    public void SetCameraAnimal()
//    {
//        Debug.Log("m_IsRewardedCameraReady : " + m_IsRewardedCameraReady);

//        if (m_IsRewardedCameraReady)
//        {
//            Debug.Log("Camera Animal Set : " + _AnimalManager.TrySetRandomCameraAnimal());
//        }
//    }
    
//    public bool CheckIfRewardTimerTriggered(RewardTimer type)
//    {
//        switch (type)
//        {
//            case RewardTimer.REWARDED_AD_GOLD:
//                if (!m_IsRewardAdGoldTimerTriggered)
//                {
//                    m_IsRewardAdGoldTimerTriggered = true;
//                    return false;
//                }
//                else
//                    return true;

//            case RewardTimer.REWARDED_AD_HEART:
//                if (!m_IsRewardAdHeartTimerTriggered)
//                {
//                    m_IsRewardAdHeartTimerTriggered = true;
//                    return false;
//                }
//                else
//                    return true;

//            case RewardTimer.TOUCH_OBJECT:
//                if (!m_IsTouchObjectTimerTriggered)
//                {
//                    m_IsTouchObjectTimerTriggered = true;
//                    return false;
//                }
//                else
//                    return true;

//            case RewardTimer.PHOTO_MISSION:
//                if (!m_IsPhotoMissionTimerTriggered)
//                {
//                    m_IsPhotoMissionTimerTriggered = true;
//                    return false;
//                }
//                else
//                    return true;

//            default:
//                Debug.Log($"Failed to check first appearance of reward because type({type.ToString()}) is not valid!");
//                return true;
//        }
//    }

//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void AddAnimalCompleteEventHandler(int typeIdx, int animalCount)
//    {
//        UpdateHeartOutputInterval();
//    }

//    private void GenerateHeartEventHandler(Vector3 pos, bool autoClicked)
//    {
//        UIGen_Text heartText = UIObjectPoolManager.Get(ObjectPoolName.TEXT_HEARTGEN).GetComponent<UIGen_Text>();

//        BigNumber heartResult;
//        if (UnityEngine.Random.Range(0.0F, 100.0F) < _RewardManager.FinalChance_DoubleHeart)
//            heartResult = IncrementHelper.Multiply(m_FinalHeartOutput, 2.0);
//        else
//            heartResult = m_FinalHeartOutput;

//        heartText.Initialize(pos, heartResult);
//        heartText.Move(heartResult);
//        Signals.Get<AddHeartSignal>().Dispatch(heartResult, (int)DeferredEventType.NONE);
//        _OnProgressQuest.Raise((int)QuestType.ANIMAL_HEART);
//    }

//    public void SetHeartGenTimerInTutorial()
//    {
//        m_HeartGenTimer = 100.0F;
//    }

//    //////////////////////////////////////////
//    // Debug
//    //////////////////////////////////////////

//    public BigNumber PureHeartOutput => m_PureHeartOutput.Value;


//    //////////////////////////////////////////
//    // Remote Initialize
//    //////////////////////////////////////////
//    public void OnRemoteInitialize(RemoteInitType type, params object[] args)
//    {
//#if UNITY_EDITOR || DEVELOPMENT_BUILD
//        if (type != RemoteInitType.COMMON_DATA || (int)args[0] != 0)
//            return;

//        const int paramsCount = 8;
//        if (args.Length != paramsCount)
//        {
//            Debug.LogError($"Arguments count is different with paramsCount({paramsCount})! Remote initialization process failed!");
//            return;
//        }

//        m_HeartOutputIntervalThresholds = ((List<int>)args[1]).ToArray();
//        m_HeartOutputIntervals = ((List<float>)args[2]).ToArray();
//        m_RewardedAdGoldInterval = (float)args[3];
//        m_RewardedAdHeartInterval = (float)args[4];
//        m_RewardedTouchGoldInterval = (float)args[5];
//        m_RewardedCameraInterval = (float)args[6];
//        m_OfflineRewardTimer_Min = (long)args[7];
//#else
//        Debug.Log("RemoteInitialize only works on UNITY_EDITOR or DEVELOPMENT_BUILD!");
//#endif
//    }
//}
