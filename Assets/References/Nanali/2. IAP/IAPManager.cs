//using System;
//using System.Collections;
//using UnityEngine;
//using UnityEngine.Purchasing;
//using Nanali;
//using System.Collections.Generic;
//using BackEnd;
//using System.Runtime.InteropServices;

//[RequireComponent(typeof(HTTPMethods))]
//public class IAPManager : MonoBehaviour, IStoreListener
//{
//    private static IAPManager _instance;
//    public static IAPManager Instance
//    {
//        get
//        {
//            if (_instance == null)
//                _instance = FindObjectOfType(typeof(IAPManager)) as IAPManager;
//            return _instance;
//        }
//    }

//    private IAPAsset Asset;
//    private static IStoreController storeController;
//    private static IExtensionProvider extensionProvider;
//    private IAPInformation pendingItem; //누락된 아이템.
//    private IAPInformation preOrderItem; //사전예약 아이템. (안드로이드에서만 사용됨)
//    private bool IsEnteredManualPurchase; //DoPurchase로 진입한 경우에만 true.

//    //Listener.
//    public Action<IAPInformation> OnPurchaseCallback;
//    public Action<Product, PurchaseFailureReason> OnPurchaseFailCallback;
//    public Action<bool> OnPreOrderCallback;

//    //subscription.
//    public List<SubscriptionInfo> subscriptionInfos { get; private set; } = new List<SubscriptionInfo>();

//    public bool IsInitialized { get { return storeController != null && extensionProvider != null; } }

//    IAPInformation GetInfo(string id)
//    {
//        IAPInformation info = null;
//        for (int i = 0; i < Asset.IAPInformations.Length; i++)
//        {
//            if (Asset.IAPInformations[i].id == id)
//            {
//                info = Asset.IAPInformations[i];
//                break;
//            }
//        }

//        return info;
//    }

//    void DebugLog(object msg)
//    {
//        if (Asset.IsDevelop)
//            Debug.Log("(Unity IAP, Nanali) " + msg);
//    }

//    void Start()
//    {
//        Asset = IAPAsset.Instance;

//#if !UNITY_EDITOR
//        InitializePurchasing();
//#else
//        DebugLog("에디터에서 초기화 할 수 없습니다.");
//#endif
//    }

//    //IAP 모듈 초기화.
//    private void InitializePurchasing()
//    {
//        if (IsInitialized)
//        {
//            DebugLog("InitializePurchasing 실패. 이미 초기화되었습니다.");
//            return;
//        }

//        if (!Utilities.IsConnectedInternet)
//        {
//            DebugLog("InitializePurchasing 실패. 인터넷에 연결되지 않았습니다.");
//            return;
//        }

//        if (Asset == null && Asset.IAPInformations.Length <= 0)
//        {
//            DebugLog("InitializePurchasing 실패. 상품이 규정되어있지 않습니다.");
//            return;
//        }

//        var module = StandardPurchasingModule.Instance();

//        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

//        for (int i = 0; i < Asset.IAPInformations.Length; i++)
//        {
//            string id = Asset.IAPInformations[i].id;
//            ProductType pType = Asset.IAPInformations[i].pType;
//            builder.AddProduct(id, pType, new IDs
//            {
//                {id, AppleAppStore.Name},
//                {id, GooglePlay.Name}
//			    //{id, AmazonApps.Name} ~~~
//		    });
//        }

//        DebugLog("InitializePurchasing 성공.");
//        UnityPurchasing.Initialize(this, builder);
//    }

//    //상품 정보 가져오기.
//    public List<IAPInformation> GetInformations()
//    {
//        if (!IsInitialized)
//        {
//            DebugLog("GetInformations 실패. 스토어가 초기화되지 않았습니다.");
//            //callback(null);
//            return null;
//        }

//        DebugLog("GetInformations 성공.");

//        Product[] products = storeController.products.all;
//        List<IAPInformation> informations = new List<IAPInformation>();

//        DebugLog($"{products.Length}");

//        for (int i = 0; i < products.Length; i++)
//        {
//            IAPInformation getInfo = GetInfo(products[i].definition.id);
//            informations.Add(new IAPInformation(getInfo.id, products[i].metadata.localizedTitle, products[i].metadata.localizedPriceString, products[i].definition.type));
//            DebugLog($" Count {informations.Count}");
//        }

//        //callback(informations.ToArray());
//        return informations;
//    }

//    //상품 구매.
//    public void DoPurchase(string id)
//    {
//        if (!IsInitialized)
//        {
//            DebugLog("DoPurchase 실패. 스토어가 초기화되지 않았습니다.");
//            OnPurchaseCallback?.Invoke(null);
//            return;
//        }

