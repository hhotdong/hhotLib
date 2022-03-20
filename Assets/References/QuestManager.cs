//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Text.RegularExpressions;
//using ScriptableObjectArchitecture;
//using deVoid.Utils;
//using System.Collections;
//using deVoid.UIFramework;

//public enum QuestType
//{
//    NONE                       = -1,

//    //동물 업그레이드 타입
//    UPG_ANIMAL_0               = 0,
//    UPG_ANIMAL_1               = 1,
//    UPG_ANIMAL_2               = 2,
//    UPG_ANIMAL_3               = 3,
//    UPG_ANIMAL_4               = 4,
//    UPG_ANIMAL_5               = 5,
//    UPG_ANIMAL_6               = 6,
//    UPG_ANIMAL_7               = 7,
//    UPG_ANIMAL_8               = 8,
//    UPG_ANIMAL_9               = 9,
//    UPG_ANIMAL_10              = 10,
//    UPG_ANIMAL_11              = 11,
//    UPG_ANIMAL_12              = 12,
//    UPG_ANIMAL_13              = 13,
//    UPG_ANIMAL_14              = 14,

//    //랜드마크 업그레이드 타입
//    UPG_LNDMRK_0               = 15,
//    UPG_LNDMRK_1               = 16,
//    UPG_LNDMRK_2               = 17,
//    UPG_LNDMRK_3               = 18,
//    UPG_LNDMRK_4               = 19,
//    UPG_LNDMRK_5               = 20,
//    UPG_LNDMRK_6               = 21,
//    UPG_LNDMRK_7               = 22,
//    UPG_LNDMRK_8               = 23,
//    UPG_LNDMRK_9               = 24,
//    UPG_LNDMRK_10              = 25,
//    UPG_LNDMRK_11              = 26,
//    UPG_LNDMRK_12              = 27,
//    UPG_LNDMRK_13              = 28,
//    UPG_LNDMRK_14              = 29,

//    GET_NORMAL_ANIMAL          = 30,
//    CALL_ALBATROSS             = 31,
//    PHOTO_MISSION              = 32,
//    GET_TOUCH_REWARD           = 33,
//    USE_TIME_TICKET            = 34,
//    OPEN_RARE_ANIMAL_BOX       = 35,
//    FEED_ANIMAL                = 36,
//    ANIMAL_HEART               = 37
//}

//[Serializable]
//public struct QuestData
//{
//    public QuestType questType;
//    public int questValue;

//    public QuestData(QuestType questType, int value)
//    {
//        this.questType = questType;
//        this.questValue = value;
//    }
//}

////[CreateAssetMenu]
//public class QuestManager : ScriptableObject, IInitializable, IRemoteInitializable
//{
//    public bool IsInitialized { get; private set; }

//    [Header("QuestData")]
//    [SerializeField] private List<QuestData> m_QuestData;
//    private int m_CurQuestNum;
//    private int m_CurQuestVal;
//    private bool m_QuestBannerShowed;

//    public List<QuestData> QuestData => m_QuestData;
//    public int CurQuestNum => m_CurQuestNum;
//    public int CurQuestVal => m_CurQuestVal;

//    [Header("Statistic Event Data")]
//    public bool CheckQuestActivationTimer = false;
//    public float QuestActivationTotalTime = 0.0F;
//    public float QuestActivationTimer = 0.0F;
//    public string QuestStartTime;

//    [Header("SO Managers")]
//    [SerializeField] private AnimalManager _AnimalManager;
//    [SerializeField] private LandmarkManager _LandmarkManager;

//    [Header("SO Events")]
//    [SerializeField] private IntGameEvent _OnProgressQuest = default(IntGameEvent);
//    [SerializeField] private IntGameEvent _OnGrantReward = default(IntGameEvent);

//    public int QuestAlertWaitingCount = 0;
//    public Queue<QuestData> QuestAlertDataQueue;
//    public bool IsOpen = false;


//    //////////////////////////////////////////
//    // Initialize & Reset
//    //////////////////////////////////////////

//    public void Initialize(SaveData saveData)
//    {
//        IsInitialized = false;

//        IsOpen = false;
//        QuestAlertDataQueue = new Queue<QuestData>();

//        SaveData.QuestData _data = saveData.GetQuestData;
//        m_CurQuestNum = _data.QUEST_NUMBER;
//        m_CurQuestVal = _data.QUEST_PROGRESS_VALUE;
//        m_QuestBannerShowed = _data.QUEST_ALERT_STATE;

//        Signals.Get<UpdateNotificationSignal>().Dispatch((int)NotificationType.QUEST);

//        _OnProgressQuest.AddListener(DoProgressQuest);
//        SaveLoadManager.SaveEvent += OnSave;
//        GameManager.DidCompleteInitialize += OnGMInitialized;

//        IsInitialized = true;
//    }

