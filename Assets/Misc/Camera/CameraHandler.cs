using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;

namespace test
{
    public class CameraHandler : MonoBehaviour
    {
        //private readonly Settings settings;
        public Settings settings;

        [Serializable]
        public class Settings
        {
            [Header("Init")]
            public Vector3 camInitPosition = new Vector3(0.0f, 1.0f, -10.0f);
            public Vector3 camInitRotation = new Vector3(0.0f, 0.0f, 0.0f);
            public Vector2 camInitClipPlane = new Vector2(0.3f, 1000.0f);
            public float camInitFOV = 60.0f;
            public float damping = 4.0f;
            public readonly float WORLD_UI_RAYCAST_DIST = 500.0f;

            [Header("Pan")]
            public float panSpeed = 300.0f;
            [Range(0.0f, 0.1f)] public float panSpeedMultiplier_Min = 0.25F;
            [Range(0.1f, 1.0f)] public float panSpeedMultiplier_Max = 1.0F;
            [NonSerialized] public float panSpeedX = 0.0f;
            public readonly float[] PAN_BOUNDS_X = { -50.0f, 50.0f };
            public readonly float[] PAN_BOUNDS_PADDING_X = { -48.0f, 48.0f };
            public readonly float[] PAN_BOUNDS_Z = { -50.0f, 50.0f };
            public readonly float[] PAN_BOUNDS_PADDING_Z = { -48.0f, 485.0f };

            [Header("Orbit")]
            public float orbitSpeed = 0.1f;
            public readonly float ORBIT_CAM_TO_TARGET_DISTANCE = 20.0f;
            public readonly float ORBIT_INIT_FOV = 25.0f;
            public readonly float ORBIT_ROT_DAMPING = 10.0f;
            public readonly float[] ORBIT_BOUNDS_Y = { -20.0f, 40.0f };
            public readonly Vector2 ORBIT_LENS_SHIFT = new Vector2(0.0f, 0.1f);

            [Header("Zoom")]
            public float zoomSpeed_Mouse = 20.0f;
            public float zoomSpeed_Touch = 0.25f;
            public readonly float[] ZOOM_BOUNDS = { 1.0f, 70.0f };
            public readonly float[] ZOOM_BOUNDS_PADDING = { 3.0f, 67.5f };
            public readonly float[] ZOOM_BOUNDS_ORBIT_MODE = { 1.0f, 120.0f };
            public readonly float[] ZOOM_BOUNDS_PADDING_ORBIT_MODE = { 3.0f, 118.0f };
            public readonly float[] ZOOM_BOUNDS_SCALER = { 5.0f, 30.0f };
        }

        [Header("Common Infos")]
        private Transform tr;
        private Camera mainCam;
        private bool isPanning = true;
        private bool isZooming = true;
        private bool isOrbiting = false;
        private bool isTransitioning = false;
        private bool isTrackTouchInput = false;

        [Header("Handling Infos")]
        private Transform camOrbitPoint;
        private Transform orbitTarget;
        private float xDeg;
        private float yDeg;
        private float initZoomValue;
        private float desiredZoomValue;
        private float prevZoomValue;
        private Vector2[] lastZoomPositions = new Vector2[2];
        private Vector3 lastPanPosition;
        private Vector3 desiredPanPosition;
        private Vector3 targetViewPoint;
        private Vector3 initPosition;
        private Vector3 previousPosition;
        private Quaternion initRot;
        private Quaternion orbitModeRot;
        private Quaternion desiredOrbitRotation;
        private IWorldUI selectedWorldUI;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        private int selectingTouchFingerId = -1;
#endif

        public static float ZoomScaler;


        //////////////////////////////////////////
        // Initialize & Reset
        //////////////////////////////////////////

