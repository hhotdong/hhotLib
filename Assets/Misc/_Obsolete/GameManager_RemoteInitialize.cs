//using System.Collections;
//using System.Collections.Generic;
//using Lib.RapidSheetData;
//using UnityEngine;
//using ForrestIsland;
//using UnityEditor;

//public enum RemoteInitType
//{
//    NONE              = -1,
//    COMMON_DATA       = 0,
//    LANDMARK_DATA     = 1,
//    REWARD_DATA       = 2,
//    QUEST_DATA        = 4,
//    ACHIEVEMENT_DATA  = 5,
//    GROWTH_DATA       = 6,
//    RARE_ANIMAL_DATA  = 7
//}

//public interface IRemoteInitializable
//{
//    /// <summary>
//    /// 유니티 에디터 또는 개발용 빌드에서 데이터 초기화를 위해 사용. 앱 실행 시마다 매번 호출된다.
//    /// </summary>
//    void OnRemoteInitialize(RemoteInitType type, params object[] args);
//}

//public partial class GameManager : SingletonScriptableObject<GameManager>
//{
//    [Header("Remote Initialize")]
//    [SerializeField] private RSDAsset _DataAsset;


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    /// <summary>
//    /// 에디터 상에서 또는 DEVELOPMENT_BUILD 에서 재시작하는 경우 Remote Initialize를 수행한다(ScriptableObject 변경사항은 임시적).
//    /// </summary>
//#if UNITY_EDITOR || DEVELOPMENT_BUILD
//    private IEnumerator FetchAndUpdateDataAsync()
//    {
//        _DataAsset.Init(Behaviour);

//        bool isDataPullComplete = false;
//        bool isDataPullSucceeded = false;
//        _DataAsset.PullData((isSucceeded) =>
//        {
//            isDataPullComplete = true;
//            isDataPullSucceeded = isSucceeded;
//            if (isSucceeded)
//                Debug.Log("Data is successfully pulled.");
//            else
//                Debug.Log("Failed to pull the data.");
//        });

//        yield return new WaitUntil(() => isDataPullComplete);

//        if (isDataPullSucceeded)
//            RemoteInitialize(false);
//        else
//            yield break;
//    }
//#endif

//    /// <summary>
//    /// 에디터 상에서 플레이 모드가 아닌 시점에 Remote Initialize를 수행한다(ScriptableObject 변경사항은 영구적).
//    /// </summary>
//#if UNITY_EDITOR
//    public void FetchAndUpdateData()
//    {
//        _DataAsset.Init();

//        _DataAsset.PullData((isSucceeded) =>
//        {
//            if (isSucceeded)
//            {
//                Debug.Log("Data is successfully pulled.");
//                RemoteInitialize(true);
//            }
//            else
//                Debug.Log("Failed to pull the data.");
//        });
//    }
//#endif

//#if UNITY_EDITOR || DEVELOPMENT_BUILD
//    private void RemoteInitialize(bool isChangePersistent)
//    {
//        RemoteInitializeCommonData(isChangePersistent);
//        RemoteInitializeLandmarkData(isChangePersistent);
//        RemoteInitializeRewardData(isChangePersistent);
//        RemoteInitializeQuestData(isChangePersistent);
//        RemoteInitializeAchievementData(isChangePersistent);
//        RemoteInitializeGrowthData(isChangePersistent);
//        RemoteInitializeRareAnimalData(isChangePersistent);
//        SetInitData(isChangePersistent);


//        void RemoteInitializeCommonData(bool isPersistent)
//        {
//            string sheetName = DataIds.SHEET_NAME_COMMONINFO_DATA;

//            m_NetworkCheckInterval = (float)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.NETWORK_CHECK_TIMER, sheetName).TypedValue);

//#if UNITY_EDITOR
//            if (isPersistent)
//                EditorUtility.SetDirty(this);
//#endif


