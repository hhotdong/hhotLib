//using System;
//using UnityEngine;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using UnityEngine.SocialPlatforms;

//public class GPGSManager : MonoBehaviour
//{
//    private static GPGSManager _instance;
//    public static GPGSManager Instance
//    {
//        get
//        {
//            if (_instance == null)
//                _instance = FindObjectOfType(typeof(GPGSManager)) as GPGSManager;
//            return _instance;
//        }
//    }

//    public bool IsLoggedIn { get; private set; }
//    public bool IsAutoLogin { get { return PlayerPrefs.GetInt("GPGSAutoLogin", 0) > 0; } set { PlayerPrefs.SetInt("GPGSAutoLogin", Convert.ToInt32(value)); } }

//    bool DoAuthenticate;
//    Action<bool> LoginCallback;

//    void Start()
//    {
//#if UNITY_IOS
//        Destroy(gameObject);
//#endif
//    }

//    public void LoginGPGS(Action<bool> completeCallback = null)
//    {
//#if UNITY_ANDROID
//        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
//            .Builder()
//            .RequestServerAuthCode(false)
//            .RequestIdToken()
//            .Build();
//        //커스텀된 정보로 GPGS 초기화
//        PlayGamesPlatform.InitializeInstance(config);
//        PlayGamesPlatform.DebugLogEnabled = true;
//        //GPGS 시작.
//        PlayGamesPlatform.Activate();

//        DoAuthenticate = true;
//        Social.localUser.Authenticate((bool success) =>
//        {
//            IsLoggedIn = success;
//            DoAuthenticate = false;

//            if (LoginCallback != null)
//            {  //Login 함수에서 callback 대기에 걸린 경우.
//                IsAutoLogin = success;
//                LoginCallback(success);
//                LoginCallback = null;
//            }

//            completeCallback?.Invoke(success);
//        });
//#endif
//    }

//    public void Login(Action<bool> callback)
//    {
//#if UNITY_ANDROID
//        if (IsLoggedIn)
//        {
//            IsAutoLogin = true;
//            callback(true);
//        }
//        else
//        {
//            if (DoAuthenticate) //아직 Authenticate 끝나지 않았는데 Login이 수행 된 경우.
//            {
//                LoginCallback = callback; //callback 대기 후 종료.
//            }
//            else
//            {
//                try
//                {
//                    Social.localUser.Authenticate((bool success) => {
//                        IsLoggedIn = success;
//                        IsAutoLogin = success;
//                        callback(success);
//                    });
//                }
//                catch(Exception e)
//                {
//                    Debug.Log("GPGS login error " + e.Message);
//                    IsLoggedIn = false;
//                    IsAutoLogin = false;
//                    callback(false);
//                }
//            }
//        }
//#endif
//    }

//    public void Logout()
//    {
//#if UNITY_ANDROID
//        //((PlayGamesPlatform)Social.Active).SignOut(); //로그아웃
//        IsLoggedIn = false;
//        IsAutoLogin = false;
//#endif
//    }

//    // 구글 토큰 받아옴
//    public string Token
//    {
//        get
//        {
//#if UNITY_ANDROID
//            if (PlayGamesPlatform.Instance.localUser.authenticated)
//                //if (Social.localUser.authenticated)
//            {
//                // 유저 토큰 받기 첫번째 방법
//                string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
//                // 두번째 방법
//                //string _IDtoken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
//                return _IDtoken;
//            }
//            else
//            {
//                Debug.Log("접속되어있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
//                return "";
//            }
//#else
//            return "";
//#endif
//        }
//    }
//}
