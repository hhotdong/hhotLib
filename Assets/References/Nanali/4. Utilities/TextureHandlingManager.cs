using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class TextureHandlingManager : MonoBehaviour
{
	private static TextureHandlingManager _instance;
	public static TextureHandlingManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType(typeof(TextureHandlingManager)) as TextureHandlingManager;
			return _instance;
		}
	}

#if UNITY_ANDROID
	private static AndroidJavaClass cls = null;
#elif UNITY_IOS
    [DllImport("__Internal")]
	private static extern void ShareImage(string path, string message);
	[DllImport("__Internal")]
	public static extern void RefreshGallary(string fileName);
#endif

	//권한 확인.
	int _SavePermissionFlag
	{ //권한을 요청 한 적이 있음.
		get
		{
			return PlayerPrefs.GetInt("_SavePermissionFlag", 0);
		}
		set
		{
			PlayerPrefs.SetInt("_SavePermissionFlag", value);
		}
	}

	//이미지 공유. default메세지 입력가능.
	public void ShareImage(Texture2D _texture, string message = "")
	{
		//save.
		byte[] bytes = _texture.EncodeToPNG();//gc가 많이발생함. 해결법은없음. 무조건이거써야 Native로 전달가능.
		string path = Path.Combine(Application.temporaryCachePath, "share_image.png");
		File.WriteAllBytes(path, bytes);

#if UNITY_IOS && !UNITY_EDITOR
		ShareImage(path,message);
#elif UNITY_ANDROID && !UNITY_EDITOR

		AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
		cls = new AndroidJavaClass("com.nanali.androidtool.MediaManager");
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
	    {
		    using (AndroidJavaObject obj = cls.CallStatic<AndroidJavaObject>("getInstance"))
	 	    {
			    obj.Call("ShareImage", path, message);
		    }
	    }));
#endif
	}

	public static void ShareVideo(string video_Path, string share_message) //비디오 공유하기
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
		cls = new AndroidJavaClass("com.nanali.androidtool.MediaManager");
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
		{
			using (AndroidJavaObject obj = cls.CallStatic<AndroidJavaObject>("getInstance"))
			{
				obj.Call("ShareVideo", activity, video_Path, share_message);
			}
		}));
#elif UNITY_IOS && !UNITY_EDITOR
#endif
	}








	//이미지 저장.
	public class SavedImageInfo
	{
		public Texture2D texture;
		public string callbackObjName;
		public string callbackMethod;

		public SavedImageInfo(Texture2D _tex, string _obj, string _method)
		{
			texture = _tex;
			callbackObjName = _obj;
			callbackMethod = _method;
		}
	}
	SavedImageInfo savedImageInfo;

	public void SaveGallary(Texture2D _tex, string callbackObj, string callbackMethod)
	{
		savedImageInfo = new SavedImageInfo(_tex, callbackObj, callbackMethod);

#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
		cls = new AndroidJavaClass("com.nanali.androidtool.MediaManager");
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
	    {
		    using (AndroidJavaObject obj = cls.CallStatic<AndroidJavaObject>("getInstance"))
	 	    {
			    obj.Call("CheckPermission", gameObject.name, "CheckPermissionCallback");
		    }
	    }));
#elif UNITY_IOS && !UNITY_EDITOR
		CheckPermissionCallback("1");
#endif
	}

	public void CheckPermissionCallback(string msg)
	{
		if (msg == "1") {
			SaveGallary();
		}
		else {
			GetPermission();
		}
	}

	void SaveGallary()
	{
		if (savedImageInfo == null)
			return;

		try
		{
			byte[] bytes = savedImageInfo.texture.EncodeToPNG();//gc가 많이발생함. 해결법은없음. 무조건이거써야 Native로 전달가능.
			string directoryPath = Application.persistentDataPath;
			//string directoryPath = "";
			string fileReName = string.Format("/{0}.png", DateTime.Now.ToString("yyyyMMddHHmmss"));

#if UNITY_ANDROID && !UNITY_EDITOR
				directoryPath += "/../../../../DCIM/ForestIsland";

				DirectoryInfo dInfo = new DirectoryInfo(directoryPath);
				if (!dInfo.Exists)
					Directory.CreateDirectory(directoryPath);
#endif

			if (!string.IsNullOrEmpty(directoryPath))
				File.WriteAllBytes(directoryPath + fileReName, bytes);

			RefreshPhotoGallary(directoryPath + fileReName, savedImageInfo.callbackObjName, savedImageInfo.callbackMethod);
		}
		catch (UnauthorizedAccessException)
		{
			try
			{
				GameObject.Find(savedImageInfo.callbackObjName).SendMessage(savedImageInfo.callbackMethod, "fail");
			}
			catch { }
		}

		savedImageInfo = null;
	}

	

	void GetPermission()
	{
#if UNITY_ANDROID //&& !UNITY_EDITOR
		AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
		cls = new AndroidJavaClass("com.nanali.androidtool.MediaManager");
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
			using (AndroidJavaObject obj = cls.CallStatic<AndroidJavaObject>("getInstance"))
            {
				obj.Call("GetPermission", gameObject.name, "GetPermissionCallback", _SavePermissionFlag);
			}
		}));
#elif UNITY_IOS && !UNITY_EDITOR
		//go option.
#endif
	}

	//0 : 사용자가 요청을 거부함 (다시 보지 않기 상태), 1 : 사용자가 요청을 허가함, 2 : 이미 획득된 권한
	void GetPermissionCallback(string callbackMsg)
	{
		int state;
		int.TryParse(callbackMsg, out state);
		_SavePermissionFlag = 1;
		switch (state)
		{
			case 0: //사용자가 요청을 거부함 (다시 보지 않기 상태)

				break;
			case 1: //거부.

				break;
			case 2: //허락.
				SaveGallary();
				break;
		}
	}

	//앨범 갱신. 직접 호출하지 않음.
	void RefreshPhotoGallary(string imagePath, string callbackObj, string callbackMethod)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
		cls = new AndroidJavaClass("com.nanali.androidtool.MediaManager");
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
	    {
		    using (AndroidJavaObject obj = cls.CallStatic<AndroidJavaObject>("getInstance"))
	 	    {
			    obj.Call("RefreshPhotoGallary", activity, imagePath, callbackObj, callbackMethod);
		    }
	    }));
#elif UNITY_IOS && !UNITY_EDITOR
		RefreshGallary(string.Format("/{0}",imagePath));
#endif
	}
}
