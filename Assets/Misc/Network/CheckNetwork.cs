using System;
using UnityEngine;

namespace hhotLib
{
    public class CheckNetwork
    {
        private float checkInterval;
        private float threshold;

        public bool IsConnected;

        public event Action<bool> NetworkStateChanged;

        public CheckNetwork(float checkInterval)
        {
            this.checkInterval = checkInterval;
            threshold = Time.time + checkInterval;
            IsConnected = false;
            NetworkStateChanged = null;
        }

        public void Update()
        {
            if (Time.time >= threshold)
            {
                threshold = Time.time + checkInterval;
                bool connected = Application.internetReachability != NetworkReachability.NotReachable;
                if (IsConnected != connected)
                {
                    UnityEngine.Debug.Log("Network changed : " + connected);
                    IsConnected = connected;
                    NetworkStateChanged?.Invoke(connected);
                }
            }
        }
    }
}