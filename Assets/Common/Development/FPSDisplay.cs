using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    [Range(30, 60)] public int targetFrameRate = 60;

    private float deltaTime;

    private void Start()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || DEBUG
        Application.targetFrameRate = targetFrameRate;
#else
        Destroy(this.gameObject);
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || DEBUG
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
#endif
    }

    private void OnGUI()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || DEBUG
        int  w          = Screen.width;
        int  h          = Screen.height;
        Rect rect       = new Rect(0, 0, w, h * 2 / 100);
        var  style      = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize  = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float fps       = 1.0f / deltaTime;
        float msec      = deltaTime * 1000.0f;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
#endif
    }
}