        private void Awake()
        {
            tr = GetComponent<Transform>();
            mainCam = Camera.main;
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            tr.position = settings.camInitPosition;
            tr.localRotation = Quaternion.Euler(settings.camInitRotation);
            mainCam.nearClipPlane = settings.camInitClipPlane.x;
            mainCam.farClipPlane = settings.camInitClipPlane.y;
            mainCam.fieldOfView = settings.camInitFOV;

            settings.panSpeedX = settings.panSpeed * mainCam.aspect;

            desiredZoomValue
                = initZoomValue
                = mainCam.fieldOfView;

            desiredPanPosition
                = initPosition
                = tr.position;

            initRot = tr.rotation;
            orbitModeRot = initRot * Quaternion.Euler(-5.0F, 0.0F, 0.0F);

            ZoomScaler = 0.0F;

            if (!camOrbitPoint)
            {
                camOrbitPoint = new GameObject("CameraOrbitPoint").transform;
                camOrbitPoint.position = Vector3.zero;
                camOrbitPoint.gameObject.hideFlags = HideFlags.DontSave;
            }
        }

        private void OnDestroy()
        {
            tr = null;
            mainCam = null;
            orbitTarget = null;
            selectedWorldUI = null;
            if (camOrbitPoint)
            {
                Destroy(camOrbitPoint.gameObject);
                camOrbitPoint = null;
            }
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            TouchManager.OnBeginMouse += DoBeginMouse;
            TouchManager.OnMouse += DoMouse;
            TouchManager.OnEndMouse += DoEndMouse;
#elif UNITY_ANDROID || UNITY_IOS
            TouchManager.OnBeginTouch += DoBeginTouch;
            TouchManager.OnTouch += DoTouch;
            TouchManager.OnEndTouch += DoEndTouch;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            TouchManager.OnBeginMouse -= DoBeginMouse;
            TouchManager.OnMouse -= DoMouse;
            TouchManager.OnEndMouse -= DoEndMouse;
#elif UNITY_ANDROID || UNITY_IOS
            TouchManager.OnBeginTouch -= DoBeginTouch;
            TouchManager.OnTouch -= DoTouch;
            TouchManager.OnEndTouch -= DoEndTouch;
#endif
        }


        //////////////////////////////////////////
        // Update
        //////////////////////////////////////////

        public void LateUpdate()
        {
#if UNITY_EDITOR
            ZoomCamera(Input.GetAxis("Mouse ScrollWheel"), settings.zoomSpeed_Mouse);
#endif

            if (isOrbiting)
            {
                Orbit();
                if (isZooming) Zoom(true);
            }
            else if (isPanning)
            {
                Pan();
                if (isZooming) Zoom(false);
            }
        }


        //////////////////////////////////////////
        // Listeners
        //////////////////////////////////////////

#if UNITY_EDITOR
        private void DoBeginMouse(Vector3 mousePos)
        {
            if (hhotLib.Common.Utils.IsPointerOverScreenUI())
                return;

            if (Physics.Raycast(mainCam.ScreenPointToRay(mousePos), out RaycastHit hit, settings.WORLD_UI_RAYCAST_DIST, 1 << LayerMask.NameToLayer("WORLD_UI"), QueryTriggerInteraction.Collide))
            {
                var worldUI = hit.transform.GetComponent<IWorldUI>();
                if (worldUI != null)
                    selectedWorldUI = worldUI;

                isTrackTouchInput = false;
            }
            else
            {
                isTrackTouchInput = true;
                lastPanPosition = mousePos;
            }
        }

        private void DoMouse(Vector3 mousePos)
        {
            if (!isTrackTouchInput)
                return;

            if (isOrbiting) OrbitCamera(mousePos);
            else if (isPanning) PanCamera(mousePos);
        }

