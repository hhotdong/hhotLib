using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using hhotLib.Common;

namespace hhotLib
{
    public enum SoundType
    {
        NONE = -1,
        BUTTON_DEFAULT = 0,
        BUTTON_UPGRADE = 1
    }

    [Serializable]
    public class SoundGroup
    {
        public SoundType type;
        public List<AudioClip> clips;

        public AudioClip GetClip()
        {
            if (clips?.Count > 0)
            {
                return clips[Random.Range(0, clips.Count)];
            }
            else
            {
                return null;
            }
        }
    }

    [Serializable]
    public struct AmbientSound
    {
        public AudioClip clip;
        public float WaitingTime;
        public bool isLoop;
        [HideInInspector] public double realWaitingTime;
    }

    public class SoundManager : Singleton<SoundManager>
    {
        public Settings settings;

        [Serializable]
        public class Settings
        {
            public float lowPitch = 0.95f;
            public float highPitch = 1.05f;
            public float fadeDuration = 1.0f;
        }

        public static class ExposedParams
        {
            public const string MASTER_VOLUME = "MasterVolume";
            public const string BGM_VOLUME = "BGMVolume";
            public const string SFX_VOLUME = "SFXVolume";
        }

        [Header("Common Infos")]
        private AudioMixer mainMixer;
        private AudioSource effectSource;
        private AudioSource musicSource;

        [SerializeField] private AudioMixerGroup mixerGroup_SFX;
        [SerializeField] private AudioMixerGroup mixerGroup_BGM;

        [Header("Clip Infos")]
        [SerializeField] private List<SoundGroup> soundGroups;
        private static readonly Dictionary<SoundType, SoundGroup> SoundGroupsDict = new Dictionary<SoundType, SoundGroup>();

        protected override void OnAwake()
        {
            mainMixer = mixerGroup_BGM.audioMixer;
            effectSource = this.gameObject.AddComponent<AudioSource>();
            musicSource = this.gameObject.AddComponent<AudioSource>();

            effectSource.playOnAwake = false;
            musicSource.playOnAwake = false;

            effectSource.loop = false;
            musicSource.loop = true;

            effectSource.outputAudioMixerGroup = mixerGroup_SFX;
            musicSource.outputAudioMixerGroup = mixerGroup_BGM;

            SoundGroupsDict.Clear();
            for (int i = 0; i < soundGroups.Count; i++)
            {
                if (soundGroups[i] != null && !SoundGroupsDict.ContainsKey(soundGroups[i].type))
                    SoundGroupsDict.Add(soundGroups[i].type, soundGroups[i]);
            }
        }

        private void Start()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.Setting.TOGGLE_AUDIO))
            {
                if (PlayerPrefs.GetInt(PlayerPrefsKeys.Setting.TOGGLE_AUDIO, 0) == 0)
                    AudioListener.pause = true;
                else
                    AudioListener.pause = false;
            }
            else
            {
                AudioListener.pause = false;
                PlayerPrefs.SetInt(PlayerPrefsKeys.Setting.TOGGLE_AUDIO, 0);
                PlayerPrefs.Save();
            }
        }

        protected override void OnDestroySingleton()
        {
            mainMixer = null;
            effectSource = null;
            musicSource = null;
        }

        public void ToggleAudio(bool toggle)
        {
            AudioListener.pause = !toggle;
            PlayerPrefs.SetInt(PlayerPrefsKeys.Setting.TOGGLE_AUDIO, toggle ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void PlaySoundEffect(SoundType type, float volume = 1.0f, bool isRandomPitch = false)
        {
            if (SoundGroupsDict.ContainsKey(type))
            {
                if (isRandomPitch)
                {
                    effectSource.pitch = Random.Range(settings.lowPitch, settings.highPitch);
                }

                effectSource.PlayOneShot(SoundGroupsDict[type].GetClip(), volume);
            }
            else
            {
                Debug.Log($"There is no such type{type.ToString()} in the dictionary!");
            }
        }

        public void PlaySoundEffectDelayed(SoundType type, float volume = 1.0f, bool isRandomPitch = false, float delay = 0.1f)
        {
            if (SoundGroupsDict.ContainsKey(type))
            {
                if (isRandomPitch)
                {
                    effectSource.pitch = Random.Range(settings.lowPitch, settings.highPitch);
                }

                effectSource.clip = SoundGroupsDict[type].GetClip();
                effectSource.volume = volume;
                effectSource.PlayDelayed(delay);
            }
            else
            {
                Debug.Log($"There is no such type{type.ToString()} in the dictionary!");
            }
        }
    }
}