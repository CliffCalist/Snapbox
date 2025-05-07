using UnityEditor;
using WhiteArrow.SnapboxSDK;

namespace WhiteArrowEditor.SnapboxSDK
{
    [CustomEditor(typeof(InitContext))]
    public class InitContextEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGuiUtility.DrawWithReadOnlyFields(serializedObject, "_state");
        }
    }
}