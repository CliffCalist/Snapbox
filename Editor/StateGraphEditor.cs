using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WhiteArrow.SnapboxSDK;

namespace WhiteArrowEditor.SnapboxSDK
{
    [CustomEditor(typeof(StateGraph))]
    public class StateGraphEditor : Editor
    {
        private readonly string[] READ_ONLY_FIELD_NAMES =
        {
            "m_Script",
            "size",
            "_isStarted",
            "_graphPhase",
            "_roots"
        };



        public override void OnInspectorGUI()
        {
            EditorGuiUtility.DrawWithReadOnlyFields(serializedObject, READ_ONLY_FIELD_NAMES);

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Graph View(only in play mode)", EditorStyles.boldLabel);

            var rootsProp = serializedObject.FindProperty("_roots");
            if (rootsProp != null && rootsProp.isArray)
            {
                var visited = new HashSet<StateNode>();
                for (int i = 0; i < rootsProp.arraySize; i++)
                {
                    var element = rootsProp.GetArrayElementAtIndex(i);
                    var nodeRef = element.objectReferenceValue as StateNode;
                    if (nodeRef != null)
                        DrawNodeRecursive(nodeRef, 0, visited);
                }
            }
        }

        private void DrawNodeRecursive(StateNode node, int indent, HashSet<StateNode> visited)
        {
            if (node == null || visited.Contains(node))
                return;

            visited.Add(node);

            EditorGUI.indentLevel = indent;
            var context = string.IsNullOrEmpty(node.Context) ? "<empty context>" : node.Context;
            EditorGUILayout.LabelField($"{node.name}  —  {context}");

            var so = new SerializedObject(node);
            var childrenProp = so.FindProperty("_children");

            if (childrenProp != null && childrenProp.isArray)
            {
                for (int i = 0; i < childrenProp.arraySize; i++)
                {
                    var childElement = childrenProp.GetArrayElementAtIndex(i);
                    var childNode = childElement.objectReferenceValue as StateNode;
                    if (childNode != null)
                        DrawNodeRecursive(childNode, indent + 1, visited);
                }
            }
        }
    }
}