using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace deVoid.UIFramework {
    /// <summary>
    /// This is a "helper" layer so Windows with higher priority can be displayed.
    /// By default, it contains any window tagged as a Popup. It is controlled by the WindowUILayer.
    /// </summary>
    public class WindowParaLayer : MonoBehaviour {
        private bool IsVisible => visibleState == VisibleState.IsAppearing || visibleState == VisibleState.IsAppeared;

        [SerializeField]
        private GameObject darkenBgObject;

        [SerializeField, Range(0.0f, 5.0f)]
        private float fadeDuration;

        [SerializeField]
        private Ease fadeEase = Ease.OutQuad;

        private CanvasGroup bgCanvasGroup;
        private VisibleState visibleState = VisibleState.IsDisappeared;

        private readonly List<GameObject> containedScreens = new List<GameObject>();

        public void AddScreen(Transform screenRectTransform) {
            screenRectTransform.SetParent(transform, false);
            screenRectTransform.SetAsLastSibling();
            containedScreens.Add(screenRectTransform.gameObject);

            if (screenRectTransform.TryGetComponent(out IWindowController window) && window.IsPopup) {
                window.InTransitionStarted  += (IUIScreenController screen) => DarkenBG(true);
                window.OutTransitionStarted += (IUIScreenController screen) => DarkenBG(false);
            }
        }

        public void DarkenBG(bool darken) {
            if (darkenBgObject.activeSelf == false) {
                darkenBgObject.SetActive(true);
            }

            if (bgCanvasGroup == null) {
                return;
            }

            if ((darken && IsVisible) || (darken == false && IsVisible == false)) {
                return;
            }

            float endVal = darken ? 1.0f : 0.0f;
            float duration = fadeDuration * Mathf.Abs(endVal - bgCanvasGroup.alpha);

            visibleState = darken ? VisibleState.IsAppearing : VisibleState.IsDisappearing;

            if (DOTween.IsTweening(bgCanvasGroup)) {
                bgCanvasGroup.DOKill();
            }

            bgCanvasGroup.DOFade(endVal, duration)
                .SetEase(fadeEase)
                .OnComplete(() => {
                    if (darken == false) {
                        darkenBgObject.SetActive(false);
                    }
                    visibleState = darken ? VisibleState.IsAppeared : VisibleState.IsDisappeared;
                })
                .Play();
        }

        private void Awake()
        {
            bgCanvasGroup = darkenBgObject.GetComponent<CanvasGroup>();
            if (bgCanvasGroup == null) {
                bgCanvasGroup = darkenBgObject.AddComponent<CanvasGroup>();
            }
            bgCanvasGroup.alpha = 0.0f;
        }

        private void OnDisable()
        {
            if (bgCanvasGroup != null) {
                if (DOTween.IsTweening(bgCanvasGroup)) {
                    bgCanvasGroup.DOKill();
                }
            }
        }

        private void OnDestroy()
        {
            bgCanvasGroup = null;
        }
    }
}
