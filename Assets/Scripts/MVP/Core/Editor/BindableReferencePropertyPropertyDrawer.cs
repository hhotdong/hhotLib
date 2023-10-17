using UnityEngine;
using UnityEditor;

namespace hhotLib.Common.MVP
{
    [CustomPropertyDrawer(typeof(BindableReferenceProperty<>))]
    public class BindableReferencePropertyPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty prop = property.FindPropertyRelative("_value");
            EditorGUI.PropertyField(position, prop, label);
        }
    }
}