using UnityEditor;
using WhiteArrow.SnapboxSDK;

namespace WhiteArrowEditor.SnapboxSDK
{
    [CustomEditor(typeof(InitPipeline))]
    public class InitPieplineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGuiUtility.DrawWithReadOnlyFields(serializedObject, "_isStarted");
        }
    }
}