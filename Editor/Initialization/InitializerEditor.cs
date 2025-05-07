using UnityEditor;
using WhiteArrow.SnapboxSDK;

namespace WhiteArrowEditor.SnapboxSDK
{
    [CustomEditor(typeof(Initializer), true)]
    public class InitializerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGuiUtility.DrawWithReadOnlyFields(serializedObject, "_isInitialized");
        }
    }
}