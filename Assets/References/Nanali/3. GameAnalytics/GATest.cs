//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Purchasing;

//public class GATest : MonoBehaviour
//{
//    GAManager manager;

//    private void Start()
//    {
//        manager = GAManager.Instance;
//    }

//    //유니티 인앱의 Product 클래스를 이용한 결제 데이터 수집 예시.
//    public void TrackPruchaseEvent(Product p)
//    {
//        //set track to in-app purchase event in game analytics.
//        string receipt = p.receipt;
//        string currency = p.metadata.isoCurrencyCode;
//        int amount = decimal.ToInt32(p.metadata.localizedPrice) * 100;
//        string receiptJson = "";
//        string signature = "";
//#if UNITY_ANDROID
//        var ht_receipt = Procurios.Public.JSON.JsonDecode(p.receipt) as Hashtable;
//        if (ht_receipt["Payload"] != null)
//        {
//            var ht_payload = Procurios.Public.JSON.JsonDecode(ht_receipt["Payload"].ToString()) as Hashtable;

//            if (ht_payload["json"] != null)
//                receiptJson = ht_payload["json"].ToString(); //setting to signedData.

//            if (ht_payload["signature"] != null)
//                signature = ht_payload["signature"].ToString();//setting to signature.
//        }
//#elif UNITY_IOS
//             receiptJson = receipt;
//#endif
//        if (amount > 0)
//            manager.GA_BussinessEvent(currency, amount, "consumable", p.definition.id, "store", receiptJson, signature);
//    }

//    //일반적인 디자인 이벤트 수집 예시.
//    public void TrackDesignEvent()
//    {
//        manager.GA_DesignEvent("Ads:Coin:Show");
//    }

//    //값이 있는 디자인 이벤트 수집 예시.
//    public void TrackDesignEventForValue(float val)
//    {
//        manager.GA_DesignEvent("Loading:LoadTime", val);
//    }

//    //재화 유동 관련 이벤트 수집 예시.
//    public void TrackResourceEvent()
//    {
//        manager.GA_ResourceEvent(true, "coin", 10, "buy", "booster");
//    }

//    //프로그래스(게임 진행도) 이벤트 수집 예시.
//    public void TrackProgressEvent()
//    {
//        //manager.GA_ProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Start, "level1");
//        //manager.GA_ProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Fail, "level1");
//        //manager.GA_ProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Complete, "level1");
//    }
//}
