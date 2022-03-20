//using System;
//using System.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using BackEnd;
//using Facebook.Unity;
//using LitJson;
//using BackEnd.GlobalSupport;
//using Utilities = Nanali.Utilities;


//[RequireComponent(typeof(Mng_Encryption))]
//public class BackendManager : MonoBehaviour
//{
//    enum BackendProcessType
//    {
//        None,
//        CustomSignIn,
//        SocialSignIn,
//        SignOut
//    }
//    BackendProcessType processType = BackendProcessType.None;
//    BackendReturnObject BRO = new BackendReturnObject();

//    class FederationSignInInformation //소셜 로그인 정보.
//    {
//        public SignInCallType callType;
//        public FederationType federationType;
//        public string token;
//    }
//    FederationSignInInformation SignInInfo;

//    static BackendManager _instance;
//    public static BackendManager Instance
//    {
//        get
//        {
//            if (_instance == null)
//                _instance = FindObjectOfType(typeof(BackendManager)) as BackendManager;
//            return _instance;
//        }
//    }

//    public BackendAsset backendAsset;
//    //public bool IsNotConnectMode = false;

//    public string[] TableKeys;

//    //얼리억세스 유저, 로그인이 수행될 경우 확인 됨.
//    //public bool IsEarlyAccessUser { get; private set; }

//    private static int TimeGap;
//    //callback.
//    Action<BackendCallbackState> InitializeCallback;
//    Action<BackendCallbackState> Callback;

//    //flag
//    private bool IsWaitForSDKInit;

//    void DebugLog(object msg)
//    {
//        if (backendAsset.IsShowLog)
//            Debug.Log("BackendManager : " + msg);
//    }

//    //DateTime.Now 대체제.
//    public static DateTime DateTimeNow
//    {
//        get
//        {
//            return DateTime.Now.AddSeconds(-TimeGap);
//        }
//    }

//    public static DateTime DateKST //한국 표준 시
//    {
//        get
//        {
//            if (Backend.IsInitialized)
//            {
//                BackendReturnObject servertime = Backend.Utils.GetServerTime();
//                string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
//                DateTime parsedDate = DateTime.Parse(time);
//                parsedDate = parsedDate.AddHours(9);

//                return parsedDate;
//            }
//            else
//            {
//                return DateTime.Now;
//            }
//        }
//    }

//    public static bool IsValidToAutoSignIn //자동로그인(소셜) 가능 여부.
//    {
//        get
//        {
//            return FB.IsLoggedIn || (GPGSManager.Instance != null && GPGSManager.Instance.IsAutoLogin)
//               || (SIWAManager.Instance != null && SIWAManager.Instance.IsAutoLoggedIn);
//        }
//    }

//    public static bool IsBackendSocialLoggedIn { get; private set; } //뒤끝 연결 여부.
//    public static bool IsInitialized { get { return Backend.IsInitialized; } } //뒤끝 초기화 상태.

//    //어플리케이션이 상단으로 올라올 때 체크.
//    void OnApplicationPause(bool paused)
//    {
//        //앱이 올라올 때 시간 체크.
//        if (!paused)
//        {
//            if (!Backend.IsInitialized) //백엔드 서비스가 활성화중이 아닐경우 작동하지 않음.
//                return;
//            //reset gap.
//            CheckTimeGap();
//        }
//    }

//    //서버 시간과의 시간 차 체크.
//    void CheckTimeGap()
//    {
//        if (!Utilities.IsConnectedInternet)
//            return;

//        if (!Backend.IsInitialized)
//            return;

//        BackendReturnObject servertime = Backend.Utils.GetServerTime();
//        if (servertime.IsSuccess())
//        {
//            string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
//            DateTime parsedDate = DateTime.Parse(time);
//            TimeGap = Convert.ToInt32((DateTime.Now - parsedDate).TotalSeconds);
//        }
//    }

//    /*
//     * 뒤끝 콘솔에서 설정한 최신의 버전정보 가져오기
//     */
//    public bool IsNeedToForceUpdate()
//    {
//        BackendReturnObject versionBRO = Backend.Utils.GetLatestVersion();

//        DebugLog(versionBRO);

