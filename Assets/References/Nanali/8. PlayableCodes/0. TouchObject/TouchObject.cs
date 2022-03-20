using System;
using System.Collections.Generic;
using UnityEngine;

public class TouchObject : MonoBehaviour
{
    public Action<string> OnTouch;
    string UsablePositionCode;

    public void SetPositionCode(string code)
    {
        UsablePositionCode = code;
    }

    public void Touched()
    {
        OnTouch?.Invoke(UsablePositionCode);
    }
}