//            // LandmarkManager 초기화
//            {
//                /// <summary>
//                /// RemoteInitializeEvent arguments list of LandmarkManager
//                /// </summary>
//                /// <param order=0, param type=int> LandmarkManagerIdx(-1) </param>
//                /// <param order=1, param type=double> Init LevelUpOutputIncreaseRate </param>
//                /// <param order=2, param type=double> Init PhaseUpOutputIncreaseRate </param>
//                /// <param order=3, param type=double> Init LevelUpCostIncreaseRate </param>
//                /// <param order=4, param type=double> Init PhaseUpCostIncreaseRate </param>
//                /// <param order=5, param type=double> Init CommonIncreaseRate </param>
//                /// <param order=6, param type=int> LandmarkTotalCount </param>
//                int arg0 = -1;
//                double arg1 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.LEVEL_UPG_OUTPUT_INCRS_RATE, sheetName).TypedValue);
//                double arg2 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.PHASE_UPG_OUTPUT_INCRS_RATE, sheetName).TypedValue);
//                double arg3 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.LEVEL_UPG_COST_INCRS_RATE, sheetName).TypedValue);
//                double arg4 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.PHASE_UPG_COST_INCRS_RATE, sheetName).TypedValue);
//                double arg5 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.COMMON_INCRS_RATE_ONE_POINT_FIVE, sheetName).TypedValue);
//                int arg6 = _DataAsset.GetSheet<LandmarkDOB>(DataIds.SHEET_NAME_LANDMARK_DATA).Count;

//                _LandmarkManager.OnRemoteInitialize(RemoteInitType.COMMON_DATA, arg0, arg1, arg2, arg3, arg4, arg5, arg6);

//#if UNITY_EDITOR
//                if (isPersistent)
//                    EditorUtility.SetDirty(_LandmarkManager);
//#endif
//            }

//            // IdleManager 초기화
//            {
//                int heartTimerThresholdCount = (int)_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.HEART_TIMER_THRESHOLD_COUNT, sheetName).TypedValue;

//                /// <summary>
//                /// RemoteInitializeEvent arguments list of IdleManager
//                /// </summary>
//                /// <param order=0, param type=int> IdleManagerIdx(0) </param>
//                /// <param order=1, param type=intList> Heart Output Interval Thresholds </param>
//                /// <param order=2, param type=floatList> Heart Output Intervals </param>
//                /// <param order=3, param type=float> Rewarded Ad(Gold) interval </param>
//                /// <param order=4, param type=float> Rewarded Ad(Heart) interval </param>
//                /// <param order=5, param type=float> Rewarded Touch(Gold) interval </param>
//                /// <param order=6, param type=float> Rewarded Picture(Gold) interval </param>
//                /// <param order=7, param type=long> Offline Reward interval min </param>
//                int arg0 = 0;
//                List<int> arg1 = new List<int>();
//                List<float> arg2 = new List<float>();
//                for (int i = 0; i < heartTimerThresholdCount; i++)
//                {
//                    arg1.Add((int)(_DataAsset.GetFromSheet<CommonInfoDOB>(string.Format("{0}{1}", DataIds.HEART_TIMER_INTVL_THRESHOLD_, i), sheetName).TypedValue));
//                    arg2.Add((float)(_DataAsset.GetFromSheet<CommonInfoDOB>(string.Format("{0}{1}", DataIds.HEART_TIMER_INTVL_, i), sheetName).TypedValue));
//                }
//                float arg3 = (float)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.REWARDED_AD_GOLD_INTVL, sheetName).TypedValue);
//                float arg4 = (float)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.REWARDED_AD_HEART_INTVL, sheetName).TypedValue);
//                float arg5 = (float)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.REWARDED_TOUCH_GOLD_INTVL, sheetName).TypedValue);
//                float arg6 = (float)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.REWARDED_PICTURE_GOLD_INTVL, sheetName).TypedValue);
//                long arg7 = (long)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.OFFLINE_REWARD_TIMER_MIN, sheetName).TypedValue);

//                _IdleManager.OnRemoteInitialize(RemoteInitType.COMMON_DATA, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

//#if UNITY_EDITOR
//                if (isPersistent)
//                    EditorUtility.SetDirty(_IdleManager);
//#endif
//            }
//        }

//        void RemoteInitializeLandmarkData(bool isPersistent)
//        {
//            string sheetName = DataIds.SHEET_NAME_LANDMARK_DATA;

//            int landmarkCount = _DataAsset.GetSheet<LandmarkDOB>(sheetName).Count;
//            if (landmarkCount == 0)
//            {
//                Debug.LogError("Landmark count is zero! RemoteInitialize failed!");
//                return;
//            }
//            else if (landmarkCount != _LandmarkManager.LandmarkTotalCount)
//            {
//                Debug.LogError($"Fetched LandmarkCount({landmarkCount}) is different with internal landmarkCount({_LandmarkManager.LandmarkTotalCount})!");
//                return;
//            }

//            for (int i = 0; i < landmarkCount; i++)
//            {
//                /// <summary>
//                /// RemoteInitializeEvent arguments list of LandmarkData
//                /// </summary>
//                /// <param order=0, param type=int> LandmarkTypeIdx </param>
//                /// <param order=1, param type=double> Init GoldOutputInterval </param>
//                int arg0 = i;
//                double arg1 = _DataAsset.GetFromSheet<LandmarkDOB>(string.Format("{0}{1}", DataIds.LANDMARK_, i), sheetName).Gold_Init_Interval;

