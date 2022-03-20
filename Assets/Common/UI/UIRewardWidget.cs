using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum RewardWidgetType
{
    NONE                      = -1,
    GOLD                      = 0,
    HEART                     = 1,
    JEWEL                     = 2,
    UTILITY                   = 3,
    ANIMAL                    = 4,
    MULTIPLIER                = 5,
    RARE_ANIMAL_RANDOM_BOX_B  = 6
}

[Serializable]
public struct RewardWidgetData
{
    public RewardWidgetType type;
    public Sprite image;
    public string descText;

    public RewardWidgetData(RewardWidgetType _type, Sprite _image, string _descText)
    {
        type = _type;
        image = _image;
        descText = _descText;
    }
}

public class UIRewardWidget : MonoBehaviour
{
    [SerializeField] private Image m_RewardDisplay;
    [SerializeField] private TextMeshProUGUI m_RewardText;

    private static readonly float FONT_SIZE_DEFAULT = 35.0F;
    private static readonly float FONT_SIZE_ANIMAL_NAME = 28.0F;
    private static readonly float[] REWARDS_WIDGET_DISPLAY_IMAGE_SCALE_FACTORS = new float[] { 2.0F, 2.0F, 2.0F, 2.0F, 0.7F, 1.0F, 0.4F };


    //////////////////////////////////////////
    // Utilities
    //////////////////////////////////////////

    public void Toggle(bool isOn, Sprite image, string text, RewardWidgetType widgetType)  //, Action<float> callback = null)
    {
        if (isOn)
        {
            if(!image || string.IsNullOrEmpty(text) || widgetType == RewardWidgetType.NONE)
            {
                Debug.LogError($"Failed to make rewardsWidget toggled on because image({image != null}) or text({!string.IsNullOrEmpty(text)}) is null! Or widgetType({widgetType}) might be null!");
                return;
            }

            m_RewardDisplay.sprite = image;
            m_RewardDisplay.SetNativeSize();
            m_RewardDisplay.rectTransform.localScale = Vector3.one * REWARDS_WIDGET_DISPLAY_IMAGE_SCALE_FACTORS[(int)widgetType];
            m_RewardText.text = text;
            m_RewardText.fontSize = widgetType == RewardWidgetType.ANIMAL
                ? FONT_SIZE_ANIMAL_NAME
                : FONT_SIZE_DEFAULT;

            //callback?.Invoke(m_RewardText.preferredWidth);
        }

        this.gameObject.SetActive(isOn);
    }
}
