// Credit: https://forum.unity.com/threads/detecting-between-a-tablet-and-mobile.367274/#post-6719620
using UnityEngine;

namespace hhotLib.Common
{
    public enum MobileDeviceType {
        None, Tablet, Phone
    }

    public static class MobileDeviceTypeChecker
    {
        public static MobileDeviceType DeviceType { get; private set; } = MobileDeviceType.None;

        public static MobileDeviceType GetDeviceType()
        {
#if !UNITY_EDITOR && UNITY_IOS
            bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
            if (deviceIsIpad)
                return MobileDeviceType.Tablet;

            bool deviceIsIphone = UnityEngine.iOS.Device.generation.ToString().Contains("iPhone");
            if (deviceIsIphone)
                return MobileDeviceType.Phone;

            return MobileDeviceType.None;
#elif !UNITY_EDITOR && UNITY_ANDROID
            float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
            bool  isTablet    = (DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2.0f);
 
            if (isTablet)
                return MobileDeviceType.Tablet;

            return MobileDeviceType.Phone;
#else
            return MobileDeviceType.Phone;
#endif
        }

        private static float DeviceDiagonalSizeInInches()
        {
            float screenWidth    = Screen.width  / Screen.dpi;
            float screenHeight   = Screen.height / Screen.dpi;
            float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
            return diagonalInches;
        }
    }
}