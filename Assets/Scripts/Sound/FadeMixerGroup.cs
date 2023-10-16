using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace hhotLib.Common
{
    public static class FadeMixerGroup
    {
        public static IEnumerator StartFadeCoroutine(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
        {
            audioMixer.GetFloat(exposedParam, out float currentVolume);
            currentVolume = Mathf.Pow(10, currentVolume / 20.0f);
            targetVolume  = Mathf.Clamp(targetVolume, 0.0001f, 1.0f);

            float timer = 0.0f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                float newVol = Mathf.Lerp(currentVolume, targetVolume, timer / duration);
                audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20.0f);
                yield return null;
            }
            audioMixer.SetFloat(exposedParam, Mathf.Log10(targetVolume) * 20.0f);
        }
    }
}