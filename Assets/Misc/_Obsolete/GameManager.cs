//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.Events;

//public class GameManager : SingletonScriptableObject<GameManager>   //, ISerializationCallbackReceiver
//{
//    public static bool IsInitialized { get; private set; } = false;
//    public static bool IsContinueInitialize { get; private set; } = false;
//    public static bool IsNeedToForceUpdate { get; private set; } = false;
//    public static bool IsNetworkConnected { get; private set; } = false;

//    private float m_NetworkCheckTimer = 0.0F;

//    [SerializeField] private float m_NetworkCheckInterval;

//    public bool IsSkipTutorial = false;

//    public static event Action WillCompleteInitialize;
//    public static event Action DidCompleteInitialize;

//    // GameManager의 모노비헤이비어 대리자에 접근하는 프로퍼티
//    public MonoBehaviour BehaviourProxy => Behaviour;

//    public static float LoadingProgress = 0.0F;

//    //------- Initialize & Reset -------//

//    //public void OnAfterDeserialize() { }
//    //public void OnBeforeSerialize() { }

//    //optional (but recommended)
//    //this method will run before the first scene is loaded. Initializing the singleton here
//    //will allow it to be ready before any other GameObjects on every scene and
//    //will prevent the "initialization on first usage". 
//    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//    public static void BeforeSceneLoad() { BuildSingletonInstance(); }

//    //optional,
//    //will run when the Singleton Scriptable Object is first created on the assets. 
//    //Usually this happens on edit mode, not runtime. (the override keyword is mandatory for this to work)
//    //public override void ScriptableObjectAwake()
//    //{
//    //    Debug.Log(GetType().Name + " created.");
//    //}

//    //optional,
//    //will run when the associated MonoBehavioir awakes. (the override keyword is mandatory for this to work)
//    public override void MonoBehaviourAwake()
//    {
//        Debug.Log(GetType().Name + " behaviour awake.");

//#if AI_TEST
//        return;
//#endif

//        IsInitialized = false;
//        Behaviour.StartCoroutine(Initialize(AddInitializables()));
//    }

//    public override void OnApplicationPause(bool isOn)
//    {
//        UnityEngine.Debug.Log($"OnApplicationPause : {isOn}");
//        if (IsInitialized)
//        {
//            if (isOn)
//            {
//                Signals.Get<UpdateOfflineTimeSignal>().Dispatch(false);
//                _SaveLoadManager.Save();
//            }
//            else
//            {
//                Signals.Get<UpdateOfflineTimeSignal>().Dispatch(true);
//                UpdateNetworkState();

//#if !UNITY_EDITOR && UNITY_ANDROID
//                Signals.Get<InvokeDeferredEventSignal>().Dispatch((int)DeferredEventType.REWARDED_AD_GRANT_REWARDS);
//                Signals.Get<InvokeDeferredEventSignal>().Dispatch((int)DeferredEventType.REWARDED_AD_CLOSED);
//#endif
//            }
//        }
//    }

//    public override void OnApplicationQuit()
//    {
//        Signals.Get<UpdateOfflineTimeSignal>().Dispatch(false);
//        _SaveLoadManager.Save();
//        DoReset(AddInitializables());
//        WillCompleteInitialize = null;
//        DidCompleteInitialize = null;
//    }


//    //////////////////////////////////////////
//    // Update
//    //////////////////////////////////////////

//    //Classic runtime Update method (the override keyword is mandatory for this to work).
//    //public override void FixedUpdate() { }
//    public override void Update()
//    {
//        if (!IsInitialized)
//            return;

//        float dt = Time.deltaTime;

//        //_FoliageManager.DrawGrass();

//        _IdleManager.UpdateIdle(dt);

//        _TouchManager.HandleTouch();

//        _QuestManager.UpdateQuestTimer(dt);

//        m_NetworkCheckTimer += dt;
//        if (m_NetworkCheckTimer >= m_NetworkCheckInterval)
//        {
//            m_NetworkCheckTimer = 0.0F;
//            UpdateNetworkState();
//        }
//    }

