//using UnityEngine;
//using UnityEngine.UI;

//public class IAPTest : MonoBehaviour
//{
//    public InputField inputField;
//    IAPManager manager;

//    void Awake()
//    {
//        manager = IAPManager.Instance;
//        inputField.text = "com.nanali.item0";
//    }

//    private void OnEnable()
//    {
//        manager.OnPurchaseCallback += OnPurchaseCallback;
//        manager.OnPreOrderCallback += OnPreOrderCallback;
//    }

//    private void OnDisable()
//    {
//        manager.OnPurchaseCallback -= OnPurchaseCallback;
//        manager.OnPreOrderCallback -= OnPreOrderCallback;
//    }

//    //결제 품목 정보 확인.
//    public void ShowItemInformations()
//    {
//        if (manager == null)
//            return;

//        //manager.GetInformations((IAPInformation[] infos) =>
//        //{
//        //    for (int i = 0; i < infos.Length; i++)
//        //        Debug.Log("(Unity IAP, Nanali) " + infos[i].id + " : " + infos[i].price + " : " + infos[i].name);
//        //});
//    }

//    //결제.
//    public void DoPurchase()
//    {
//        manager.DoPurchase(inputField.text);
//    }

//    //구매 복원. 안드로이드 작동 안함.
//    public void DoRestore()
//    {
//        manager.DoRestorePurchase();
//    }

//    //결제중 누락상품 여부 확인.
//    public void GetPendingItem()
//    {
//        manager.GetePendingItem();
//    }

//    //사전예약 상품 여부 확인.
//    public void GetPreOrderReward()
//    {
//        manager.GetPreOrderReward();
//    }

//    //구독 리스트 확인.
//    public void GetSubscriptionProductList()
//    {
//        manager.GetSubscriptionProduct((infos) =>
//        {
//            for (int i = 0; i < infos.Length; i++)
//            {
//                //구독 상품에 대한 정보 처리.
//                Debug.Log("(Unity IAP, Nanali) 제품 ID : " + infos[i].getProductId());
//                //Debug.Log("제품 ID : " + infos[i].getProductId());
//                //Debug.Log("구매된 날짜 (UTC시간입니다. GMT+9) : " + infos[i].getPurchaseDate());
//                //Debug.Log("자동갱신 혹은 만료되는 날짜 (UTC시간입니다. GMT+9) : " + infos[i].getExpireDate());
//                //Debug.Log("구독중? : " + infos[i].isSubscribed().ToString());
//                //Debug.Log("만료됨? : " + infos[i].isExpired().ToString());
//                //Debug.Log("구독 취소? : " + infos[i].isCancelled());
//                //Debug.Log("무료 체험? : " + infos[i].isFreeTrial());
//                //Debug.Log("자동 갱신? : " + infos[i].isAutoRenewing());
//                //Debug.Log("다음 구매까지 남은 구독 유효 시간 (double) : " + infos[i].getRemainingTime().TotalSeconds);
//                //Debug.Log("최초 할인 구매 중? : " + infos[i].isIntroductoryPricePeriod());
//                //Debug.Log("최초 할인 구매 가격 : " + infos[i].getIntroductoryPrice());
//                //Debug.Log("최초 할인 구매 남은 구독 유효 시간 (double) : " + infos[i].getIntroductoryPricePeriod().TotalSeconds);
//                //Debug.Log("최초 할인 적용 가능 횟수 : " + infos[i].getIntroductoryPricePeriodCycles());
//            }
//        });
//    }

//    //callbacks.
//    void OnPurchaseCallback(IAPInformation info)
//    {
//        //실패시 null값이 리턴되므로 반드시 null 체크 진행.
//        if (info != null)
//        {
//            //상품 지급.
//            switch (info.id)
//            {
//                case "nanali.forestisland.adcoupon1":
//                    break;
//                case "nanali.forestisland.adcoupon2":
//                    break;
//            }
//        }
//    }

//    void OnPreOrderCallback(bool success)
//    {
//        if (success)
//        {
//            //사전예약 상품 지급.
//            //영수증이 소비처리되지 않으므로 반복적으로 성공처리됩니다.
//            //따라서 게임 이용중에 1회만 수령하도록 내부적인 처리가 필요합니다.
//        }
//    }
//}