//        if (versionBRO.IsSuccess())
//        {
//            string version = versionBRO.GetReturnValuetoJSON()["version"].ToString();
//            DebugLog(version);
//            Version serverVersion = new Version(version);
//            Version clientVersion = new Version(Application.version);

//            if (serverVersion.Major > clientVersion.Major) //서버에 세팅된 메이저버전이 클라이언트보다 높다면.
//            {
//                //강제 업데이트.
//                DebugLog("업데이트가 필요합니다.");
//                return true;
//            }
//            else
//            {
//                //Minor 버전 체크.
//                if (serverVersion.Minor > clientVersion.Minor)
//                {
//                    //강제 업데이트.
//                    DebugLog("업데이트가 필요합니다.");
//                    return true;
//                }
//                else
//                {
//                    DebugLog("버전 체크 정상");
//                return false;
//                }
//            }
//        }
//        else
//        {
//            return false; //연결 실패의 상황. 정상으로 처리.
//        }
//    }

//    //서버 상태 체크.
//    public void CheckServer(Action<bool> callback) //state = 0 : 온라인, state = 1 : 오프라인, state = 2 : 점검
//    {
//        if (!Utilities.IsConnectedInternet || !Backend.IsInitialized)
//        {
//            callback(false);
//            return;
//        }

//        string json = Backend.Utils.GetServerStatus().GetReturnValue();
//        Hashtable data = Procurios.Public.JSON.JsonDecode(json) as Hashtable;
//        DebugLog("server status (0 = online, 1 = offline, 2 = check), current state : " + data["serverStatus"]);
//        int state;
//        int.TryParse(data["serverStatus"].ToString(), out state);
//        callback(state == 0);
//    }

//    //페이스북 초기화 콜백.
//    void FacebookInitCallback()
//    {
//        if (FB.IsInitialized) {
//            FB.ActivateApp();
//        }
//        else {
//            DebugLog("Failed to Initialize the Facebook SDK");
//        }

//        IsWaitForSDKInit = false;
//    }

//    private void Awake()
//    {
//        BackendManager[] bm = FindObjectsOfType<BackendManager>();
//        if (bm.Length >= 2)
//        {
//            Destroy(this.gameObject);
//        }
//    }

//    private void Start()
//    {
//        Debug.Log("Initialize BackendManager");

//    }

//    //초기화.
//    public void Initialize(Action<BackendCallbackState> callback)
//    {
//        processType = BackendProcessType.None;
//        InitializeCallback = callback;

//        StartCoroutine(InitializeCoroutine());
//    }

//    IEnumerator InitializeCoroutine()
//    {
//        if (!Utilities.IsConnectedInternet)
//        {
//            InitializeCallback?.Invoke(BackendCallbackState.NotConnectInternet);
//            yield break;
//        }

//        if (Backend.IsInitialized)
//        {
//            BackendInitializedAction();
//        }
//        else
//        {
//            //페이스북 초기화.
//            if (!FB.IsInitialized)
//            {
//                IsWaitForSDKInit = true;
//                FB.Init(FacebookInitCallback);
//            }
//            else //페북이 이미 초기화 상태라면.
//            {
//                FB.ActivateApp(); //지정 함수 실행.
//            }

//            while (IsWaitForSDKInit)
//                yield return null;

//#if UNITY_ANDROID
//            //구글 초기화.
//            IsWaitForSDKInit = true;
//            GPGSManager.Instance.LoginGPGS((success) =>
//            {
//                IsWaitForSDKInit = false;
//            });

//            while (IsWaitForSDKInit)
//                yield return null;
//#endif

//            Backend.Initialize(bro =>
//            {
//                if (bro.IsSuccess())
//                {
//                    BackendInitializedAction();
//                }
//                else
//                {
//                    // 초기화 실패 시 로직
//                    DebugLog("Failed to initialize the backend");

//                    InitializeCallback?.Invoke(BackendCallbackState.Fail);
//                }
//            });
//        }        
//    }

//    void BackendInitializedAction()
//    {
//        // 초기화 성공 시 로직
//        DebugLog("Google hash - " + Backend.Utils.GetGoogleHash());

//        CheckTimeGap();

//        if (IsNeedToForceUpdate())
//        {
//            //강제 업데이트 팝업.
//            GameManager.SetForceUpdateAppVersion();
//            return;
//        }

