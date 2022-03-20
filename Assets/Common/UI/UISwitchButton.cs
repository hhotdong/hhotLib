//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;
//using TMPro;

//public class UISwitchButton : MonoBehaviour
//{
//    public ButtonState m_CurrentState { get; private set; } = ButtonState.NONE;

//    [SerializeField] private Button m_SwitchButton;
//    [SerializeField] private Image m_SwitchFill;
//    [SerializeField] private Transform m_SwitchHandle;

//    private Color[] m_FillColor = { new Color(0, 21f / 255f, 21f / 255f, 204f / 255f), new Color(171f / 255f, 207f / 255f, 69f / 255f, 1) };

//    [Header("Move Info"), Space(5)]
//    [SerializeField] private float m_MoveTime = 0.5f;
//    [SerializeField] private float m_MoveValue = 38f;
//    [SerializeField] private Ease m_MoveEase = Ease.OutSine;

//    Action<ButtonState> m_Action;

//    private void Awake()
//    {
//        m_SwitchButton.onClick.AddListener(OnClickButton);
//    }

//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void OnClickButton()
//    {
//        SoundManager.PlaySoundEffect(SoundType.BUTTON_DEFAULT);

//        ButtonState state = ButtonState.NONE;

//        if (m_CurrentState == ButtonState.SWITCH_ON) state = ButtonState.SWITCH_OFF;
//        if (m_CurrentState == ButtonState.SWITCH_OFF) state = ButtonState.SWITCH_ON;

//        SetState(state);
//        m_Action.Invoke(state);
//    }

//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public void SetListener(Action<ButtonState> onSwitch)
//    {
//        m_Action = onSwitch;
//    }

//    public void SetState(ButtonState state, bool smooth = true)
//    {
//        if (state == ButtonState.NONE) return;

//        float move = 0f;

//        if (state == ButtonState.SWITCH_ON) move = m_MoveValue;
//        if (state == ButtonState.SWITCH_OFF) move = -m_MoveValue;

//        if (smooth)
//        {
//            m_SwitchButton.interactable = false;
//            m_SwitchFill.DOColor(m_FillColor[(int)state], m_MoveTime)
//                .SetEase(m_MoveEase);
//            m_SwitchHandle.DOLocalMoveX(move, m_MoveTime)
//                .SetEase(m_MoveEase)
//                .OnComplete(() =>
//                {
//                    m_SwitchButton.interactable = true;
//                });

//        }
//        else
//        {
//            m_SwitchFill.color = m_FillColor[(int)state];
//            m_SwitchHandle.localPosition = new Vector3(
//                move, m_SwitchButton.transform.localPosition.y, m_SwitchButton.transform.localPosition.z);
//        }

//        m_CurrentState = state;
//    }

//}
