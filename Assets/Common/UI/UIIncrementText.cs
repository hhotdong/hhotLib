using System;
using UnityEngine;
using TMPro;

public class UIIncrementText<T> : UIIncrementText
{
    protected bool m_IsInitialized = false;
    protected TextMeshProUGUI m_ValueText;

    /// <summary>
    /// 텍스트 업데이트 시 시작값
    /// </summary>
    protected T m_FollowingValue;

    /// <summary>
    /// 텍스트 업데이트 여부를 판단하기 위해 비교 기준이 되는 값
    /// </summary>
    protected T m_TargetValue;

    private void Start() { Initialize(); }
}

public abstract class UIIncrementText : MonoBehaviour
{
    public virtual void Initialize() { }
    public virtual void Increment(Action completeCallback = null) { }
}