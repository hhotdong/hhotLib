//using UnityEngine;
//using deVoid.Utils;

//public class RefreshUICoinEffectPointsSignal : ASignal { }

//public class OnEndCollectCoin : MonoBehaviour
//{
//    private RectTransform rt;

//    [SerializeField] private UIReferencePointType m_UIRefPointType = UIReferencePointType.NONE;
//    [SerializeField] private SoundType m_CollisionSoundEffect = SoundType.BUTTON_UPGRADE;


//    //////////////////////////////////////////
//    // Initialize & Reset
//    //////////////////////////////////////////

//    private void Awake()
//    {
//        rt = GetComponent<RectTransform>();
//        Signals.Get<RefreshUICoinEffectPointsSignal>().AddListener(OnRefreshUICoinEffectPoints);
//    }

//    private void Start()
//    {
//        switch (m_UIRefPointType)
//        {
//            case UIReferencePointType.GOLD: gameObject.layer = HelperManager.LAYER_COIN_DESTINATION_GOLD; break;
//            case UIReferencePointType.HEART: gameObject.layer = HelperManager.LAYER_COIN_DESTINATION_HEART; break;
//            case UIReferencePointType.JEWEL: gameObject.layer = HelperManager.LAYER_COIN_DESTINATION_JEWEL; break;
//            case UIReferencePointType.UTILITY: gameObject.layer = HelperManager.LAYER_COIN_DESTINATION_UTILITY; break;
//            case UIReferencePointType.SHOP: gameObject.layer = HelperManager.LAYER_COIN_DESTINATION_SHOP; break;
//            default: break;
//        }
//    }

//    private void OnDestroy()
//    {
//        Signals.Get<RefreshUICoinEffectPointsSignal>().RemoveListener(OnRefreshUICoinEffectPoints);
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void OnParticleCollision(GameObject other)
//    {
//        SoundManager.PlaySoundEffect(m_CollisionSoundEffect);
//    }

//    private void OnRefreshUICoinEffectPoints()
//    {
//        if(m_UIRefPointType != UIReferencePointType.NONE)
//        {
//            int typeIdx = (int)m_UIRefPointType;
//            if (UIReferencePoint.UIRefViewportpointsByType.ContainsKey(typeIdx))
//            {
//                Vector3 refViewportPoint = UIReferencePoint.UIRefViewportpointsByType[typeIdx];

//                rt.anchorMin
//                    = rt.anchorMax
//                    = refViewportPoint;

//                rt.anchoredPosition3D = Vector3.zero;
//                rt.sizeDelta = Vector2.zero;
//            }
//        }
//    }
//}
