using System;
using System.Collections;
using UnityEngine;
using hhotLib.Common;

namespace hhotLib
{
    public class GameManager : Singleton<GameManager>
    {
        public static bool IsInitialized { get; private set; }

        public static float LoadingProgress;

        public static event Action WillCompleteInitialize;
        public static event Action DidCompleteInitialize;

        [SerializeField] private bool checkNetworkPeriodically;

        private CheckNetwork checkNetwork;

        protected override void OnAwake()
        {
            if (checkNetworkPeriodically)
                checkNetwork = new CheckNetwork(1.0F);
        }

        protected override void OnStart()
        {
            if (IsInitialized)
                return;

            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            LoadingProgress = 0.0f;

            yield return null;

            WillCompleteInitialize?.Invoke();

            DidCompleteInitialize? .Invoke();

            LoadingProgress = 1.0f;

            IsInitialized = true;
        }

        private void Update()
        {
            if (checkNetworkPeriodically)
                checkNetwork.Update();
        }

        private void OnApplicationPause(bool pause)
        {
            UnityEngine.Debug.Log($"OnApplicationPause : {pause}");

            if (pause)
                Time.timeScale = 0.0F;
            else
                Time.timeScale = 1.0F;
        }

        private void OnApplicationQuit()
        {
            UnityEngine.Debug.Log($"OnApplicationQuit");
        }
    }
}