using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace hhotLib.Common
{
    [Serializable]
    public class AudioClipGroup
    {
        public string      groupName;
        public AudioClip[] clips;

        public AudioClip GetRandomClip()
        {
            return clips[Random.Range(0, clips.Length)];
        }
    }

    public class SoundManager : Singleton<SoundManager>
    {
        [Serializable]
        public class Settings
        {
            public float LowPitch     => lowPitch;
            public float HighPitch    => highPitch;
            public float FadeDuration => fadeDuration;

            [SerializeField, Range(0.1f, 1.0f)] private float lowPitch     = 0.95f;
            [SerializeField, Range(1.0f, 1.9f)] private float highPitch    = 1.05f;
            [SerializeField, Range(0.1f, 2.0f)] private float fadeDuration = 1.0f;
        }

        [SerializeField] private Settings         settings;
        [SerializeField] private AudioMixerGroup  mixerGroup_SFX;
        [SerializeField] private AudioMixerGroup  mixerGroup_BGM;
        [SerializeField] private AudioClipGroup[] audioClipGroups;

        private AudioMixer  mainMixer;
        private AudioSource effectSource;
        private AudioSource musicSource;

        private static readonly Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

        protected override void OnAwake()
        {
            effectSource = gameObject.AddComponent<AudioSource>();
            effectSource.playOnAwake = false;
            effectSource.loop        = false;
            effectSource.outputAudioMixerGroup = mixerGroup_SFX;

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop        = true;
            musicSource.outputAudioMixerGroup = mixerGroup_BGM;

            LoadAudioClips();
        }

        private void LoadAudioClips()
        {
            audioClips.Add(AudioClipName.BUTTON_CLICK_A, Resources.Load<AudioClip>("ButtonClick_A"));
            audioClips.Add(AudioClipName.BUTTON_CLICK_B, Resources.Load<AudioClip>("ButtonClick_B"));
        }

        private void Start()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKey.Setting.TOGGLE_AUDIO, 1) == 0)
                AudioListener.pause = true;
            else
                AudioListener.pause = false;
        }

        public void ToggleAudio(bool toggle)
        {
            AudioListener.pause = !toggle;
            PlayerPrefs.SetInt(PlayerPrefsKey.Setting.TOGGLE_AUDIO, toggle ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void PlaySoundEffect(string clipName, float volume = 1.0f)
        {
            if (audioClips.ContainsKey(clipName) == false)
            {
                Debug.LogWarning($"AudioClip({clipName}) not found!");
                return;
            }
            effectSource.PlayOneShot(audioClips[clipName], volume);
        }

        public void PlayRandomSoundEffect(string audioGroupName, float volume = 1.0f)
        {
            AudioClipGroup acGroup = null;
            for (int i = 0; i < audioClipGroups.Length; i++)
            {
                if (audioClipGroups[i].groupName == audioGroupName)
                {
                    acGroup = audioClipGroups[i];
                    break;
                }
            }

            if (acGroup == null)
            {
                Debug.LogWarning($"AudioClipGroup({audioGroupName}) not found!");
                return;
            }
            effectSource.PlayOneShot(acGroup.GetRandomClip(), volume);
        }

        public void PlaySoundEffectDelayed(string clipName, float volume = 1.0f, float delay = 0.1f)
        {
            if (audioClips.ContainsKey(clipName) == false)
            {
                Debug.LogWarning($"AudioClip({clipName}) not found!");
                return;
            }
            effectSource.clip   = audioClips[clipName];
            effectSource.volume = volume;
            effectSource.PlayDelayed(delay);
        }
    }
}