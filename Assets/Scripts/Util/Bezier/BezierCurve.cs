// Credit: https://catlikecoding.com/unity/tutorials/curves-and-splines/
using UnityEngine;

namespace hhotLib.Common
{
    public class BezierCurve : MonoBehaviour
    {
        public Vector3[] points;

        public Vector3 GetPoint(float t)
        {
            return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], points[3], t));
        }

        public Vector3 GetVelocity(float t)
        {
            // The TransformDirection method only takes the object's rotation into account, but we also need to apply its scale.
            // So we transform our vector as if it were a point and then undo the positioning.
            // This way it always produces the correct velocity, even when using a negative scale.
            return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], points[3], t)) - transform.position;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public void Reset()
        {
            points = new Vector3[]
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(2.0f, 0.0f, 0.0f),
                new Vector3(3.0f, 0.0f, 0.0f),
                new Vector3(4.0f, 0.0f, 0.0f),
            };
        }
    }
}