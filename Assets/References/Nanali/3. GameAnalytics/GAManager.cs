//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using GameAnalyticsSDK;
//using Nanali;

//public class GAManager : MonoBehaviour
//{
//	private static GAManager _instance;
//	public static GAManager Instance
//	{
//		get
//		{
//			if (_instance == null)
//				_instance = FindObjectOfType(typeof(GAManager)) as GAManager;
//			return _instance;
//		}
//	}

//	public void Initialize()
//	{
//		GameAnalytics.Initialize();
//	}

//	//인앱 결제 이벤트.
//	public void GA_BussinessEvent(string currency, int amount, string itemType, string itemId, string cartType, string receipt, string signature = "")
//	{
//		if (!Utilities.IsConnectedInternet)
//			return;

//#if UNITY_ANDROID
//		GameAnalytics.NewBusinessEventGooglePlay(currency, amount, itemType, itemId, cartType, receipt, signature);
//#elif UNITY_IOS
//        GameAnalytics.NewBusinessEventIOS(currency, amount, itemType, itemId, cartType, receipt);
//#endif
//    }

//	//게임 흐름 파악 이벤트.
//	public void GA_DesignEvent(string eventName)
//	{
//		GameAnalytics.NewDesignEvent(eventName);
//	}

//    //디자인 이벤트.
//	public void GA_DesignEvent(string eventName, float val)
//	{
//		GameAnalytics.NewDesignEvent(eventName, val);
//	}

//	//재화 사용 관련 이벤트.
//	public void GA_ResourceEvent(bool spend, string currencyName, float amount, string type, string description)
//	{
//		GameAnalytics.NewResourceEvent(spend ? GAResourceFlowType.Sink : GAResourceFlowType.Source, currencyName, amount, type, description);
//	}

//    //레벨 진행 관련 이벤트.
//	//public void GA_ProgressionEvent(GAProgressionStatus status, string progress)
//	//{
//	//	GameAnalytics.NewProgressionEvent(status, progress);
//	//}
//}
