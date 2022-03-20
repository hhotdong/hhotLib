using System;
using System.Collections.Generic;
using UnityEngine;
using hhotLib.Save;

namespace hhotLib.Tutorial
{
    [CreateAssetMenu]
    public class TutorialPageData : SavableSO
    {
        [Header("Common Infos")]
        public TutorialType tutorialType = TutorialType.NONE;
        public bool toggleActive = true;
        public bool pauseTime = false;

        public TutorialPageData nextPageData = null;

        /// <summary> It can be performed again and again after complete. </summary>
        public bool repetitive = false;

        [Header("Timer-Based")]
        public bool enableTimer = false;
        public float timerPeriod = 0.0F;

        [Header("Broadcast-Action-Based")]
        public bool enableAction = false;
        public string awaitingAction = "";

        [Header("Save Infos")]
        public string key = "";
        public bool complete = false;
        public bool triggered = false;

        public virtual void OnBegin()
        {
            Debug.Log($"Tutorial {tutorialType.ToString()} starts.");
            triggered = true;

            // Timer-based tutorial is considered to be complete once it's triggered.
            if (enableTimer)
            {
                complete = true;
            }
        }

        public virtual void OnEnd()
        {
            Debug.Log($"Tutorial {tutorialType.ToString()} ends.");

            complete = true;

            if (nextPageData)
            {
                TutorialManager.Instance.ShowTutorial(nextPageData.tutorialType);
            }
        }

        public virtual void OnCancel()
        {
            Debug.Log($"Tutorial {tutorialType.ToString()} canceled.");
        }

        public override void OnLoad()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out Dictionary<string, SaveData.Tutorial> tutorials))
            {
                if (tutorials.TryGetValue(key, out SaveData.Tutorial value))
                {
                    complete = value.COMPLETE;
                    triggered = value.TRIGGERED; 
                }
            }
        }

        public override void OnSave()
        {
            if (SaveLoadSystem.SaveDataContainer.TryGetSaveData(this, out Dictionary<string, SaveData.Tutorial> tutorials))
            {
                var saveData = new SaveData.Tutorial();
                saveData.COMPLETE = complete;
                saveData.TRIGGERED = triggered;

                if (string.IsNullOrEmpty(key))
                {
                    Debug.LogError($"Key must not be null or empty!");
                }

                if (tutorials.ContainsKey(key))
                {
                    tutorials[key] = saveData;
                }
                else
                {
                    tutorials.Add(key, saveData);
                }
            }
        }

        public override void OnReset()
        {
            complete = false;
            triggered = false;
        }

        private void OnValidate()
        {
            if (enableAction && tutorialType != TutorialType.NONE)
            {
                awaitingAction = $"{tutorialType.ToString()}_COMPLETE";
            }
        }
    }
}