//    public void DoReset()
//    {
//        if (QuestActivationTimer > 0.0F)
//            QuestActivationTotalTime += QuestActivationTimer;

//        EncryptedPlayerPrefs.SetFloat(PlayerPrefsKeys.QUEST_ACTIVATION_TIME, QuestActivationTotalTime);

//        if(!string.IsNullOrEmpty(QuestStartTime))
//            EncryptedPlayerPrefs.SetString(PlayerPrefsKeys.QUEST_START_TIME, QuestStartTime);

//        _OnProgressQuest.RemoveListener(DoProgressQuest);
//        GameManager.DidCompleteInitialize -= OnGMInitialized;
//        SaveLoadManager.SaveEvent -= OnSave;
//        IsInitialized = false;
//    }

//    public void OnSave(SaveData data)
//    {
//        SaveData.QuestData dataToSave = data.GetQuestData;
//        if (dataToSave == null)
//        {
//            Debug.LogError("Failed to save QuestManager because QuestData is null!");
//            return;
//        }

//        dataToSave.QUEST_NUMBER = m_CurQuestNum;
//        dataToSave.QUEST_PROGRESS_VALUE = m_CurQuestVal;
//        dataToSave.QUEST_ALERT_STATE = m_QuestBannerShowed;
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void OnGMInitialized()
//    {
//        CheckQuestActivationTimer = TutorialEventTracker.NewbieTutorialPhase >= TutorialPhase.COMPLETE;
//        QuestActivationTotalTime = EncryptedPlayerPrefs.GetFloat(PlayerPrefsKeys.QUEST_ACTIVATION_TIME, 0.0F);
//        QuestActivationTimer = 0.0F;
//        QuestStartTime = EncryptedPlayerPrefs.GetString(PlayerPrefsKeys.QUEST_START_TIME, string.Empty);
//    }

//    private void DoProgressQuest(int typeIdx)
//    {
//        QuestType currentType = m_QuestData[m_CurQuestNum].questType;
//        if (typeIdx != (int)currentType)
//        {
//            Debug.Log($"Failed to progress quest because parameter type({typeIdx}) is different with current type idx({(int)currentType})!");
//            return;
//        }

//        switch (currentType)
//        {
//            case QuestType.UPG_ANIMAL_0:
//            case QuestType.UPG_ANIMAL_1:
//            case QuestType.UPG_ANIMAL_2:
//            case QuestType.UPG_ANIMAL_3:
//            case QuestType.UPG_ANIMAL_4:
//            case QuestType.UPG_ANIMAL_5:
//            case QuestType.UPG_ANIMAL_6:
//            case QuestType.UPG_ANIMAL_7:
//            case QuestType.UPG_ANIMAL_8:
//            case QuestType.UPG_ANIMAL_9:
//            case QuestType.UPG_ANIMAL_10:
//            case QuestType.UPG_ANIMAL_11:
//            case QuestType.UPG_ANIMAL_12:
//            case QuestType.UPG_ANIMAL_13:
//            case QuestType.UPG_ANIMAL_14:
//            case QuestType.UPG_LNDMRK_0:
//            case QuestType.UPG_LNDMRK_1:
//            case QuestType.UPG_LNDMRK_2:
//            case QuestType.UPG_LNDMRK_3:
//            case QuestType.UPG_LNDMRK_4:
//            case QuestType.UPG_LNDMRK_5:
//            case QuestType.UPG_LNDMRK_6:
//            case QuestType.UPG_LNDMRK_7:
//            case QuestType.UPG_LNDMRK_8:
//            case QuestType.UPG_LNDMRK_9:
//            case QuestType.UPG_LNDMRK_10:
//            case QuestType.UPG_LNDMRK_11:
//            case QuestType.UPG_LNDMRK_12:
//            case QuestType.UPG_LNDMRK_13:
//            case QuestType.UPG_LNDMRK_14:
//            case QuestType.GET_NORMAL_ANIMAL:
//                Debug.Log($"This type({currentType.ToString()}) of quest is independent from m_CurQuestVal!");
//                break;

//            case QuestType.CALL_ALBATROSS:
//            case QuestType.PHOTO_MISSION:
//            case QuestType.GET_TOUCH_REWARD:
//            case QuestType.USE_TIME_TICKET:
//            case QuestType.OPEN_RARE_ANIMAL_BOX:
//            case QuestType.FEED_ANIMAL:
//            case QuestType.ANIMAL_HEART:
//                ++m_CurQuestVal;
//                Debug.Log($"m_CurQuestVal is added and now {m_CurQuestVal}");
//                break;

//            default:
//                Debug.Log($"Failed to progress quest because type({m_QuestData[m_CurQuestNum].questType.ToString()}) is not appropritate!");
//                return;
//        }

//        Signals.Get<UpdateQuestProgressSignal>().Dispatch();    
//        Signals.Get<UpdateNotificationSignal>().Dispatch((int)NotificationType.QUEST);

