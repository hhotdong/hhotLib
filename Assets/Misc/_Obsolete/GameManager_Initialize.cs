//using System;
//using System.Collections;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using deVoid.UIFramework;
//using deVoid.Utils;
//using ScriptableObjectArchitecture;

//public enum LocalizationLanguageType { NONE = -1, English, Korean /*, RUSSIAN, KOREAN, JAPANESE, FRENCH, PORTUGUESE, GERMAN, ITALIAN, HINDI, ARABIC, TURKISH, S_CHINESE, T_CHINESE*/ }

//public partial class GameManager : SingletonScriptableObject<GameManager>
//{
//    [Header("Tutorial Infos")]
//    private TutorialEventTracker _TutorialEventTracker;

//    [Header("SO Events")]
//    [SerializeField] private IntGameEvent _OnPlayTimeline = default(IntGameEvent);

//    [Header("Debug Infos")]
//    [SerializeField] private LandmarkData _FirstLandmarkData;
//    [SerializeField] private LandmarkData _SecondLandmarkData;
//    [SerializeField] private IntGameEvent _OnShowFirstLandmark = default(IntGameEvent);
//    [SerializeField] private IntGameEvent _OnShowSecondLandmark = default(IntGameEvent);
//    [SerializeField] private IntGameEvent _OnAddAnimal = default(IntGameEvent);


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void DoCompleteNewbieTutorial()
//    {
//        // 1번 랜드마크 임시 언락제한 상태 해제
//        _LandmarkManager.ForceToggleLandmarkAllowedToUnlock((int)LandmarkType.LANDMARK_1, true);

//        // 상하단 UI 활성화
//        Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.PlayerStatePanel, true, new PlayerStatePanelProperty(true, true, true, true));
//        Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.NavigationPanel, true, new NavigationPanelProperty(true, true, true, true));
//        Signals.Get<ToggleNavigationSubButtonsSignal>().Dispatch(new NavigationPanelProperty(true, true, true, true));
//        Signals.Get<TogglePlayerStateSubButtonsSignal>().Dispatch(new PlayerStatePanelProperty(true, true, true, true));

//        // 마일스톤 UI(Landmark, Buoy) 활성화
//        UIObjectPoolManager.Get(ObjectPoolName.BUTTON_MILESTONE).GetComponent<UIGen_Milestone>().Initialize();
//        ObjectPoolManager.Get(ObjectPoolName.MILESTONE_BUOY).GetComponent<MilestoneBuoy>().Initialize();

//        // 튜토리얼이 종료되면 희귀동물 탭 버튼UI를 활성화
//        _TutorialEventTracker.animalWindowCloseButton.transform.parent.Find("Category_Rare_btn").GetComponent<CategoryButton>().Toggle(true);

//        // 원터치 버튼 활성화
//        _TutorialEventTracker.easyClickButton.ToggleButtonTemporarily(true);

//        // Water 컬라이더 활성화
//        CameraManager.WaterPlane.GetComponent<Collider>().enabled = true;

//        // 튜토리얼 보상 제공
//        Signals.Get<AddAdCouponSignal>().Dispatch(RewardManager.NEWBIE_TUTORIAL_REWARDS_AD_COUPON_AMOUNT);
//        Signals.Get<AddRandomBoxSignal>().Dispatch(1, RewardManager.NEWBIE_TUTORIAL_REWARDS_RARE_ANIMAL_RANDOM_BOX_AMOUNT);
//        Signals.Get<PlayCoinEffectSignal>().Dispatch((int)CoinEffectType.RARE_BOX, -1);

//        // Quest 시작시간 초기화
//        _QuestManager.CheckQuestActivationTimer = true;
//        _QuestManager.QuestStartTime = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        
//        // 초보자 튜토리얼 예외 처리 되돌리기
//        Signals.Get<SetNewbieTutorialStateSignal>().Dispatch(false);

//        // 개별 튜토리얼 시작
//        TutorialEventTracker.ShowSubTutorials();

//        // 튜토리얼 종료 이벤트 구독 해지
//        TutorialEventTracker.OnCompleteNewbieTutorial -= DoCompleteNewbieTutorial;
//    }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    /// <summary>
//    /// 앱 버전에 따라 저장 데이터를 수정합니다.
//    /// </summary>
//    private void OverrideByAppVersion(SaveData data)
//    {
//        Version version = new Version(Application.version);
//        if (version.Major <= 1 && version.Minor == 0 && version.Build < 5)
//        {
//            SaveData.CommonData _commonData = data.GetCommonData;
//            _commonData.TIMER_TRIGGERED_REWARDED_AD_GOLD
//                = _commonData.TIMER_TRIGGERED_REWARDED_AD_HEART
//                = _commonData.TIMER_TRIGGERED_TOUCH_OBJECT
//                = _commonData.TIMER_TRIGGERED_PHOTO_MISSION
//                = true;
//        }

