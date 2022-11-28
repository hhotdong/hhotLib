using System;
using UnityEngine;

namespace hhotLib
{
    public class CheckNetwork
    {
        public bool IsConnected { get; private set; }

        public event Action<bool> ChangeNetworkStatusEvent;

        private float checkInterval;
        private float threshold;

        public CheckNetwork(float interval)
        {
            checkInterval            = interval;
            threshold                = Time.time + interval;
            IsConnected              = false;
            ChangeNetworkStatusEvent = null;
        }

        public void Update()
        {
            if (Time.time >= threshold)
            {
                threshold = Time.time + checkInterval;

                bool connected = Application.internetReachability != NetworkReachability.NotReachable;
                if (IsConnected != connected)
                {
                    Debug.Log("Network status changed: " + connected);
                    IsConnected = connected;
                    ChangeNetworkStatusEvent?.Invoke(connected);
                }
            }
        }
    }
}