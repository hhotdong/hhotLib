//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;
//using TMPro;

//public enum ButtonState
//{
//    NONE = -1,
//    SWITCH_OFF = 0,
//    SWITCH_ON = 1
//}

//[Serializable]
//public class UIGroupButton_Content : MonoBehaviour
//{
//    public ButtonState m_CurrentState { get; private set; } = ButtonState.NONE;

//    [SerializeField] private Button m_Button;
//    [SerializeField] private Image m_Image;
//    [SerializeField] private TextMeshProUGUI m_Text;

//    [Space(5)]
//    [SerializeField] private Color[] m_ImageColor;
//    [SerializeField] private Color[] m_TextColor;

//    [Header("Move Info"), Space(5)]
//    [SerializeField] private float m_FadeTime = 0.5f;
//    [SerializeField] private Ease m_FadeEase = Ease.OutSine;

//    Action m_Action;

//    private void Awake()
//    {
//        m_Button.onClick.AddListener(OnClickButton);
//    }

//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void OnClickButton()
//    {
//        SoundManager.PlaySoundEffect(SoundType.BUTTON_DEFAULT);

//        m_Action.Invoke();
//    }

//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public void SetListener(Action onClick)
//    {
//        m_Action = onClick;
//    }

//    public void SetState(ButtonState state, bool smooth = true)
//    {
//        if (state == ButtonState.NONE) return;

//        if (smooth)
//        {
//            m_Button.interactable = false;
//            m_Image.DOColor(m_ImageColor[(int)state], m_FadeTime)
//                .SetEase(m_FadeEase);
//            m_Text.DOColor(m_TextColor[(int)state], m_FadeTime)
//                .SetEase(m_FadeEase)
//                .OnComplete(() =>
//                {
//                    m_Button.interactable = true;
//                });
//        }
//        else
//        {
//            m_Image.color = m_ImageColor[(int)state];
//            m_Text.color = m_TextColor[(int)state];
//        }

//        m_CurrentState = state;
//    }
//}
