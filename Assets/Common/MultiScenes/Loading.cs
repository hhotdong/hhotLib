using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace hhotLib.Common
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Loading : MonoBehaviour
    {
        private CanvasGroup cg;
        
        [SerializeField] private Slider m_ProgressBar;
        [SerializeField] private TextMeshProUGUI m_ProgressText;

        private void Awake()
        {
            cg = GetComponent<CanvasGroup>();
            cg.alpha = 0.0f;
            SceneLoader.LoadingStartedEvent += OnLoadingStarted;
            SceneLoader.LoadingEvent += OnLoading;
            SceneLoader.LoadingCompletedEvent += OnLoadingCompleted;
            QueryManager.RegisterProvider<CheckLoadingWindowVisibleRequest, bool>(CheckIfVisible);
        }

        private bool CheckIfVisible(CheckLoadingWindowVisibleRequest request)
        {
            return request.CheckVisible ? cg.enabled && cg.alpha > 0.9999f : !DOTween.IsTweening(cg) && cg.alpha < 0.0001f;
        }

        private void OnDestroy()
        {
            cg = null;
            SceneLoader.LoadingStartedEvent -= OnLoadingStarted;
            SceneLoader.LoadingEvent -= OnLoading;
            SceneLoader.LoadingCompletedEvent -= OnLoadingCompleted;
        }

        private void OnLoadingStarted(string sceneName)
        {
            if (DOTween.IsTweening(cg))
                cg.DOKill();

            cg.DOFade(1.0f, 1.0f)
                .OnStart(() =>
                {
                    cg.alpha = 0.0f;
                    m_ProgressBar.value = 0.0f;
                    m_ProgressText.text = "0%";
                })
                .Play();
        }

        private void OnLoading(float progress)
        {
            progress = Mathf.Clamp01(progress / 0.9f);
            m_ProgressBar.value = progress;
            m_ProgressText.text = Mathf.Round(progress * 100.0f).ToString("{0}%");
            UnityEngine.Debug.Log($"Loading next scene...{progress * 100.0f}%");
        }

        private void OnLoadingCompleted(string sceneName)
        {
            if (DOTween.IsTweening(cg))
                cg.DOKill();

            cg.DOFade(0.0f, 1.0f)
                .OnStart(() =>
                {
                    m_ProgressBar.value = 1.0f;
                    m_ProgressText.text = "100%";
                })
                .OnComplete(() =>
                {
                    cg.alpha = 0.0f;
                })
                .Play();
        }
    }
}
