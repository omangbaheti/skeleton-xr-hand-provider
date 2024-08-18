using UnityEngine;
using UnityEditor;
using ubco.ovilab.SkeletonXRHandProvider;

namespace ubco.ovilab.SkeletonXRHandProvider.Editor
{
    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(SkeletonKeyPair), true)]
    public class SkeletonKeyPairPropertyDrawer: PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty typeProp = property.FindPropertyRelative("jointID");
            SerializedProperty targetProp = property.FindPropertyRelative("transform");

            float width = EditorGUIUtility.currentViewWidth;
            float targetHeight = EditorGUIUtility.singleLineHeight;

            // Calculate rects
            Rect typeRect = new Rect(position.x, position.y, position.width * 0.33f, targetHeight);
            Rect taregetRect = new Rect(position.x + position.width * 0.35f, position.y, position.width * 0.65f, targetHeight);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);
            EditorGUI.PropertyField(taregetRect, targetProp, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}
