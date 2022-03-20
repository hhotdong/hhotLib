using System;
using System.Collections;
using UnityEngine;

public class CaptureScreen : MonoBehaviour
{
    private bool isCapturing = false;
    private Texture2D screenCaptured = null;

    public static event Action WillCaptureScreen;
    public static event Action DidCaptureScreen;

    private void OnEnable()
    {
        isCapturing = false;
        screenCaptured = null;
    }

    private void OnDisable()
    {
        isCapturing = false;
        screenCaptured = null;
    }

    public void Capture()
    {
        if (isCapturing) return;
        isCapturing = true;
        StartCoroutine(CaptureProcess());

        IEnumerator CaptureProcess()
        {
            //Disable unwanted visible cameras
            //UICam.enabled = false;
            //WorldUICam.enabled = false;

            yield return new WaitForEndOfFrame();

            WillCaptureScreen?.Invoke();

            screenCaptured = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenCaptured.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenCaptured.Apply();

            DidCaptureScreen?.Invoke();
            isCapturing = false;

            //Restore
            //UICam.enabled = true;
            //WorldUICam.enabled = true;
        }
    }

    public void Save()
    {
        if (screenCaptured == null)
        {
            Debug.Log("Failed to SaveTextureToFile because screencap is null!");
            return;
        }

        byte[] bytes = screenCaptured.EncodeToPNG();
        string fileName = string.Format("img_{0}.png", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        NativeGallery.SaveImageToGallery(bytes, "Picture", fileName);
        screenCaptured = null;
    }
}
