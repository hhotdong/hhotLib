//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public enum NotificationType
//{
//    NONE                         = -1,
//    ANIMAL_NORMAL_LEVELUP        = 0,
//    ANIMAL_NORMAL_GENERATE       = 1,
//    ANIMAL_RARE_LEVELUP          = 2,
//    ANIMAL_RARE_LEVELUP_UNITBOX  = 3,
//    ZENMODE                      = 4,
//    CAMERA                       = 5,
//    ACHIEVEMENT                  = 6,
//    QUEST                        = 7,
//    PRODUCT                      = 8,
//    LANDMARK_LEVELUP             = 9,
//    LANDMARK_LOOK_CHANGE         = 10,
//    GROWTH_LEVELUP               = 11,
//    PRODUCT_AD                   = 12,
//}

//public class UINotification : MonoBehaviour
//{
//    [SerializeField] TextMeshProUGUI m_NotiText;
//    [SerializeField] Image m_NotiImg;
//    [SerializeField] NotificationType m_NotiType;

//    private void Awake()
//    {
//        InitializeByType();


//        void InitializeByType()
//        {
//            switch (m_NotiType)
//            {
//                case NotificationType.ANIMAL_NORMAL_LEVELUP:
//                    m_NotiImg.color = HelperManager.BUTTON_COLOR_RED;
//                    break;

//                case NotificationType.ANIMAL_NORMAL_GENERATE:
//                    m_NotiText.text = "!";
//                    m_NotiImg.color = HelperManager.BUTTON_COLOR_GREEN;
//                    break;

//                case NotificationType.ANIMAL_RARE_LEVELUP:
//                    m_NotiImg.color = HelperManager.BUTTON_COLOR_YELLOW;
//                    break;

//                case NotificationType.ANIMAL_RARE_LEVELUP_UNITBOX:
//                    m_NotiText.text = "!";
//                    m_NotiImg.color = HelperManager.BUTTON_COLOR_YELLOW;
//                    break;

//                case NotificationType.ZENMODE:
//                case NotificationType.ACHIEVEMENT:
//                case NotificationType.PRODUCT:
//                case NotificationType.LANDMARK_LEVELUP:
//                    m_NotiImg.color = HelperManager.BUTTON_COLOR_RED;
//                    break;

//                case NotificationType.CAMERA:
//                case NotificationType.PRODUCT_AD:
//                    m_NotiText.text = "!";
//                    m_NotiImg.color = HelperManager.BUTTON_COLOR_RED;
//                    break;

//                case NotificationType.QUEST:
//                    m_NotiText.text = "1";
//                    m_NotiImg.color = HelperManager.BUTTON_COLOR_RED;
//                    break;

//                case NotificationType.LANDMARK_LOOK_CHANGE:
//                    m_NotiImg.color = HelperManager.BUTTON_COLOR_BLUE;
//                    break;

//                case NotificationType.GROWTH_LEVELUP:
//                    m_NotiImg.color = HelperManager.BUTTON_COLOR_YELLOWGREEN;
//                    break;

//                default:
//                    m_NotiImg.color = HelperManager.BUTTON_COLOR_GRAY_200;
//                    Debug.Log($"Set notification color to gray because type({m_NotiType.ToString()}) is not appropriate!");
//                    break;
//            }
//        }
//    }

//    public void UpdateNotification(bool shouldActivate, string text = "")
//    {
//        if(shouldActivate && !string.IsNullOrEmpty(text))
//        {
//            m_NotiText.text = text;
//        }
//        gameObject.SetActive(shouldActivate);
//    }
//}
