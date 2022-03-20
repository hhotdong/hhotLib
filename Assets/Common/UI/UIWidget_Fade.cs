//using System;
//using UnityEngine;
//using DG.Tweening;

//public class UIWidget_Fade : MonoBehaviour
//{
//    private CanvasGroup canvasGroup;
//    [SerializeField] protected float duration = 0.2F;
//    [SerializeField] protected Ease ease = Ease.OutSine;

//    public void Animate(bool isOn, Action onStart = null, Action onEnd = null)
//    {
//        if (!canvasGroup)
//        {
//            canvasGroup = GetComponent<CanvasGroup>();
//            if (!canvasGroup)
//                canvasGroup = gameObject.AddComponent<CanvasGroup>();
//        }

//        if (DOTween.IsTweening(canvasGroup))
//            DOTween.Kill(canvasGroup);

//        float endVal = isOn ? 1.0F : 0.0F;

//        canvasGroup
//            .DOFade(endVal, duration)
//            .OnStart(DoStart)
//            .OnComplete(DoComplete)
//            .SetEase(ease)
//            .SetUpdate(true);


//        void DoStart()
//        {
//            gameObject.SetActive(true);
//            onStart?.Invoke();
//        }

//        void DoComplete()
//        {
//            onEnd?.Invoke();
//            canvasGroup.alpha = endVal;
//            if(!isOn)
//                gameObject.SetActive(false);
//        }
//    }
//}
