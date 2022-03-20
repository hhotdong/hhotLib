//using UnityEngine;
//using DG.Tweening;
//using deVoid.Utils;

//public class ChangeOrbitTargetSignal : ASignal<Collider> { }

//public class UIGen_ViewFinder : MonoBehaviour
//{
//    private Transform tr;
//    private SpriteRenderer rd;
//    private Transform cam;
//    private Vector3 m_InitScale;
//    private float m_MinScale;
//    private static readonly string TWEEN_ID_VIEW_FINDER = "VIEW_FINDER";
//    private static readonly float FADE_OUT_DURATION = 0.35F;
//    [SerializeField] private Transform m_Content;
    

//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    private void Awake()
//    {
//        tr = GetComponent<Transform>();
//        rd = GetComponentInChildren<SpriteRenderer>();
//        cam = Camera.main.transform;
//        m_InitScale = tr.localScale;
//        m_MinScale = 0.45F;

//        Signals.Get<ChangeOrbitTargetSignal>().AddListener(OnChangeOrbitTarget);
//        this.gameObject.SetActive(false);
//    }

//    private void OnDestroy()
//    {
//        Signals.Get<ChangeOrbitTargetSignal>().RemoveListener(OnChangeOrbitTarget);
//    }


//    //////////////////////////////////////////
//    // Listeners
//    //////////////////////////////////////////

//    private void OnChangeOrbitTarget(Collider target)
//    {
//        if (DOTween.IsTweening(TWEEN_ID_VIEW_FINDER))
//            DOTween.Kill(TWEEN_ID_VIEW_FINDER);

//        UIManager.GetSquashAndStretchSequence(m_Content)
//                .AppendInterval(1.0F)
//                .Append(rd.DOFade(0.0F, FADE_OUT_DURATION).SetEase(Ease.OutSine))
//                .OnStart(DoStart)
//                .OnUpdate(DoUpdate)
//                .OnComplete(DoComplete)
//                .SetId(TWEEN_ID_VIEW_FINDER)
//                .SetUpdate(true)
//                .Play();


//        void DoStart()
//        {
//            this.gameObject.SetActive(true);
//            m_Content.localScale = Vector3.zero;
//            tr.position = target.bounds.center;
//            rd.color = Color.white;
//        }

//        void DoUpdate()
//        {
//            if (!target.enabled)
//                return;

//            // 타겟 위치를 트랙킹
//            tr.position = target.bounds.center;

//            // 카메라 줌 상태에 따른 스케일링
//            tr.localScale = m_InitScale * Mathf.Clamp(CameraHandler.ZoomScaler, m_MinScale, 1.0F);

//            // 빌보드 효과
//            tr.rotation = Quaternion.LookRotation(cam.forward);
//        }

//        void DoComplete()
//        {
//            this.gameObject.SetActive(false);
//        }
//    }
//}
