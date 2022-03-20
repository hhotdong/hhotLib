using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class BackgroundPlayAudioManager : MonoBehaviour
{
	private static BackgroundPlayAudioManager _instance;
	public static BackgroundPlayAudioManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType(typeof(BackgroundPlayAudioManager)) as BackgroundPlayAudioManager;
			return _instance;
		}
	}

#if UNITY_ANDROID
	private static AndroidJavaClass cls = null;
#elif UNITY_IOS

#endif

	private IEnumerator GetResourcePath(string fileName, Action<string> callback)
	{
		string path = Application.streamingAssetsPath + "/" + fileName;
		if (Application.platform == RuntimePlatform.Android)
		{
			using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS))
			{
				DebugLog("가져오기 프로세스 시작");
				yield return www.SendWebRequest();
				DebugLog("가져오기 프로세스 종료.");
				if (www.isHttpError || www.isNetworkError)
				{
					DebugLog(www.error);
				}
				else
				{
					path = Path.Combine(Application.persistentDataPath, fileName);
					DebugLog("가져오기 성공. 접근 가능한 경로에 파일을 생성합니다.");
					File.WriteAllBytes(path, www.downloadHandler.data);
					callback(path);
				}
			}
		}
		else
		{
			callback(Application.streamingAssetsPath + "/" + fileName);
		}
	}

	private void DebugLog(object msg)
	{
		Debug.Log("BGAudioTest : " + msg);
	}

	public void PlayAudio(string fileName, int seconds = 0)
	{
		StartCoroutine(GetResourcePath(fileName, path =>
		{
			DebugLog("오디오 재생. 경로 : " + path);

#if !NITY_EDITOR
			StartCoroutine(PlayAudioInUnity("file://" + path, seconds));
#elif UNITY_IOS
			//IOSBackgroundMusicPlayer.musicStart(path, seconds, fileName);
#elif UNITY_ANDROID
		    AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		    AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
		    cls = new AndroidJavaClass("com.nanali.androidtool.MediaManager");
		    activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
	        {
		        using (AndroidJavaObject obj = cls.CallStatic<AndroidJavaObject>("getInstance"))
	 	        {
			        obj.Call("PlayAudio", path, seconds);
		        }
	        }));
#endif
		}));
	}

	public void StopAudio()
	{
		DebugLog("오디오 중지.");
#if UNITY_EDITOR
		AudioSource[] sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		for (int i = 0; i < sources.Length; i++)
			Destroy(sources[i].gameObject);
#elif UNITY_IOS
		//IOSBackgroundMusicPlayer.musicStop();
#elif UNITY_ANDROID
		AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
		cls = new AndroidJavaClass("com.nanali.androidtool.MediaManager");
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
	    {
		    using (AndroidJavaObject obj = cls.CallStatic<AndroidJavaObject>("getInstance"))
	 	    {
			    obj.Call("StopAudio");
		    }
	    }));
#endif
	}

	//어플리케이션이 상단으로 올라올 때 체크.
	void OnApplicationPause(bool paused)
	{
		//앱이 올라올 때 시간 체크.
		if (!paused)
		{
			DebugLog("오디오 타이머 중지.");
#if UNITY_EDITOR
			CancelInvoke("StopAudio");
#elif UNITY_IOS
			//IOSBackgroundMusicPlayer.musicRestart(0);
#elif UNITY_ANDROID
		AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
		cls = new AndroidJavaClass("com.nanali.androidtool.MediaManager");
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
	    {
		    using (AndroidJavaObject obj = cls.CallStatic<AndroidJavaObject>("getInstance"))
	 	    {
			    obj.Call("StopAudioKillThread");
		    }
	    }));
#endif
		}
	}


	IEnumerator PlayAudioInUnity(string path, int seconds)
	{
		using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS))
		{
			yield return www.SendWebRequest();

			if (www.isHttpError || www.isNetworkError)
			{
				Debug.Log(www.error);
			}
			else
			{
				AudioClip myClip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;

				AudioSource source = new GameObject().AddComponent<AudioSource>();
				source.clip = myClip;
				source.loop = true;
				source.Play();

				if (seconds > 0)
					Invoke("StopAudio", seconds);
			}
		}
	}
}

//#if UNITY_IOS
//public class IOSBackgroundMusicPlayer
//{
//	[DllImport("__Internal")]
//	private static extern void _musicStart(string _path, int _time, string _fileName);
//	public static void musicStart(string _path, int _time, string _fileName)
//	{
//		if (Application.platform == RuntimePlatform.IPhonePlayer)
//		{
//			_musicStart(_path, _time, _fileName);
//		}
//	}

//	[DllImport("__Internal")]
//	private static extern void _musicStop();
//	public static void musicStop()
//	{
//		if (Application.platform == RuntimePlatform.IPhonePlayer)
//		{
//			_musicStop();
//		}
//	}

//	[DllImport("__Internal")]
//	private static extern void _musicRestart(int _time);
//	public static void musicRestart(int _time)
//	{
//		if (Application.platform == RuntimePlatform.IPhonePlayer)
//		{
//			_musicRestart(_time);
//		}
//	}

//	[DllImport("__Internal")]
//	private static extern void _musicPause();
//	public static void musicPause()
//	{
//		if (Application.platform == RuntimePlatform.IPhonePlayer)
//		{
//			_musicPause();
//		}
//	}

//	[DllImport("__Internal")]
//	private static extern void _musicResume();
//	public static void musicResume()
//	{
//		if (Application.platform == RuntimePlatform.IPhonePlayer)
//		{
//			_musicResume();
//		}
//	}

//	[DllImport("__Internal")]
//	private static extern bool _isPlay();
//	public static bool isPlay()
//	{
//		if (Application.platform == RuntimePlatform.IPhonePlayer)
//		{
//			return _isPlay();
//		}
//		return false;
//	}
//}
//#endif