//                LandmarkData landmark = _LandmarkManager.GetLandmarkData(i);
//                landmark.OnRemoteInitialize(RemoteInitType.LANDMARK_DATA, arg0, arg1);

//#if UNITY_EDITOR
//                if (isPersistent)
//                    EditorUtility.SetDirty(landmark);
//#endif
//            }
//        }

//        void RemoteInitializeRewardData(bool isPersistent)
//        {
//            string sheetName = DataIds.SHEET_NAME_REWARD_DATA;
//            List<AnimalLevelRewardDOB> _AnimalLevelRewards = _DataAsset.GetSheet<AnimalLevelRewardDOB>(sheetName);
//            List<NewLandmarkRewardDOB> _NewLandmarkRewards = _DataAsset.GetSheet<NewLandmarkRewardDOB>(DataIds.SHEET_NAME_REWARD_DATA_NEWLANDMARK);

//            /// <summary>
//            /// RemoteInitializeEvent arguments list of RewardManager
//            /// </summary>
//            /// <param order=0, param type=intList> grantLevels </param>
//            /// <param order=1, param type=stringList> rewardSubTypes </param>
//            /// <param order=2, param type=doubleList> increaseRates </param>
//            /// <param order=3, param type=double> rewarded_Ad_Gold multiplier </param>
//            /// <param order=4, param type=double> rewarded_Ad_Heart multiplier </param>
//            /// <param order=5, param type=double> rewarded_Touch_Gold multiplier </param>
//            /// <param order=6, param type=double> rewarded_Picture_Gold multiplier </param>
//            /// <param order=7, param type=double> growth reward multiplier - UPG_SALE </param>
//            /// <param order=8, param type=double> growth reward multiplier - GOLD_PRODUCTION_INC </param>
//            /// <param order=9, param type=double> growth reward multiplier - ALBATROSS_UPG </param>
//            /// <param order=10, param type=double> growth reward multiplier - GIFTBOX_UPG </param>
//            /// <param order=11, param type=double> growth reward multiplier - MORE_HEART </param>
//            /// <param order=12, param type=double> growth reward additive - HEART_DOUBLE </param>
//            /// <param order=13, param type=double> growth reward additive - OFFLINE_UPG </param>
//            /// <param order=14, param type=double> growth reward multiplier - CAMERA_UPG </param>
//            /// <param order=15, param type=intListList> LookChange threshold levels </param>

//            /// <param order=16, param type=BigNumberList> New Landmark Rewards - Jewel </param>
//            /// <param order=17, param type=intList> New Landmark Rewards - RareAnimalRandomBox_Unique </param>
//            /// <param order=18, param type=BigNumberList> New Landmark Rewards - Heart </param>

//            List<int> arg0 = new List<int>();
//            List<string> arg1 = new List<string>();
//            List<double> arg2 = new List<double>();
//            for (int i = 0; i < _AnimalLevelRewards.Count; i++)
//            {
//                arg0.Add(_AnimalLevelRewards[i].GrantLevel);
//                arg1.Add(_AnimalLevelRewards[i].RewardSubType);
//                arg2.Add(_AnimalLevelRewards[i].IncreaseRate);
//            }

//            sheetName = DataIds.SHEET_NAME_COMMONINFO_DATA;
//            double arg3 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.REWARDED_AD_GOLD_MULTIPLIER, sheetName).TypedValue);
//            double arg4 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.REWARDED_AD_HEART_MULTIPLIER, sheetName).TypedValue);
//            double arg5 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.REWARDED_TOUCH_GOLD_MULTIPLIER, sheetName).TypedValue);
//            double arg6 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.REWARDED_PICTURE_GOLD_MULTIPLIER, sheetName).TypedValue);

//            double arg7 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.GROWTH_REWARD_AMOUNT_UPG_SALE, sheetName).TypedValue);
//            double arg8 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.GROWTH_REWARD_AMOUNT_GOLD_PRODUCTION_INC, sheetName).TypedValue);
//            double arg9 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.GROWTH_REWARD_AMOUNT_ALBATROSS_UPG, sheetName).TypedValue);
//            double arg10 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.GROWTH_REWARD_AMOUNT_GIFTBOX_UPG, sheetName).TypedValue);
//            double arg11 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.GROWTH_REWARD_AMOUNT_MORE_HEART, sheetName).TypedValue);
//            double arg12 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.GROWTH_REWARD_AMOUNT_HEART_DOUBLE, sheetName).TypedValue);
//            double arg13 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.GROWTH_REWARD_AMOUNT_OFFLINE_UPG, sheetName).TypedValue);
//            double arg14 = (double)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.GROWTH_REWARD_AMOUNT_CAMERA_UPG, sheetName).TypedValue);