//        if (version.Major <= 1 && version.Minor == 0 && version.Build < 8)
//        {
//            SaveData.CommonData _commonData = data.GetCommonData;
//            _commonData.STAR_RATING_COMPLETE
//                = true;
//        }
//    }

//    /// <summary>
//    /// 튜토리얼 단계에 따른 초기화 및 예외 처리를 수행한다.
//    /// </summary>
//    private void InitializeByTutorialState(TutorialPhase tutorialPhase)
//    {
//        // 메인 튜토리얼을 완료한 경우
//        if (tutorialPhase >= TutorialPhase.COMPLETE)
//        {
//            TutorialEventTracker.NewbieTutorialPhase = TutorialPhase.COMPLETE;

//            // 마일스톤 UI(Landmark, Buoy) 활성화
//            UIObjectPoolManager.Get(ObjectPoolName.BUTTON_MILESTONE).GetComponent<UIGen_Milestone>().Initialize();
//            ObjectPoolManager.Get(ObjectPoolName.MILESTONE_BUOY).GetComponent<MilestoneBuoy>().Initialize();

//            Signals.Get<IgnoreAllInputSignal>().Dispatch(false);
//            Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.TitlePanel, true, new TitlePanelPropery(PlayIngame, false));
//        }
//        // 메인 튜토리얼을 아직 완료하지 않은 경우 튜토리얼을 재개한다.
//        else
//        {
//            CameraManager.WaterPlane.GetComponent<Collider>().enabled = false;

//            _TutorialEventTracker.animalWindowCloseButton.transform.parent.Find("Category_Rare_btn").GetComponent<CategoryButton>().Toggle(false);

//            _LandmarkManager.ForceToggleLandmarkAllowedToUnlock((int)LandmarkType.LANDMARK_1, false);

//            TutorialEventTracker.OnCompleteNewbieTutorial += DoCompleteNewbieTutorial;

//            if (tutorialPhase > TutorialPhase.PURIFY)
//            {
//                // 튜토리얼 단계가 ZenMode 이상인 경우 보상획득 단계로 건너뛴다
//                if (tutorialPhase >= TutorialPhase.ZENMODE_ONE && tutorialPhase < TutorialPhase.NEWBIE_TUTORIAL_END)
//                {
//                    TutorialEventTracker.NewbieTutorialPhase = TutorialPhase.NEWBIE_TUTORIAL_END;
//                }

//                // 아래 튜토리얼 도중에 종료한 내역이 있는 경우 다음 단계로 건너뛴다.
//                if (tutorialPhase == TutorialPhase.PURIFY || tutorialPhase == TutorialPhase.AUTO_GEN_ENERGY)
//                    ++TutorialEventTracker.NewbieTutorialPhase;

//                Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.TitlePanel, true, new TitlePanelPropery(PlayTutorial, false));
//                Signals.Get<IgnoreAllInputSignal>().Dispatch(false);
//            }
//            // 최초 시작 시 또는 튜토리얼 단계가 PURIFY 이하인 경우 튜토리얼을 처음부터 재시작한다.
//            else if (tutorialPhase >= TutorialPhase.NONE)
//            {
//                CameraManager.CamHandler.enabled = false;
//                Shader.SetGlobalFloat("_TextureRadius", 0.0F);
//                _TutorialEventTracker.onShowFirstLandmark.Raise((int)ShowOrder.BASE);
//                _OnPlayTimeline.Raise((int)TimelineEvent.OPENING_SEEDING);
//                Signals.Get<ToggleWorldUIInteractable>().Dispatch(false);
//                Signals.Get<SetNewbieTutorialStateSignal>().Dispatch(true);
//            }
//        }


//        void PlayIngame()
//        {
//            Debug.Log("Play Ingame");
//            Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.PlayerStatePanel, true, new PlayerStatePanelProperty(true, true, true, true));
//            Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.NavigationPanel, true, new NavigationPanelProperty(true, true, true, true));
//            Signals.Get<ToggleWorldUIVisible>().Dispatch(true, false);
//            TutorialEventTracker.ShowSubTutorials();
//            _IdleManager.SetCameraAnimal();
//        }

