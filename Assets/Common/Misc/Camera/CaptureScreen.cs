using System;
using System.Collections;
using UnityEngine;

public class CaptureScreen : MonoBehaviour
{
    private bool      isCapturing;
    private Texture2D screenCaptured;

    public static event Action WillCaptureScreen;
    public static event Action DidCaptureScreen;

    public void Capture()
    {
        if (isCapturing)
            return;
        isCapturing = true;

        StopAllCoroutines();
        StartCoroutine(CaptureCoroutine());
    }

    public void Dispose()
    {
        if (screenCaptured != null)
        {
            Destroy(screenCaptured);
            screenCaptured = null;
        }
    }

    public void Save()
    {
        if (screenCaptured == null)
        {
            Debug.LogWarning("Screen captured texture is null!");
            return;
        }

        byte[] bytes    = screenCaptured.EncodeToPNG();
        string fileName = string.Format("img_{0}.png", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        //NativeGallery.SaveImageToGallery(bytes, "Picture", fileName);

        Dispose();
    }

    private IEnumerator CaptureCoroutine()
    {
        // Disable unwanted rendering cameras here.

        yield return new WaitForEndOfFrame();

        WillCaptureScreen?.Invoke();

        screenCaptured = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenCaptured.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenCaptured.Apply();

        DidCaptureScreen?.Invoke();
        isCapturing = false;

        // Restore disabled cameras here.
    }

    private void OnDisable()
    {
        Dispose();
    }
}