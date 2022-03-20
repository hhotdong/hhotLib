﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
	/// <summary>
	/// Quadratic bezier curve point
	/// B(t) = (1 - t) * ((1 - t) * P0 + t * P1) + t * ((1 - t) * P1 + t * P2)
	/// B(t) = (1 - t)^2 * P0 + 2 * (1 - t) * t * P1 + t^2 * P2
	/// </summary>
	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
		//return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);

		t = Mathf.Clamp01(t);
		float oneMinusT = 1.0F - t;
		return
			oneMinusT * oneMinusT * p0 +
			2.0F * oneMinusT * t * p1 +
			t * t * p2;
    }

	/// <summary>
	/// Quadratic first derivative
	/// B'(t) = 2 * (1 - t) * (P1 - P0) + 2 * t * (P2 - P1)
	/// </summary>
	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		return
			2.0F * (1.0F - t) * (p1 - p0) +
			2.0F * t * (p2 - p1);
	}

	/// <summary>
	/// Cubic bezier curve point
	/// B(t) = (1 - t)^3 * P0 + 3*(1 - t)^2 * t * P1 + 3 * (1 - t) * t^2 * P2 + t^3 * P3
	/// </summary>
	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float oneMinusT = 1.0F - t;
		return
			oneMinusT * oneMinusT * oneMinusT * p0 +
			3.0F * oneMinusT * oneMinusT * t * p1 +
			3.0F * oneMinusT * t * t * p2 +
			t * t * t * p3;
	}

	/// <summary>
	/// Cubic first derivative
	/// B'(t) = 3 * (1 - t)^2 * (P1 - P0) + 6 * (1 - t) * t * (P2 - P1) + 3 * t^2 * (P3 - P2)
	/// </summary>
	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float oneMinusT = 1.0F - t;
		return
			3.0F * oneMinusT * oneMinusT * (p1 - p0) +
			6.0F * oneMinusT * t * (p2 - p1) +
			3.0F * t * t * (p3 - p2);
	}
}
