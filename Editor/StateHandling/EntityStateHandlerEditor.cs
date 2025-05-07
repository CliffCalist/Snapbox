using UnityEditor;
using WhiteArrow.SnapboxSDK;

namespace WhiteArrowEditor.SnapboxSDK
{
    [CustomEditor(typeof(EntityStateHandler), true)]
    public class EntityStateHandlerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGuiUtility.DrawWithReadOnlyFields(serializedObject, "_isInitialized");
        }
    }
}