//            int landmarkCount = _DataAsset.GetSheet<LandmarkDOB>(DataIds.SHEET_NAME_LANDMARK_DATA).Count;
//            List<List<int>> arg15 = new List<List<int>>();
//            for (int i = 0; i < landmarkCount; i++)
//            {
//                arg15.Add(new List<int>());
//                arg15[i].Add(_DataAsset.GetFromSheet<LandmarkDOB>(string.Format("{0}{1}", DataIds.LANDMARK_, i), DataIds.SHEET_NAME_LANDMARK_DATA).Evo_Threshold_0);
//                arg15[i].Add(_DataAsset.GetFromSheet<LandmarkDOB>(string.Format("{0}{1}", DataIds.LANDMARK_, i), DataIds.SHEET_NAME_LANDMARK_DATA).Evo_Threshold_1);
//                arg15[i].Add(_DataAsset.GetFromSheet<LandmarkDOB>(string.Format("{0}{1}", DataIds.LANDMARK_, i), DataIds.SHEET_NAME_LANDMARK_DATA).Evo_Threshold_2);
//                arg15[i].Add(_DataAsset.GetFromSheet<LandmarkDOB>(string.Format("{0}{1}", DataIds.LANDMARK_, i), DataIds.SHEET_NAME_LANDMARK_DATA).Evo_Threshold_3);
//            }

//            if(landmarkCount != _NewLandmarkRewards.Count)
//                Debug.LogError($"Landamark's count({landmarkCount}) is different with NewLandmarkRewards' count({_NewLandmarkRewards.Count})!");

//            List<BigNumber> arg16 = new List<BigNumber>();
//            List<int> arg17 = new List<int>();
//            List<BigNumber> arg18 = new List<BigNumber>();
//            for (int i = 0; i < _NewLandmarkRewards.Count; i++)
//            {
//                arg16.Add(ConvertStringIntoBigNumber(_NewLandmarkRewards[i].Reward_Jewel));
//                arg17.Add(_NewLandmarkRewards[i].Reward_RandomBox);
//                arg18.Add(ConvertStringIntoBigNumber(_NewLandmarkRewards[i].Reward_Heart));
//            }

//            _RewardManager.OnRemoteInitialize(RemoteInitType.REWARD_DATA, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);

//#if UNITY_EDITOR
//            if (isPersistent)
//                EditorUtility.SetDirty(_RewardManager);
//#endif
//        }

//        void RemoteInitializeQuestData(bool isPersistent)
//        {
//            string sheetName = DataIds.SHEET_NAME_QUEST_DATA;
//            List<QuestDOB> _quests = _DataAsset.GetSheet<QuestDOB>(sheetName);

//            int arg0 = 0;
//            List<string> arg1 = new List<string>();
//            List<string> arg2 = new List<string>();
//            List<int> arg3 = new List<int>();

//            for (int i = 0; i < _quests.Count; i++)
//            {
//                arg1.Add(_quests[i].Id);
//                arg2.Add(_quests[i].Type);
//                arg3.Add(_quests[i].Value);
//            }

//            _QuestManager.OnRemoteInitialize(RemoteInitType.QUEST_DATA, arg0, arg1, arg2, arg3);

//#if UNITY_EDITOR
//            if (isPersistent)
//                EditorUtility.SetDirty(_QuestManager);
//#endif
//        }

//        void RemoteInitializeAchievementData(bool isPersistent)
//        {
//            string sheetName = DataIds.SHEET_NAME_ACHIEVEMENT_DATA;
//            List<AchievementDOB> _Achievements = _DataAsset.GetSheet<AchievementDOB>(sheetName);

//            List<int> arg0 = new List<int>();
//            List<int> arg1 = new List<int>();
//            List<int> arg2 = new List<int>();
//            List<int> arg3 = new List<int>();
//            List<int> arg4 = new List<int>();
//            List<int> arg5 = new List<int>();
//            List<int> arg6 = new List<int>();
//            List<int> arg7 = new List<int>();