//        if(IsQuestClear() && !m_QuestBannerShowed && TutorialEventTracker.NewbieTutorialPhase >= TutorialPhase.COMPLETE)
//        {
//            ShowQuestAlertPopup();
//        }
//    }

//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public void ShowQuestAlertPopup()
//    {
//        SetQuestBannerShowState(true);
//        QuestAlertDataQueue.Enqueue(m_QuestData[m_CurQuestNum]);

//        if (!IsOpen)
//        {
//            IsOpen = true;
//            GameManager.Instance.BehaviourProxy.StartCoroutine(SetPopupDelay());
//        }

//        IEnumerator SetPopupDelay()
//        {
//            yield return HelperManager.WS_HALF_SEC;
//            Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestBannerPanel, true, null);
//        }
//    }

//    public bool TryGetCurrentQuest(out QuestData questData, out int currVal)
//    {
//        if(m_CurQuestNum < m_QuestData.Count)
//        {
//            questData = m_QuestData[m_CurQuestNum];
//            currVal = GetQuestCurrentValue(questData);
//            return true;
//        }
//        else
//        {
//            Debug.Log("All quests are already cleared!");
//            questData = new QuestData(QuestType.NONE, -1);
//            currVal = -1;
//            return false;
//        }
//    }

//    public bool TryGetCurrentQuestType(out QuestType _type)
//    {
//        if (m_CurQuestNum < m_QuestData.Count)
//        {
//            _type = m_QuestData[m_CurQuestNum].questType;
//            return true;
//        }
//        else
//        {
//            Debug.Log("All quests are already cleared!");
//            _type = QuestType.NONE;
//            return false;
//        }
//    }

//    public bool IsQuestClear()
//    {
//        if (TryGetCurrentQuest(out QuestData questData, out int currVal))
//        {
//            if (currVal >= questData.questValue)
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    private int GetQuestCurrentValue(QuestData questData)
//    {
//        switch (questData.questType)
//        {
//            case QuestType.UPG_ANIMAL_0:
//            case QuestType.UPG_ANIMAL_1:
//            case QuestType.UPG_ANIMAL_2:
//            case QuestType.UPG_ANIMAL_3:
//            case QuestType.UPG_ANIMAL_4:
//            case QuestType.UPG_ANIMAL_5:
//            case QuestType.UPG_ANIMAL_6:
//            case QuestType.UPG_ANIMAL_7:
//            case QuestType.UPG_ANIMAL_8:
//            case QuestType.UPG_ANIMAL_9:
//            case QuestType.UPG_ANIMAL_10:
//            case QuestType.UPG_ANIMAL_11:
//            case QuestType.UPG_ANIMAL_12:
//            case QuestType.UPG_ANIMAL_13:
//            case QuestType.UPG_ANIMAL_14:
//                if (_LandmarkManager.TryGetLandmarkPhase((int)questData.questType, out int animalLevel))
//                    return animalLevel;
//                else
//                    return 0;

//            case QuestType.UPG_LNDMRK_0:
//            case QuestType.UPG_LNDMRK_1:
//            case QuestType.UPG_LNDMRK_2:
//            case QuestType.UPG_LNDMRK_3:
//            case QuestType.UPG_LNDMRK_4:
//            case QuestType.UPG_LNDMRK_5:
//            case QuestType.UPG_LNDMRK_6:
//            case QuestType.UPG_LNDMRK_7:
//            case QuestType.UPG_LNDMRK_8:
//            case QuestType.UPG_LNDMRK_9:
//            case QuestType.UPG_LNDMRK_10:
//            case QuestType.UPG_LNDMRK_11:
//            case QuestType.UPG_LNDMRK_12:
//            case QuestType.UPG_LNDMRK_13:
//            case QuestType.UPG_LNDMRK_14:
//                if (_LandmarkManager.TryGetLandmarkLevel((int)questData.questType - (int)QuestType.UPG_LNDMRK_0, out int landmarkLevel))
//                    return landmarkLevel;
//                else
//                    return 0;

//            case QuestType.GET_NORMAL_ANIMAL:
//                return _AnimalManager.NormalAnimalTotalCount;

//            case QuestType.CALL_ALBATROSS:
//            case QuestType.PHOTO_MISSION:
//            case QuestType.GET_TOUCH_REWARD:
//            case QuestType.USE_TIME_TICKET:
//            case QuestType.OPEN_RARE_ANIMAL_BOX:
//            case QuestType.FEED_ANIMAL:
//            case QuestType.ANIMAL_HEART:
//                return m_CurQuestVal < questData.questValue
//                         ? m_CurQuestVal
//                         : questData.questValue;

//            default:
//                Debug.Log($"Failed to get the current value of quest because type({questData.questType.ToString()}) is not appropritate!");
//                return -1;
//        }
//    }

