using UnityEngine;

public class BezierCurve : MonoBehaviour
{
	public Vector3[] points;

    /// <summary>
    /// Quadratic
    /// </summary>
    //public Vector3 GetPoint(float t)
    //{
    //    return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], t));
    //}
    //public Vector3 GetVelocity (float t)
    //{
	//	return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], t)) - transform.position;
	//}

    /// <summary>
    /// Cubic
    /// </summary>
    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], points[3], t));
    }

    public Vector3 GetVelocity (float t)
    {
		return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], points[3], t)) - transform.position;
	}

    public Vector3 GetDirection (float t)
    {
		return GetVelocity(t).normalized;
	}

    public void Reset()
	{
		points = new Vector3[]
		{
			new Vector3(1.0F, 0.0F, 0.0F),
			new Vector3(2.0F, 0.0F, 0.0F),
			new Vector3(3.0F, 0.0F, 0.0F),
            new Vector3(4.0F, 0.0F, 0.0F),
		};
	}
}
