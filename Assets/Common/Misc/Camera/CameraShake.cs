// Credit: Mirza Beig
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CameraShakeAmplitudeCurve {
    Constant,
    FadeInOut25,
    FadeInOut50,
    FadeInOut75,
    Custom
}

public enum CameraShakeAmplitudeOverDistanceCurve {
    Constant,
    LinearFadeIn,
    LinearFadeOut
}

public class CameraShake : MonoBehaviour
{
    public bool IsShaking { get; private set; }

    [SerializeField] private float                     amplitude      = 1.0f;
    [SerializeField] private float                     frequency      = 5.0f;
    [SerializeField] private float                     duration       = 2.5f;
    [SerializeField] private float                     smoothDampTime = 0.045f;
    [SerializeField] private CameraShakeAmplitudeCurve amplitudeCurve = CameraShakeAmplitudeCurve.FadeInOut75;
    [SerializeField] private AnimationCurve            customCurve;

    private Transform tr;
    private Shake     curShake;
    private float     smoothDampRotationVelocityX;
    private float     smoothDampRotationVelocityY;
    private float     smoothDampRotationVelocityZ;

    private readonly float ROT_DIFF_MIN = 0.1F;

    private void Awake()
    {
        tr = GetComponent<Transform>();
    }

    public void ShakeNow(Action callback)
    {
        if (curShake != null)
        {
            Debug.LogWarning("There is already playing Shake!");
            return;
        }
        StartCoroutine(ShakeCoroutine(callback));
    }

    private IEnumerator ShakeCoroutine(Action callback)
    {
        IsShaking = true;

        Vector3 originEulerAngles = tr.eulerAngles;
        Vector3 rotationOffset    = originEulerAngles;

        Vector3 eulerAngles;
        curShake = new Shake(amplitude, frequency, duration, amplitudeCurve, customCurve);
        while (curShake != null && curShake.IsAlive())
        {
            curShake.Update();
            rotationOffset = originEulerAngles;
            rotationOffset += curShake.noise;
            eulerAngles    = tr.eulerAngles;
            eulerAngles.x  = Mathf.SmoothDampAngle(eulerAngles.x, rotationOffset.x, ref smoothDampRotationVelocityX, smoothDampTime);
            eulerAngles.y  = Mathf.SmoothDampAngle(eulerAngles.y, rotationOffset.y, ref smoothDampRotationVelocityY, smoothDampTime);
            eulerAngles.z  = Mathf.SmoothDampAngle(eulerAngles.z, rotationOffset.z, ref smoothDampRotationVelocityZ, smoothDampTime);
            tr.eulerAngles = eulerAngles;
            yield return null;
        }
        curShake = null;

        do
        {
            eulerAngles    = tr.eulerAngles;
            eulerAngles.x  = Mathf.SmoothDampAngle(eulerAngles.x, rotationOffset.x, ref smoothDampRotationVelocityX, smoothDampTime);
            eulerAngles.y  = Mathf.SmoothDampAngle(eulerAngles.y, rotationOffset.y, ref smoothDampRotationVelocityY, smoothDampTime);
            eulerAngles.z  = Mathf.SmoothDampAngle(eulerAngles.z, rotationOffset.z, ref smoothDampRotationVelocityZ, smoothDampTime);
            tr.eulerAngles = eulerAngles;
        }
        while (Vector3.Angle(eulerAngles, tr.eulerAngles) >= ROT_DIFF_MIN);

        tr.eulerAngles = originEulerAngles;
        IsShaking      = false;
        callback?.Invoke();
    }

    [Serializable]
    public class Shake
    {
        public float amplitude = 1.0f;
        public float frequency = 1.0f;
        public float duration;

        [HideInInspector] public Vector3 noise;

        private float  timeRemaining;
        private Vector2 perlinNoiseX;
        private Vector2 perlinNoiseY;
        private Vector2 perlinNoiseZ;

        public AnimationCurve amplitudeOverLifetimeCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));

        public void Init()
        {
            timeRemaining = duration;
            ApplyRandomSeed();
        }

        private void Init(float amplitude, float frequency, float duration)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;

            this.duration = duration;
            timeRemaining = duration;

            ApplyRandomSeed();
        }

        public void ApplyRandomSeed()
        {
            float randomRange = 32.0f;

            perlinNoiseX.x = Random.Range(-randomRange, randomRange);
            perlinNoiseX.y = Random.Range(-randomRange, randomRange);

            perlinNoiseY.x = Random.Range(-randomRange, randomRange);
            perlinNoiseY.y = Random.Range(-randomRange, randomRange);

            perlinNoiseZ.x = Random.Range(-randomRange, randomRange);
            perlinNoiseZ.y = Random.Range(-randomRange, randomRange);
        }

        public Shake(float amplitude, float frequency, float duration, AnimationCurve amplitudeOverLifetimeCurve)
        {
            Init(amplitude, frequency, duration);
            this.amplitudeOverLifetimeCurve = amplitudeOverLifetimeCurve;
        }

        public Shake(float amplitude, float frequency, float duration, CameraShakeAmplitudeCurve amplitudeOverLifetimeCurve, AnimationCurve curve = null)
        {
            Init(amplitude, frequency, duration);

            switch (amplitudeOverLifetimeCurve)
            {
                case CameraShakeAmplitudeCurve.Constant:
                    {
                        this.amplitudeOverLifetimeCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
                        break;
                    }
                case CameraShakeAmplitudeCurve.FadeInOut25:
                    {
                        this.amplitudeOverLifetimeCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(0.25f, 1.0f), new Keyframe(1.0f, 0.0f));
                        break;
                    }
                case CameraShakeAmplitudeCurve.FadeInOut50:
                    {
                        this.amplitudeOverLifetimeCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(0.50f, 1.0f), new Keyframe(1.0f, 0.0f));
                        break;
                    }
                case CameraShakeAmplitudeCurve.FadeInOut75:
                    {
                        this.amplitudeOverLifetimeCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(0.75f, 1.0f), new Keyframe(1.0f, 0.0f));
                        break;
                    }

                case CameraShakeAmplitudeCurve.Custom:
                    {
                        this.amplitudeOverLifetimeCurve = curve;
                        break;
                    }

                default:
                    {
                        throw new System.Exception("Unknown enum.");
                    }
            }
        }

        public bool IsAlive()
        {
            return timeRemaining > 0.0f;
        }

        public void Update()
        {
            if (timeRemaining < 0.0f)
                return;

            Vector2 frequencyVector = Time.deltaTime * new Vector2(frequency, frequency);

            perlinNoiseX += frequencyVector;
            perlinNoiseY += frequencyVector;
            perlinNoiseZ += frequencyVector;

            noise.x = Mathf.PerlinNoise(perlinNoiseX.x, perlinNoiseX.y) - 0.5f;
            noise.y = Mathf.PerlinNoise(perlinNoiseY.x, perlinNoiseY.y) - 0.5f;
            noise.z = Mathf.PerlinNoise(perlinNoiseZ.x, perlinNoiseZ.y) - 0.5f;

            float amplitudeOverLifetime = amplitudeOverLifetimeCurve.Evaluate(1.0f - (timeRemaining / duration));

            noise *= amplitude * amplitudeOverLifetime;

            timeRemaining -= Time.deltaTime;
        }
    }
}