//    public bool FinishCurrentQuest()
//    {
//        QuestData curQuestData = m_QuestData[m_CurQuestNum];
//        if (curQuestData.questType == QuestType.NONE)
//        {
//            Debug.Log("Failed to finish current quest because QuestType is NONE!");
//            return false;
//        }

//        int curVal = GetQuestCurrentValue(curQuestData);
//        int destVal = curQuestData.questValue;

//        if (curVal >= destVal)
//        {
//            GrantRewards(curQuestData.questType);
//            m_CurQuestNum++;
//            m_CurQuestVal = 0;

//            Signals.Get<UpdateNotificationSignal>().Dispatch((int)NotificationType.QUEST);
//            Signals.Get<PlayCoinEffectSignal>().Dispatch((int)CoinEffectType.UTILITY, (int)UIReferencePointType.QUEST_CLEAR);

//            if (QuestActivationTimer > 0.0F)
//                QuestActivationTotalTime += QuestActivationTimer;

//            GAManager.Instance.GA_DesignEvent(string.Format(StatisticEventInfo.Balance.QUEST_TIME_ACTIVATION_, m_CurQuestNum), QuestActivationTotalTime);
//            GAManager.Instance.GA_DesignEvent(string.Format(StatisticEventInfo.Progression.QUEST_CLEAR_, m_CurQuestNum));

//            QuestActivationTimer 
//                = QuestActivationTotalTime
//                = 0.0F;

//            if (!string.IsNullOrEmpty(QuestStartTime))
//            {
//                if (DateTime.TryParseExact(QuestStartTime, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime lastTime))
//                {
//                    DateTime now = DateTime.Now;
//                    TimeSpan span = now - lastTime;
//                    GAManager.Instance.GA_DesignEvent(string.Format(StatisticEventInfo.Balance.QUEST_TIME_TOTAL_, m_CurQuestNum), (int)span.TotalSeconds);
//                    //Debug.Log($"Quest({m_CurQuestNum}) Complete : {lastTime} , {now} , {(int)span.TotalSeconds}");
//                    QuestStartTime = now.ToString("yyyyMMddHHmmss");
//                }
//            }

//            Signals.Get<UpdateQuestProgressSignal>().Dispatch();

//            return true;
//        }
//        else
//        {
//            Debug.Log($"Failed to finish current quest because curVal({curVal}) is lower than destVal({destVal})!");
//            return false;
//        }


//        void GrantRewards(QuestType type)
//        {
//            int rewardTypeIdx = -1;
//            switch (type)
//            {
//                case QuestType.UPG_ANIMAL_0: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_0; break;
//                case QuestType.UPG_ANIMAL_1: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_1; break;
//                case QuestType.UPG_ANIMAL_2: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_2; break;
//                case QuestType.UPG_ANIMAL_3: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_3; break;
//                case QuestType.UPG_ANIMAL_4: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_4; break;
//                case QuestType.UPG_ANIMAL_5: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_5; break;
//                case QuestType.UPG_ANIMAL_6: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_6; break;
//                case QuestType.UPG_ANIMAL_7: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_7; break;
//                case QuestType.UPG_ANIMAL_8: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_8; break;
//                case QuestType.UPG_ANIMAL_9: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_9; break;
//                case QuestType.UPG_ANIMAL_10: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_10; break;
//                case QuestType.UPG_ANIMAL_11: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_11; break;
//                case QuestType.UPG_ANIMAL_12: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_12; break;
//                case QuestType.UPG_ANIMAL_13: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_13; break;
//                case QuestType.UPG_ANIMAL_14: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_14; break;
//                case QuestType.UPG_LNDMRK_0: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_0; break;
//                case QuestType.GET_NORMAL_ANIMAL: rewardTypeIdx = (int)GrantRewardType.QUEST_GET_NORMAL_ANIMAL; break;
//                case QuestType.CALL_ALBATROSS: rewardTypeIdx = (int)GrantRewardType.QUEST_CALL_ALBATROSS; break;
//                case QuestType.PHOTO_MISSION: rewardTypeIdx = (int)GrantRewardType.QUEST_PHOTO_MISSION; break;
//                case QuestType.GET_TOUCH_REWARD: rewardTypeIdx = (int)GrantRewardType.QUEST_GET_TOUCH_REWARD; break;
//                case QuestType.USE_TIME_TICKET: rewardTypeIdx = (int)GrantRewardType.QUEST_USE_TIME_TICKET; break;
//                case QuestType.OPEN_RARE_ANIMAL_BOX: rewardTypeIdx = (int)GrantRewardType.QUEST_OPEN_RARE_ANIMAL_BOX; break;
//                case QuestType.FEED_ANIMAL: rewardTypeIdx = (int)GrantRewardType.QUEST_FEED_ANIMAL; break;
//                case QuestType.ANIMAL_HEART: rewardTypeIdx = (int)GrantRewardType.QUEST_ANIMAL_HEART; break;
//                default:
//                    Debug.Log($"Failed to GrantRewards because type({type.ToString()}) is not appropriate!");
//                    return;
//            }

