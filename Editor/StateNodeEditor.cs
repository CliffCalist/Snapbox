using System.Linq;
using UnityEditor;
using UnityEngine;
using WhiteArrow.SnapboxSDK;

namespace WhiteArrowEditor.SnapboxSDK
{
    [CustomEditor(typeof(StateNode), true)]
    public class StateNodeEditor : Editor
    {
        private const string INIT_INDEX_FIELD_NAME = "_initIndex";
        private const string CHILDREN_FIELD_NAME = "_children";



        private readonly string[] FIELD_NAMES_FOR_HIDE =
        {
            INIT_INDEX_FIELD_NAME,
            CHILDREN_FIELD_NAME,
            "m_Script",
            "size"
        };



        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var initIndexProp = serializedObject.FindProperty(INIT_INDEX_FIELD_NAME);
            var childrenProp = serializedObject.FindProperty(CHILDREN_FIELD_NAME);

            // Draw _initIndex as editable
            EditorGUILayout.PropertyField(initIndexProp);

            // Draw _children as read-only
            GUI.enabled = false;
            EditorGUILayout.PropertyField(childrenProp, includeChildren: true);
            GUI.enabled = true;

            // Draw all other visible serialized properties
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (FIELD_NAMES_FOR_HIDE.Contains(iterator.name))
                    continue;

                EditorGUILayout.PropertyField(iterator, includeChildren: true);
                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}