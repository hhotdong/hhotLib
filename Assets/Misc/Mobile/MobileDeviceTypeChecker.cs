//https://forum.unity.com/threads/detecting-between-a-tablet-and-mobile.367274/#post-6719620

using UnityEngine;

namespace hhotLib
{
    public enum MobileDeviceType { None, Tablet, Phone }

    public static class MobileDeviceTypeChecker
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        Debug.Log($"Mobile Device type is {DeviceType = GetDeviceType()}");
#endif
        }

        public static MobileDeviceType DeviceType { get; private set; } = MobileDeviceType.None;

        private static MobileDeviceType GetDeviceType()
        {
#if !UNITY_EDITOR && UNITY_IOS
        bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
        if (deviceIsIpad) return MobileDeviceType.Tablet;

        bool deviceIsIphone = UnityEngine.iOS.Device.generation.ToString().Contains("iPhone");
        if (deviceIsIphone) return MobileDeviceType.Phone;

        return MobileDeviceType.None;
#elif !UNITY_EDITOR && UNITY_ANDROID
        float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        bool isTablet = (DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2f);
 
        if (isTablet) return MobileDeviceType.Tablet;

        return MobileDeviceType.Phone;
#else
            return MobileDeviceType.Phone;
#endif
        }

        private static float DeviceDiagonalSizeInInches()
        {
            float screenWidth = Screen.width / Screen.dpi;
            float screenHeight = Screen.height / Screen.dpi;
            float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));

            return diagonalInches;
        }
    }
}