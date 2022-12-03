using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace hhotLib.Common
{
    public class UICoinEffectAgent : MonoBehaviour
    {
        [SerializeField] private Vector2 imageScalerRange = new Vector2(0.1f, 3.0f);

        private RectTransform rt;
        private Image         coinImage;

        private void Awake()
        {
            rt        = GetComponent<RectTransform>();
            coinImage = GetComponentInChildren<Image>();
        }

        public void SetImage(Sprite coinSprite, float scaler)
        {
            coinImage.sprite = coinSprite;
            coinImage.SetNativeSize();
            coinImage.transform.localScale = Vector3.one * Mathf.Clamp(scaler, imageScalerRange.x, imageScalerRange.y);
        }

        public void Play(CoinEffectType type, float popDuration, float fadeDuration, float moveDuration, Vector2 popDistanceRange, Ease popEase, Ease moveEase, Action onEnd)
        {
            Transform destination = UICoinEffectDestination.GetDestination(type);
            if (destination == null)
            {
                Debug.LogWarning($"{nameof(UICoinEffectDestination)} not found!");
                return;
            }

            rt.anchoredPosition3D = Vector3.zero;

            var initCol     = coinImage.color;
            initCol.a       = 0.0f;
            coinImage.color = initCol;

            Vector3 popDirection = Random.insideUnitCircle;

            DOTween.Sequence()
                .Append(rt.DOMove(rt.position + popDirection * Random.Range(popDistanceRange.x, popDistanceRange.y), popDuration).SetEase(popEase))
                .Join(coinImage.DOFade(1.0F, fadeDuration))
                .Append(rt.DOMove(destination.position, moveDuration).SetEase(moveEase))
                .OnComplete(() => onEnd?.Invoke())
                .Play();
        }
    }
}