//        if (IsValidToAutoSignIn)
//        {
//            if (FB.IsInitialized && FB.IsLoggedIn)
//            {
//                //페이스북으로 자동 로그인.
//                DebugLog("페이스북 자동 로그인 성공");

//                FederationSignIn(SignInCallType.Auto, FederationType.Facebook, AccessToken.CurrentAccessToken.TokenString);
//            }
//            else if (GPGSManager.Instance != null && GPGSManager.Instance.IsAutoLogin) //GPGS에 로그인 되어 있음.
//            {
//                //구글로 자동 로그인.
//                DebugLog("구글 자동 로그인 시작");
//                GPGSManager.Instance.Login((bool success) => {
//                    if (success)
//                    {
//                        DebugLog("구글 자동 로그인 성공");
//                        FederationSignIn(SignInCallType.Auto, FederationType.Google, GPGSManager.Instance.Token);
//                    }
//                    else
//                    {
//                        CustomSignIn();
//                    }
//                });
//            }
//            else if (SIWAManager.Instance != null)// && SIWAManager.Instance.IsAutoLoggedIn) //애플에 로그인 되어 있음.
//            {
//                //애플로 자동 로그인.
//                DebugLog("애플 자동 로그인 시작");
//                SIWAManager.Instance.CheckCreditialStateDoneConfirm((bool DoAutoSignIn) => { //자격증명이 끝났는지 체크.
//                    if (DoAutoSignIn)
//                    {
//                        DebugLog("애플 자동 로그인 성공");
//                        FederationSignIn(SignInCallType.Auto, FederationType.Apple, SIWAManager.Instance.Token);
//                    }
//                    else
//                        CustomSignIn();
//                });
//            }
//            else //모든 자동로그인 실패.
//            {
//                CustomSignIn();
//            }
//        }
//        else //소셜 로그인 상태가 아니면 커스텀 로그인.
//        {
//            CustomSignIn();
//        }
//    }



//#if UNITY_IOS
//    class KeyChainForiOS
//    {
//        public string userId;
//        public string uuid;
//    }
//#endif

//    void CustomSignIn() //디바이스 uuid값으로 로그인 진행. 필요에 따라 유저가 직접 입력하는 로그인 과정을 추가 할 수 있습니다.
//    {
//        string uniqueID = "";
//#if UNITY_ANDROID || UNITY_EDITOR
//        uniqueID = SystemInfo.deviceUniqueIdentifier;
//#elif UNITY_IOS
//        KeyChainForiOS keyChainClass = JsonUtility.FromJson<KeyChainForiOS>(iOSKeyChain.BindGetKeyChainUser());
//        uniqueID = keyChainClass.uuid;

//        if (string.IsNullOrEmpty(uniqueID)) {
//            iOSKeyChain.BindSetKeyChainUser("0", SystemInfo.deviceUniqueIdentifier);
//            uniqueID = SystemInfo.deviceUniqueIdentifier;
//        }

//        //uniqueID = UnityEngine.iOS.Device.advertisingIdentifier;
//#endif
//        string pw = Encryptor.GetEncryptString(uniqueID, true);

//        Backend.BMember.CustomSignUp(uniqueID, pw);
//        DebugLog("유저 커스텀 아이디 : " + uniqueID);

//        Backend.BMember.CustomLogin(uniqueID, pw, (bro) =>
//        {
//            BRO = bro;
//            processType = BackendProcessType.CustomSignIn;
//        });
//    }

   

//    void FederationSignIn(SignInCallType _callType, FederationType _federationType, string token)
//    {
//        Backend.BMember.AuthorizeFederation(token, _federationType, "etc", (bro) =>
//        {
//            DebugLog("소셜 로그인 프로세스 시작");

//            BRO = bro;
//            SignInInfo = new FederationSignInInformation();
//            SignInInfo.callType = _callType;
//            SignInInfo.federationType = _federationType;
//            SignInInfo.token = token;
//            processType = BackendProcessType.SocialSignIn;
//        });
//    }

//    public void SignOut(Action<BackendCallbackState> callback = null)
//    {
//        //뒤끝 로그아웃.
//        Backend.BMember.Logout((bro) =>
//        {
//            Callback = callback;
//            BRO = bro;
//            processType = BackendProcessType.SignOut;
//        });
//    }

