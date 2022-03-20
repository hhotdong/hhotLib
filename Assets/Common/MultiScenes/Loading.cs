using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace hhotLib.Common
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Loading : MonoBehaviour
    {
        private CanvasGroup cg;
        private float m_LoadingProgress;
        private bool m_IsLoading;
        
        [SerializeField] private Slider m_ProgressBar;
        [SerializeField] private TextMeshProUGUI m_ProgressText;

        private readonly float FADE_DURATION = 0.5f;

        private void Awake()
        {
            cg = GetComponent<CanvasGroup>();
            m_IsLoading = false;
            SceneLoader.OnStartLoading += StartLoading;
        }

        private void OnDestroy()
        {
            m_IsLoading = false;
            SceneLoader.OnStartLoading -= StartLoading;
        }

        private void StartLoading(string sceneName, Action callback)
        {
            if (m_IsLoading)
                return;
            m_IsLoading = true;

            StartCoroutine(Load());

            IEnumerator Load()
            {
                WillLoading();
                yield return new WaitForSeconds(0.2f);  // async.allowSceneActivation 의 정상 작동을 위한 딜레이
                yield return cg.DOFade(1.0f, FADE_DURATION).WaitForCompletion();

                AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                async.allowSceneActivation = false;

                while (async.progress < 0.9f)
                {
                    LoadingProgress();
                    yield return null;
                }

                DidLoading();
                yield return cg.DOFade(0.0f, FADE_DURATION).WaitForCompletion();
                callback?.Invoke();

                void WillLoading()
                {
                    cg.interactable = true;
                    cg.alpha = 0.0f;
                    m_LoadingProgress = 0.0f;
                    m_ProgressBar.value = 0.0f;
                    m_ProgressText.text = "0%";
                }

                void LoadingProgress()
                {
                    m_LoadingProgress = Mathf.Clamp01(async.progress / 0.9f);
                    m_ProgressBar.value = m_LoadingProgress;
                    m_ProgressText.text = Mathf.Round(m_LoadingProgress * 100.0f).ToString("{0}%");
                    UnityEngine.Debug.Log($"Loading next scene({sceneName})...{m_LoadingProgress}%");
                }

                void DidLoading()
                {
                    m_LoadingProgress = 1.0f;
                    m_ProgressBar.value = 1.0f;
                    m_ProgressText.text = "100%";
                    async.allowSceneActivation = true;
                    m_IsLoading = false;
                }
            }
        }
    }
}