//        //인터넷이 안되면 못함.
//        if (!Utilities.IsConnectedInternet)
//        {
//            DebugLog("DoPurchase 실패. 인터넷에 연결되지 않았습니다.");
//            OnPurchaseCallback?.Invoke(null);
//            return;
//        }

//        try
//        {
//            Product p = storeController.products.WithID(id);
//            if (p != null && p.availableToPurchase)
//            {
//                DebugLog(string.Format("DoPurchase 성공. 제품의 구매를 시작합니다. ID : {0}", p.definition.id));

//                IsEnteredManualPurchase = true;
//                storeController.InitiatePurchase(p);
//            }
//            else
//            {
//                DebugLog("DoPurchase 실패. 제품을 찾을 수 없거나 구입할 수 없습니다.");
//                OnPurchaseCallback?.Invoke(null);
//            }
//        }
//        catch (Exception e)
//        {
//            DebugLog("DoPurchase 실패. 예외가 발생했습니다. ERROR : " + e);
//            OnPurchaseCallback?.Invoke(null);
//        }
//    }

//    //구매 복원.
//    public void DoRestorePurchase()
//    {
//        if (!IsInitialized)
//        {
//            DebugLog("DoRestorePurchase 실패. 스토어가 초기화되지 않았습니다.");
//            return;
//        }

//        //인터넷이 안되면 못함.
//        if (!Utilities.IsConnectedInternet)
//        {
//            DebugLog("DoRestorePurchase 실패. 인터넷에 연결되지 않았습니다.");
//            return;
//        }

//        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
//        {
//            DebugLog("DoRestorePurchase 실행중. 애플환경에서만 나타나는 메세지입니다.");

//            var apple = extensionProvider.GetExtension<IAppleExtensions>();

//            apple.RestoreTransactions((result) =>
//            {
//                DebugLog(string.Format("DoRestorePurchase {0}. 추가적인 로그가 없다면 복원품이 없거나 실패입니다.", result ? "성공" : "실패"));
//            });
//        }
//        else
//        {
//            DebugLog("DoRestorePurchase 실패. 지원되지 않는 플랫폼입니다. 현재 플랫폼 = " + Application.platform);
//        }
//    }

//    //결제완료까지 진행되지 않아 누락된 상품이 있는지 확인.
//    public void GetePendingItem()
//    {
//        if (!IsInitialized)
//        {
//            DebugLog("DoRestorePendingItem 실패. 스토어가 초기화되지 않았습니다.");
//            OnPurchaseCallback?.Invoke(null);
//            return;
//        }

//        if (pendingItem == null)
//        {
//            DebugLog("DoRestorePendingItem 실패. 누락된 상품이 없습니다.");
//            OnPurchaseCallback?.Invoke(null);
//            return;
//        }


//        //완료 처리.
//        Product[] items = storeController.products.all;
//        for (int i = 0; i < items.Length; i++)
//        {
//            if (string.Equals(items[i].definition.id, pendingItem.id))
//            {
//                DebugLog(string.Format("DoRestorePendingItem 성공. 콜백에서 {0} 에 대한 상품수령 처리를 할 수 있습니다.", items[i].definition.id));
//                storeController.ConfirmPendingPurchase(items[i]);
//                pendingItem = null;
//                break;
//            }
//        }

//        OnPurchaseCallback?.Invoke(pendingItem); //pending 아이템이 소비처리되었으면 성공.
//    }

//    //영수증 검증.
//    void ValidReceipt(Product p, Action<bool> _purchaseCallback)
//    {
//        Backend.Receipt.IsValidateGooglePurchase(p.receipt, "receiptDescription", false, (callback) =>
//        {
//            UnityEngine.Debug.Log("Nanali : " + callback.GetReturnValue());
//            _purchaseCallback(callback.IsSuccess());
//        });
//        //_purchaseCallback(true);
//    }

//    //결제 성공 콜백. 앱이 내려갔다 올라올 때도 호출된다.
//    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
//    {
//        //결제된 상품 확인.
//        DebugLog(string.Format("ProcessPurchase : {0} 상품에 대한 프로세스가 진행됩니다.", args.purchasedProduct.definition.id));
//        Product p = args.purchasedProduct;
//        IAPInformation info = GetInfo(p.definition.id); //내부에 규정된 상품정보.