//            for (int i = 0; i < _Achievements.Count; i++)
//            {
//                arg0.Add(_Achievements[i].UPG_LNDMRK_0);
//                arg1.Add(_Achievements[i].GET_NORMAL_ANIMAL);
//                arg2.Add(_Achievements[i].CALL_ALBATROSS);
//                arg3.Add(_Achievements[i].PHOTO_MISSION);
//                arg4.Add(_Achievements[i].GET_TOUCH_REWARD);
//                arg5.Add(_Achievements[i].USE_TIME_TICKET);
//                arg6.Add(_Achievements[i].LNDMRK_EVOLUTION);
//                arg7.Add(_Achievements[i].OPEN_RARE_ANIMAL_BOX);
//            }
            
//            _AchievementManager.OnRemoteInitialize(RemoteInitType.ACHIEVEMENT_DATA, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

//#if UNITY_EDITOR
//            if (isPersistent)
//            {
//                EditorUtility.SetDirty(_AchievementManager);

//                AchievementData[] data = _AchievementManager.GetAchievementData();
//                for (int i = 0; i < data.Length; i++)
//                    EditorUtility.SetDirty(data[i]);
//            }
//#endif
//        }

//        void RemoteInitializeGrowthData(bool isPersistent)
//        {
//            string sheetName = DataIds.SHEET_NAME_GROWTH_DATA;
//            List<GrowthDOB> growth = _DataAsset.GetSheet<GrowthDOB>(sheetName);

//            List<string> arg0 = new List<string>();
//            List<int> arg1 = new List<int>();
//            List<int> arg2 = new List<int>();

//            for (int i = 0; i < growth.Count; i++)
//            {
//                //Debug.Log($"{i} : {growth[i].Id}, {growth[i].Level}, {growth[i].Type}");
//                arg0.Add(growth[i].Type);
//                arg1.Add(growth[i].Level);
//                arg2.Add(growth[i].TargetValue);
//            }

//            _GrowthManager.OnRemoteInitialize(RemoteInitType.GROWTH_DATA, arg0, arg1, arg2);

//#if UNITY_EDITOR
//            if (isPersistent)
//            {
//                EditorUtility.SetDirty(_GrowthManager);

//                GrowthData[] data = _GrowthManager.GetGrowthData();
//                for (int i = 0; i < data.Length; i++)
//                    EditorUtility.SetDirty(data[i]);
//            }
//#endif
//        }

//        void RemoteInitializeRareAnimalData(bool isPersistent)
//        {
//            string sheetName = DataIds.SHEET_NAME_RAREANIMAL_DATA;
//            List<RareAnimalDOB> data = _DataAsset.GetSheet<RareAnimalDOB>(sheetName);
//            const string RARE_ANIMAL_ID_ = "RARE_ANIMAL_";

//            for (int i = 0; i < data.Count; i++)
//            {
//                /// <summary>
//                /// RemoteInitializeEvent arguments list of RareAnimalData
//                /// </summary>
//                /// <param order=0, param type=int> data index </param>
//                /// <param order=1, param type=string> tier of animal card </param>
//                /// <param order=2, param type=int> increase rate </param>
//                /// <param order=3, param type=int> max count </param>
//                /// <param order=4, param type=int> max level </param>
//                /// <param order=5, param type=doubleList> multipliers by level </param>
//                /// <param order=6, param type=string> reward type </param>
//                /// <param order=7, param type=string> unlock condition </param>

//                string id = data[i].Id;
//                id = id.Replace(RARE_ANIMAL_ID_, string.Empty);
//                if (!int.TryParse(id, out int idx))
//                {
//                    Debug.LogError($"Id({id}) cannot be parsed to int!");
//                    return;
//                }

//                int arg0 = idx;
//                string arg1 = data[i].Tier;
//                int arg2 = data[i].IncreaseRate;
//                int arg3 = data[i].MaxCount;
//                int arg4 = data[i].MaxLevel;
//                List<double> arg5 = new List<double>();
//                arg5.Add(data[i].Multiplier_0);
//                arg5.Add(data[i].Multiplier_1);
//                arg5.Add(data[i].Multiplier_2);
//                arg5.Add(data[i].Multiplier_3);
//                arg5.Add(data[i].Multiplier_4);
//                arg5.Add(data[i].Multiplier_5);
//                arg5.Add(data[i].Multiplier_6);
//                arg5.Add(data[i].Multiplier_7);
//                arg5.Add(data[i].Multiplier_8);
//                arg5.Add(data[i].Multiplier_9);
//                arg5.Add(data[i].Multiplier_10);
//                string arg6 = data[i].RewardType;
//                string arg7 = data[i].UnlockCondition;