//    public override void LateUpdate()
//    {
//        if (!IsInitialized)
//            return;

//        UIManager.UpdateWorldUIs();
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void StartCameraCaptureEventHandler()
//    {
//        Time.timeScale = 0.0F;
//    }

//    private void EndCameraCaptureEventHandler()
//    {
//        Time.timeScale = 1.0F;
//    }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    private List<IInitializable> AddInitializables()
//    {
//        List<IInitializable> initializables = new List<IInitializable>();
//        initializables.Add(_ObjectPoolManager);
//        initializables.Add(_UIObjectPoolManager);
//        initializables.Add(_RewardManager);
//        initializables.Add(_QuestManager);
//        initializables.Add(_AchievementManager);
//        initializables.Add(_GrowthManager);
//        initializables.Add(_PlayerStateManager);
//        initializables.Add(_SoundManager);
//        initializables.Add(_DayNightManager);
//        //initializables.Add(_FoliageManager);
//        initializables.Add(_LandmarkManager);
//        initializables.Add(_AnimalManager);
//        initializables.Add(_CameraManager);
//        initializables.Add(_UIManager);
//        initializables.Add(_IdleManager);
//        initializables.Add(_TouchManager);
//        initializables.Add(_CrossPlatformManager);
//        initializables.Add(_EventManager);
//        initializables.Add(_BoidManager);
//        initializables.Add(_NavMeshManager);
//        initializables.Add(_AssetReferenceManager);
//        initializables.Add(_ShopManager);
//        return initializables;
//    }

//    private IEnumerator Initialize(List<IInitializable> initializables)
//    {
//        Loading loadingUI = FindObjectOfType<Loading>();

//        #region Launch App

//        GAManager.Instance.Initialize();

//        float loadingStartTime = Time.time;
//        GAManager.Instance.GA_DesignEvent(StatisticEventInfo.Launch.GAME);
//        GAManager.Instance.GA_DesignEvent(StatisticEventInfo.Launch.LOADING_START);

//        float loadingOffset = 0.05F;
//        LoadingProgress = 0.0F;

//        IsNetworkConnected = Application.internetReachability != NetworkReachability.NotReachable;

//        #endregion



//        #region Set Language

//        InitializeLanguageSettings();

//        #endregion



//        #region Init SDK

//        AppsFlyerObject.Instance.Init();

//        #endregion



//        #region Check App Version

//        IsContinueInitialize
//            = IsNeedToForceUpdate
//            = false;

//        BackendManager.Instance.Initialize(OnInitializeCallback);

//        while (!IsContinueInitialize)
//            yield return null;

//        if (IsNeedToForceUpdate)
//        {
//            UnityEngine.Debug.Log("Should update app to latest version!");
//            loadingUI.ShowUI_ForceUpdate();
//            yield break;
//        }

//        #endregion



//        #region Common Settings

//        loadingUI.Initialize();
//        DG.Tweening.DOTween.SetTweensCapacity(625, 250);

//        #endregion



//        #region Reset InitData

//#if UNITY_EDITOR || DEVELOPMENT_BUILD
//        if (IsNetworkConnected)
//        {
//            yield return Behaviour.StartCoroutine(FetchAndUpdateDataAsync());
//        }
//#endif

//        #endregion



//        #region Initialize SaveLoadManager

//        _SaveLoadManager.Initialize();
//        yield return new WaitUntil(() => _SaveLoadManager.IsInitialized);

//        #endregion



//        #region Dummy Progress

//        var WU_LOADING = new WaitUntil(() => Loading.IsReady);
//        var WFS = new WaitForFixedUpdate();

//        yield return WU_LOADING;

//        for (int i = 0; i < 15; i++)
//        {
//            loadingOffset += 0.02F;
//            LoadingProgress = loadingOffset;
//            yield return WFS;
//        }

//        #endregion



//        #region Override TutorialPhase

//        SaveData saveData = _SaveLoadManager.SaveDataForInit;
//        SaveData.TutorialData tutorialData = saveData.GetTutorialData;
//        TutorialPhase newbieTutorialPhase = (TutorialPhase)tutorialData.NEWBIE_TUTORIAL_PHASE;

