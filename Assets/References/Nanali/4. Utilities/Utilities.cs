using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nanali
{
    public static class Utilities
    {
        //네트워크 상태 체크.
        public static bool IsConnectedInternet
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        public static string RandomNickname()
        {
            string randomTag = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Range(0, 10).Select(x => randomTag[Mathf.Clamp(Random.Range(0, randomTag.Length), 0, randomTag.Length)]).ToArray());
        }

        public static Texture2D GetScreenShot(Camera cam, int targetLayers, int width, int height, Texture2D waterMark = null)
        {
            Texture2D _tex;
            //capture.
            cam.cullingMask = targetLayers;

            RenderTexture rt = new RenderTexture(width, height, 24);
            cam.targetTexture = rt;
            _tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            cam.Render();
            RenderTexture.active = rt;
            _tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            //destroy.
            cam.targetTexture = null;
            RenderTexture.active = null;
            UnityEngine.Object.Destroy(rt);

            Texture2D myTexture = ScaleTexture(_tex, width, height);

            AddWatermark(myTexture, waterMark);

            return myTexture;
        }

        static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
            Color[] rpixels = result.GetPixels(0);
            float incX = (1.0f / targetWidth);
            float incY = (1.0f / targetHeight);
            for (int px = 0; px < rpixels.Length; px++)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * Mathf.Floor(px / targetWidth));
            }
            result.SetPixels(rpixels, 0);
            result.Apply();
            return result;
        }

        static Texture2D AddWatermark(Texture2D target, Texture2D waterMark)
        {
            if (waterMark != null)
            {
                int startX = target.width - waterMark.width;

                for (int x = startX; x < target.width; x++)
                {

                    for (int y = 0; y < target.height; y++)
                    {
                        Color bgColor = target.GetPixel(x, y);
                        Color wmColor = waterMark.GetPixel(x - startX, y);

                        Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                        target.SetPixel(x, y, final_color);
                    }
                }

                target.Apply();
            }

            return target;
        }
    }

    //리스트 직렬화.
    [Serializable]
    public class ListSerialization<T>
    {
        [SerializeField]
        List<T> target;
        public List<T> ToList() { return target; }

        public ListSerialization(List<T> _target)
        {
            target = _target;
        }
    }
}