//                AnimalData_Rare rareAnimalData = _AnimalManager.GetRareAnimalData(arg0);
//                if (rareAnimalData)
//                {
//                    rareAnimalData.OnRemoteInitialize(RemoteInitType.RARE_ANIMAL_DATA, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

//#if UNITY_EDITOR
//                    if (isPersistent)
//                        EditorUtility.SetDirty(rareAnimalData);
//#endif
//                }
//                else
//                    Debug.LogError("Failed to RemoteInitialize of RareAnimalData because data is null!");
//            }
//        }


//        /// <summary>
//        /// 초기화 데이터 값을 설정합니다.
//        /// 매니저 클래스들의 초기화 로직에 연동되는 데이터(세이브 파일로 저장되거나 세이브 파일로부터 로드되는 데이터)는 이 함수 내부에서 값을 수정합니다.
//        /// 그렇지 않은 데이터(에디터 상에서 또는 재시작 시에만 한 번 초기화되는 데이터)는 상단의 RemoteInitialize- 함수 내부에서 값을 수정합니다.
//        /// InitData는 테스트 도중 임의로 값이 바뀔 수 있으므로 이 함수 내부에서 InitData의 '모든' 값을 초기화해야 합니다.
//        /// </summary>
//        void SetInitData(bool isPersistent)
//        {
//            SaveData initData = _SaveLoadManager.InitData;


//            #region CommonInfo Data

//            string sheetName = DataIds.SHEET_NAME_COMMONINFO_DATA;
//            SaveData.CommonData commonData = initData.GetCommonData;
//            commonData.USER_ID = string.Empty;
//            commonData.USER_EXIT_TIME = 0;
//            commonData.USER_PLAY_TIME = 0;
//            commonData.USER_COUNTRY = 0;
//            commonData.SETTING_LANGUAGE = 0;
//            commonData.SETTING_QUALITY = 0;
//            commonData.SETTING_BGM = 0;
//            commonData.SETTING_PUSH_NOTI = 0;
//            commonData.RESOURCE_GOLD = new BigNumber(50.0, 0);
//            commonData.RESOURCE_HEART = new BigNumber(100.0, 0);
//            commonData.RESOURCE_JEWEL= new BigNumber(0.0, 0);
//            commonData.RESOURCE_UTILITY = 0;
//            commonData.RESOURCE_MILEAGE = 0;
//            commonData.PURE_HEART_OUTPUT = (BigNumber)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.HEART_TIMER_INIT_OUTPUT, sheetName).TypedValue);
//            commonData.EVENT_PROGRESS_0 = false;
//            commonData.EVENT_PROGRESS_1 = false;
//            commonData.EVENT_PROGRESS_2 = false;
//            commonData.REWARDED_AD_GOLD_READY = false;
//            commonData.REWARDED_AD_GOLD_TIMER = 300.0F;     // 300초 MAX 기준. 보상형 광고(골드)의 최초 쿨타임이 0초가 되도록 예외 처리
//            commonData.REWARDED_AD_HEART_READY = false;
//            commonData.REWARDED_AD_HEART_TIMER = 300.0F;    // 300초 MAX 기준. 보상형 광고(하트)의 최초 쿨타임이 0초가 되도록 예외 처리
//            commonData.REWARDED_TOUCH_GOLD_READY = false;
//            commonData.REWARDED_TOUCH_GOLD_TIMER = 0;
//            commonData.REWARDED_CAMERA_READY = false;
//            commonData.REWARDED_CAMERA_TIMER = 120.0F;        // 120초 MAX 기준. 촬영 미션의 최초 쿨타임이 0초가 되도록 예외 처리
//            commonData.OFFLINE_REWARD_TIMER = 0.0;
//            commonData.REWARDED_ADV_ACCUMUATION_COUNT = 0;
//            commonData.FIRST_LAUNCH = true;
//            commonData.JEWEL_TIMER = 1800f;
//            commonData.RANDOM_BOX_TIMER = 1800f;
//            commonData.JEWEL_READY = true;
//            commonData.RANDOM_BOX_READY = true;
//            commonData.STAR_RATING_COMPLETE = false;
//            commonData.TIMER_TRIGGERED_REWARDED_AD_GOLD = false;
//            commonData.TIMER_TRIGGERED_REWARDED_AD_HEART = false;
//            commonData.TIMER_TRIGGERED_TOUCH_OBJECT = false;
//            commonData.TIMER_TRIGGERED_PHOTO_MISSION = false;

//    #endregion


//    #region Tutorial Data

