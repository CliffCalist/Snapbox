using UnityEditor;
using UnityEngine;
using WhiteArrow.SnapboxSDK;

namespace WhiteArrowEditor.SnapboxSDK
{
    [InitializeOnLoad]
    public class StateNodeHierarchyDecorator
    {
        static StateNodeHierarchyDecorator()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            var node = go.GetComponent<StateNode>();
            if (node == null) return;

            string label = $"init:{node.InitIndex}";
            var style = new GUIStyle(EditorStyles.label)
            {
                fontSize = 9,
                alignment = TextAnchor.MiddleRight,
                normal = { textColor = Color.gray }
            };

            var labelRect = new Rect(selectionRect.xMax - 35, selectionRect.y, 30, selectionRect.height);
            EditorGUI.LabelField(labelRect, label, style);
        }
    }
}