//        if (newbieTutorialPhase <= TutorialPhase.PURIFY)
//        {
//            Debug.Log("Override TutorialPhase to NONE!");
//            TutorialEventTracker.NewbieTutorialPhase = TutorialPhase.NONE;
//            for (int i = 0; i < TutorialEventTracker.SubTutorial_Clear.Length; i++)
//                TutorialEventTracker.SubTutorial_Clear[i] = false;
//            saveData = _SaveLoadManager.InitData;
//        }
//        else
//        {
//            TutorialEventTracker.NewbieTutorialPhase = newbieTutorialPhase;
//            TutorialEventTracker.SubTutorial_Clear[0] = tutorialData.SUB_TUTORIAL_0_CLEAR;
//            TutorialEventTracker.SubTutorial_Clear[1] = tutorialData.SUB_TUTORIAL_1_CLEAR;
//            TutorialEventTracker.SubTutorial_Clear[2] = tutorialData.SUB_TUTORIAL_2_CLEAR;
//            TutorialEventTracker.SubTutorial_Clear[3] = tutorialData.SUB_TUTORIAL_3_CLEAR;
//            TutorialEventTracker.SubTutorial_Clear[4] = tutorialData.SUB_TUTORIAL_4_CLEAR;
//        }

//        if (saveData.GetCommonData.FIRST_LAUNCH)
//            GAManager.Instance.GA_DesignEvent(StatisticEventInfo.Launch.FIRST_GAME);

//        #endregion



//        #region Override by version

//        OverrideByAppVersion(saveData);

//        #endregion Override by version



//        #region Initialize remaining SO Managers

//        SaveLoadManager.SaveEvent += OnSave;

//        for (int i = 0; i < initializables.Count; i++)
//        {
//            //float initTime = Time.time;
//            initializables[i].Initialize(saveData);
//            yield return new WaitUntil(() => initializables[i].IsInitialized);
//            LoadingProgress = loadingOffset + ((i + 1) / (float)initializables.Count) * (1 - loadingOffset);
//            //UnityEngine.Debug.Log($"Initialize Complete({i}) : {Time.time - initTime}s elapsed.");
//        }
//        LoadingProgress = 1.0F;

//        #endregion



//        #region Initialize EventTrackers

//        List<EventTracker> eventTrackers = _EventManager.EventTrackers;
//        for (int i = 0; i < eventTrackers.Count; i++)
//            eventTrackers[i].Initialize();


//        yield return new WaitUntil(() =>
//        {
//            for (int i = 0; i < eventTrackers.Count; i++)
//            {
//                if (!eventTrackers[i].IsInitialized)
//                    return false;
//            }

//            return true;
//        });
//        _TutorialEventTracker = (TutorialEventTracker)eventTrackers.Find((a) => a.GetType() == typeof(TutorialEventTracker));

//        #endregion



//        #region Initialize ShowLandmarkGroups

//        WillCompleteInitialize?.Invoke();

//        List<ShowLandmarkGroup> landmarkGroups = new List<ShowLandmarkGroup>(FindObjectsOfType<ShowLandmarkGroup>());

//        yield return new WaitUntil(() =>
//        {
//            for (int i = 0; i < landmarkGroups.Count; i++)
//            {
//                if (!landmarkGroups[i].IsInitialized)
//                    return false;
//            }

//            return true;
//        });

//        Signals.Get<BakeLandmarkNavMeshSignal>().Dispatch((int)LandmarkNavMeshType.ALL);

//        #endregion



//        #region Initialize by TutorialState

//        InitializeByTutorialState(TutorialEventTracker.NewbieTutorialPhase);

//        #endregion



//        #region Prewarm

//        //Shader.WarmupAllShaders();

//        #endregion



//        Signals.Get<ChangeTouchContextSignal>().Dispatch(TouchContext.CAM_HANDLING);
//        Signals.Get<UpdatePlayerStatsSignal>().Dispatch(PlayerStat.ALL);
//        Signals.Get<ToggleWorldUIVisible>().Dispatch(false, true);

