using System.Collections;
using UnityEngine;

namespace hhotLib.Common
{
    public sealed class UIIncrementText_int : UIIncrementText<int>
    {
        public override void Increment(int targetVal)
        {
            if (isInitialized == false)
            {
                Debug.LogWarning($"{nameof(UIIncrementText_int)} isn't initialized!");
                return;
            }

            if (targetVal == targetValue)
                return;

            targetValue = targetVal;

            StopAllCoroutines();
            StartCoroutine(IncrementCoroutine(curValue, targetValue, incrementDuration));
        }

        private IEnumerator IncrementCoroutine(int startVal, int endVal, float duration)
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float lerp   = elapsedTime / duration;
                int   newVal = (int)Mathf.Lerp(startVal, endVal, lerp);
                UpdateValue(newVal);
                yield return null;
            }
            UpdateValue(endVal);
        }
    }
}