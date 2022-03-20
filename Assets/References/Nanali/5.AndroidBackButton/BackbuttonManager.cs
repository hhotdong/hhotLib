//using System;
//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//using UnityEngine.SceneManagement;
//using UnityEngine.Events;
//using System.Reflection;
//using deVoid.UIFramework;

//public class BackbuttonManager : MonoBehaviour
//{
//	private static BackbuttonManager _instance;
//	public static BackbuttonManager Instance
//	{
//		get
//		{
//			if (_instance == null)
//				_instance = FindObjectOfType(typeof(BackbuttonManager)) as BackbuttonManager;
//			return _instance;
//		}
//	}

//	public float delay = 0.3f;
//	public bool IsValid { get; private set; }

//	private Stack stack = new Stack();
//	private float time;

//	[SerializeField] private UIManager _UIManager;

//	//안드로이드 환경이 아닌 경우 파괴됨. 함수 호출시 ? 혹은 NULL 체크를 통해 에러방지 필요. BackbuttonTestObject에서 확인 가능.
//	private void Start()
//	{
//#if !UNITY_EDITOR
//		if (Application.platform != RuntimePlatform.Android)
//			Destroy (gameObject);
//#endif
//		IsValid = GameManager.IsInitialized;
//        if(IsValid)
//			GameManager.DidCompleteInitialize += OnGMInitialized;
//	}

//    private void OnGMInitialized()
//    {
//		GameManager.DidCompleteInitialize -= OnGMInitialized;
//		IsValid = true;
//	}

//	//등록.
//	public void Push(UnityAction callback)
//	{
//		UnityEvent evt = new UnityEvent();
//		evt.AddListener(callback);

//		stack.Push(evt);
//		time = Time.realtimeSinceStartup;
//	}

//    //동작 상태 강제 조정. (튜토리얼 등 뒤로가기 버튼을 사용하지 않아야 할 때 활용)
//    public void SetState(bool on)
//    {
//		IsValid = on;
//	}

//    //뒤로가기 버튼 누를 경우 작동. 뒤로가기가 아닌 버튼을 이용하여 닫기를 시도 한 경우, Pop 호출 필요.
//	public void Pop(bool isNeedInvoke = true)
//	{
//		if (stack.Count <= 0)
//			return;

//		UnityEvent evt = stack.Pop() as UnityEvent;
//		if (isNeedInvoke)
//			evt?.Invoke();
//	}

//    //강제 청소. 여러 상황에 의해 스택순으로 노출되지 말아야 할 경우에 스택 청소 후 재등록 등의 용도로 사용.
//	public void Clear()
//	{
//		stack.Clear();
//		IsValid = true;
//	}

//	//실제 동작. 스택이 없는 경우 게임종료와 연결됨.
//	private void Update()
//	{
//		if (Input.GetKeyDown(KeyCode.Escape))
//		{
//			if (!IsValid)
//				return;

//			Debug.Log($"Backbutton nowTime : {Time.realtimeSinceStartup}");
//			Debug.Log($"Backbutton Time : {time}");

//			//연속터치 방지.
//			if (Time.realtimeSinceStartup - time < delay)
//				return;

//			time = Time.realtimeSinceStartup;

//			if (stack.Count > 0)
//			{
//				Pop();
//			}
//			else
//			{
//				//씬별로 다른 동작 제어 가능.
//				//switch (SceneManager.GetActiveScene().buildIndex)
//				//{
//				//  case 0: break;
//				//  case 1: break;
//				//  case 2: break;
//				//	case 3: break;
//				//	case 4: break;
//				//	case 5: break;
//				//	case 6: break;
//				//}
//				//NanaliTestManager.Instance.LoadScene(0);

//				if (_UIManager.GetCurrentWindow == null && _UIManager.IsPanelOpen(ScreenIds.NavigationPanel))
//                {
//#if UNITY_EDITOR
//				EditorApplication.isPlaying = false;
//#elif UNITY_ANDROID
//				PopupView.OnDialogPopUp();
//				//Application.Quit();
//				//GotoAndroidHome();
//#endif
//                }
//            }
//        }
//	}

//	void OnEnable()
//	{
//		SceneManager.sceneLoaded += OnLevelFinishedLoading;
//	}

//	void OnDisable()
//	{
//		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
//	}

//    //씬이 전환되는경우 뒤로가기 버튼 관련 전체 초기화 진행됨.
//	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
//	{
//		Clear();
//	}

//    private void OnDestroy()
//    {
//		GameManager.DidCompleteInitialize -= OnGMInitialized;
//	}

//    //private void GotoAndroidHome()
//    //{
//    //	AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.MAIN");
//    //	intent.Call<AndroidJavaObject>("addCategory", "android.intent.category.HOME");
//    //	var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//    //	var activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
//    //	activity.Call("startActivity", intent);
//    //}

//    //private void ShowAndroidToastMessage(string message)
//    //{
//    //	AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//    //	AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

//    //	if (unityActivity != null)
//    //	{
//    //		AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
//    //		unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
//    //		{
//    //			AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
//    //			toastObject.Call("show");
//    //		}));
//    //	}
//    //}
//}
