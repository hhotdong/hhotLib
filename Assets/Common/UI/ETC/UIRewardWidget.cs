using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace hhotLib.Common
{
    public class RewardWidgetPayload
    {
        public readonly Sprite RewardImg;
        public readonly string RewardText;
        public readonly float  ImageScaler;
        public readonly float  FontSize;

        public RewardWidgetPayload(Sprite rewardImg, string rewardText, float imageScaler, float fontSize)
        {
            RewardImg   = rewardImg;
            RewardText  = rewardText;
            ImageScaler = imageScaler;
            FontSize    = fontSize;
        }
    }

    public class UIRewardWidget : MonoBehaviour
    {
        private Image           rewardImage;
        private TextMeshProUGUI rewardText;

        public void UpdateReward(RewardWidgetPayload payload)
        {
            if (payload == null)
                return;

            rewardImage.sprite = payload.RewardImg;
            rewardImage.SetNativeSize();
            rewardImage.transform.localScale = Vector3.one * payload.ImageScaler;

            rewardText.text     = payload.RewardText;
            rewardText.fontSize = payload.FontSize; 

        }

        private void Awake()
        {
            rewardImage = GetComponentInChildren<Image>();
            rewardText  = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}