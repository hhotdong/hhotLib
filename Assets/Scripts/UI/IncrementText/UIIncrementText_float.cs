using System.Collections;
using UnityEngine;

namespace hhotLib.Common
{
    public sealed class UIIncrementText_float : UIIncrementText<float>
    {
        [SerializeField] private int precision = 3;

        public override void Increment(float targetVal)
        {
            if (isInitialized == false)
            {
                Debug.LogWarning($"{nameof(UIIncrementText_float)} isn't initialized!");
                return;
            }

            bool isSame = Mathf.Abs(targetVal - targetValue) <= float.Epsilon;
            if (isSame)
                return;

            targetValue = targetVal;

            StopAllCoroutines();
            StartCoroutine(IncrementCoroutine(curValue, targetValue, incrementDuration));
        }

        protected override void UpdateValue(float val)
        {
            curValue       = val;
            valueText.text = val.ToString($"F{precision}");
        }

        private IEnumerator IncrementCoroutine(float startVal, float endVal, float duration)
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float lerp   = elapsedTime / duration;
                float newVal = Mathf.Lerp(startVal, endVal, lerp);
                UpdateValue(newVal);
                yield return null;
            }
            UpdateValue(endVal);
        }
    }
}