//        void PlayTutorial()
//        {
//            Debug.Log("Play Tutorial");

//            Signals.Get<SetNewbieTutorialStateSignal>().Dispatch(true);

//            if (TutorialEventTracker.NewbieTutorialPhase == TutorialPhase.NEWBIE_TUTORIAL_END)
//            {
//                Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.NavigationPanel, true, new NavigationPanelProperty(false, false, false, false, () => TutorialEventTracker.ShowCurrentNewbieTutorial()));
//                return;
//            }
//            else if (TutorialEventTracker.NewbieTutorialPhase >= TutorialPhase.UPGRADE_LANDMARK)
//            {
//                Signals.Get<TogglePanelSignal>().Dispatch(ScreenIds.NavigationPanel, true, new NavigationPanelProperty(false, false, false, false));
//            }

//            TutorialEventTracker.ShowCurrentNewbieTutorial();
//        }
//    }

//    private BigNumber ConvertStringIntoBigNumber(string num)
//    {
//        double doubleVal;
//        string[] splitVal = num.Replace(" ", string.Empty).Split('_');
//        if (double.TryParse(splitVal[0], out doubleVal))
//        {
//            if (splitVal.Length > 1)
//            {
//                if (splitVal[1].ToLower().All(char.IsLetter))
//                    return new BigNumber(doubleVal, IncrementHelper.GetConvertedExponent(splitVal[1]));
//                else
//                {
//                    Debug.Log($"{splitVal[1]} is not letters!");
//                    return new BigNumber(doubleVal, 0);
//                }
//            }
//            else
//            {
//                //Debug.Log($"doubleVal A({num}) : {doubleVal}");
//                return new BigNumber(doubleVal, 0);
//            }
//        }
//        else
//        {
//            //Debug.Log($"doubleVal B({num}): {doubleVal}");
//            return new BigNumber(doubleVal, 0);
//        }
//    }

//    private void InitializeLanguageSettings()
//    {
//        LocalizationLanguageType selectedLang = LocalizationLanguageType.NONE;

//        if (PlayerPrefs.HasKey(PlayerPrefsKeys.USER_LANGUAGE))
//        {
//            int langIdx = PlayerPrefs.GetInt(PlayerPrefsKeys.USER_LANGUAGE);
//            selectedLang = (LocalizationLanguageType)langIdx;
//        }
//        else
//        {
//            switch (Application.systemLanguage)
//            {
//                //case SystemLanguage.Spanish:
//                //    selectedLang = LocalizationLanguageType.SPANISH;
//                //    break;
//                //case SystemLanguage.Russian:
//                //    selectedLang = LocalizationLanguageType.RUSSIAN;
//                //    break;
//                //case SystemLanguage.Japanese:
//                //    selectedLang = LocalizationLanguageType.JAPANESE;
//                //    break;
//                //case SystemLanguage.French:
//                //    selectedLang = LocalizationLanguageType.FRENCH;
//                //    break;
//                //case SystemLanguage.Portuguese:
//                //    selectedLang = LocalizationLanguageType.PORTUGUESE;
//                //    break;
//                //case SystemLanguage.German:
//                //    selectedLang = LocalizationLanguageType.GERMAN;
//                //    break;
//                //case SystemLanguage.Italian:
//                //    selectedLang = LocalizationLanguageType.ITALIAN;
//                //    break;
//                //case SystemLanguage.ChineseSimplified:
//                //    selectedLang = LocalizationLanguageType.S_CHINESE;
//                //    break;
//                //case SystemLanguage.Chinese:
//                //case SystemLanguage.ChineseTraditional:
//                //    selectedLang = LocalizationLanguageType.T_CHINESE;
//                //    break;

//                case SystemLanguage.Korean:
//                    selectedLang = LocalizationLanguageType.Korean;
//                    break;

//                case SystemLanguage.English:
//                default:
//                    System.Globalization.CultureInfo myCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
//                    //if (myCulture.ThreeLetterISOLanguageName == "hin")
//                    //{
//                    //    selectedLang = LocalizationLanguageType.HINDI;
//                    //    break;
//                    //}
//                    selectedLang = LocalizationLanguageType.English;
//                    break;
//            }
//        }

//        string selectedLangString = selectedLang.ToString();
//        if (I2.Loc.LocalizationManager.HasLanguage(selectedLangString))
//            I2.Loc.LocalizationManager.CurrentLanguage = selectedLangString;

