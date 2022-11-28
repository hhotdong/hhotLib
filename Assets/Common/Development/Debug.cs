using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngineInternal;
/// 
/// It overrides UnityEngine.Debug to mute debug messages completely on a platform-specific basis.
/// 
/// Putting this inside of 'Plugins' foloder is ok.
/// 
/// Important:
///     Other preprocessor directives than 'UNITY_EDITOR' does not correctly work.
/// 
/// Note:
///     [Conditional] attribute indicates to compilers that a method call or attribute should be 
///     ignored unless a specified conditional compilation symbol is defined.
/// 
/// See Also: 
///     http://msdn.microsoft.com/en-us/library/system.diagnostics.conditionalattribute.aspx
/// 
/// 2012.11. @kimsama
/// 
public static class Debug
{
    public static bool isDebugBuild
    {
        get { return UnityEngine.Debug.isDebugBuild; }
    }

    private readonly static StringBuilder sb = new StringBuilder();

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object message, string tag = "")
    {
        if (string.IsNullOrEmpty(tag))
            tag = DebugTagConstant.Default;

        if (DebugSettings.Instance.CheckIfDebugTagValid(tag))
        {
            sb.Append($"[{tag}] ");
            sb.Append(message.ToString());
            UnityEngine.Debug.Log(sb.ToString());
            sb.Clear();
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object message, UnityEngine.Object context, string tag = "")
    {
        if (string.IsNullOrEmpty(tag))
            tag = DebugTagConstant.Default;

        if (DebugSettings.Instance.CheckIfDebugTagValid(tag))
        {
            sb.Append($"[{tag}] ");
            sb.Append(message.ToString());
            UnityEngine.Debug.Log(sb.ToString(), context);
            sb.Clear();
        }
    }

    //[System.Diagnostics.Conditional("UNITY_EDITOR")]  // Make error messages to be displayed on any platforms.
    public static void LogError(object message, string tag = "")
    {
        if (string.IsNullOrEmpty(tag))
            tag = DebugTagConstant.Default;

        sb.Append($"[{tag}] ");
        sb.Append(message.ToString());
        UnityEngine.Debug.LogError(sb.ToString());
        sb.Clear();
    }

    //[System.Diagnostics.Conditional("UNITY_EDITOR")]  // Make error messages to be displayed on any platforms.
    public static void LogError(object message, UnityEngine.Object context, string tag = "")
    {
        if (string.IsNullOrEmpty(tag))
            tag = DebugTagConstant.Default;

        sb.Append($"[{tag}] ");
        sb.Append(message.ToString());
        UnityEngine.Debug.LogError(sb.ToString(), context);
        sb.Clear();
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message, string tag = "")
    {
        if (string.IsNullOrEmpty(tag))
            tag = DebugTagConstant.Default;

        if (DebugSettings.Instance.CheckIfDebugTagValid(tag))
        {
            sb.Append($"[{tag}] ");
            sb.Append(message.ToString());
            UnityEngine.Debug.LogWarning(sb.ToString());
            sb.Clear();
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message, UnityEngine.Object context, string tag = "")
    {
        if (string.IsNullOrEmpty(tag))
            tag = DebugTagConstant.Default;

        if (DebugSettings.Instance.CheckIfDebugTagValid(tag))
        {
            sb.Append($"[{tag}] ");
            sb.Append(message.ToString());
            UnityEngine.Debug.LogWarning(sb.ToString(), context);
            sb.Clear();
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition)
    {
        if (!condition) throw new Exception();
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogException(Exception e)
    {
        UnityEngine.Debug.LogException(e);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogException(Exception e, UnityEngine.Object obj)
    {
        UnityEngine.Debug.LogException(e, obj);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarningFormat(string format, params object[] objs)
    {
        UnityEngine.Debug.LogWarningFormat(format, objs);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogFormat(string format, params object[] objs)
    {
        UnityEngine.Debug.LogFormat(format, objs);
    }

    //[System.Diagnostics.Conditional("UNITY_EDITOR")]  // Make error messages to be displayed on any platforms.
    public static void LogErrorFormat(string format, params object[] objs)
    {
        UnityEngine.Debug.LogErrorFormat(format, objs);
    }
}