//        //안드로이드의 경우 PreOrder 아이템이 구매와 동일한 콜백으로 요청됨. static에 저장 후 종료.
//        if (info.IsPreOrderItem)
//        {
//            if (preOrderItem == null)
//            {
//                DebugLog("ProcessPurchase : 사전예약 상품이 지정되었습니다. (이 로그는 안드로이드에만 노출됩니다)");
//                preOrderItem = info;
//            }
//            else
//            {
//                DebugLog("ProcessPurchase : 사전예약 상품은 이미 지정되어있습니다. GetPreOrderReward를 호출하여 상품을 지급하세요. (이 로그는 안드로이드에만 노출됩니다)");
//            }

//            return PurchaseProcessingResult.Pending;
//        }

//        //사전예약 상품이 아닌 경우에만 진행.
//        if (!info.IsPreOrderItem)
//        {
//            DebugLog("ProcessPurchase : 영수증을 검증합니다.");
//            //ValidReceipt(p, (bool success) =>
//            //{
//            //    if (success) //영수증 검증 성공.
//            //    {
//                    DebugLog("ProcessPurchase : 영수증이 검증되었습니다.");
//                    IAPInformation item = new IAPInformation(p.definition.id, p.metadata.localizedTitle, p.metadata.localizedPriceString, p.definition.type);
//                    DebugLog($"{p.definition.id} , {p.metadata.localizedTitle} , {p.metadata.localizedPriceString} , {p.definition.type}");
//                    if (!IsEnteredManualPurchase && info.pType != ProductType.Subscription) //DoPurchase로 진입하지 않은 Consumable 상품의 경우. (누락된 상품 복원)
//                    {
//                        DebugLog("ProcessPurchase : 누락된 상품에 대한 복원 프로세스입니다. DoRestorePendingItem을 호출하여 상품 수령 처리를 진행하세요.");
//                        pendingItem = item;
//                    }
//                    else //일반적인 구매.
//                    {
//                        DebugLog(string.Format("ProcessPurchase : 구매된 상품에 대한 프로세스입니다. 콜백에서 {0} 에 대한 상품 수령 처리를 할 수 있습니다.", p.definition.id));
//                        DebugLog("ProcessPurchase A");
//                        //storeController.ConfirmPendingPurchase(p); //pending처리된 상품을 complete로 변환.
//                        DebugLog("ProcessPurchase B");
//                        OnPurchaseCallback?.Invoke(item);
//                        DebugLog("ProcessPurchase C");
//                        IsEnteredManualPurchase = false;
//                        DebugLog("ProcessPurchase D");
//                    }

//                    DebugLog("ProcessPurchase : 구매 성공.");

//                    //결제 관련 이벤트 수집.
//            //    }
//            //    else
//            //    {
//            //        DebugLog("ProcessPurchase : 영수증이 검증에 실패하였습니다.");
//            //        OnPurchaseCallback?.Invoke(null);
//            //    }

//            //    DebugLog("ProcessPurchase : 결제 프로세스가 완료되었습니다.");
//            //});
//        }


//        return PurchaseProcessingResult.Complete; //결제가 성공된 건은 무조건 pending으로 종료. 영수증 검증 프로세스 후에 ConfirmPendingPurchase 호출.
//    }

//    //구독 상품 정보 확인.
//    public void GetSubscriptionProduct(Action<SubscriptionInfo[]> callback)
//    {
//        if (!IsInitialized)
//        {
//            DebugLog("GetSubscriptionProduct 실패. 스토어가 초기화되지 않았습니다.");
//            callback(subscriptionInfos.ToArray());
//            return;
//        }

//        CheckSubScriptionProduct();

//        if (subscriptionInfos.Count <= 0)
//        {
//            DebugLog("GetSubscriptionProduct : 구독 상품이 없습니다.");
//            callback(subscriptionInfos.ToArray());
//            return;
//        }

//        DebugLog("GetSubscriptionProduct : 구독 상품 반환 성공.");
//        callback(subscriptionInfos.ToArray());
//    }

//    void CheckSubScriptionProduct()
//    {
//        subscriptionInfos.Clear();
//        DebugLog("CheckSubScriptionProduct : 구독 상품의 상태를 확인합니다.");
//        //subscription.
//        var apple = extensionProvider.GetExtension<IAppleExtensions>();
//        Dictionary<string, string> introductory_info_dict = apple.GetIntroductoryPriceDictionary();
//        foreach (var item in storeController.products.all)
//        {
//            // this is the usage of SubscriptionManager class
//            if (item.definition.type == ProductType.Subscription)
//            {
//                DebugLog("CheckSubScriptionProduct : 구독 상품으로 정의된 제품인 " + item.definition.id + " 에 대한 정보입니다.");

