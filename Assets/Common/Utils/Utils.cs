using System.Reflection;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

namespace hhotLib.Common
{
    public class Utils
    {
        public static bool IsPointerOverScreenUI()
        {
            int pointerId;
#if UNITY_EDITOR
            pointerId = -1;
#elif UNITY_ANDROID || UNITY_IOS
            pointerId = 0;
#endif

            EventSystem eventSystem = EventSystem.current;
            if (eventSystem != null)
            {
                return eventSystem.IsPointerOverGameObject(pointerId);
            }
            else
            {
                Debug.LogError("There is no eventsystem.");
                return false;
            }
        }

        public static void AlignWithSameDistanceFromCenter(TextMeshProUGUI text, Image coinImg, float blankspaceOffset = 15.0F, bool isScaled = false)
        {
            Transform costTr = text.transform;
            RectTransform coinTr = coinImg.rectTransform;
            float costTextWidth = text.preferredWidth;
            float coinImageWidth = coinTr.sizeDelta.x;
            if (isScaled)
            {
                costTextWidth *= costTr.localScale.x;
                coinImageWidth *= coinTr.localScale.x;
            }
            float costPosXOffset = (coinImageWidth + blankspaceOffset) * 0.5F;
            float coinPosXOffset = -(blankspaceOffset + costTextWidth) * 0.5F;
            costTr.localPosition = new Vector3(costPosXOffset, costTr.localPosition.y, 0.0F);
            coinTr.localPosition = new Vector3(coinPosXOffset, coinTr.localPosition.y, 0.0F);
        }

        public static Vector3 GetRandomXZDirection(float minDist, float maxDist)
        {
            Vector2 randomXZ = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 randomDirection = new Vector3(randomXZ.x, 0.0F, randomXZ.y);
            float randomDistance = UnityEngine.Random.Range(minDist, maxDist);
            randomDirection *= randomDistance;

            return randomDirection;
        }

        public static Tween GetFloatingTween(Transform target, float amplitude, float interval, Ease ease = Ease.InOutSine)
        {
            if (target == null)
                return null;

            if (DOTween.IsTweening(target))
                DOTween.Kill(target);

            return target.DOLocalMove(target.up * amplitude, interval).SetRelative(true).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
        }

        public static Tweener GetTextBlinkTween(TextMeshProUGUI target, float blinkEndVal, float duration, Ease ease = Ease.InOutSine)
        {
            if (target == null)
                return null;

            if (DOTween.IsTweening(target))
                DOTween.Kill(target);

            return target.DOFade(blinkEndVal, duration).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
        }

        public static Tween GetLensShiftTween(Camera target, Vector2 endShift, float duration = 0.5f, Ease ease = Ease.InOutSine)
        {
            if (!target)
                return null;

            return DOTween.Sequence()
                .Append(DOTween.To(() => target.lensShift, x => target.lensShift = x, endShift, duration))
                .SetEase(ease);
        }

        public static Tween GetFOVTween(Camera target, float endVal, float duration = 0.5f, Ease ease = Ease.InOutSine)
        {
            if (!target)
                return null;

            return DOTween.Sequence()
                .Append(DOTween.To(() => target.fieldOfView, x => target.fieldOfView = x, endVal, duration))
                .SetEase(ease);
        }

        public static Tween GetRotationTween(Transform target, Quaternion endVal, float duration = 0.5F, Ease ease = Ease.InOutSine)
        {
            if (!target)
                return null;

            return target.DORotateQuaternion(endVal, duration)
                .SetEase(ease);
        }

        public static FieldInfo[] GetConstants(System.Type type)
        {
            ArrayList constants = new ArrayList();

            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (FieldInfo fi in fieldInfos)
            {
                // IsLiteral determines if its value is written at compile time and not changeable.
                // IsInitOnly determines if the field can be set in the body of the constructor.
                // For C# a field which is readonly keyword would have both true but a const field would have only IsLiteral equal to true.
                if (fi.IsLiteral && fi.IsInitOnly == false)
                    constants.Add(fi);
            }
            return (FieldInfo[])constants.ToArray(typeof(FieldInfo));
        }
    }
}