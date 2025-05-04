using UnityEditor;
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
        }
    }
}