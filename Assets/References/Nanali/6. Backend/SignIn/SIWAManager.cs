//using System;
//using System.Collections;
//using System.Text;
//using System.Security.Cryptography;
//using UnityEngine;
//using AppleAuth;
//using AppleAuth.Interfaces;
//using AppleAuth.Enums;
//using AppleAuth.Native;

//public class SIWAManager : MonoBehaviour
//{
//    private static SIWAManager _instance;
//    public static SIWAManager Instance
//    {
//        get
//        {
//            if (_instance == null)
//                _instance = FindObjectOfType(typeof(SIWAManager)) as SIWAManager;
//            return _instance;
//        }
//    }

//    private IAppleAuthManager appleAuthManager;

//    public string Token { get { return PlayerPrefs.GetString("SIWATOKEN", ""); } set { PlayerPrefs.SetString("SIWATOKEN", value); } }
//    public string ID { get { return PlayerPrefs.GetString("SIWAUSERID", ""); } set { PlayerPrefs.SetString("SIWAUSERID", value); } }
//    public string RawNonce { get; private set; }
//    public string Nonce { get; private set; }
//    public bool IsLoggedIn { get; private set; }
//    public bool IsAutoLoggedIn { get; private set; }

//    bool DoCheckCredentialState;
//    Action<bool> CheckCreditialStateDoneConfirmCallback;

//    void Start()
//    {
//#if !UNITY_EDITOR
//        if (!AppleAuthManager.IsCurrentPlatformSupported)
//            Destroy(gameObject);
//        else
//        {
//            var deserializer = new PayloadDeserializer();
//            appleAuthManager = new AppleAuthManager(deserializer);
//        }
//#endif

//        CheckCredentialState(); //앱 접근 권한 상태 체크.
//    }

//    void Update()
//    {
//        appleAuthManager?.Update();
//    }

//    void CheckCredentialState()
//    {
//        if (appleAuthManager == null)
//            return;

//        if (string.IsNullOrEmpty(ID)) //저장된 ID가 없으면 자격증명을 가져오지 않음.
//            return;

//        DoCheckCredentialState = true;
//        // If there is an apple ID available, we should check the credential state
//        appleAuthManager.GetCredentialState(ID, state =>
//        {
//            switch (state)
//            {
//                // If it's authorized, login with that user id
//                case CredentialState.Authorized:
//                    IsAutoLoggedIn = true;
//                    IsLoggedIn = true;
//                    break;

//                // If it was revoked, or not found, we need a new sign in with apple attempt
//                // Discard previous apple user id
//                case CredentialState.Revoked:
//                case CredentialState.NotFound:
//                    IsAutoLoggedIn = false;
//                    IsLoggedIn = false;
//                    Logout();
//                    break;
//            }

//            DoCheckCredentialState = false;

//            if (CheckCreditialStateDoneConfirmCallback != null) { //자격증명 검사 끝났는지 체크가 자격증명 검사중에 발동했었으면 리턴.
//                CheckCreditialStateDoneConfirmCallback(IsAutoLoggedIn);
//                CheckCreditialStateDoneConfirmCallback = null;
//            }
//        },
//        error =>
//        {
//            Debug.LogWarning("Error while trying to get credential state " + error.ToString());
//            IsAutoLoggedIn = false;
//            IsLoggedIn = false;
//            DoCheckCredentialState = false;
//        });
//    }

//    public void CheckCreditialStateDoneConfirm(Action<bool> callback) //자격증명 검사가 끝났는지 체크하는 함수. 자격증명에 따른 오토로그인 여부를 리턴.
//    {
//        if (appleAuthManager == null)
//        {
//            callback(false);
//            return;
//        }

//        if (DoCheckCredentialState)
//            CheckCreditialStateDoneConfirmCallback = callback;
//        else
//            callback(IsAutoLoggedIn);
//    }

//    public void Login(Action<bool> callback)
//    {
//        if (appleAuthManager == null)
//            return;

//        SetNonce();

//        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail, Nonce);

//        appleAuthManager.LoginWithAppleId(
//            loginArgs,
//            credential => {
//                try
//                {
//                    var appleIdCredential = credential as IAppleIDCredential;
//                    Token = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
//                    ID = credential.User;

//                    IsLoggedIn = true;
                    
//                    if (callback != null)
//                        callback(true);
//                }
//                catch (Exception e)
//                {
//                    Debug.LogException(e);
//                    IsLoggedIn = false;

//                    if (callback != null)
//                        callback(false);
//                }
//            },
//            error => {
//                IsLoggedIn = false;

//                if (callback != null)
//                    callback(false);
//            });
//    }

//    public void Logout()
//    {
//        IsLoggedIn = false;
//        Token = "";
//        RawNonce = "";
//        Nonce = "";
//        ID = "";
//    }

//    void SetNonce()
//    {
//        // Nonce 초기화
//        // Nonce는 Apple로그인 시 접속 세션마다 새로 생성
//        RawNonce = Guid.NewGuid().ToString();
//        Nonce = GenerateNonce(RawNonce);
//    }

//    // Nonce는 SHA256으로 만들어서 전달해야함
//    private static string GenerateNonce(string _rawNonce)
//    {
//        SHA256 sha = new SHA256Managed();
//        var sb = new StringBuilder();
//        // Encoding은 반드시 ASCII여야 함
//        byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(_rawNonce));
//        // ToString에서 "x2"로 소문자 변환해야 함. 대문자면 실패함. ㅠㅠ
//        foreach (var b in hash) sb.Append(b.ToString("x2"));
//        return sb.ToString();
//    }
//}
