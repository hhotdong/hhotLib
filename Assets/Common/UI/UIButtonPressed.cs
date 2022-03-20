using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonPressed : Button
{
    private Coroutine m_ButtonPressedCoroutine;
    public bool IsAutoClickPlaying => m_ButtonPressedCoroutine != null;


    //////////////////////////////////////////
    // Initialize & Reset
    //////////////////////////////////////////

    protected override void OnDisable()
    {
        base.OnDisable();
        ClearButtonPressedCoroutine();
    }


    //////////////////////////////////////////
    // Listeners
    //////////////////////////////////////////

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (IsPressed() && interactable)
            onClick?.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        ClearButtonPressedCoroutine();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // 연속클릭이 가능한 버튼은 PointerDown 이벤트에서 OnClick 이벤트를 호출하므로 이 부분에서 base.OnPointerClick() 함수는 주석 처리한다.
        //base.OnPointerClick(eventData);

        ClearButtonPressedCoroutine();
    }


    //////////////////////////////////////////
    // Utilities
    //////////////////////////////////////////

    public void StartAutoClick(Func<bool> checkCondition)
    {
        if (!gameObject.activeInHierarchy || !checkCondition())
            return;

        m_ButtonPressedCoroutine = StartCoroutine(ButtonPressedProcess(checkCondition));


        IEnumerator ButtonPressedProcess(Func<bool> _checkCondition)
        {
            float buttonClickTime = Time.timeSinceLevelLoad;
            float clickPerSecond = 0.0F;
            float timeOut = 0.0F;
            float timer = 0.0F;

            const float CPS_MIN = 3.0F;
            const float CPS_MAX = 13.0F;
            const float TIMEOUT_MAX = 1.0F / CPS_MAX;
            const float AUTO_CLICK_THRESHOLD_A = 0.5F;
            const float AUTO_CLICK_THRESHOLD_B = 1.6F;

            while (IsPressed() && interactable && _checkCondition())
            {
                float elapsedTime = Time.timeSinceLevelLoad - buttonClickTime;
                if (elapsedTime >= AUTO_CLICK_THRESHOLD_B)
                {
                    //Debug.Log("Elapsed more than 2.0 sec!");
                    if (timer < TIMEOUT_MAX)
                        timer += Time.unscaledDeltaTime;
                    else
                    {
                        onClick?.Invoke();
                        timer = 0.0F;
                    }
                }
                else if (elapsedTime >= AUTO_CLICK_THRESHOLD_A)
                {
                    //Debug.Log("Elapsed more than 0.5 sec!");
                    float perc = Mathf.InverseLerp(AUTO_CLICK_THRESHOLD_A, AUTO_CLICK_THRESHOLD_B, elapsedTime);
                    clickPerSecond = Mathf.Lerp(CPS_MIN, CPS_MAX, perc);
                    timeOut = 1.0F / clickPerSecond;

                    if (timer < timeOut)
                        timer += Time.unscaledDeltaTime;
                    else
                    {
                        onClick?.Invoke();
                        timer = 0.0F;
                    }

                    //Debug.Log($"{buttonClickTime} , {perc} , {clickPerSecond} , {timeOut} , {timer}");
                }
                //else
                //Debug.Log("Elapsed less than 0.5 sec!");

                yield return null;
            }
        }
    }

    public void ClearButtonPressedCoroutine()
    {
        if (m_ButtonPressedCoroutine != null)
        {   
            StopCoroutine(m_ButtonPressedCoroutine);
            m_ButtonPressedCoroutine = null;
        }
    }
}
