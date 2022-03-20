using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Globalization;

public static class ServerTimer
{
    private static bool isUpdating = false;
    private static bool isServerTimeValidated = false;
    public static bool IsServerTimeValidated => isServerTimeValidated;
    public static long ServerTimeStampOnStart;
    private static DateTime serverTime;
    //private static TimeSpan LocalTimeMinusServerTime;
    private static readonly float TIMEOUT = 30.0F;
    private static readonly DateTime EPOCHTIME = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToUniversalTime();
    private static readonly TimeSpan LOCALTIME_OFFSET = DateTime.Now - DateTime.UtcNow;
    private static readonly int ONE_DAY = 86400;
    private static readonly string url = "https://script.google.com/macros/s/AKfycby1X1TjWrKiQUbCyqX8uGa24UQRlzDNdpIc2K6FGxYCC5Jh_Ck/exec";

    /// <summary>
    /// Get the server time from the url.
    /// Must be called in a coroutine function and with StartCorutine and yield the result so that the execution will wait for it
    /// to finish before going on to execute the rest of the code below.
    /// </summary>
    public static IEnumerator UpdateServerTime()
    {
        if (isUpdating)
        {
            Debug.Log("Already updating the server time!");
            yield break;
        }
        isUpdating = true;

        Debug.Log("Getting server time.");

        bool isServerTimeSuccessfullyRetrieved = true;
        isServerTimeValidated = false;

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        float startTime = Time.unscaledTime;
        while (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("www.isNetworkError!");
            if ((Time.unscaledTime - TIMEOUT) > startTime)
            {
                Debug.Log("It takes too much time so that gives up updating server time!");
                isServerTimeSuccessfullyRetrieved = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        if (isServerTimeSuccessfullyRetrieved)
        {
            string time = www.downloadHandler.text;

            Debug.Log($"Retrieved time : {time}");
            DateTime result;
            if (DateTime.TryParse(time, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                Debug.Log($"Servertime result : {result}");
                serverTime = result;
            }
            else
            {
                Debug.Log($"Failed to parse time {time}. DateTime ctor is used.");
                serverTime = new DateTime();
            }

            //m_ServerTime = DateTime.Parse(time);

            //LocalTimeMinusServerTime = DateTime.Now - m_ServerTime;
            isServerTimeValidated = true;
            Debug.Log("UpdateServerTime finished successfully.");
        }
        else
        {
            Debug.Log("Failed to retrieve serveTime!");
            serverTime = new DateTime();
            //LocalTimeMinusServerTime = TimeSpan.Zero;
        }

        yield return new WaitForEndOfFrame();
    }

    public static bool TryGetServerTime(out DateTime serverTime, out long timeStamp, bool isLocalTime = false)
    {
        if (isServerTimeValidated)
        {
            Debug.Log("Succeeded to get server time.");
            serverTime = isLocalTime
                        ? ServerTimer.serverTime + LOCALTIME_OFFSET
                        : ServerTimer.serverTime;

            timeStamp = ConvertDateTimeIntoTimeStamp(serverTime);
        }
        else
        {
            Debug.Log("Failed to get server time.");
            serverTime = new DateTime();
            timeStamp = -1L;
        }

        return isServerTimeValidated;
    }

    public static int ConvertDateTimeIntoTimeStamp(DateTime dtNow)
    {
        TimeSpan span = dtNow - EPOCHTIME;
        return (int)span.TotalSeconds;
    }

    public static bool IsOneDayElapsedSince(int lastTime, out int remainingTimeInSecond)
    {
        if (TryGetServerTime(out DateTime dtNow, out long temp, false) == false)
        {
            remainingTimeInSecond = -1;
            return false;
        }

        int timeDiff = ConvertDateTimeIntoTimeStamp(dtNow) - lastTime;
        bool isElapsed = timeDiff >= ONE_DAY;
        if (isElapsed == true)
        {
            Debug.LogFormat("Elapsed : true / lastTime: {0} , currentTime: {1}", lastTime, ConvertDateTimeIntoTimeStamp(dtNow));
            remainingTimeInSecond = 0;
            return isElapsed;
        }
        else
        {
            remainingTimeInSecond = ONE_DAY - timeDiff;
            Debug.LogFormat($"Not yey elapsed / lastTime: {lastTime} , currentTime: {ConvertDateTimeIntoTimeStamp(dtNow)}, remainingTime : {remainingTimeInSecond}");
            return isElapsed;
        }
    }

    //var now = DateTime.Now.ToLocalTime();
    //var span = (now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
    //int timestamp = (int)span.TotalSeconds;

    /// <summary>
    /// Return the server time using localTime + offset. Faster than quering the web again for server time.
    /// For this to work well, must make sure GetServerTime has finished before calling this function.
    /// </summary>
    /// <param name="localTime"></param>
    /// <returns></returns>
    //public static DateTime ConvertLocalToServerTime(DateTime localTime)
    //{
    //	return localTime - LocalTimeMinusServerTime;
    //}
}