//            SaveData.TutorialData tutorialData = initData.GetTutorialData;
//            tutorialData.NEWBIE_TUTORIAL_PHASE = (int)TutorialPhase.NONE;
//            tutorialData.SUB_TUTORIAL_0_CLEAR
//                = tutorialData.SUB_TUTORIAL_1_CLEAR
//                = tutorialData.SUB_TUTORIAL_2_CLEAR
//                = tutorialData.SUB_TUTORIAL_3_CLEAR
//                = tutorialData.SUB_TUTORIAL_4_CLEAR
//                = false;

//            #endregion


//            #region Landmark Animal Data

//            sheetName = DataIds.SHEET_NAME_LANDMARK_DATA;
//            int landmarkCount = _DataAsset.GetSheet<LandmarkDOB>(sheetName).Count;
//            if (landmarkCount == 0)
//            {
//                Debug.LogError("Landmark count is zero! RemoteInitialize failed!");
//                return;
//            }
//            else if (landmarkCount != _LandmarkManager.LandmarkTotalCount)
//            {
//                Debug.LogError($"Fetched LandmarkCount({landmarkCount}) is different with internal landmarkCount({_LandmarkManager.LandmarkTotalCount})!");
//                return;
//            }

//            List<SaveData.LandmarkAnimalData> landmarkAnimalData = initData.GetLandmarkAnimalData;
//            if (landmarkAnimalData == null)
//            {
//                Debug.Log("LandmarkAnimalData in InitData is null!");
//                return;
//            }
//            else
//                landmarkAnimalData.Clear();

//            for (int i = 0; i < landmarkCount; i++)
//            {
//                SaveData.LandmarkAnimalData landmarkData = new SaveData.LandmarkAnimalData();
//                landmarkData.TYPE_IDX = i;
//                landmarkData.LOCKED = true;
//                landmarkData.UPG_LEVEL = 0;
//                landmarkData.PURE_GOLD_OUTPUT = ConvertStringIntoBigNumber(_DataAsset.GetFromSheet<LandmarkDOB>(string.Format("{0}{1}", DataIds.LANDMARK_, i), sheetName).Gold_Init_Output);
//                landmarkData.LEVEL_UPG_COST = ConvertStringIntoBigNumber(_DataAsset.GetFromSheet<LandmarkDOB>(string.Format("{0}{1}", DataIds.LANDMARK_, i), sheetName).Level_Upg_Init_Cost);
//                landmarkData.PHASE_LEVEL = 0;
//                landmarkData.ANIMAL_COUNT = 0;
//                landmarkData.PHASE_MULTIPLIER = 1.0;
//                landmarkData.PHASE_UPG_COST = ConvertStringIntoBigNumber(_DataAsset.GetFromSheet<LandmarkDOB>(string.Format("{0}{1}", DataIds.LANDMARK_, i), sheetName).Phase_Upg_Init_Cost);
//                landmarkData.CURRENT_SHOW_ORDER = (int)ShowOrder.NONE;
//                landmarkData.NEXT_SHOW_PROGRESS = 0.0F;
//                landmarkData.NEW_ANIMAL_PROGRESS = 0.0F;
//                landmarkAnimalData.Add(landmarkData);
//            }

//            #endregion


//            #region Rare Animal Data

//            sheetName = DataIds.SHEET_NAME_RAREANIMAL_DATA;
//            List<RareAnimalDOB> _rareAnimaldata = _DataAsset.GetSheet<RareAnimalDOB>(sheetName);

//            List<SaveData.RareAnimalData> initRareAnimalData = initData.GetRareAnimalData;
//            if (initRareAnimalData == null)
//            {
//                Debug.Log("RareAnimalData in InitData is null!");
//                return;
//            }
//            else
//                initRareAnimalData.Clear();

//            const string RARE_ANIMAL_ID_ = "RARE_ANIMAL_";

//            for (int i = 0; i < _rareAnimaldata.Count; i++)
//            {
//                string id = _rareAnimaldata[i].Id;
//                id = id.Replace(RARE_ANIMAL_ID_, string.Empty);
//                if (!int.TryParse(id, out int idx))
//                {
//                    Debug.LogError($"Id({id}) cannot be parsed to int!");
//                    return;
//                }

//                SaveData.RareAnimalData _initRareAnimalData = new SaveData.RareAnimalData();
//                _initRareAnimalData.TYPE_IDX = idx;
//                _initRareAnimalData.LEVEL = 0;
//                _initRareAnimalData.CARD_COUNT = 0;
//                _initRareAnimalData.ANIMAL_COUNT = 0;
//                initRareAnimalData.Add(_initRareAnimalData);
//            }