        private void DoEndMouse(Vector3 mousePos)
        {
            if (selectedWorldUI != null)
            {
                if (Physics.Raycast(mainCam.ScreenPointToRay(mousePos), out RaycastHit hit, settings.WORLD_UI_RAYCAST_DIST, 1 << LayerMask.NameToLayer("WORLD_UI"), QueryTriggerInteraction.Collide))
                {
                    var worldUI = hit.transform.GetComponent<IWorldUI>();
                    if (worldUI != null && worldUI.Equals(selectedWorldUI))
                        worldUI.ButtonClicked();
                }
                selectedWorldUI = null;
            }

            isTrackTouchInput = false;
        }
#elif UNITY_ANDROID || UNITY_IOS
        private void DoBeginTouch(Touch[] touches)
        {
            if (hhotLib.Utilities.IsPointerOverUI())
                return;

            isTrackTouchInput = true;

            if (TouchManager.CurrentTouchState == TouchState.ONE_FINGER)
            {
                lastPanPosition = touches[0].position;

                if (Physics.Raycast(mainCam.ScreenPointToRay(touches[0].position), out RaycastHit hit, settings.WORLD_UI_RAYCAST_DIST, 1 << LayerMask.NameToLayer("WORLD_UI"), QueryTriggerInteraction.Collide))
                {
                    var worldUI = hit.transform.GetComponent<IWorldUI>();
                    if (worldUI != null)
                        selectedWorldUI = worldUI;

                    selectingTouchFingerId = touches[0].fingerId;
                    isTrackTouchInput = false;
                }
                else
                {
                    isTrackTouchInput = true;
                }
            }
            else if (TouchManager.CurrentTouchState == TouchState.TWO_FINGER)
            {
                lastZoomPositions[0] = touches[0].position;
                lastZoomPositions[1] = touches[1].position;
            }
        }

        private void DoTouch(Touch[] touches)
        {
            if (!isTrackTouchInput)
                return;

            if (TouchManager.CurrentTouchState == TouchState.ONE_FINGER)
            {
                if (isOrbiting)
                    OrbitCamera(touches[0].deltaPosition);
                else if (isPanning)
                    PanCamera(touches[0].position);
            }
            else if (TouchManager.CurrentTouchState == TouchState.TWO_FINGER)
            {
                if (isZooming)
                {
                    float newDistance = Vector2.Distance(touches[0].position, touches[1].position);
                    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    float offset = newDistance - oldDistance;

                    ZoomCamera(offset, settings.zoomSpeed_Touch);
                    lastZoomPositions[0] = touches[0].position;
                    lastZoomPositions[1] = touches[1].position;
                }
            }
        }

        private void DoEndTouch(Touch[] touches)
        {
            if (selectedWorldUI != null)
            {
                if (Physics.Raycast(mainCam.ScreenPointToRay(touches[0].position), out RaycastHit hit, settings.WORLD_UI_RAYCAST_DIST, LayerMask.NameToLayer("WORLD_UI"), QueryTriggerInteraction.Collide))
                {
                    if (selectingTouchFingerId == touches[0].fingerId)
                    {
                        var worldUI = hit.transform.GetComponent<IWorldUI>();
                        if (worldUI != null && worldUI.Equals(selectedWorldUI))
                            worldUI.ButtonClicked();
                    }
                }
                selectedWorldUI = null;
                selectingTouchFingerId = -1;
            }

            isTrackTouchInput = false;
        }
#endif


        //////////////////////////////////////////
        // Utilities
        //////////////////////////////////////////

        private void PanCamera(Vector3 newPanPosition)
        {
            Vector3 screenDelta = mainCam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
            Vector3 posOffset = Vector3.zero;
            float speedMultiplier = Mathf.Clamp(ZoomScaler, settings.panSpeedMultiplier_Min, settings.panSpeedMultiplier_Max);
            posOffset.x = screenDelta.x * settings.panSpeedX * speedMultiplier;
            posOffset.z = screenDelta.y * settings.panSpeed * speedMultiplier;
            desiredPanPosition = tr.position + posOffset;
            lastPanPosition = newPanPosition;
        }

        private void ZoomCamera(float offset, float speed)
        {
            if (isOrbiting)
                desiredZoomValue = Mathf.Clamp(mainCam.fieldOfView - (offset * speed), settings.ZOOM_BOUNDS_ORBIT_MODE[0], settings.ZOOM_BOUNDS_ORBIT_MODE[1]);
            else if (isPanning)
                desiredZoomValue = Mathf.Clamp(mainCam.fieldOfView - (offset * speed), settings.ZOOM_BOUNDS[0], settings.ZOOM_BOUNDS[1]);
        }

