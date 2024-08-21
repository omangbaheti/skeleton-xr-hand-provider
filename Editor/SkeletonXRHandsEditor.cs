using UnityEditor;
using UnityEngine;

namespace ubco.ovilab.SkeletonXRHandProvider.Editor
{
    [CustomEditor(typeof(SkeletonXRHands), true)]
    [CanEditMultipleObjects]
    public class SkeletonXRHandsEditor: UnityEditor.Editor
    {
        bool showWarn = false;

        SerializedProperty m_script;
        SerializedProperty rightHandTransforms;
        SerializedProperty leftHandTransforms;
        SerializedProperty forwardAxis;
        SerializedProperty upAxis;
        SerializedProperty disableOtherSubsystems;
        SerializedProperty rightHand;

        protected void OnEnable()
        {
            if (FindObjectsOfType<SkeletonXRHands>().Length > 1)
            {
                showWarn = true;
            }

            m_script = serializedObject.FindProperty("m_Script");
            rightHandTransforms = serializedObject.FindProperty("rightHandTransforms");
            leftHandTransforms = serializedObject.FindProperty("leftHandTransforms");
            forwardAxis = serializedObject.FindProperty("forwardAxis");
            upAxis = serializedObject.FindProperty("upAxis");
            disableOtherSubsystems = serializedObject.FindProperty("disableOtherSubsystems");
            rightHand = serializedObject.FindProperty("rightHand");
        }

        public override void OnInspectorGUI()
        {
            SkeletonXRHands skeleton = (SkeletonXRHands)target;
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_script);
            GUI.enabled = !Application.isPlaying;
            EditorGUILayout.PropertyField(rightHandTransforms);
            EditorGUILayout.PropertyField(leftHandTransforms);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(forwardAxis);
            EditorGUILayout.PropertyField(upAxis);

            GUI.enabled = !Application.isPlaying;
            EditorGUILayout.PropertyField(disableOtherSubsystems);
            EditorGUILayout.PropertyField(rightHand);
            GUI.enabled = true;
            if (showWarn)
            {
                EditorGUILayout.HelpBox("There are more than one SkeletonXRHands. Only the first one that awakes will be used.", MessageType.Warning);
            }
            if (GUILayout.Button("AutoPopulate"))
            {
                skeleton.AutoPopulate();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