//    private void FixedUpdate()
//    {
//        //커스텀 로그인.
//        if (processType == BackendProcessType.CustomSignIn)
//        {
//            if (BRO.IsSuccess())
//            {
//                DebugLog("커스텀 로그인 성공");

//                //BackendReturnObject broUserInfo = Backend.BMember.GetUserInfo();
//                //string indate = broUserInfo.GetReturnValuetoJSON()["row"]["inDate"].ToString();
//                //DateTime parsedInDate = DateTime.Parse(indate);
//                //Version clientVersion = new Version(Application.version);
//                //IsEarlyAccessUser = (new DateTime(2020, 9, 25) - parsedInDate).TotalDays > 0 && clientVersion.Major >= 2; //2.0.0 버전 이상에서 확인 할 때 가입된 날짜가 2020년 9월 25일 이전인 경우 얼리억세스 유저.

//                //DebugLog("얼리억세스 유저 : " + IsEarlyAccessUser);
//                InitializeCallback?.Invoke(BackendCallbackState.Success);
//            }
//            else
//            {
//                DebugLog("커스텀 로그인 실패: " + BRO.ToString());
//                InitializeCallback?.Invoke(BackendCallbackState.Fail);
//            }

//            processType = BackendProcessType.None;
//            BRO.Clear();
//        }

//        //소셜 로그인.
//        if (processType == BackendProcessType.SocialSignIn)
//        {
//            if (BRO.IsSuccess())
//            {
//                DebugLog("로그인 완료.");

//                IsBackendSocialLoggedIn = true;

//                SetUserDefaultInformation();
//                SetPushToken(false); //동의상태에 따라 알람 수신 여부 결정.

//                //아래와 같이 정보 가져오기 진행 가능.
//                //BackendReturnObject user_obj = Backend.GameInfo.GetPrivateContents("User");

//                DownloadMails();

//                if (SignInInfo.callType == SignInCallType.Manual)
//                {
//                    DebugLog("수동 로그인 완료.");
//                }
//                else
//                {
//                    DebugLog("자동 로그인 완료.");
//                }

//                Callback?.Invoke(BackendCallbackState.Success);
//                InitializeCallback?.Invoke(BackendCallbackState.Success);
//            }
//            else
//            {
//                DebugLog("로그인 실패 : " + BRO);

//                //소셜 로그인 해제.
//                if (SignInInfo.federationType == FederationType.Facebook)
//                    FB.LogOut();
//                else if (SignInInfo.federationType == FederationType.Google && GPGSManager.Instance != null)
//                    GPGSManager.Instance.Logout();
//                else if (SignInInfo.federationType == FederationType.Apple && SIWAManager.Instance != null)
//                    SIWAManager.Instance.Logout();

//                if (SignInInfo.callType == SignInCallType.Manual) //수동 로그인일 경우 실패 전달.
//                {
//                    Callback?.Invoke(BackendCallbackState.Fail);
//                    InitializeCallback?.Invoke(BackendCallbackState.Fail);
//                }
//                //자동 소셜로그인 실패. 뒤끝 일반 로그인 진행.
//                CustomSignIn();
//            }

//            SignInInfo = null;
//            processType = BackendProcessType.None;
//            BRO.Clear();
//        }

//        //로그아웃.
//        if (processType == BackendProcessType.SignOut)
//        {
//            if (BRO.IsSuccess())
//            {
//                if (IsBackendSocialLoggedIn)
//                {
//                    IsBackendSocialLoggedIn = false;
//                    DebugLog("소셜 로그아웃 성공");

//                    //소셜 로그아웃.
//                    if (FB.IsLoggedIn)
//                        FB.LogOut();
//                    else if (GPGSManager.Instance != null && GPGSManager.Instance.IsLoggedIn)
//                        GPGSManager.Instance.Logout();
//                    else if (SIWAManager.Instance != null && SIWAManager.Instance.IsLoggedIn)
//                        SIWAManager.Instance.Logout();

//                    CustomSignIn();
//                }
//                else
//                {
//                    DebugLog("로그아웃 성공");
//                }

