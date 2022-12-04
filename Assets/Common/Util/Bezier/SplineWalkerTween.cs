//using System;
//using System.Collections;
//using UnityEngine;
//using DG.Tweening;

//public class SplineWalkerTween : MonoBehaviour
//{
//    private Transform tr;
//    private float perc = 0.0F;
//    private bool m_IsWalking = false;
//    private static readonly float WALKER_MIN_DIST_SQR = 0.1F * 0.1F;
//    private static readonly float WALKER_MIN_ROT = 0.5F;
//    private static readonly float WALKER_MIN_FOV_DIFF = 1.0F;
//    [SerializeField] private Ease m_Ease = Ease.InOutSine;

//    [Header("Datum")]
//    [SerializeField] private CameraManager _CameraManager;


//    //////////////////////////////////////////
//    // Initialize & Reset
//    //////////////////////////////////////////

//    private void Awake()
//    {
//        tr = GetComponent<Transform>();
//    }


//    //////////////////////////////////////////
//    // Utilities
//    //////////////////////////////////////////

//    public void Walk(int actingPointIdx, Action callback = null)
//    {
//        CameraActingPoint[] actingPoints = CameraManager.CamActingPoints;
//        CameraActingPoint actingPoint;
//        BezierSpline spline;
//        if (actingPointIdx >= 0 && actingPointIdx < actingPoints.Length && actingPointIdx < CameraManager.ZenModeSplines.Length)
//        {
//            actingPoint = actingPoints[actingPointIdx];
//            spline = CameraManager.ZenModeSplines[actingPointIdx];
//        }
//        else
//        {
//            Debug.Log("ZenMode Walk : CamActingPoint idx is not appropriate!");
//            return;
//        }

//        if (m_IsWalking)
//        {
//            Debug.Log("ZenMode Walk : Already walking!");
//            return;
//        }
//        m_IsWalking = true;

//        float zoomScaler = Mathf.InverseLerp(_CameraManager.ZOOM_BOUNDS_PADDING[0], _CameraManager.ZOOM_BOUNDS_PADDING[1], CameraManager.MainCam.fieldOfView);
//        float startOffset = 1.0F - zoomScaler;
//        float speedScaler = 1.0F + startOffset;

//        DOTween.Sequence()
//            .Append(DOTween.To(() => perc, (x) => perc = x, 1.0F, actingPoint.m_PercDuration).SetEase(m_Ease))
//            .OnStart(Init)
//            .Play();


//        void Init()
//        {
//            perc = startOffset;
//            StartCoroutine(WalkProcess());
//        }

//        IEnumerator WalkProcess()
//        {
//            Vector3 destPos = spline.GetPoint(1);
//            float destFov = actingPoint.m_FOV + WALKER_MIN_FOV_DIFF;  // 목표 FOV의 값과 정확히 일치시키기 위한 트릭
//            Quaternion destRot;
//            do
//            {
//                float dt = Time.deltaTime;
//                Debug.DrawLine(new Vector3(0.4F, 25.68F, -8.8F), spline.GetPoint(perc), Color.yellow, 1.0F);
//                tr.position = Vector3.Lerp(tr.position, spline.GetPoint(perc), dt * actingPoint.m_LerpSpeed_Pos * speedScaler);
//                destRot = Quaternion.LookRotation(actingPoint.m_ActingTargetPosition - tr.position);
//                tr.rotation = Quaternion.Slerp(tr.rotation, destRot, dt * actingPoint.m_LerpSpeed_Rot);

//                CameraManager.WorldUICam.fieldOfView
//                    = CameraManager.MainCam.fieldOfView = Mathf.Lerp(CameraManager.MainCam.fieldOfView, destFov, dt);                

//                yield return null;
//            }
//            while ((tr.position - destPos).sqrMagnitude >= WALKER_MIN_DIST_SQR
//                     || Quaternion.Angle(tr.rotation, destRot) >= WALKER_MIN_ROT
//                     || Mathf.Abs(CameraManager.MainCam.fieldOfView - destFov) >= WALKER_MIN_FOV_DIFF);

//            m_IsWalking = false;
//            perc = 0.0F;
//            callback?.Invoke();
//        }
//    }

//    public bool TryGetCameraZenModeDestination(int landmarkIdx, out Vector3 destPos)
//    {
//        if(landmarkIdx >= 0 && landmarkIdx <= CameraManager.ZenModeSplines.Length - 1)
//        {
//            destPos = CameraManager.ZenModeSplines[landmarkIdx].GetPoint(1);
//            return true;
//        }
//        else
//        {
//            destPos = Vector3.zero;
//            return false;
//        }
//    }
//}
