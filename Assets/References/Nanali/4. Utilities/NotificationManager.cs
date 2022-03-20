using System;
using System.Collections;
using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
public class NotificationManager : MonoBehaviour
{
    private static NotificationManager _instance;
    public static NotificationManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType(typeof(NotificationManager)) as NotificationManager;
            return _instance;
        }
    }

    const string channelID = "com.nanali.notification_channel_controller.NotiChannelCreator";
    const string smallIconID = "notify_icon_small";

    void Start()
	{
#if UNITY_ANDROID
        AndroidNotificationChannel channel = AndroidNotificationCenter.GetNotificationChannel(channelID);
        if (channel.Enabled)
            return;

        channel = new AndroidNotificationChannel()
        {
            Id = channelID,
            Name = "Forest Island",
            Importance = Importance.High,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#elif UNITY_IOS
        StartCoroutine(RequestAuthorization());
#endif
    }

#if UNITY_IOS
    //iOS 알림 권한 요청.
    IEnumerator RequestAuthorization()
    {
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            if (!req.Granted) {
                while (!req.IsFinished)
                {
                    yield return null;
                };
            }            
        }
    }
#endif

    //알림 등록.
    public void SetNotification(string title, string description, DateTime now, int seconds)
    {
        if (seconds <= 0)
            return;

#if UNITY_ANDROID
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = description;
        notification.SmallIcon = smallIconID;
        notification.FireTime = now.AddSeconds(seconds);

        AndroidNotificationCenter.SendNotification(notification, channelID);
#elif UNITY_IOS
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(0, 0, seconds),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            // You can specify a custom identifier which can be used to manage the notification later.
            // If you don't provide one, a unique string will be generated automatically.
            Identifier = channelID,
            Title = title,
            Body = description,
            Subtitle = title,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }

    public void CancelAllNotifications()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
    }
}
