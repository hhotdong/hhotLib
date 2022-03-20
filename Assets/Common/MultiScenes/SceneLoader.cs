using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace hhotLib.Common
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        public static bool IsLoading { get; private set; }

        public static Action<string, Action> OnStartLoading;

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
            if (sceneToLoad.isLoaded)
            {
                Debug.LogWarning($"Requested scene({sceneName}) has already been loaded!");
                return;
            }

            if (IsLoading)
            {
                Debug.LogWarning("It's already loading scene now!");
                return;
            }
            IsLoading = true;

            if (showLoadingScreen)
            {
                SceneManager.LoadSceneAsync(SceneName.LOADING, LoadSceneMode.Additive).completed += s =>
                {
                    OnStartLoading?.Invoke(sceneName, OnComplete);
                };
            }
            else
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).completed += s =>
                {
                    OnComplete();
                };
            }

            void OnComplete()
            {
                if (showLoadingScreen)
                    SceneManager.UnloadSceneAsync(SceneName.LOADING);

                if (activate)
                    StartCoroutine(Activate());

                if (!string.IsNullOrEmpty(unloadSceneName))
                    SceneManager.UnloadSceneAsync(unloadSceneName);

                IsLoading = false;
            }

            IEnumerator Activate()
            {
                yield return null;
                SceneManager.SetActiveScene(sceneToLoad);
            }
        }
    }
}