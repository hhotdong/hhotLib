//using UnityEngine;
//using DG.Tweening;

//public class ToggleCloseButton : MonoBehaviour
//{
//    private CanvasGroup cg;
//    private bool m_IsToggled = true;

//    private void Awake()
//    {
//        cg = GetComponent<CanvasGroup>();
//        if (!cg)
//            cg = gameObject.AddComponent<CanvasGroup>();
//    }

//    private void OnEnable()
//    {
//        cg.interactable = true;
//        cg.alpha = 1.0F;
//        m_IsToggled = true;

//        CloseButtonManager.Instance?.Push(this);
//    }

//    private void OnDisable()
//    {
//        CloseButtonManager.Instance?.Pop();
//    }

//    public void Toggle(bool toggle)
//    {
//        if (m_IsToggled == toggle)
//            return;

//        m_IsToggled = toggle;

//        float endVal = toggle ? 1.0F : 0.0F;
//        const float DURATION = 0.35F;

//        cg.DOFade(endVal, DURATION)
//            .SetEase(Ease.InOutSine)
//            .OnStart(DoStart)
//            .OnComplete(DoComplete)
//            .Play();


//        void DoStart()
//        {
//            cg.interactable = false;
//        }

//        void DoComplete()
//        {
//            cg.interactable = toggle;
//        }
//    }
//}