        private void OrbitCamera(Vector3 pointerPos)
        {
#if UNITY_EDITOR
            Vector2 mouseDelta = pointerPos - lastPanPosition;
            xDeg += mouseDelta.x * settings.orbitSpeed;
            yDeg -= mouseDelta.y * settings.orbitSpeed;
            yDeg = ClampAngle(yDeg, settings.ORBIT_BOUNDS_Y[0], settings.ORBIT_BOUNDS_Y[1]);
            desiredOrbitRotation = Quaternion.Euler(yDeg, xDeg, 0.0F);
            lastPanPosition = pointerPos;
#elif UNITY_ANDROID || UNITY_IOS
            Vector2 touchDeltaPos = pointerPos;
            xDeg += touchDeltaPos.x * settings.orbitSpeed;
            yDeg -= touchDeltaPos.y * settings.orbitSpeed;
            yDeg = ClampAngle(yDeg, settings.ORBIT_BOUNDS_Y[0], settings.ORBIT_BOUNDS_Y[1]);
            desiredOrbitRotation = Quaternion.Euler(yDeg, xDeg, 0.0F);
#endif
        }

        private void Pan()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0) == false)
            {
                if (tr.position.x <= settings.PAN_BOUNDS_PADDING_X[0]) desiredPanPosition.x = settings.PAN_BOUNDS_PADDING_X[0];
                else if (tr.position.x >= settings.PAN_BOUNDS_PADDING_X[1]) desiredPanPosition.x = settings.PAN_BOUNDS_PADDING_X[1];

                if (tr.position.z <= settings.PAN_BOUNDS_PADDING_Z[0]) desiredPanPosition.z = settings.PAN_BOUNDS_PADDING_Z[0];
                else if (tr.position.z >= settings.PAN_BOUNDS_PADDING_Z[1]) desiredPanPosition.z = settings.PAN_BOUNDS_PADDING_Z[1];
            }

            tr.position = Vector3.Lerp(tr.position, desiredPanPosition, Time.deltaTime * settings.damping);
#elif UNITY_ANDROID || UNITY_IOS
            if (TouchManager.CurrentTouchState == TouchState.NONE)
            {
                if (tr.position.x <= settings.PAN_BOUNDS_PADDING_X[0]) desiredPanPosition.x = settings.PAN_BOUNDS_PADDING_X[0];
                else if (tr.position.x >= settings.PAN_BOUNDS_PADDING_X[1]) desiredPanPosition.x = settings.PAN_BOUNDS_PADDING_X[1];

                if (tr.position.z <= settings.PAN_BOUNDS_PADDING_Z[0]) desiredPanPosition.z = settings.PAN_BOUNDS_PADDING_Z[0];
                else if (tr.position.z >= settings.PAN_BOUNDS_PADDING_Z[1]) desiredPanPosition.z = settings.PAN_BOUNDS_PADDING_Z[1];
            }

            desiredPanPosition.x = Mathf.Clamp(desiredPanPosition.x, settings.PAN_BOUNDS_X[0], settings.PAN_BOUNDS_X[1]);
            desiredPanPosition.z = Mathf.Clamp(desiredPanPosition.z, settings.PAN_BOUNDS_Z[0], settings.PAN_BOUNDS_Z[1]);
            tr.position = Vector3.Lerp(tr.position, desiredPanPosition, Time.deltaTime * settings.damping);
