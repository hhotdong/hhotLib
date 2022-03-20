//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using Nanali;

//public class UtilityTest : MonoBehaviour
//{
//    NotificationManager notiManager;
//    TextureHandlingManager imgManager;

//    //ui - notification.
//    public InputField notiInputField;
//    public GameObject Go_NotiOn;
//    public GameObject Go_NotiOff;

//    //ui - share image.
//    public InputField shareMessage;
//    public Texture2D WaterMark;
//    public GameObject Go_WatermarkOn;
//    public GameObject Go_WatermarkOff;
//    public GameObject Go_FullscreenOn;
//    public GameObject Go_FullscreenOff;


    

//    bool IsFullSize;
//    bool IsWaterMark;
//    bool IsNotification;

//    void Start()
//    {
//        notiManager = NotificationManager.Instance;
//        imgManager = TextureHandlingManager.Instance;

//        SetNotification(true);
//        SetFullScreen(true);
//        SetWaterMark(false);
//        notiInputField.text = "5";
//        shareMessage.text = "안녕하세요.";
//    }

//    //앱이 활성화되면 모든 로컬알림 삭제, 비활성화시 로컬알림 등록.
//    void OnApplicationPause(bool pauseStatus)
//    {
//        if (notiManager == null)
//            return;

//        if (!IsNotification)
//            return;

//        if (!pauseStatus)
//        {
//            notiManager.CancelAllNotifications();
//        }
//        else
//        {
//            //set notifications.
//            int seconds;
//            int.TryParse(notiInputField.text, out seconds);

//            notiManager.SetNotification("Forest Island", "Local notification test", BackendManager.DateTimeNow, seconds);
//        }
//    }

//    public void SetWaterMark(bool state)
//    {
//        IsWaterMark = state;
//        Go_WatermarkOn.SetActive(IsWaterMark);
//        Go_WatermarkOff.SetActive(!IsWaterMark);
//    }

//    public void SetFullScreen(bool state)
//    {
//        IsFullSize = state;
//        Go_FullscreenOn.SetActive(IsFullSize);
//        Go_FullscreenOff.SetActive(!IsFullSize);
//    }

//    public void SetNotification(bool state)
//    {
//        IsNotification = state;
//        Go_NotiOn.SetActive(IsNotification);
//        Go_NotiOff.SetActive(!IsNotification);
//    }

//    //이미지 공유.
//    public void ShareImage()
//    {
//        int height = IsFullSize ? Screen.height : Screen.height / 2;

//        Texture2D screenshot = Utilities.GetScreenShot(Camera.main, 1 << LayerMask.NameToLayer("UI"), Screen.width, height, IsWaterMark ? WaterMark : null);

//        imgManager.ShareImage(screenshot, shareMessage.text);
//    }

//    //이미지 저장.
//    public void SaveImage()
//    {
//        int height = IsFullSize ? Screen.height : Screen.height / 2;

//        Texture2D screenshot = Utilities.GetScreenShot(Camera.main, 1 << LayerMask.NameToLayer("UI"), Screen.width, height, IsWaterMark ? WaterMark : null);

//        imgManager.SaveGallary(screenshot, gameObject.name, "SaveImageCallback");
//    }

//    //이미지 저장 후 콜백.
//    public void SaveImageCallback(string callbackType)
//    {
//        if (callbackType.Equals("success"))
//        {
//            //저장 성공 연출.
//        }
//    }
//}