//            _OnGrantReward.Raise(rewardTypeIdx);
//        }
//    }

//    public bool IsCurrentQuestReadyToClear()
//    {
//        if (m_CurQuestNum >= m_QuestData.Count)
//        {
//            Debug.Log($"There is no any next quest({m_CurQuestNum}) in the list because all quests were cleared!");
//            return false;
//        }

//        QuestData curQuestData = m_QuestData[m_CurQuestNum];
//        int curVal = GetQuestCurrentValue(curQuestData);
//        int destVal = curQuestData.questValue;

//        return curVal >= destVal;
//    }

//    /// <summary>
//    /// 다음번 퀘스트에 대한 클리어 여부를 알 필요가 있는 경우 재귀적으로 체크합니다.
//    /// </summary>
//    public bool TryGetNotificationCount(out int count)
//    {
//        int readyCount = 0;
//        CheckRecursive(m_CurQuestNum);

//        if (readyCount > 0)
//        {
//            count = readyCount;
//            return true;
//        }
//        else
//        {
//            count = -1;
//            return false;
//        }


//        void CheckRecursive(int questNum)
//        {
//            if (questNum >= m_QuestData.Count)
//            {
//                Debug.Log($"There is no any next quest({questNum}) in the list because all quests were cleared!");
//                return;
//            }

//            QuestData curQuestData = m_QuestData[questNum];
//            int curVal = GetQuestCurrentValue(curQuestData);
//            int destVal = curQuestData.questValue;

//            if (curVal >= destVal)
//            {
//                ++readyCount;
//                CheckRecursive(questNum + 1);
//            }
//        }
//    }

//    public void UpdateQuestTimer(float dt)
//    {
//        if (CheckQuestActivationTimer)
//            QuestActivationTimer += dt;
//    }

//    void ForceShowQuestAlert()
//    {
//        if (TutorialEventTracker.NewbieTutorialPhase >= TutorialPhase.COMPLETE)
//        {
//            ShowQuestAlertPopup();
//        }
//    }

//    public void ForceFinishQuest()
//    {
//#if UNITY_EDITOR || DEVELOPMENT_BUILD
//        ForceShowQuestAlert();
//        QuestData curQuestData = m_QuestData[m_CurQuestNum];
//        GrantRewards(curQuestData.questType);
//        m_CurQuestNum++;
//        m_CurQuestVal = 0;

//        Signals.Get<UpdateNotificationSignal>().Dispatch((int)NotificationType.QUEST);
//        Signals.Get<PlayCoinEffectSignal>().Dispatch((int)CoinEffectType.UTILITY, (int)UIReferencePointType.QUEST_CLEAR);

//        if (QuestActivationTimer > 0.0F)
//            QuestActivationTotalTime += QuestActivationTimer;

//        GAManager.Instance.GA_DesignEvent(string.Format(StatisticEventInfo.Balance.QUEST_TIME_ACTIVATION_, m_CurQuestNum), QuestActivationTotalTime);
//        GAManager.Instance.GA_DesignEvent(string.Format(StatisticEventInfo.Progression.QUEST_CLEAR_, m_CurQuestNum));

//        QuestActivationTimer
//            = QuestActivationTotalTime
//            = 0.0F;

//        if (!string.IsNullOrEmpty(QuestStartTime))
//        {
//            if (DateTime.TryParseExact(QuestStartTime, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime lastTime))
//            {
//                DateTime now = DateTime.Now;
//                TimeSpan span = now - lastTime;
//                GAManager.Instance.GA_DesignEvent(string.Format(StatisticEventInfo.Balance.QUEST_TIME_TOTAL_, m_CurQuestNum), (int)span.TotalSeconds);
//                //Debug.Log($"Quest({m_CurQuestNum}) Complete : {lastTime} , {now} , {(int)span.TotalSeconds}");
//                QuestStartTime = now.ToString("yyyyMMddHHmmss");
//            }
//        }

