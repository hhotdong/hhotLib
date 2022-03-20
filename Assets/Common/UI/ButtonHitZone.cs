// https://gist.github.com/sid68v/1de7b7765201e03ecab313ec50ba67c9

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHitZone : MonoBehaviour
{
    private float m_PointerDownTime = 0.0F;

    private bool IsTimerBasedClick => m_ValidClickTime > 0.0F;

    [SerializeField] private bool m_CheckPointerUpEvent = true;
    [SerializeField] private bool m_CheckPointerDownEvent = true;
    //[SerializeField] private bool m_CheckPointerEnterEvent = false;
    [SerializeField] private bool m_CheckPointerExitEvent = true;
    [SerializeField] private bool m_CheckPointerClickEvent = true;
    [SerializeField] private bool m_CheckScrollEvent = false;
    [SerializeField] private float m_ValidClickTime = 0.0F;

    public float width;
    public float height;

    public class EmptyGraphic : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }

    private void OnValidate()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            width = Mathf.Max(width, rectTransform.sizeDelta.x);
            height = Mathf.Max(height, rectTransform.sizeDelta.y);
        }
    }

    private void Awake()
    {
        CreateHitZone();
    }

    private void CreateHitZone()
    {
        // create child object
        GameObject gobj = new GameObject("Button Hit Zone");
        RectTransform hitzoneRectTransform = gobj.AddComponent<RectTransform>();
        hitzoneRectTransform.SetParent(transform);
        hitzoneRectTransform.localPosition = Vector3.zero;
        hitzoneRectTransform.anchoredPosition = Vector3.zero;
        hitzoneRectTransform.localScale = Vector3.one;
        hitzoneRectTransform.sizeDelta = new Vector2(width, height);

        // create transparent graphic
        gobj.AddComponent<EmptyGraphic>();

        // delegate events
        EventTrigger eventTrigger = gobj.AddComponent<EventTrigger>();

        // pointer up
        if (m_CheckPointerUpEvent)
        {
            AddEventTriggerListener(eventTrigger, EventTriggerType.PointerUp,
                (BaseEventData data) =>
                {
                    ExecuteEvents.Execute(gameObject, data,
                        ExecuteEvents.pointerUpHandler);
                });
        }

        // pointer down
        if (m_CheckPointerDownEvent)
        {
            if (IsTimerBasedClick)
            {
                AddEventTriggerListener(eventTrigger, EventTriggerType.PointerDown,
                    (BaseEventData data) =>
                    {
                        m_PointerDownTime = Time.timeSinceLevelLoad;
                        ExecuteEvents.Execute(gameObject, data,
                            ExecuteEvents.pointerDownHandler);
                    });
            }
            else
            {
                AddEventTriggerListener(eventTrigger, EventTriggerType.PointerDown,
                    (BaseEventData data) =>
                    {
                        ExecuteEvents.Execute(gameObject, data,
                           ExecuteEvents.pointerDownHandler);
                    });
            }
        }

        // pointer click
        if (m_CheckPointerClickEvent)
        {
            if (IsTimerBasedClick)
            {
                AddEventTriggerListener(eventTrigger, EventTriggerType.PointerClick,
                    (BaseEventData data) =>
                    {
                        float timeDiff = Time.timeSinceLevelLoad - m_PointerDownTime;
                        if (timeDiff < m_ValidClickTime)
                        {
                            ExecuteEvents.Execute(gameObject, data,
                            ExecuteEvents.pointerClickHandler);
                        }

                        m_PointerDownTime = 0.0F;
                    });
            }
            else
            {
                AddEventTriggerListener(eventTrigger, EventTriggerType.PointerClick,
                    (BaseEventData data) =>
                    {
                        ExecuteEvents.Execute(gameObject, data,
                            ExecuteEvents.pointerClickHandler);
                    });
            }
        }

        // pointer enter
        //if (m_CheckPointerEnterEvent)
        //{
        //    AddEventTriggerListener(eventTrigger, EventTriggerType.PointerEnter,
        //        (BaseEventData data) =>
        //        {
        //            ExecuteEvents.Execute(gameObject, data,
        //                ExecuteEvents.pointerEnterHandler);
        //        });
        //}

        // pointer exit
        if (m_CheckPointerExitEvent)
        {
            AddEventTriggerListener(eventTrigger, EventTriggerType.PointerExit,
                (BaseEventData data) =>
                {
                    ExecuteEvents.Execute(gameObject, data,
                        ExecuteEvents.pointerExitHandler);
                });
        }

        if (m_CheckScrollEvent)
        {
            ScrollRectFaster scrollView = GetComponentInParent<ScrollRectFaster>();

            if (scrollView)
            {
                AddEventTriggerListener(eventTrigger, EventTriggerType.InitializePotentialDrag,
                    (BaseEventData data) =>
                    {
                        scrollView.OnInitializePotentialDrag((PointerEventData)data);
                    });
                AddEventTriggerListener(eventTrigger, EventTriggerType.BeginDrag,
                    (BaseEventData data) =>
                    {
                        scrollView.OnBeginDrag((PointerEventData)data);
                    });
                AddEventTriggerListener(eventTrigger, EventTriggerType.Drag,
                    (BaseEventData data) =>
                    {
                        scrollView.OnDrag((PointerEventData)data);
                    });
                AddEventTriggerListener(eventTrigger, EventTriggerType.EndDrag,
                    (BaseEventData data) =>
                    {
                        scrollView.OnEndDrag((PointerEventData)data);
                    });
            }
        }
    }

    private static void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType,
                                         System.Action<BaseEventData> method)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(method));
        trigger.triggers.Add(entry);
    }
}