#endif
        }

        private void Zoom(bool orbitMode)
        {
#if UNITY_EDITOR
            if (Input.GetAxis("Mouse ScrollWheel") > -0.01F && Input.GetAxis("Mouse ScrollWheel") < 0.01F)
            {
                if (orbitMode)
                {
                    if (mainCam.fieldOfView <= settings.ZOOM_BOUNDS_PADDING_ORBIT_MODE[0]) desiredZoomValue = settings.ZOOM_BOUNDS_PADDING_ORBIT_MODE[0];
                    else if (mainCam.fieldOfView >= settings.ZOOM_BOUNDS_PADDING_ORBIT_MODE[1]) desiredZoomValue = settings.ZOOM_BOUNDS_PADDING_ORBIT_MODE[1];
                }
                else
                {
                    if (mainCam.fieldOfView <= settings.ZOOM_BOUNDS_PADDING[0]) desiredZoomValue = settings.ZOOM_BOUNDS_PADDING[0];
                    else if (mainCam.fieldOfView >= settings.ZOOM_BOUNDS_PADDING[1]) desiredZoomValue = settings.ZOOM_BOUNDS_PADDING[1];
                }
            }
#elif UNITY_ANDROID || UNITY_IOS
            if (TouchManager.CurrentTouchState == TouchState.NONE)
            {
                if (orbitMode)
                {
                    if (mainCam.fieldOfView <= settings.ZOOM_BOUNDS_PADDING_ORBIT_MODE[0]) desiredZoomValue = settings.ZOOM_BOUNDS_PADDING_ORBIT_MODE[0];
                    else if (mainCam.fieldOfView >= settings.ZOOM_BOUNDS_PADDING_ORBIT_MODE[1]) desiredZoomValue = settings.ZOOM_BOUNDS_PADDING_ORBIT_MODE[1];
                }
                else
                {
                    if (mainCam.fieldOfView <= settings.ZOOM_BOUNDS_PADDING[0]) desiredZoomValue = settings.ZOOM_BOUNDS_PADDING[0];
                    else if (mainCam.fieldOfView >= settings.ZOOM_BOUNDS_PADDING[1]) desiredZoomValue = settings.ZOOM_BOUNDS_PADDING[1];
                }
            }
#endif

            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, desiredZoomValue, Time.deltaTime * settings.damping);
            ZoomScaler = Mathf.InverseLerp(settings.ZOOM_BOUNDS_SCALER[0], settings.ZOOM_BOUNDS_SCALER[1], mainCam.fieldOfView);
        }

        private void Orbit()
        {
            camOrbitPoint.rotation = Quaternion.Slerp(camOrbitPoint.rotation, desiredOrbitRotation, Time.deltaTime * settings.damping);

            if (orbitTarget != null)
            {
                camOrbitPoint.position = Vector3.Lerp(camOrbitPoint.position, orbitTarget.position, Time.deltaTime * settings.damping);
                tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(orbitTarget.position - tr.position), Time.deltaTime * settings.ORBIT_ROT_DAMPING);
            }

            tr.localPosition = Vector3.Lerp(tr.localPosition, targetViewPoint, Time.deltaTime * settings.damping);
        }

        public void EnterOrbitMode(Transform target, Action onStart = null, Action onComplete = null)
        {
            if (!target)
            {
                Debug.LogError("There is no target to orbit around!");
                return;
            }

            if (isOrbiting || isTransitioning)
            {
                Debug.Log("It's already transitioning or orbiting!");
                return;
            }

            DOTween.Sequence()
                //.Append(hhotLib.Utilities.GetLensShiftTween(mainCam, settings.ORBIT_LENS_SHIFT))
                .Join(hhotLib.Common.Utils.GetFOVTween(mainCam, settings.ORBIT_INIT_FOV))
                .OnStart(DoStart)
                .OnComplete(DoComplete)
                .Play();

            void DoStart()
            {
                isTransitioning = true;
                isPanning = false;
                isOrbiting = true;
                isZooming = false;
                orbitTarget = target;
                desiredOrbitRotation = Quaternion.identity;
                xDeg = Vector3.Angle(Vector3.right, tr.right);
                yDeg = Vector3.Angle(Vector3.up, tr.up);
                previousPosition = tr.position;
                prevZoomValue = mainCam.fieldOfView;
                camOrbitPoint.position = orbitTarget.position;
                tr.SetParent(camOrbitPoint, true);
                targetViewPoint = camOrbitPoint.InverseTransformPoint(orbitTarget.position - (orbitModeRot * Vector3.forward * settings.ORBIT_CAM_TO_TARGET_DISTANCE));
                onStart?.Invoke();
            }

            void DoComplete()
            {
                desiredZoomValue = mainCam.fieldOfView;
                isZooming = true;
                isTransitioning = false;
                onComplete?.Invoke();
            }
        }

        public void ResetPoseFromOrbit()
        {
            if (isOrbiting)
            {
                ResetPose(false, false, false, 1.25F, () =>
                {
                    //hhotLib.Utilities.GetLensShiftTween(mainCam, Vector2.zero).Play();
                    orbitTarget = null;
                    tr.SetParent(null);
                    camOrbitPoint.rotation = Quaternion.identity;
                });
            }
        }

        public void ResetPose(bool forceNotMove, bool forceInitPos, bool forceInitZoom, float duration, Action onStart = null, Action onComplete = null)
        {
            if (isTransitioning)
            {
                Debug.LogWarning("Failed to reset pose : It's now transitioning!");
                return;
            }

            if (forceNotMove)
            {
                desiredPanPosition = tr.position;
                desiredZoomValue = mainCam.fieldOfView;
                isTransitioning = false;
                isPanning = true;
                isZooming = true;
                onComplete?.Invoke();
            }
            else
            {
                Vector3 destPos = forceInitPos ? initPosition : previousPosition;
                float destZoom = forceInitZoom ? initZoomValue : prevZoomValue;

                DOTween.Sequence()
                    .Append(tr.DOMove(destPos, duration))
                    .Join(tr.DORotateQuaternion(initRot, duration))
                    .Join(mainCam.DOFieldOfView(destZoom, duration))
                    .InsertCallback(duration * 0.5F, OnMid)
                    .SetEase(Ease.InOutSine)
                    .OnStart(DoStart)
                    .OnUpdate(DoUpdate)
                    .OnComplete(DoComplete)
                    .Play();

                void DoStart()
                {
                    isTransitioning = true;
                    isPanning = false;
                    isOrbiting = false;
                    isZooming = false;
                    desiredPanPosition = destPos;
                    onStart?.Invoke();
                }

                void OnMid()
                {

                }

                void DoUpdate()
                {
                    ZoomScaler = Mathf.InverseLerp(settings.ZOOM_BOUNDS_SCALER[0], settings.ZOOM_BOUNDS_SCALER[1], mainCam.fieldOfView);
                }

                void DoComplete()
                {
                    desiredZoomValue = destZoom;
                    isTransitioning = false;
                    isPanning = true;
                    isOrbiting = false;
                    isZooming = true;
                    onComplete?.Invoke();
                }
            }
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360.0F) angle += 360.0F;
            if (angle > 360.0F) angle -= 360.0F;
            return Mathf.Clamp(angle, min, max);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (settings == null || Application.isPlaying == false)
                return;

            // Cam Bounds
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(0.5F * (new Vector3(settings.PAN_BOUNDS_X[0], 0.0F, settings.PAN_BOUNDS_Z[0]) + new Vector3(settings.PAN_BOUNDS_X[1], 1.0F, settings.PAN_BOUNDS_Z[1])) + Vector3.up * this.transform.position.y,
                                new Vector3(settings.PAN_BOUNDS_X[0] * 2.0F, 1.0F, Mathf.Abs(settings.PAN_BOUNDS_Z[1] - settings.PAN_BOUNDS_Z[0])));

            // Cam Bounds-Padding
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(0.5F * (new Vector3(settings.PAN_BOUNDS_PADDING_X[0], 0.0F, settings.PAN_BOUNDS_PADDING_Z[0]) + new Vector3(settings.PAN_BOUNDS_PADDING_X[1], 1.0F, settings.PAN_BOUNDS_PADDING_Z[1])) + Vector3.up * this.transform.position.y,
                                new Vector3(settings.PAN_BOUNDS_PADDING_X[0] * 2.0F, 1.0F, Mathf.Abs(settings.PAN_BOUNDS_PADDING_Z[1] - settings.PAN_BOUNDS_PADDING_Z[0])));
        }
#endif
    }
}