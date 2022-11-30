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

        [SerializeField] private string nextSceneName = "MainMenu";

        private void Awake()
        {
            cg = GetComponent<CanvasGroup>();
            cg.alpha = 0.0f;
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.2f);  // async.allowSceneActivation 정상 작동을 위한 딜레이

            AsyncOperation async = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
            async.allowSceneActivation = false;

            const float FADE_DURATION = 0.5f;
            const float WAIT_DURATION = 1.5f;

            Sequence seq = DOTween.Sequence()
                .Append(cg.DOFade(1.0f, FADE_DURATION))
                .AppendInterval(WAIT_DURATION)
                .Append(cg.DOFade(0.0f, FADE_DURATION))
                .OnComplete(() => {
                    Debug.Log("Splash complete.");
                }).Play();

            yield return seq.WaitForCompletion();

            while (async.progress < 0.9f)
            {
                var progressPerc = Mathf.Round(Mathf.Clamp01(async.progress / 0.9f) * 100.0f);
                UnityEngine.Debug.Log($"Loading next scene({nextSceneName})...{progressPerc}%");
                yield return null;
            }

            async.allowSceneActivation = true;
        }
    }
}