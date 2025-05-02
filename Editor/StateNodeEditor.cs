using UnityEditor;
using UnityEngine;
using WhiteArrow.SnapboxSDK;

namespace WhiteArrowEditor.SnapboxSDK
{
    [CustomEditor(typeof(StateNode))]
    public class StateNodeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var initIndexProp = serializedObject.FindProperty("_initIndex");
            var childrenProp = serializedObject.FindProperty("_children");

            EditorGUILayout.PropertyField(initIndexProp);

            GUI.enabled = false;
            EditorGUILayout.PropertyField(childrenProp, includeChildren: true);
            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
        }
    }
}