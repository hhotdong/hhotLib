using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInputTracker : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public event Action<Vector3> OnBeginDragEvent;
    public event Action<Vector3> OnDragEvent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragEvent?.Invoke(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragEvent?.Invoke(eventData.position);
    }
}
