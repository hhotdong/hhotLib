using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace hhotLib.Common
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Splash : MonoBehaviour
    {
        private CanvasGroup cg;

        [SerializeField] private string m_NextSceneName = "MainMenu";

        private readonly float FADE_DURATION = 0.5f;
        private readonly float LOGO_INTERVAL = 1.5f;

        private void Awake()
        {
            cg = GetComponent<CanvasGroup>();
            cg.alpha = 0.0f;
        }

        private void OnDestroy()
        {
            cg = null;
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.2f);  // async.allowSceneActivation 의 정상 작동을 위한 딜레이

            AsyncOperation async = SceneManager.LoadSceneAsync(m_NextSceneName, LoadSceneMode.Single);
            async.allowSceneActivation = false;

            // Show logo
            Sequence seq = DOTween.Sequence()
                        .Append(cg.DOFade(1.0f, FADE_DURATION))
                        .AppendInterval(LOGO_INTERVAL)
                        .Append(cg.DOFade(0.0f, FADE_DURATION))
                        .OnComplete(DoComplete)
                        .Play();

            yield return seq.WaitForCompletion();

            while (async.progress < 0.9f)
            {
                var progressPerc = Mathf.Round(Mathf.Clamp01(async.progress / 0.9f) * 100.0f);
                UnityEngine.Debug.Log($"Loading next scene({m_NextSceneName})...{progressPerc}%");
                yield return null;
            }

            async.allowSceneActivation = true;

            void DoComplete() => Debug.Log("Splash complete");
        }
    }
}