//                Callback?.Invoke(BackendCallbackState.Success);
//            }
//            else
//            {
//                DebugLog("로그아웃 실패: " + BRO.ToString());
//                Callback?.Invoke(BackendCallbackState.Fail);
//            }

//            processType = BackendProcessType.None;
//            BRO.Clear();
//        }
//    }

//    //gpgs SignIn.
//    public void GPGSSignIn(Action<BackendCallbackState> callback = null)
//    {
//        if (!Utilities.IsConnectedInternet)
//        {
//            callback(BackendCallbackState.NotConnectInternet);
//            return;
//        }

//        CheckServer((bool success) =>
//        {
//            if (success)
//            {
//                DebugLog("구글 플레이 로그인 시작");

//                Callback = callback;

//                //try to gpgs SignIn.
//                if (GPGSManager.Instance != null)
//                    GPGSManager.Instance.Login(OnGPGSSignIn);
//            }
//            else
//            {
//                callback(BackendCallbackState.ClosedServer);
//            }
//        });
//    }

//    void OnGPGSSignIn(bool success)
//    {
//        if (success)
//        {
//            DebugLog("구글 로그인 성공");

//            Backend.BMember.Logout((bro) =>
//            {
//                if (bro.IsSuccess())
//                    FederationSignIn(SignInCallType.Manual, FederationType.Google, GPGSManager.Instance.Token);
//            });
//        }
//        else
//        {
//            DebugLog("구글 로그인 실패");
//            Callback(BackendCallbackState.Fail);
//        }
//    }


//    //apple SignIn.
//    public void AppleSignIn(Action<BackendCallbackState> callback = null)
//    {
//        if (!Utilities.IsConnectedInternet)
//        {
//            callback(BackendCallbackState.NotConnectInternet);
//            return;
//        }

//        CheckServer((bool success) =>
//        {
//            if (success)
//            {
//                DebugLog("애플 로그인 시작");

//                Callback = callback;

//                if (SIWAManager.Instance != null)
//                    SIWAManager.Instance.Login(OnAppleSignIn);
//            }
//            else
//            {
//                callback(BackendCallbackState.ClosedServer);
//            }
//        });
//    }

//    void OnAppleSignIn(bool success)
//    {
//        if (success)
//        {
//            DebugLog("애플 로그인 성공");
//            Backend.BMember.Logout((bro) =>
//            {
//                if (bro.IsSuccess())
//                    FederationSignIn(SignInCallType.Manual, FederationType.Apple, SIWAManager.Instance.Token);
//            });
//        }
//        else
//        {
//            DebugLog("애플 로그인 실패");
//            Callback(BackendCallbackState.Fail);
//        }
//    }


//    //facebook SignIn.
//    public void FacebookSignIn(Action<BackendCallbackState> callback = null) //페이스북 로그인 후 Backend 페더레이션 인증(회원가입 및 로그인)
//    {
//        if (!Utilities.IsConnectedInternet)
//        {
//            callback(BackendCallbackState.NotConnectInternet);
//            return;
//        }

//        CheckServer((bool success) =>
//        {
//            if (success)
//            {
//                Callback = callback;

//                FB.LogInWithReadPermissions(new string[] { "email", "public_profile" }, OnFacebookSignIn); //"user_birthday", "user_friends"
//            }
//            else
//            {
//                callback(BackendCallbackState.ClosedServer);
//            }
//        });
//    }

//    void OnFacebookSignIn(ILoginResult result) //페이스북 로그인
//    {
//        if (FB.IsLoggedIn) //성공 - 서버에 정보 전달.
//        {
//            DebugLog("페이스북 로그인 성공");
//            Backend.BMember.Logout((bro) =>
//            {
//                if (bro.IsSuccess())
//                    FederationSignIn(SignInCallType.Manual, FederationType.Facebook, AccessToken.CurrentAccessToken.TokenString);
//            });
//        }
//        else //fail
//        {
//            DebugLog("페이스북 로그인 실패or취소");
//            Callback(result.Cancelled ? BackendCallbackState.UserCancel : BackendCallbackState.Fail);
//        }
//    }