//        void GrantRewards(QuestType type)
//        {
//            int rewardTypeIdx = -1;
//            switch (type)
//            {
//                case QuestType.UPG_ANIMAL_0: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_0; break;
//                case QuestType.UPG_ANIMAL_1: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_1; break;
//                case QuestType.UPG_ANIMAL_2: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_2; break;
//                case QuestType.UPG_ANIMAL_3: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_3; break;
//                case QuestType.UPG_ANIMAL_4: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_4; break;
//                case QuestType.UPG_ANIMAL_5: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_5; break;
//                case QuestType.UPG_ANIMAL_6: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_6; break;
//                case QuestType.UPG_ANIMAL_7: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_7; break;
//                case QuestType.UPG_ANIMAL_8: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_8; break;
//                case QuestType.UPG_ANIMAL_9: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_9; break;
//                case QuestType.UPG_ANIMAL_10: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_10; break;
//                case QuestType.UPG_ANIMAL_11: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_11; break;
//                case QuestType.UPG_ANIMAL_12: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_12; break;
//                case QuestType.UPG_ANIMAL_13: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_13; break;
//                case QuestType.UPG_ANIMAL_14: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_14; break;
//                case QuestType.UPG_LNDMRK_0: rewardTypeIdx = (int)GrantRewardType.QUEST_UPG_ANIMAL_0; break;
//                case QuestType.GET_NORMAL_ANIMAL: rewardTypeIdx = (int)GrantRewardType.QUEST_GET_NORMAL_ANIMAL; break;
//                case QuestType.CALL_ALBATROSS: rewardTypeIdx = (int)GrantRewardType.QUEST_CALL_ALBATROSS; break;
//                case QuestType.PHOTO_MISSION: rewardTypeIdx = (int)GrantRewardType.QUEST_PHOTO_MISSION; break;
//                case QuestType.GET_TOUCH_REWARD: rewardTypeIdx = (int)GrantRewardType.QUEST_GET_TOUCH_REWARD; break;
//                case QuestType.USE_TIME_TICKET: rewardTypeIdx = (int)GrantRewardType.QUEST_USE_TIME_TICKET; break;
//                case QuestType.OPEN_RARE_ANIMAL_BOX: rewardTypeIdx = (int)GrantRewardType.QUEST_OPEN_RARE_ANIMAL_BOX; break;
//                case QuestType.FEED_ANIMAL: rewardTypeIdx = (int)GrantRewardType.QUEST_FEED_ANIMAL; break;
//                case QuestType.ANIMAL_HEART: rewardTypeIdx = (int)GrantRewardType.QUEST_ANIMAL_HEART; break;
//                default:
//                    Debug.Log($"Failed to GrantRewards because type({type.ToString()}) is not appropriate!");
//                    return;
//            }

//            _OnGrantReward.Raise(rewardTypeIdx);
//        }
//#endif
//    }

//    public void SetQuestBannerShowState(bool alert)
//    {
//        m_QuestBannerShowed = alert;
//    }

//    public void GoQuestPointShortcut()
//    {
//        QuestType currentType = m_QuestData[m_CurQuestNum].questType;

//        switch (currentType)
//        {
//            case QuestType.UPG_ANIMAL_0:
//            case QuestType.UPG_ANIMAL_1:
//            case QuestType.UPG_ANIMAL_2:
//            case QuestType.UPG_ANIMAL_3:
//            case QuestType.UPG_ANIMAL_4:
//            case QuestType.UPG_ANIMAL_5:
//            case QuestType.UPG_ANIMAL_6:
//            case QuestType.UPG_ANIMAL_7:
//            case QuestType.UPG_ANIMAL_8:
//            case QuestType.UPG_ANIMAL_9:
//            case QuestType.UPG_ANIMAL_10:
//            case QuestType.UPG_ANIMAL_11:
//            case QuestType.UPG_ANIMAL_12:
//            case QuestType.UPG_ANIMAL_13:
//            case QuestType.UPG_ANIMAL_14:
//                GAManager.Instance.GA_DesignEvent("Quest:Shortcut:UPG_ANIMAL");
//                int animalIdx = (int)m_QuestData[m_CurQuestNum].questType;
//                if (animalIdx < _LandmarkManager.GetLastLandmarkNum())
//                {
//                    QuestWindowController.DoShortcut = () =>
//                    {
//                        Signals.Get<NavigateToWindowSignal>().Dispatch(ScreenIds.AnimalWindow, null);
//                        Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestGuideAlertPanel, true, null);
//                        if (animalIdx - 2 > 0)
//                            Signals.Get<UpdateScrollContentPositionSignal>().Dispatch(ScreenIds.AnimalWindow, animalIdx - 2);
//                        else
//                            Signals.Get<UpdateScrollContentPositionSignal>().Dispatch(ScreenIds.AnimalWindow, 0);
//                    };
//                }
//                else
//                {
//                    QuestWindowController.DoShortcut = () =>
//                    {
//                        Signals.Get<NavigateToWindowSignal>().Dispatch(ScreenIds.RewardWindow, new RewardWindowProperties(RewardWindowType.LANDMARK_UNLOCK, animalIdx));
//                        Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestGuideAlertPanel, true, null);
//                    };
//                }

