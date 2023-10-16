using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace hhotLib.Common
{
    public enum NotificationType {
        LevelUp, Message
    }

    public class UINotification : MonoBehaviour
    {
        [SerializeField] private NotificationType notiType;
        [SerializeField] private TextMeshProUGUI  notiText;
        [SerializeField] private Image            notiImg;

        public void UpdateNotification(bool activate, string text = "")
        {
            if (activate && string.IsNullOrEmpty(text) == false)
                notiText.text = text;
            gameObject.SetActive(activate);
        }

        private void Start()
        {
            switch (notiType)
            {
                default:
                    Debug.LogWarning($"Invalid notification type({notiType})!");
                    notiText.color = Color.clear;
                    notiImg .color = Color.clear;
                    break;

                case NotificationType.LevelUp:
                    notiText.text = "!";
                    notiText.color = Color.white;
                    notiImg .color = Color.red;
                    break;

                case NotificationType.Message:
                    notiText.color = Color.white;
                    notiImg .color = Color.yellow;
                    break;
            }
        }
    }
}