//                if (item.receipt != null)
//                {
//                    try
//                    {
//                        string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
//                        SubscriptionManager p = new SubscriptionManager(item, intro_json);
//                        SubscriptionInfo info = p.getSubscriptionInfo();

//                        subscriptionInfos.Add(info);

//                        //https://docs.unity3d.com/Manual/UnityIAPSubscriptionProducts.html
//                        DebugLog("------------ 구독중인 상품에 대한 정보입니다. subscriptionInfos 리스트에 저장됩니다. ----------------------");
//                        DebugLog("제품 ID : " + info.getProductId());
//                        DebugLog("구매된 날짜 (UTC시간입니다. GMT+9) : " + info.getPurchaseDate());
//                        DebugLog("자동갱신 혹은 만료되는 날짜 (UTC시간입니다. GMT+9) : " + info.getExpireDate());
//                        DebugLog("구독중? : " + info.isSubscribed().ToString());
//                        DebugLog("만료됨? : " + info.isExpired().ToString());
//                        DebugLog("구독 취소? : " + info.isCancelled());
//                        DebugLog("무료 체험? : " + info.isFreeTrial());
//                        DebugLog("자동 갱신? : " + info.isAutoRenewing());
//                        DebugLog("다음 구매까지 남은 구독 유효 시간 (double) : " + info.getRemainingTime().TotalSeconds);
//                        DebugLog("최초 할인 구매 중? : " + info.isIntroductoryPricePeriod());
//                        DebugLog("최초 할인 구매 가격 : " + info.getIntroductoryPrice());
//                        DebugLog("최초 할인 구매 남은 구독 유효 시간 (double) : " + info.getIntroductoryPricePeriod().TotalSeconds);
//                        DebugLog("최초 할인 적용 가능 횟수 : " + info.getIntroductoryPricePeriodCycles());
//                        DebugLog("----------------------------------------------------------------------------------------------");
//                    }
//                    catch (Exception e)
//                    {
//                        DebugLog("CheckSubScriptionProduct : 에러가 발생했습니다. Reason : " + e.Message);
//                    }

//                }
//                else
//                {
//                    DebugLog("CheckSubScriptionProduct : 제품에 유효한 영수증이 없습니다.");
//                }
//            }
//        }

//        DebugLog("CheckSubScriptionProduct : 구독 상품의 상태를 확인을 종료합니다.");
//    }






//    //IStoreListener에서 콜백됨. 초기화 성공.
//    public void OnInitialized(IStoreController sc, IExtensionProvider ep)
//    {
//        DebugLog("SDK 초기화 성공.");

//        storeController = sc;
//        extensionProvider = ep;

//        //구독 상품 정보 확인.
//        CheckSubScriptionProduct();
//    }
//    //IStoreListener에서 콜백됨. 초기화 실패.
//    public void OnInitializeFailed(InitializationFailureReason reason)
//    {
//        DebugLog("SDK 초기화 실패. 실패사유 = " + reason);
//    }

//    //결제 실패.
//    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
//    {
//        //application block end.
//        UnityEngine.Debug.Log(string.Format("결제 실패. 상품 = {0}, 이유 = {1}", product.definition.storeSpecificId, failureReason));

//        OnPurchaseFailCallback?.Invoke(product, failureReason);
//    }









//    //사전예약 보상.
//    //안드로이드의 경우, 콘솔에서 사전주문 보상용 인앱을 따로 만들어야합니다. 따라서 보유 영수증 체크를 통해, 사전주문 보상과 동일한 아이디가 나올 경우 지급하면 됩니다.
//    //iOS의 경우, 앱과 관련된 영수증을 조회하면 영수증 정보 중, preorder_date_ms 값이 포함된 영수증이 있다면 사전주문보상을 지급하면 됩니다.
//    //영수증정보는 삭제되지 않습니다.
//#if UNITY_IOS
//    [DllImport("__Internal")]
//    private static extern void GetReceipt(string callbackObject, string callbackMethod);
//#endif
//    public void GetPreOrderReward()
//    {
//        if (!IsInitialized)
//        {
//            DebugLog("GetPreOrderReward 실패. 스토어가 초기화되지 않았습니다.");
//            OnPreOrderCallback?.Invoke(false);
//            return;
//        }

//        //인터넷이 안되면 못함.
//        if (!Utilities.IsConnectedInternet)
//        {
//            DebugLog("GetPreOrderReward 실패. 인터넷에 연결되지 않았습니다.");
//            OnPreOrderCallback?.Invoke(false);
//            return;
//        }

