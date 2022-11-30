using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace hhotLib.Common
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Loading : MonoBehaviour
    {
        [SerializeField] private Slider          progressBar;
        [SerializeField] private TextMeshProUGUI progressText;

        private CanvasGroup cg;

        private void Awake()
        {
            cg = GetComponent<CanvasGroup>();
            cg.alpha = 0.0f;
            SceneLoader.StartLoadingEvent    += OnStartLoading;
            SceneLoader.LoadingEvent         += OnLoading;
            SceneLoader.CompleteLoadingEvent += OnCompleteLoading;
            QueryManager.RegisterProvider<QueryLoadingWindowVisible, bool>(IsVisible);
        }

        private bool IsVisible(QueryLoadingWindowVisible request)
        {
            if (request.isVisible)
                return cg.enabled && cg.alpha > 0.9999f;
            else
                return DOTween.IsTweening(cg) == false && cg.alpha < 0.0001f;
        }

        private void OnDestroy()
        {
            SceneLoader.StartLoadingEvent    -= OnStartLoading;
            SceneLoader.LoadingEvent         -= OnLoading;
            SceneLoader.CompleteLoadingEvent -= OnCompleteLoading;
        }

        private void OnStartLoading(string sceneName)
        {
            if (DOTween.IsTweening(cg))
                cg.DOKill();

            cg.DOFade(1.0f, 1.0f)
                .OnStart(() => {
                    cg.alpha          = 0.0f;
                    progressBar.value = 0.0f;
                    progressText.text = "0%";
                }).Play();
        }

        private void OnLoading(float progress)
        {
            progress          = Mathf.Clamp01(progress / 0.9f);
            progressBar.value = progress;
            progressText.text = Mathf.Round(progress * 100.0f).ToString("{0}%");
            UnityEngine.Debug.Log($"Loading scene...{progress * 100.0f}%");
        }

        private void OnCompleteLoading(string sceneName)
        {
            if (DOTween.IsTweening(cg))
                cg.DOKill();

            cg.DOFade(0.0f, 1.0f)
                .OnStart(() => {
                    progressBar.value = 1.0f;
                    progressText.text = "100%";
                })
                .OnComplete(() => {
                    cg.alpha = 0.0f;
                }).Play();
        }
    }
}
