using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace hhotLib.Common
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        public static Action<string> StartLoadingEvent;
        public static Action<float>  LoadingEvent;
        public static Action<string> CompleteLoadingEvent;

        public bool IsLoading { get; private set; }

        public void Load(string sceneName, bool showLoadingScreen, bool activate = false, string unloadSceneName = "")
        {
            Scene sceneToLoad = SceneManager.GetSceneByName(sceneName);
            if (sceneToLoad.IsValid() && sceneToLoad.isLoaded)
            {
                Debug.LogWarning($"Requested scene({sceneName}) has already been loaded!");
                return;
            }

            if (IsLoading)
            {
                Debug.LogWarning("Scene is already loading!");
                return;
            }
            IsLoading = true;

            StopAllCoroutines();
            StartCoroutine(LoadCoroutine(sceneName, showLoadingScreen, activate, unloadSceneName));
        }

        private IEnumerator LoadCoroutine(string sceneName, bool showLoadingScreen, bool activate, string unloadSceneName)
        {
            if (showLoadingScreen)
            {
                var asyncLoading = SceneManager.LoadSceneAsync(SceneName.LOADING, LoadSceneMode.Additive);
                yield return new WaitUntil(() => asyncLoading.isDone);

                StartLoadingEvent?.Invoke(sceneName);

                // Wait until loading window is opened completely.
                yield return new WaitUntil(() => QueryManager.Query<QueryLoadingWindowVisible, bool>(new QueryLoadingWindowVisible(true)));
            }

            if (string.IsNullOrEmpty(unloadSceneName) == false)  // If there exists scene to unload, do that first.
            {
                var unloadedScene = SceneManager.GetSceneByName(unloadSceneName);
                if (unloadedScene.IsValid() && unloadedScene.isLoaded)
                {
                    var asyncUnloading = SceneManager.UnloadSceneAsync(unloadSceneName);
                    yield return new WaitUntil(() => asyncUnloading.isDone);
                }
            }

            var asyncLoadingNext = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return new WaitForSeconds(1.0f);
            asyncLoadingNext.allowSceneActivation = false;

            while (asyncLoadingNext.progress < 0.9f)
            {
                LoadingEvent?.Invoke(asyncLoadingNext.progress);
                yield return null;
            }
            asyncLoadingNext.allowSceneActivation = true;

            if (activate)
            {
                Scene nextScene = default;
                yield return new WaitUntil(() => {
                        nextScene = SceneManager.GetSceneByName(sceneName);
                        return nextScene.IsValid() && nextScene.isLoaded;
                });
                SceneManager.SetActiveScene(nextScene);
            }

            CompleteLoadingEvent?.Invoke(sceneName);

            if (showLoadingScreen)
            {
                yield return new WaitUntil(() => QueryManager.Query<QueryLoadingWindowVisible, bool>(new QueryLoadingWindowVisible(false)));
                var asyncUnloading = SceneManager.UnloadSceneAsync(SceneName.LOADING);
                yield return new WaitUntil(() => asyncUnloading.isDone);
            }

            IsLoading = false;
        }
    }
}