// Credit: https://catlikecoding.com/unity/tutorials/curves-and-splines/
using UnityEditor;
using UnityEngine;

namespace hhotLib.Common
{
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveInspector : Editor
    {
        private BezierCurve curve;
        private Transform   handleTransform;
        private Quaternion  handleRotation;

	    private const int   LINE_STEPS      = 10;
	    private const float DIRECTION_SCALE =  0.5F;

        private void OnSceneGUI()
        {
            curve           = target as BezierCurve;
            handleTransform = curve.transform;
            handleRotation  = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            Vector3 p0 = ShowPoint(0);
            Vector3 p1 = ShowPoint(1);
            Vector3 p2 = ShowPoint(2);
		    Vector3 p3 = ShowPoint(3);

		    Handles.color = Color.red;
		    Handles.DrawLine(p0, p1);
		    Handles.DrawLine(p2, p3);

		    ShowDirections();
		    Handles.DrawBezier(p0, p3, p1, p2, Color.blue, null, 2.0f);
	    }

	    private Vector3 ShowPoint(int index)
        {
            Vector3 point = handleTransform.TransformPoint(curve.points[index]);
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                EditorUtility.SetDirty(curve);
                curve.points[index]= handleTransform.InverseTransformPoint(point);
            }
            return point;
        }

	    private void ShowDirections()
	    {
		    Handles.color = Color.green;
		    Vector3 point = curve.GetPoint(0.0f);
		    Handles.DrawLine(point, point + curve.GetDirection(0.0f) * DIRECTION_SCALE);
		    for (int i = 1; i <= LINE_STEPS; i++)
		    {
			    point = curve.GetPoint(i / (float)LINE_STEPS);
			    Handles.DrawLine(point, point + curve.GetDirection(i / (float)LINE_STEPS) * DIRECTION_SCALE);
		    }
	    }
    }
}