//#if UNITY_EDITOR
//        OnPreOrderCallback?.Invoke(true);
//#elif UNITY_ANDROID
//        DebugLog("GetPreOrderReward 성공." + preOrderItem != null ? "사전 예약 보상 지급 진행." : "사전 예약 보상 없음." + " (이 로그는 안드로이드에만 노출됩니다)");
//        OnPreOrderCallback?.Invoke(preOrderItem != null);
//#elif UNITY_IOS //Native에서 영수증값 체크.
//        GetReceipt(gameObject.name, "CheckPreOrderUserCallbackForiOS");
//#endif
//    }

//    public void CheckPreOrderUserCallbackForiOS(string encodeReceiptString) // = For iOS
//    {
//        if (string.IsNullOrEmpty(encodeReceiptString)) //영수증 정보 없음.
//        {
//            OnPreOrderCallback?.Invoke(false);
//            return;
//        }

//        CheckReceipt(false, encodeReceiptString);
//    }

//    void CheckReceipt(bool sandbox, string encodeReceipt) // = For iOS
//    {
//        if (HTTPMethods.Instance != null)
//        {
//            Hashtable ht_parameter = new Hashtable();
//            ht_parameter["receipt-data"] = encodeReceipt;

//            string json = Procurios.Public.JSON.JsonEncode(ht_parameter);
//            string uri = sandbox ? "https://sandbox.itunes.apple.com/verifyReceipt" : "https://buy.itunes.apple.com/verifyReceipt";

//            HTTPMethods.Instance.POST(uri, json, (string error, string responseText) =>
//            {
//                if (string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(responseText))
//                {

//                    Hashtable result = Procurios.Public.JSON.JsonDecode(responseText) as Hashtable;

//                    if (result != null)
//                    {
//                        if (result.ContainsKey("status"))
//                        {
//                            int status = Convert.ToInt32(result["status"]);

//                            if (status == 0)
//                            {
//                                //유효한 영수증.
//                                if (result.ContainsKey("receipt"))
//                                {
//                                    Hashtable receipt = result["receipt"] as Hashtable;
//                                    //https://developer.apple.com/documentation/appstorereceipts/responsebody/receipt 참고. 사전주문일 경우에만 preorder_date_ms 필드가 있음.

//                                    //The time the user ordered the app available for pre-order, in UNIX epoch time format, in milliseconds.
//                                    //This field is only present if the user pre-orders the app. Use this time format for processing dates.
//                                    DebugLog("CheckReceipt 성공. preorder_date_ms 필드 보유 여부에 따라 성공값이 리턴됩니다. (이 로그는 iOS에만 노출됩니다)");
//                                    OnPreOrderCallback?.Invoke(receipt != null && receipt.ContainsKey("preorder_date_ms"));
//                                }
//                                else
//                                {
//                                    OnPreOrderCallback?.Invoke(false);
//                                }
//                            }
//                            else if (status == 21007)
//                            {
//                                //이 영수증은 샌드박스 환경에서 온 것이지만 검증을 위해 프로덕션 환경으로 전송되었습니다. (=샌드박스 환경에서 프로덕션 uri로 호출 한 경우 리턴됨.)
//                                //샌드박스 환경으로 다시 체크.
//                                DebugLog("CheckReceipt 실패. 샌드박스 환경으로 다시 시도합니다. (이 로그는 iOS에만 노출됩니다)");
//                                CheckReceipt(true, encodeReceipt);
//                            }
//                        }
//                        else
//                        {
//                            OnPreOrderCallback?.Invoke(false);
//                        }
//                    }
//                    else
//                    {
//                        OnPreOrderCallback?.Invoke(false);
//                    }
//                }
//                else
//                {
//                    OnPreOrderCallback?.Invoke(false);
//                }
//            });
//        }
//    }
//}

//[Serializable]
//public class IAPInformation
//{
//    public bool IsPreOrderItem;
//    public string id; //아이템 ID.
//    public string name; //지역별 이름. (세팅 안되어있으면 마켓에 등록된 기본 국가 기준으로 리턴)
//    public string price; //지역별 가격. (세팅 안되어있으면 마켓에 등록된 기본 국가 기준으로 리턴)
//    public ProductType pType;

//    public IAPInformation(string _id, string _name, string _price, ProductType _pType, bool _isPreOrderItem = false)
//    {
//        id = _id;
//        name = _name;
//        price = _price;
//        IsPreOrderItem = _isPreOrderItem;
//        pType = _pType;
//    }
//}