using UnityEditor;
using WhiteArrow.SnapboxSDK;

namespace WhiteArrowEditor.SnapboxSDK
{
    [CustomEditor(typeof(SceneContext))]
    public class SceneContextEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGuiUtility.DrawWithReadOnlyFields(serializedObject, "_restoringPhase");
        }
    }
}