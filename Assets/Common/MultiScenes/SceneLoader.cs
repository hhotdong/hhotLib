using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace hhotLib.Common
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        public bool IsLoading { get; private set; }

        public static Action<string> LoadingStartedEvent;
        public static Action<float> LoadingEvent;
        public static Action<string> LoadingCompletedEvent;

        protected override void OnAwake()
        {
            IsLoading = false;
        }

        protected override void OnDestroySingleton()
        {
            IsLoading = false;
        }

        public void Load(string sceneName, bool showLoadingScreen, bool activate = false, string unloadSceneName = "")
        {
            var sceneToLoad = SceneManager.GetSceneByName(sceneName);
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

            StartCoroutine(LoadProcess());

            IEnumerator LoadProcess()
            {
                if (showLoadingScreen)
                {
                    var asyncLoading = SceneManager.LoadSceneAsync(SceneName.LOADING, LoadSceneMode.Additive);
                    yield return new WaitUntil(() => asyncLoading.isDone);
                    LoadingStartedEvent?.Invoke(sceneName);
                    yield return new WaitUntil(
                        () => QueryManager.Query<CheckLoadingWindowVisibleRequest, bool>(new CheckLoadingWindowVisibleRequest(true)));
                }
                
                if (!string.IsNullOrEmpty(unloadSceneName))  // 언로드돼야 하는 씬이 있다면 우선 처리한다.
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
                    Scene scene = default;
                    yield return new WaitUntil(() =>
                    {
                        scene = SceneManager.GetSceneByName(sceneName);
                        return scene.IsValid() && scene.isLoaded;
                    });
                    SceneManager.SetActiveScene(scene);
                }

                LoadingCompletedEvent?.Invoke(sceneName);

                if (showLoadingScreen)
                {
                    yield return new WaitUntil(
                        () => QueryManager.Query<CheckLoadingWindowVisibleRequest, bool>(new CheckLoadingWindowVisibleRequest(false)));
                    UnityEditor.EditorApplication.isPaused = true;
                    SceneManager.UnloadSceneAsync(SceneName.LOADING);
                }
                IsLoading = false;
            }
        }
    }
}