//        if (selectedLang != LocalizationLanguageType.NONE)
//        {
//            PlayerPrefs.SetInt(PlayerPrefsKeys.USER_LANGUAGE, (int)selectedLang);
//            PlayerPrefs.Save();
//        }
//    }


//    // Backend(뒤끝) 콜백
//    private void OnInitializeCallback(BackendCallbackState state)
//    {
//        switch (state)
//        {
//            case BackendCallbackState.Success:
//            case BackendCallbackState.Fail:
//            case BackendCallbackState.UserCancel:
//            case BackendCallbackState.NotConnectInternet:
//            case BackendCallbackState.NotSignIn:
//            case BackendCallbackState.DuplicateNickname:
//            case BackendCallbackState.ClosedServer:
//                break;
//        }

//        IsContinueInitialize = true;
//        OnBackendCallbackDebugFunction("OnInitializeCallback", state);
//    }

//    private void OnLoginCallback(BackendCallbackState state)
//    {
//        OnBackendCallbackDebugFunction("OnLoginCallback", state);
//    }

//    private void OnLogoutCallback(BackendCallbackState state)
//    {
//        OnBackendCallbackDebugFunction("OnLogoutCallback", state);
//    }

//    private void OnSaveDataCallback(BackendCallbackState state)
//    {
//        OnBackendCallbackDebugFunction("OnSaveDataCallback", state);
//    }

//    private void OnBackendCallbackDebugFunction(string tag, BackendCallbackState state)
//    {
//        switch (state)
//        {
//            case BackendCallbackState.Success:
//                UnityEngine.Debug.Log(tag + " - 성공");
//                break;
//            case BackendCallbackState.UserCancel:
//                UnityEngine.Debug.Log(tag + " - 사용자가 직접 취소");
//                break;
//            case BackendCallbackState.NotConnectInternet:
//                UnityEngine.Debug.Log(tag + " - 인터넷 연결이 되어있지 않습니다.");
//                break;
//            case BackendCallbackState.NotSignIn:
//                UnityEngine.Debug.Log(tag + " - 로그인이 되어있지 않습니다.");
//                break;
//            case BackendCallbackState.ClosedServer:
//                UnityEngine.Debug.Log(tag + " - 닫힌 서버.");
//                break;
//            case BackendCallbackState.DuplicateNickname:
//                UnityEngine.Debug.Log(tag + " - 중복된 이름.");
//                break;
//            case BackendCallbackState.Fail:
//                UnityEngine.Debug.Log(tag + " - 실패.");
//                break;
//        }
//    }


//    //////////////////////////////////////////
//    // Debug functions
//    //////////////////////////////////////////

//    public void Restart(bool skipTutorial)
//    {
//#if DEVELOPMENT_BUILD || UNITY_EDITOR
//        _SaveLoadManager.DisableSave();

//        IsSkipTutorial = skipTutorial;
//        ResetData();
//        Behaviour.StopAllCoroutines();
//        Behaviour.StartCoroutine(LoadSceneAsync());


//        void ResetData()
//        {
//            PlayerPrefs.DeleteAll();
//            SaveLoadSystem.Delete();
//            UIManager.WorldUIs.Clear();
//            Signals.Get<UpdatePlayerStatsSignal>().Dispatch(PlayerStat.ALL);
//            _TutorialEventTracker?.ClearAllListenersWhenReset();
//        }

//        IEnumerator LoadSceneAsync()
//        {
//            WaitForSeconds WS = new WaitForSeconds(0.1F);
//            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Play");

//            IsInitialized = false;
//            asyncLoad.allowSceneActivation = false;

//            while (!asyncLoad.isDone)
//            {
//                if (asyncLoad.progress >= 0.9F)
//                {
//                    DoReset(AddInitializables());

//                    yield return WS;

//                    asyncLoad.allowSceneActivation = true;
//                    yield return new WaitUntil(() => asyncLoad.isDone);

//                    if (!UpdateManager.CheckIfArrayContainsItem((OverridableMonoBehaviour)Behaviour))
//                        UpdateManager.AddItem((OverridableMonoBehaviour)Behaviour);

//                    yield return Behaviour.StartCoroutine(Initialize(AddInitializables()));

//                    yield return WS;
//                }
//            }
//        }
//#else
//        Debug.Log("Restart function can only be called on development build!");
//        return;
//#endif
//    }
//}
