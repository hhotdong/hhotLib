using UnityEngine;

namespace hhotLib.Common
{
    public enum SplineWalkerMode {
        Once,
        Loop,
        PingPong
    }

    public class SplineWalker : MonoBehaviour
    {
	    [SerializeField] private BezierSpline     spline;
        [SerializeField] private SplineWalkerMode mode;
	    [SerializeField] private float            duration;
        [SerializeField] private bool             lookForward;

        private float progress;
        private bool  goingForward = true;

        private void Update()
        {
            if (goingForward)
            {
                progress += Time.deltaTime / duration;
                if (progress > 1.0f)
                {
                    if (mode == SplineWalkerMode.Once)
                        progress = 1.0f;
                    else if (mode == SplineWalkerMode.Loop)
                        progress -= 1.0f;
                    else
                    {
                        progress = 2.0f - progress;
                        goingForward = false;
                    }
                }
            }
            else
            {
                progress -= Time.deltaTime / duration;
                if (progress < 0.0f)
                {
                    progress = -progress;
                    goingForward = true;
                }
            }

            Vector3 position = spline.GetPoint(progress);
            transform.localPosition = position;
            if (lookForward)
                transform.LookAt(position + spline.GetDirection(progress));
        }
    }
}