//    //유저 기본 정보 설정. (닉네임, 국가)
//    void SetUserDefaultInformation() //    {
//        if (!Utilities.IsConnectedInternet)
//            return;  //        //set nickname. //        DebugLog("닉네임 확인 시작"); //        BackendReturnObject bro0 = Backend.BMember.GetUserInfo(); //        if (bro0.IsSuccess()) //        { //            JsonData Userdata = bro0.GetReturnValuetoJSON()["row"]; //            JsonData nicknameJson = Userdata["nickname"];  //            // 닉네임 여부를 확인해서 없는 경우에만 진행. //            if (nicknameJson == null) //            { //                DebugLog("닉네임 없음. 닉네임 생성 시작."); //                bool isSetNickname = false; //                while (!isSetNickname) //                {
//                    string nickname = Utilities.RandomNickname();  //                    BackendReturnObject bro1 = Backend.BMember.CheckNicknameDuplication(nickname); //                    if (bro1.IsSuccess())
//                    {
//                        int statusCode = Convert.ToInt32(bro1.GetStatusCode());
//                        if (statusCode == 204)
//                        {
//                            BackendReturnObject bro2 = Backend.BMember.CreateNickname(nickname);
//                            if (bro2.IsSuccess())
//                            {
//                                isSetNickname = true;
//                                DebugLog("CreateNickname : " + bro2.IsSuccess());
//                            }
//                        }
//                    } //                } //            } //         }  //        //set country. //        CountryCode country = CountryCode.SouthKorea; //        SystemLanguage language = Application.systemLanguage; //        switch (language) //        { //            case SystemLanguage.Korean: //                country = CountryCode.SouthKorea;
//                break;
//            default: //                country = CountryCode.UnitedStates; //                break; //        }  //        Backend.BMember.UpdateCountryCode(country, bro =>
//        {
//            // 이후 처리
//            DebugLog("UpdateCountryCode : " + bro.IsSuccess());
//        }); //    }

//    //내 이름.
//    public string GetNickName()
//    {
//        if (!Utilities.IsConnectedInternet || !IsBackendSocialLoggedIn)
//            return "";

//        string nickname = "";
//        BackendReturnObject userinfo = Backend.BMember.GetUserInfo();

//        if (userinfo.IsSuccess())
//        {
//            JsonData Userdata = userinfo.GetReturnValuetoJSON()["row"];
//            JsonData nicknameJson = Userdata["nickname"];

//            // 닉네임 여부를 확인
//            if (nicknameJson != null)
//            {
//                nickname = nicknameJson.ToString();
//            }
//        }
//        DebugLog(nickname);
//        return nickname;
//    }

//    //이름 변경.
//    public void ChangeNickName(string val, Action<BackendCallbackState> callback)
//    {
//        if (!Utilities.IsConnectedInternet || !IsBackendSocialLoggedIn)
//            return;

//        BackendReturnObject obj = Backend.BMember.UpdateNickname(val);

//        switch (obj.GetStatusCode())
//        {
//            case "400": callback(BackendCallbackState.Fail); break;
//            case "409": callback(BackendCallbackState.DuplicateNickname); break;
//            default: callback(BackendCallbackState.Success); break;
//        }
//    }






//    //------------------------------SAVE and LOAD----------------------------//
//    //테이블 정보 가져오기.
//    void GetDataTableInfo(string key, Action<DataTableInfo> callback)
//    {
//        for (int i = 0; i < TableKeys.Length; i++)
//        {
//            if (string.Equals(TableKeys[i], key))
//            {
//                DebugLog(TableKeys[i] + "키값으로 테이블을 검색합니다.");
//                Backend.GameInfo.GetPrivateContents(TableKeys[i], (data_callback) =>
//                {
//                    // 이후 처리.
//                    if (data_callback.IsSuccess())
//                    {
//                        DebugLog(TableKeys[i] + "테이블 검색됨.");
//                        JsonData json = data_callback.GetReturnValuetoJSON()["rows"];
//                        DataTableInfo info = null;
//                        foreach (JsonData _data in json)
//                        { //rows에 값이 없으면 루프가 돌지 않으므로 괜찮음.
//                            DebugLog("값이 검색되었습니다. set indate, " + TableKeys[i] + " : " + _data["inDate"]["S"].ToString());
//                            info = new DataTableInfo(TableKeys[i], _data["inDate"]["S"].ToString(), _data["val"]["S"].ToString());
//                        }

