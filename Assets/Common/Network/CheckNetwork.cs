using System;
using UnityEngine;

namespace hhotLib.Common
{
    public class CheckNetwork
    {
        public bool IsConnected { get; private set; }

        public event Action<bool> NetworkStatusChangedEvent;

        private float checkInterval;
        private float threshold;

        public CheckNetwork(float interval)
        {
            checkInterval             = interval;
            threshold                 = Time.time + interval;
            IsConnected               = false;
            NetworkStatusChangedEvent = null;
        }

        public void UpdateStatus()
        {
            if (Time.time < threshold)
                return;

            threshold = Time.time + checkInterval;

            bool connected = Application.internetReachability != NetworkReachability.NotReachable;
            if (IsConnected != connected)
            {
                Debug.Log($"Network status changed: {connected}");
                IsConnected = connected;
                NetworkStatusChangedEvent?.Invoke(connected);
            }
        }
    }
}