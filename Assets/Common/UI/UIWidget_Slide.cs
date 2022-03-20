//using System;
//using UnityEngine;
//using DG.Tweening;

//public class UIWidget_Slide : MonoBehaviour
//{
//    public enum Position
//    {
//        None = 0,
//        Left = 1,
//        Right = 2,
//        Top = 3,
//        Bottom = 4,
//    }

//    private RectTransform rectTr;
//    [SerializeField] protected Position origin = Position.Left;
//    [SerializeField] protected float duration = 0.5F;
//    [SerializeField] protected Ease ease = Ease.Linear;

//    private void Awake()
//    {
//        rectTr = GetComponent<RectTransform>();
//    }

//    public void Animate(bool isOn, Action onStart = null, Action onEnd = null)
//    {
//        onStart?.Invoke();

//        Vector3 startPosition = Vector3.zero;
//        switch (origin)
//        {
//            case Position.Left:
//                startPosition = new Vector3(-rectTr.rect.width, 0.0F, 0.0F);
//                break;

//            case Position.Right:
//                startPosition = new Vector3(rectTr.rect.width, 0.0F, 0.0F);
//                break;

//            case Position.Top:
//                startPosition = new Vector3(0.0F, rectTr.rect.height, 0.0F);
//                break;

//            case Position.Bottom:
//                startPosition = new Vector3(0.0F, -rectTr.rect.height, 0.0F);
//                break;
//            default:
//                break;
//        }

//        rectTr.anchoredPosition = isOn ? startPosition : Vector3.zero;

//        if (DOTween.IsTweening(rectTr))
//            rectTr.DOKill();        

//        rectTr
//            .DOAnchorPos(isOn ? Vector3.zero : startPosition, duration, true)
//            .OnComplete(DoComplete)
//            .SetEase(ease)
//            .SetUpdate(true)
//            .Play();


//        void DoComplete()
//        {
//            onEnd?.Invoke();
//        }
//    }
//}