//                        callback(info);
//                    }
//                    else
//                    {
//                        DebugLog(TableKeys[i] + "테이블 검색되지 않음. - error code = " + data_callback.GetErrorCode());
//                        callback(null);
//                    }
//                });

//                break;
//            }
//        }
//    }

//    //데이터 저장하기.
//    public void SaveData(string tableKey, string data, Action<BackendCallbackState> callback = null)
//    {
//        if (!Utilities.IsConnectedInternet)
//        {
//            callback?.Invoke(BackendCallbackState.NotConnectInternet);
//            return;
//        }

//        if (!IsBackendSocialLoggedIn)
//        {
//            callback?.Invoke(BackendCallbackState.NotSignIn);
//            return;
//        }

//        StartCoroutine(SaveDataSequence(tableKey, data, callback));
//    }

//    IEnumerator SaveDataSequence(string tableKey, string data, Action<BackendCallbackState> callback)
//    {
//        //테이블 정보 검색.
//        DataTableInfo dataInfo = null;
//        bool NextBlock = true;

//        GetDataTableInfo(tableKey, (DataTableInfo info) =>
//        {
//            dataInfo = info;
//            NextBlock = false;
//        });

//        while (NextBlock)
//            yield return null;

//        NextBlock = true;
//        BackendReturnObject bro = new BackendReturnObject();
//        Param param = new Param();
//        param.Add("val", data);

//        if (dataInfo != null) //검색된 indate가 존재하는 경우 = Udate
//        {
//            Backend.GameInfo.Update(tableKey, dataInfo.indate, param, _bro =>
//            {
//                bro = _bro;
//                NextBlock = false;
//            });
//        }
//        else //검색된 indate가 존재하지 않는 경우 = Insert
//        {
//            Backend.GameInfo.Insert(tableKey, param, _bro =>
//            {
//                bro = _bro;
//                NextBlock = false;
//            });
//        }

//        while (NextBlock)
//            yield return null;

//        callback?.Invoke(bro.IsSuccess() ? BackendCallbackState.Success : BackendCallbackState.Fail);
//    }

//    //데이터 불러오기.
//    public void LoadData(string tableKey, Action<BackendCallbackState, DataTableInfo> callback)
//    {
//        if (!Utilities.IsConnectedInternet)
//        {
//            callback(BackendCallbackState.NotConnectInternet, null);
//            return;
//        }

//        if (!IsBackendSocialLoggedIn)
//        {
//            callback(BackendCallbackState.NotSignIn, null);
//            return;
//        }

//        StartCoroutine(LoadDataSequence(tableKey, callback));
//    }

//    IEnumerator LoadDataSequence(string tableKey, Action<BackendCallbackState, DataTableInfo> callback)
//    {
//        //데이터 교체.
//        DataTableInfo dataInfo = null;
//        bool NextBlock = true;

//        GetDataTableInfo(tableKey, (info) =>
//        {
//            dataInfo = info;
//            NextBlock = false;
//        });

//        while (NextBlock)
//            yield return null;

//        callback(BackendCallbackState.Success, dataInfo);
//    }

//    //데이터 모두 불러오기.
//    public void LoadAllData(Action<BackendCallbackState, DataTableInfo[]> callback)
//    {
//        if (!Utilities.IsConnectedInternet)
//        {
//            callback(BackendCallbackState.NotConnectInternet, null);
//            return;
//        }

//        if (!IsBackendSocialLoggedIn)
//        {
//            callback(BackendCallbackState.NotSignIn, null);
//            return;
//        }

//        StartCoroutine(LoadAllDataSequence(callback));
//    }

//    IEnumerator LoadAllDataSequence(Action<BackendCallbackState, DataTableInfo[]> callback)
//    {
//        //데이터 교체.
//        List<DataTableInfo> infoList = new List<DataTableInfo>();
//        for (int i = 0; i < TableKeys.Length; i++)
//        {
//            bool NextBlock = true;

//            GetDataTableInfo(TableKeys[i], (info)=>
//            {
//                if (info != null)
//                    infoList.Add(info);

//                NextBlock = false;
//            });

//            while (NextBlock)
//                yield return null;
//        }

//        callback(BackendCallbackState.Success, infoList.ToArray());
//    }


