using System;
using UnityEngine;
using DG.Tweening;

namespace hhotLib.Common
{
    public sealed class UIWidget_Slide : MonoBehaviour
    {
        private enum Position {
            Left, Right, Top, Bottom,
        }

        [SerializeField] private Position origin = Position.Left;

        private RectTransform rt;

        public void Animate(bool animIn)
        {
            Animate(animIn, 0.5f, Ease.Linear, null, null);
        }

        public void Animate(bool animIn, Action onStart, Action onEnd)
        {
            Animate(animIn, 0.5f, Ease.Linear, onStart, onEnd);
        }

        public void Animate(bool animIn, float duration, Ease ease, Action onStart, Action onEnd)
        {
            Vector3 startPosition = default;
            switch (origin)
            {
                default:
                    Debug.LogWarning($"Invalid origin({origin})!");
                    return;

                case Position.Left:
                    startPosition = new Vector3(-rt.rect.width, 0.0f);
                    rt.pivot = new Vector2(1.0f, rt.pivot.y);
                    break;

                case Position.Right:
                    startPosition = new Vector3(rt.rect.width, 0.0f);
                    rt.pivot = new Vector2(0.0f, rt.pivot.y);
                    break;

                case Position.Top:
                    startPosition = new Vector3(0.0f, rt.rect.height);
                    rt.pivot = new Vector2(rt.pivot.x, 1.0f);
                    break;

                case Position.Bottom:
                    startPosition = new Vector3(0.0f, -rt.rect.height);
                    rt.pivot = new Vector2(rt.pivot.x, 0.0f);
                    break;
            }

            rt.anchoredPosition = animIn ? startPosition : Vector3.zero;

            if (DOTween.IsTweening(rt))
                rt.DOKill();

            rt.DOAnchorPos(animIn ? Vector3.zero : startPosition, duration, true)
                .OnStart(() => {
                    onStart?.Invoke();
                })
                .OnComplete(() => {
                    onEnd?.Invoke();
                })
                .SetEase(ease)
                .SetUpdate(true)
                .Play();
        }

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
        }
    }
}