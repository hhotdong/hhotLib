using System.Collections.Generic;
using UnityEngine;

public enum TouchState
{
    NONE               = 0,
    ONE_FINGER         = 1 << 0,
    TWO_FINGER         = 1 << 1,
    ONE_OR_TWO_FINGER  = ONE_FINGER | TWO_FINGER
}

public class TouchManager : MonoBehaviour
{
    public delegate void CustomMouseEventHandler(Vector3 mousePos);
    public static event CustomMouseEventHandler OnBeginMouse, OnMouse, OnEndMouse;
    public delegate void CustomTouchEventHandler(Touch[] touches);
    public static event CustomTouchEventHandler OnBeginTouch, OnTouch, OnEndTouch;

    private readonly List<int> validFingerIDs = new List<int>();
    private readonly Dictionary<int, Touch> touchHandler = new Dictionary<int, Touch>();

    public static bool IsTouchDetecting = true;
    public static TouchState CurrentTouchState { get; private set; } = TouchState.NONE;

    private void OnDestroy()
    {
        CurrentTouchState = TouchState.NONE;
        validFingerIDs.Clear();
        touchHandler.Clear();
    }

    private void Update()
    {
        if (!IsTouchDetecting)
            return;

        HandleTouch();
    }

    public void HandleTouch()
    {
#if UNITY_EDITOR
        Vector3 mousePos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0)) OnBeginMouse?.Invoke(mousePos); 
        else if (Input.GetMouseButton(0)) OnMouse?.Invoke(mousePos);
        else if (Input.GetMouseButtonUp(0)) OnEndMouse?.Invoke(mousePos);
#elif UNITY_ANDROID || UNITY_IOS
        int count = Input.touchCount;
        if (count == 0)
        {
            ClearTouches();
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Touch temp = Input.GetTouch(i);
            int tempID = temp.fingerId;

            if (temp.phase == TouchPhase.Began)
            {
                if (hhotLib.Utilities.IsPointerOverUI() == false)
                {
                    touchHandler.Add(tempID, temp);
                    validFingerIDs.Add(tempID);
                }
            }
            else if (validFingerIDs.Contains(tempID))
            {
                touchHandler[tempID] = temp;
            }
        }

        if (validFingerIDs.Count == 1)
        {
            CurrentTouchState = TouchState.ONE_FINGER;
            Touch touch = touchHandler[validFingerIDs[0]];

            if (touch.phase == TouchPhase.Began)
            {
                OnBeginTouch?.Invoke(new[] { touch });
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                OnEndTouch?.Invoke(new[] { touch });
                ClearTouches();
            }
            else
            {
                OnTouch?.Invoke(new[] { touch });
            }
        }
        else if (validFingerIDs.Count == 2)
        {
            CurrentTouchState = TouchState.TWO_FINGER;
            Touch firstTouch = touchHandler[validFingerIDs[0]];
            Touch secondTouch = touchHandler[validFingerIDs[1]];

            if (secondTouch.phase == TouchPhase.Began)
            {
                OnBeginTouch?.Invoke(new[] { firstTouch, secondTouch });
            }
            else if (firstTouch.phase == TouchPhase.Ended
                    || firstTouch.phase == TouchPhase.Canceled
                    || secondTouch.phase == TouchPhase.Ended
                    || secondTouch.phase == TouchPhase.Canceled)
            {
                OnEndTouch?.Invoke(new[] { firstTouch });
                ClearTouches();
            }
            else 
            {
                OnTouch?.Invoke(new[] { firstTouch, secondTouch });
            }
        }
        else
        {
            Debug.Log("Invalid count of fingers.");
            ClearTouches();
        }

        void ClearTouches()
        {
            CurrentTouchState = TouchState.NONE;

            if (touchHandler != null && touchHandler.Count > 0)
                touchHandler.Clear();

            if (validFingerIDs != null && validFingerIDs.Count > 0)
                validFingerIDs.Clear();
        }
#endif
    }
}