//    //FCM 토큰 저장
//    public void SetPushToken(bool activation)
//    {
//        DebugLog("SetPushToken");
//        if (!Utilities.IsConnectedInternet || !IsBackendSocialLoggedIn)
//            return;

//        //Backend의 GCM system 구현.
//#if UNITY_ANDROID && !UNITY_EDITOR
//        if (activation)
//            Backend.Android.PutDeviceToken();
//        else
//            Backend.Android.DeleteDeviceToken();
//#elif UNITY_IOS && !UNITY_EDITOR
//        DebugLog("activation : " + activation);
//        if (activation)
//            Backend.iOS.PutDeviceToken(isDevelopment.iosDev); //isDevelopment.iosProd
//        else
//            Backend.iOS.DeleteDeviceToken();
//#endif
//    }




//    //mail.
//    public Action MailDownloadCompleteCallback;
//    public List<MailInformation> MailList { get; } = new List<MailInformation>();
//    public MailInformation GetTargetMail(string indate)
//    {
//        IEnumerable<MailInformation> target = from mail in MailList where mail.indate == indate select mail;
//        return target.ToList().Count == 1 ? target.ToList()[0] : null;
//    }

//    public void DownloadMails() //메일 가져오기.
//    {
//        if (!Utilities.IsConnectedInternet || !IsBackendSocialLoggedIn)
//        {
//            MailDownloadCompleteCallback?.Invoke();
//            return;
//        }

//        MailList.Clear();

//        Backend.Social.Post.GetPostListV2((_bro) =>
//        {
//            // 이후 처리
//            if (_bro.IsSuccess())
//            {
//                JsonData adminMailData = _bro.GetReturnValuetoJSON()["fromAdmin"];

//                if (adminMailData.Count > 0)
//                {
//                    foreach (JsonData _data in adminMailData) {
//                        MailList.Add(new MailInformation(_data));
//                    }
//                }
//            }

//            MailDownloadCompleteCallback?.Invoke();
//        });
//    }

//    public void ReceiveMail(string mail_indate, Action<bool> callback) //메일 수령하기.
//    {
//        Backend.Social.Post.ReceiveAdminPostItemV2(mail_indate, (_bro) => {
//            callback?.Invoke(_bro.IsSuccess());
//        });
//    }

//    public void ReceiveAllMail(Action<bool> callback) //메일 전체 수령하기.
//    {
//        Backend.Social.Post.ReceiveAdminPostAllV2((_bro) => {
//            callback?.Invoke(_bro.IsSuccess());
//        });
//    }

//    public void RemoveMailList(MailInformation info)
//    {
//        if(MailList.Contains(info))
//            MailList.Remove(info);
//    }

//    public void RemoveAllMail()
//    {
//        MailList.Clear();
//    }
//}

//public class DataTableInfo
//{
//    public string tableName;
//    public string indate;
//    public string Data;

//    public DataTableInfo(string _name, string _indate, string _data)
//    {
//        tableName = _name;
//        indate = _indate;
//        Data = _data;
//    }
//}

//public class MailInformation //관리자 메일만. 보상지급 등의 용도로 사용됨.
//{
//    //data
//    public string indate; //메일의 indate값. (식별 키)
//    //public RewardType type; //보상 정보.
//    public int rewardCount; //보상 갯수.

//    public DateTime expireDate; //메일 만료 날짜.

//    //string data.
//    public string Title; //메일 제목.
//    public string Description; //메일 내용.

//    public MailInformation() { }

//    public MailInformation(JsonData mail)
//    {
//        indate = mail["inDate"]["S"].ToString();
//        //type = BackendManager.CodeToRewardType(mail["item"]["M"]["code"]["S"].ToString());
//        rewardCount = Convert.ToInt32(mail["itemCount"]["N"].ToString());

//        expireDate = DateTime.Parse(mail["expirationDate"]["S"].ToString());

//        Title = mail["title"]["S"].ToString();
//        Description = mail["content"]["S"].ToString();
//    }
//}

//public enum BackendCallbackState
//{
//    Success,
//    Fail,
//    UserCancel,
//    NotConnectInternet,
//    NotSignIn,
//    DuplicateNickname,
//    ClosedServer,
//}

////로그인 방식.
//public enum SignInCallType
//{
//    Auto,
//    Manual
//}