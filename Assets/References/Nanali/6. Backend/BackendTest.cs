//using System.Collections;
//using System.Collections.Generic;

//using UnityEngine;
//using UnityEngine.UI;

//public class BackendTest : MonoBehaviour
//{
//    public GameObject Go_LoadingMsg;
//    public GameObject Go_Items;

//    public GameObject Go_LoginBtns;
//    public GameObject Go_FBLoginBtn;
//    public GameObject Go_GPGSLoginBtn;
//    public GameObject Go_SIWABtn;

//    public GameObject Go_ContentBtns;
//    public InputField InputDataField;
//    public Text DebugText;

    

//    BackendManager manager;

//    private void DebugLog(object msg)
//    {
//        Debug.Log("BackendManager : " + msg);
//        DebugText.text = msg.ToString();
//    }


//    //initialize.
//    private void Start()
//    {
//        manager = BackendManager.Instance;

//        SetDefaultUI();
//        manager.Initialize(OnInitializeCallback);
//    }

//    void SetDefaultUI()
//    {
//        //플랫폼별 버튼 노출 세팅.
//        Go_GPGSLoginBtn.SetActive(Application.platform == RuntimePlatform.Android);
//        Go_SIWABtn.SetActive(Application.platform == RuntimePlatform.IPhonePlayer);

//        //로그인버튼 노출세팅.
//        Go_LoginBtns.SetActive(true);
//        Go_ContentBtns.SetActive(false);

//        //Init 메세지 노출 세팅.
//        Go_LoadingMsg.SetActive(true);
//        Go_Items.SetActive(false);
//    }

    
//    //functions.
//    public void SignInFacebook() //페이스북 로그인.
//    {
//        Go_LoadingMsg.SetActive(true);
//        Go_LoginBtns.SetActive(false);
//        Go_ContentBtns.SetActive(false);

//        manager.FacebookSignIn(OnLoginCallback);
//    }

//    public void SignInApple() //애플 로그인.
//    {
//        Go_LoadingMsg.SetActive(true);
//        Go_LoginBtns.SetActive(false);
//        Go_ContentBtns.SetActive(false);

//        manager.AppleSignIn(OnLoginCallback);
//    }

//    public void SignInGoogle() //구글 로그인.
//    {
//        Go_LoadingMsg.SetActive(true);
//        Go_LoginBtns.SetActive(false);
//        Go_ContentBtns.SetActive(false);

//        manager.GPGSSignIn(OnLoginCallback);
//    }

//    public void SignOut() //로그아웃.
//    {
//        Go_LoadingMsg.SetActive(true);
//        Go_LoginBtns.SetActive(false);
//        Go_ContentBtns.SetActive(false);

//        manager.SignOut(OnLogoutCallback);
//    }

//    public void SaveData()
//    {
//        //manager.SaveData("TEST", dataField.text);
//        Go_LoadingMsg.SetActive(true);
//        Go_ContentBtns.SetActive(false);

//        manager.SaveData("TEST", InputDataField.text, OnSaveDataCallback);
//    }

//    public void LoadData()
//    {
//        Go_LoadingMsg.SetActive(true);
//        Go_ContentBtns.SetActive(false);

//        manager.LoadData("TEST", (state, info) =>
//        {
//            Go_LoadingMsg.SetActive(false);
//            Go_ContentBtns.SetActive(true);

//            switch (state)
//            {
//                case BackendCallbackState.Success:
//                    DebugLog("테이블 이름 - " + info.tableName + "\n인데이트(고유 키) - " + info.indate + "\n데이터 - " + info.Data);
//                    break;
//                case BackendCallbackState.NotConnectInternet:
//                    DebugLog("인터넷 연결이 되어있지 않습니다.");
//                    break;
//                case BackendCallbackState.NotSignIn:
//                    DebugLog("로그인이 되어있지 않습니다.");
//                    break;
//                case BackendCallbackState.Fail:
//                    DebugLog("값이 검색되지 않았습니다.");
//                    break;
//            }
//        });
//    }

//    public void LoadAllData()
//    {
//        Go_LoadingMsg.SetActive(true);
//        Go_ContentBtns.SetActive(false);

//        manager.LoadAllData((state, info) =>
//        {
//            Go_LoadingMsg.SetActive(false);
//            Go_ContentBtns.SetActive(true);

//            switch (state)
//            {
//                case BackendCallbackState.Success:
//                    string data = "";
//                    for (int i = 0; i < info.Length; i++)
//                    {
//                        data += "테이블 이름 - " + info[i].tableName + "\n인데이트(고유 키) - " + info[i].indate + "\n데이터 - " + info[i].Data + "\n";
//                    }
//                    DebugLog(data);
//                    break;
//                case BackendCallbackState.NotConnectInternet:
//                    DebugLog("인터넷 연결이 되어있지 않습니다.");
//                    break;
//                case BackendCallbackState.NotSignIn:
//                    DebugLog("로그인이 되어있지 않습니다.");
//                    break;
//            }
//        });
//    }





//    //callbacks.
//    void OnInitializeCallback(BackendCallbackState state)
//    {
//        Go_LoadingMsg.SetActive(false);
//        switch (state)
//        {
//            case BackendCallbackState.Success:
//                Go_Items.SetActive(true);
//                Go_LoginBtns.SetActive(!BackendManager.IsBackendSocialLoggedIn);
//                Go_ContentBtns.SetActive(BackendManager.IsBackendSocialLoggedIn);
//                break;
//        }

//        OnBackendCallbackDebugFunction("OnInitializeCallback", state);
//    }

//    void OnLoginCallback(BackendCallbackState state)
//    {
//        Go_LoadingMsg.SetActive(false);
//        Go_LoginBtns.SetActive(state != BackendCallbackState.Success);
//        Go_ContentBtns.SetActive(state == BackendCallbackState.Success);

//        OnBackendCallbackDebugFunction("OnLoginCallback", state);
//    }

//    void OnLogoutCallback(BackendCallbackState state)
//    {
//        Go_LoadingMsg.SetActive(false);
//        Go_ContentBtns.SetActive(state != BackendCallbackState.Success);
//        Go_LoginBtns.SetActive(state == BackendCallbackState.Success);

//        OnBackendCallbackDebugFunction("OnLogoutCallback", state);
//    }

//    void OnSaveDataCallback(BackendCallbackState state)
//    {
//        Go_LoadingMsg.SetActive(false);
//        Go_ContentBtns.SetActive(true);

//        OnBackendCallbackDebugFunction("OnSaveDataCallback", state);
//    }

//    void OnBackendCallbackDebugFunction(string tag, BackendCallbackState state)
//    {
//        switch (state)
//        {
//            case BackendCallbackState.Success:
//                DebugLog(tag + " - 성공");
//                break;
//            case BackendCallbackState.UserCancel:
//                DebugLog(tag + " - 사용자가 직접 취소");
//                break;
//            case BackendCallbackState.NotConnectInternet:
//                DebugLog(tag + " - 인터넷 연결이 되어있지 않습니다.");
//                break;
//            case BackendCallbackState.NotSignIn:
//                DebugLog(tag + " - 로그인이 되어있지 않습니다.");
//                break;
//            case BackendCallbackState.ClosedServer:
//                DebugLog(tag + " - 닫힌 서버.");
//                break;
//            case BackendCallbackState.DuplicateNickname:
//                DebugLog(tag + " - 중복된 이름.");
//                break;
//            case BackendCallbackState.Fail:
//                DebugLog(tag + " - 실패.");
//                break;
//        }
//    }
//}
