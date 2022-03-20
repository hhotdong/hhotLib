using System;
using System.Collections;
using UnityEngine;
using hhotLib.Save;
using hhotLib.Common;

namespace hhotLib
{
    public partial class GameManager : Singleton<GameManager>, ISavable
    {
        public static bool IsInitialized { get; private set; }

        public static event Action WillCompleteInitialize;
        public static event Action DidCompleteInitialize;

        public static float LoadingProgress;

        [Header("Network"), Space(5)]
        public bool CheckNetworkPeriodically;
        private CheckNetwork checkNetwork;

        protected override void OnAwake()
        {
            if (CheckNetworkPeriodically)
                checkNetwork = new CheckNetwork(1.0F);
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
#endif
            {
                Register();
                OnLoad();
            }
        }

        private void OnDisable()
        {
            Unregister();
            OnSave();
        }

        protected override void OnStart()
        {
            if (IsInitialized)
                return;

            StartCoroutine(Initialize());

            IEnumerator Initialize()
            {
                LoadingProgress = 0.0f;

                yield return null;

                WillCompleteInitialize?.Invoke();

                IsInitialized = true;
            }
        }

        private void Update()
        {
            if (CheckNetworkPeriodically) checkNetwork.Update();
        }

        private void OnApplicationPause(bool pause)
        {
            UnityEngine.Debug.Log($"OnApplicationPause : {pause}");
            if (pause)
            {
                Time.timeScale = 0.0F;
                if (IsInitialized)
                    SaveLoadSystem.Save();
            }
            else
            {
                Time.timeScale = 1.0F;
            }
        }

        private void OnApplicationQuit()
        {
            UnityEngine.Debug.Log($"OnApplicationQuit");
            if (IsInitialized)
                SaveLoadSystem.Save();
        }
    }
}