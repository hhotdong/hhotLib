using System;
using UnityEngine;
using DG.Tweening;

namespace hhotLib.Common
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UIWidget_Fade : MonoBehaviour
    {
        private CanvasGroup cg;

        public void Animate(bool animIn)
        {
            Animate(animIn, 0.5f, Ease.OutSine, null, null);
        }

        public void Animate(bool animIn, Action onStart, Action onEnd)
        {
            Animate(animIn, 0.5f, Ease.OutSine, onStart, onEnd);
        }

        public void Animate(bool animIn, float duration, Ease ease, Action onStart, Action onEnd)
        {
            if (DOTween.IsTweening(cg))
                DOTween.Kill(cg);

            float endVal = animIn ? 1.0f : 0.0f;

            if (gameObject.activeSelf == false)
                gameObject.SetActive(true);

            cg.DOFade(endVal, duration)
                .OnStart(() => {
                    onStart?.Invoke();
                })
                .OnComplete(() => {
                    cg.alpha = endVal;
                    if (animIn == false)
                        gameObject.SetActive(false);
                    onEnd?.Invoke();
                })
                .SetEase(ease)
                .SetUpdate(true)
                .Play();
        }

        private void Awake()
        {
            cg = GetComponent<CanvasGroup>();
        }
    }
}