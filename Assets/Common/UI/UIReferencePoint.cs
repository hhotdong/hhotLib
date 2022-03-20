//using System.Collections.Generic;
//using UnityEngine;
//using deVoid.Utils;

//public enum UIReferencePointType
//{
//    NONE = -1,
//    GOLD,
//    HEART,
//    JEWEL,
//    UTILITY,
//    SHOP,
//    ACHIEVEMENT_CLEAR,
//    QUEST_CLEAR,
//    GROWTH_TIER_LEVELUP,
//    LANDMARK_RECOVER,
//    CLAM_CLEAR,
//    ALL
//}

//public class RefreshUIReferencePointsSignal : ASignal<int> { }

//public class UIReferencePoint : MonoBehaviour
//{
//    private RectTransform rt;
//    [SerializeField] private UIReferencePointType m_UIRefPointType = UIReferencePointType.NONE;
//    public static Dictionary<int, Vector3> UIRefViewportpointsByType;


//    //////////////////////////////////////////
//    // Initialize & Reset
//    //////////////////////////////////////////

//    private void Awake()
//    {
//        rt = GetComponent<RectTransform>();

//        if (UIRefViewportpointsByType == null)
//            UIRefViewportpointsByType = new Dictionary<int, Vector3>();

//        OnRefreshUIReferencePoints((int)m_UIRefPointType);
//        Signals.Get<RefreshUIReferencePointsSignal>().AddListener(OnRefreshUIReferencePoints);
//    }

//    private void OnDestroy()
//    {
//        Signals.Get<RefreshUIReferencePointsSignal>().RemoveListener(OnRefreshUIReferencePoints);
//        UIRefViewportpointsByType?.Clear();
//    }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public void OnRefreshUIReferencePoints(int typeIdx)
//    {
//        if (typeIdx == (int)UIReferencePointType.ALL || typeIdx == (int)m_UIRefPointType)
//        {
//            Vector3 viewportPoint = CameraManager.UICam.WorldToViewportPoint(rt.position);

//            int thisIdx = (int)m_UIRefPointType;

//            if (UIRefViewportpointsByType != null)
//            {
//                if (UIRefViewportpointsByType.ContainsKey(thisIdx))
//                    UIRefViewportpointsByType[thisIdx] = viewportPoint;
//                else
//                    UIRefViewportpointsByType?.Add(thisIdx, viewportPoint);
//            }
//        }
//    }
//}