//                break;
//            case QuestType.UPG_LNDMRK_0:
//            case QuestType.UPG_LNDMRK_1:
//            case QuestType.UPG_LNDMRK_2:
//            case QuestType.UPG_LNDMRK_3:
//            case QuestType.UPG_LNDMRK_4:
//            case QuestType.UPG_LNDMRK_5:
//            case QuestType.UPG_LNDMRK_6:
//            case QuestType.UPG_LNDMRK_7:
//            case QuestType.UPG_LNDMRK_8:
//            case QuestType.UPG_LNDMRK_9:
//            case QuestType.UPG_LNDMRK_10:
//            case QuestType.UPG_LNDMRK_11:
//            case QuestType.UPG_LNDMRK_12:
//            case QuestType.UPG_LNDMRK_13:
//            case QuestType.UPG_LNDMRK_14:
//                GAManager.Instance.GA_DesignEvent("Quest:Shortcut:UPG_LANDMARK");
//                int landmarkIdx = (int)m_QuestData[m_CurQuestNum].questType - (int)QuestType.UPG_LNDMRK_0;

//                if (landmarkIdx < _LandmarkManager.GetLastLandmarkNum())
//                {
//                    QuestWindowController.DoShortcut = () =>
//                    {
//                        Signals.Get<NavigateToWindowSignal>().Dispatch(ScreenIds.UpgradeWindow, null);
//                        Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestGuideAlertPanel, true, null);
//                        if (landmarkIdx - 2 > 0)
//                            Signals.Get<UpdateScrollContentPositionSignal>().Dispatch(ScreenIds.UpgradeWindow, landmarkIdx - 2);
//                        else
//                            Signals.Get<UpdateScrollContentPositionSignal>().Dispatch(ScreenIds.UpgradeWindow, 0);
//                    };
//                }

//                else
//                {
//                    QuestWindowController.DoShortcut = () =>
//                    {
//                        Signals.Get<NavigateToWindowSignal>().Dispatch(ScreenIds.RewardWindow, new RewardWindowProperties(RewardWindowType.LANDMARK_UNLOCK, landmarkIdx));
//                        Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestGuideAlertPanel, true, null);
//                    };
//                }
//                break;

//            case QuestType.GET_NORMAL_ANIMAL:
//                GAManager.Instance.GA_DesignEvent("Quest:Shortcut:GET_NORMAL_ANIMAL");
//                QuestWindowController.DoShortcut = () =>
//                {
//                    Signals.Get<NavigateToWindowSignal>().Dispatch(ScreenIds.AnimalWindow, null);
//                    Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestGuideAlertPanel, true, null);
//                };
//                break;
//            case QuestType.ANIMAL_HEART:
//                GAManager.Instance.GA_DesignEvent("Quest:Shortcut:ANIMAL_HEART");
//                QuestWindowController.DoShortcut = () =>
//                {
//                    Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestGuideAlertPanel, true, null);
//                    CameraManager.CamHandler.ResetPoseForShortcut();
//                };
//                break;

//            case QuestType.CALL_ALBATROSS:
//                GAManager.Instance.GA_DesignEvent("Quest:Shortcut:CALL_ALBATROSS");
//                QuestWindowController.DoShortcut = () =>
//                {
//                    Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestGuideAlertPanel, true, null);
//                    CameraManager.CamHandler.ResetPoseForShortcut();
//                };
//                break;
//            case QuestType.PHOTO_MISSION:
//                GAManager.Instance.GA_DesignEvent("Quest:Shortcut:PHOTO_MISSION");
//                QuestWindowController.DoShortcut = () =>
//                {
//                    Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestGuideAlertPanel, true, null);
//                    CameraManager.CamHandler.ResetPoseForShortcut();
//                };
//                break;
//            case QuestType.GET_TOUCH_REWARD:
//                GAManager.Instance.GA_DesignEvent("Quest:Shortcut:GET_TOUCH_REWARD");
//                QuestWindowController.DoShortcut = () =>
//                {
//                    Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestGuideAlertPanel, true, null);
//                    CameraManager.CamHandler.ResetPoseForShortcut();
//                };
//                break;

//            case QuestType.USE_TIME_TICKET:
//                GAManager.Instance.GA_DesignEvent("Quest:Shortcut:USE_TIME_TICKET");
//                QuestWindowController.DoShortcut = () =>
//                {
//                    Signals.Get<NavigateToWindowSignal>().Dispatch(ScreenIds.ShopWindow, null);
//                    Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestGuideAlertPanel, true, null);
//                    Signals.Get<UpdateScrollContentPositionSignal>().Dispatch(ScreenIds.ShopWindow, 1);
//                };
//                break;
//            case QuestType.OPEN_RARE_ANIMAL_BOX:
//                GAManager.Instance.GA_DesignEvent("Quest:Shortcut:OPEN_RARE_ANIMAL_BOX");
//                QuestWindowController.DoShortcut = () =>
//                {
//                    Signals.Get<NavigateToWindowSignal>().Dispatch(ScreenIds.ShopWindow, null);
//                    Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.QuestGuideAlertPanel, true, null);
//                    Signals.Get<UpdateScrollContentPositionSignal>().Dispatch(ScreenIds.ShopWindow, 0);
//                };
//                break;
//            case QuestType.FEED_ANIMAL:
//                GAManager.Instance.GA_DesignEvent("Quest:Shortcut:FEED_ANIMAL");
//                break;