//            #endregion

//            #region Store Data

//            SaveData.StoreData storeData = initData.GetStoreData;
//            storeData.AD_COUPON_SUBSCRIBE_PROGRESS = false;
//            storeData.PACKAGE_0_BUY_PROGRESS = false;
//            storeData.AD_COUPON_COUNT = 0;
//            storeData.NORMAL_BOX_COUNT = 0;
//            storeData.RARE_BOX_COUNT = 0;
//            storeData.GOLD_1H_TICKET_COUNT = 0;
//            storeData.GOLD_6H_TICKET_COUNT = 0;
//            storeData.GOLD_12H_TICKET_COUNT = 0;
//            storeData.HEART_1H_TICKET_COUNT = 0;
//            storeData.HEART_6H_TICKET_COUNT = 0;
//            storeData.HEART_12H_TICKET_COUNT = 0;

//            #endregion


//            #region Growth Data

//            SaveData.GrowthData growthData = initData.GetGrowthData;
//            growthData.GROWTH_LEVEL = 0;
//            growthData.UPG_SALE_LEVEL = 0;
//            growthData.UPG_SALE_VALUE = 100.0;
//            growthData.GOLD_PRODUCTION_INC_LEVEL= 0;
//            growthData.GOLD_PRODUCTION_INC_VALUE = 100.0;
//            growthData.ALBATROSS_UPG_LEVEL = 0;
//            growthData.ALBATROSS_UPG_VALUE = 100.0;
//            growthData.GIFTBOX_UPG_LEVEL = 0;
//            growthData.GIFTBOX_UPG_VALUE = 100.0;
//            growthData.MORE_HEART_LEVEL = 0;
//            growthData.MORE_HEART_VALUE = 100.0;
//            growthData.HEART_DOUBLE_LEVEL = 0;
//            growthData.HEART_DOUBLE_VALUE = 0;
//            growthData.OFFLINE_UPG_LEVEL = 0;
//            growthData.OFFLINE_UPG_VALUE = (double)((long)(_DataAsset.GetFromSheet<CommonInfoDOB>(DataIds.OFFLINE_REWARD_TIMER_MAX_INIT_VALUE, DataIds.SHEET_NAME_COMMONINFO_DATA).TypedValue));
//            growthData.CAMERA_UPG_LEVEL = 0;
//            growthData.CAMERA_UPG_VALUE = 100.0;

//            #endregion


//            #region Quest Data

//            SaveData.QuestData questData = initData.GetQuestData;
//            questData.QUEST_NUMBER = 0;
//            questData.QUEST_PROGRESS_VALUE = 0;
//            questData.QUEST_ALERT_STATE = false;

//            #endregion


//            #region Achievement Data

//            SaveData.AchievementData achievementData = initData.GetAchievementData;
//            achievementData.UPG_LNDMRK_0_LEVEL = 0;
//            achievementData.UPG_LNDMRK_0_VALUE = 0;
//            achievementData.GET_NORMAL_ANIMAL_LEVEL = 0;
//            achievementData.GET_NORMAL_ANIMAL_VALUE = 0;
//            achievementData.CALL_ALBATROSS_LEVEL = 0;
//            achievementData.CALL_ALBATROSS_VALUE = 0;
//            achievementData.PHOTO_MISSION_LEVEL = 0;
//            achievementData.PHOTO_MISSION_VALUE = 0;
//            achievementData.GET_TOUCH_REWARD_LEVEL = 0;
//            achievementData.GET_TOUCH_REWARD_VALUE = 0;
//            achievementData.USE_TIME_TICKET_LEVEL = 0;
//            achievementData.USE_TIME_TICKET_VALUE = 0;
//            achievementData.LNDMRK_EVOLUTION_LEVEL = 0;
//            achievementData.LNDMRK_EVOLUTION__VALUE = 0;
//            achievementData.OPEN_RARE_ANIMAL_BOX_LEVEL = 0;
//            achievementData.OPEN_RARE_ANIMAL_BOX_VALUE = 0;

//            #endregion


//            #region Multiplier Data

//            SaveData.MultiplierData multiplierData = initData.GetMultiplierData;
//            multiplierData.ANIMAL_MULTIPLIER = 100.0;
//            multiplierData.EVO_MULTIPLIER = 100.0;

//            #endregion

//#if UNITY_EDITOR
//            if (isPersistent)
//                EditorUtility.SetDirty(initData);
//#endif
//        }
//    }
//#endif
//}