//        DidCompleteInitialize?.Invoke();

//        CameraManager.StartCameraCaptureEvent += StartCameraCaptureEventHandler;
//        CameraManager.EndCameraCaptureEvent += EndCameraCaptureEventHandler;

//        initializables = null;

//        GAManager.Instance.GA_DesignEvent(StatisticEventInfo.Launch.LOADING_END);
//        GAManager.Instance.GA_DesignEvent(StatisticEventInfo.Launch.LOADING_TIME, Mathf.Floor(Time.time - loadingStartTime));

//        IsInitialized = true;
//    }

//    private void DoReset(List<IInitializable> resettables)
//    {
//        for (int i = 0; i < resettables.Count; i++)
//            resettables[i].DoReset();

//        _SaveLoadManager.DoReset();

//        resettables = null;
//        _TutorialEventTracker = null;

//        CameraManager.StartCameraCaptureEvent -= StartCameraCaptureEventHandler;
//        CameraManager.EndCameraCaptureEvent -= EndCameraCaptureEventHandler;
//        TutorialEventTracker.OnCompleteNewbieTutorial -= DoCompleteNewbieTutorial;
//        SaveLoadManager.SaveEvent -= OnSave;

//        IsInitialized = false;
//    }

//    public void OnSave(SaveData data)
//    {
//        SaveData.TutorialData tutorialData = data.GetTutorialData;
//        tutorialData.NEWBIE_TUTORIAL_PHASE = (int)TutorialEventTracker.NewbieTutorialPhase;
//        tutorialData.SUB_TUTORIAL_0_CLEAR = TutorialEventTracker.SubTutorial_Clear[0];
//        tutorialData.SUB_TUTORIAL_1_CLEAR = TutorialEventTracker.SubTutorial_Clear[1];
//        tutorialData.SUB_TUTORIAL_2_CLEAR = TutorialEventTracker.SubTutorial_Clear[2];
//        tutorialData.SUB_TUTORIAL_3_CLEAR = TutorialEventTracker.SubTutorial_Clear[3];
//        tutorialData.SUB_TUTORIAL_4_CLEAR = TutorialEventTracker.SubTutorial_Clear[4];

//        SaveData.CommonData commonData = data.GetCommonData;
//        commonData.FIRST_LAUNCH = false;
//    }

//    private void UpdateNetworkState()
//    {
//        bool networkConnected = Application.internetReachability != NetworkReachability.NotReachable;
//        if (IsNetworkConnected != networkConnected)
//        {
//            IsNetworkConnected = networkConnected;
//            Signals.Get<UpdateNetworkStateSignal>().Dispatch(IsNetworkConnected);
//        }
//    }

//    public static void SetForceUpdateAppVersion()
//    {
//        IsNeedToForceUpdate = true;
//    }

//    public static void BackbuttonPush(UnityAction callback)
//    {
//#if UNITY_ANDROID || UNITY_EDITOR
//        BackbuttonManager.Instance.Push(callback);
//#endif
//    }

//    public static bool BackbuttonPop(bool isNeedInvoke = true)
//    {
//#if UNITY_ANDROID || UNITY_EDITOR
//        BackbuttonManager.Instance.Pop(isNeedInvoke);
//        Debug.Log("BackbuttonManager Pop");
//        return true;
//#else
//        return false;
//#endif
//    }

//    public static void SetBackbuttonState(bool isValid)
//    {
//#if UNITY_ANDROID || UNITY_EDITOR
//        BackbuttonManager.Instance.SetState(isValid);
//        Debug.Log($"BackbuttonManager SetBackbuttonState : {isValid}");
//#endif
//    }
//}

///*
//*  Notes:
//*  - Remember that you have to create the singleton asset on edit mode before using it. You have to put it on the Resources folder and of course it should be only one. 
//*  - Like other Unity Singleton this one is accessible anywhere in your code using the "Instance" property i.e: GameManager.Instance
//*/
