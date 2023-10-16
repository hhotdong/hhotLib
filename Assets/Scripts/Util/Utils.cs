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
            if (eventSystem == null)
            {
                Debug.LogError("There is no eventsystem!");
                return false;
            }
            return eventSystem.IsPointerOverGameObject(pointerId);
        }

        public static void AlignTextAndImageFromCenter(TextMeshProUGUI text, Image img, float interval = 15.0F)
        {
            RectTransform textTr = text.GetComponent<RectTransform>();
            RectTransform imgTr  = img.rectTransform;
            textTr.anchorMin
                = textTr.anchorMax
                = imgTr.anchorMin
                = imgTr.anchorMax
                = new Vector2(0.5f, 0.5f);

            textTr.pivot
                = imgTr.pivot
                = new Vector2(0.0f, 0.5f);

            text.alignment = TextAlignmentOptions.MidlineLeft;

            float textWidth  = text.preferredWidth;
            float imageWidth = imgTr.rect.width;
            textWidth  *= textTr.localScale.x;
            imageWidth *= imgTr .localScale.x;

            textTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth);

            float sumWidth = textWidth + imageWidth + interval;
            imgTr .anchoredPosition = new Vector2(0.0f - sumWidth * 0.5f , 0.0f);
            textTr.anchoredPosition = new Vector2(imageWidth + interval - sumWidth * 0.5f, 0.0f);
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

            return target.DOLocalMove(target.up * amplitude, interval)
                .SetRelative(true)
                .SetEase(ease)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public static Tweener GetTextBlinkTween(TextMeshProUGUI target, float blinkEndVal, float duration, Ease ease = Ease.InOutSine)
        {
            if (target == null)
                return null;

            if (DOTween.IsTweening(target))
                DOTween.Kill(target);

            return target.DOFade(blinkEndVal, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(ease);
        }

        public static Tween GetLensShiftTween(Camera target, Vector2 endShift, float duration, Ease ease = Ease.InOutSine)
        {
            if (target == null)
                return null;

            return DOTween.Sequence()
                .Append(DOTween.To(() => target.lensShift, x => target.lensShift = x, endShift, duration))
                .SetEase(ease);
        }

        public static Tween GetFOVTween(Camera target, float endVal, float duration, Ease ease = Ease.InOutSine)
        {
            if (target == null)
                return null;

            return DOTween.Sequence()
                .Append(DOTween.To(() => target.fieldOfView, x => target.fieldOfView = x, endVal, duration))
                .SetEase(ease);
        }

        public static Tween GetRotationTween(Transform target, Quaternion endVal, float duration, Ease ease = Ease.InOutSine)
        {
            if (target == null)
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