using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using ubco.ovilab.SkeletonXRHandProvider;

namespace ubco.ovilab.SkeletonXRHandProvider.Editor
{
    [CustomEditor(typeof(SkeletonXRHands), true)]
    [CanEditMultipleObjects]
    public class SkeletonXRHandsEditor: UnityEditor.Editor
    {
        bool showWarn = false;
        protected void OnEnable()
        {
            if (FindObjectsOfType<SkeletonXRHands>().Length > 1)
            {
                showWarn = true;
            }
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = !Application.isPlaying;
            base.OnInspectorGUI();
            GUI.enabled = true;
            if (showWarn)
            {
                EditorGUILayout.HelpBox("There are more than one SkeletonXRHands. Only the first one that awakes will be used.", MessageType.Warning);
            }
        }
    }
}