//            default:
//                return;
//        }
//    }

//    static public int ConvertLandmarkTypeToQuestIndex(QuestType type)
//    {
//        int index = (int)type - (int)QuestType.UPG_LNDMRK_0;
//        if (index > 0 && index < 15) return index;
//        else return -1;
//    }

//    //////////////////////////////////////////
//    // Remote Initialize
//    //////////////////////////////////////////

//    public void OnRemoteInitialize(RemoteInitType type, params object[] args)
//    {
//#if UNITY_EDITOR || DEVELOPMENT_BUILD
//        if (type != RemoteInitType.QUEST_DATA)
//            return;

//        const int paramsCount = 4;

//        if (args.Length != paramsCount)
//        {
//            Debug.Log($"Arguments count is different with paramsCount({paramsCount})! Remote initialization process failed!");
//            return;
//        }

//        List<string> ids = (List<string>)args[1];
//        List<string> questTypes = (List<string>)args[2];
//        List<int> questValue = (List<int>)args[3];

//        if ((ids == null || ids.Count == 0)
//            || (questTypes == null || questTypes.Count == 0)
//            || (questValue == null || questValue.Count == 0)
//            || (ids.Count != questTypes.Count)
//            || (questTypes.Count != questValue.Count)
//            || (questValue.Count != ids.Count))
//        {
//            Debug.LogError("UpdateData cannot be done!");
//            return;
//        }

//        if ((int)args[0] == 0)
//        {
//            if (m_QuestData == null)
//                m_QuestData = new List<QuestData>();
//            else
//                m_QuestData.Clear();

//            for (int i = 0; i < questTypes.Count; i++)
//            {
//                // 향후 1번 이상의 랜드마크에 대한 예외 처리가 필요할 수 있을 것 같아서 if문 조건 유지
//                if (questTypes[i].Contains("UPG_LNDMRK_"))
//                {
//                    string newType = Regex.Replace(questTypes[i], @"\D", "");
//                    int landmarkIdx = int.Parse(newType);
//                    QuestType landmarkQuestType = (QuestType)(landmarkIdx + (int)QuestType.UPG_LNDMRK_0);

//                    if (_LandmarkManager.IsValidLandmarkType(landmarkIdx))
//                        m_QuestData.Add(new QuestData(landmarkQuestType, questValue[i]));
//                    else
//                        Debug.LogError($"LandmarkIndex({landmarkIdx}) is not appropriate!");
//                }

//                else if (questTypes[i].Contains("UPG_ANIMAL_"))
//                {
//                    string animalIdxString = Regex.Replace(questTypes[i], @"\D", "");
//                    int animalIdx = int.Parse(animalIdxString);

//                    if (_LandmarkManager.IsValidLandmarkType(animalIdx))
//                        m_QuestData.Add(new QuestData((QuestType)animalIdx, questValue[i]));
//                    else
//                        Debug.LogError($"AnimalIndex({animalIdx}) is not appropriate!");
//                }
//                else
//                {
//                    switch (questTypes[i])
//                    {
//                        case "GET_NORMAL_ANIMAL":
//                            m_QuestData.Add(new QuestData(
//                                QuestType.GET_NORMAL_ANIMAL,
//                                questValue[i]));
//                            break;

//                        case "CALL_ALBATROSS":
//                            m_QuestData.Add(new QuestData(
//                                QuestType.CALL_ALBATROSS,
//                                questValue[i]));
//                            break;

//                        case "PHOTO_MISSION":
//                            m_QuestData.Add(new QuestData(
//                                QuestType.PHOTO_MISSION,
//                                questValue[i]));
//                            break;

//                        case "GET_TOUCH_REWARD":
//                            m_QuestData.Add(new QuestData(
//                                QuestType.GET_TOUCH_REWARD,
//                                questValue[i]));
//                            break;

//                        case "USE_TIME_TICKET":
//                            m_QuestData.Add(new QuestData(
//                                QuestType.USE_TIME_TICKET,
//                                questValue[i]));
//                            break;

//                        case "OPEN_RARE_ANIMAL_BOX":
//                            m_QuestData.Add(new QuestData(
//                                QuestType.OPEN_RARE_ANIMAL_BOX,
//                                questValue[i]));
//                            break;

//                        case "FEED_ANIMAL":
//                            m_QuestData.Add(new QuestData(
//                                QuestType.FEED_ANIMAL,
//                                questValue[i]));
//                            break;

//                        case "ANIMAL_HEART":
//                            m_QuestData.Add(new QuestData(
//                                QuestType.ANIMAL_HEART,
//                                questValue[i]));
//                            break;

//                        default:
//                            break;
//                    }
//                }                
//            }
//        }
//#else
//        Debug.Log("RemoteInitialize only works on UNITY_EDITOR or DEVELOPMENT_BUILD!");
//#endif
//    }
//}
