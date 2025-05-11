using UnityEditor;
using WhiteArrow.SnapboxSDK;

namespace WhiteArrowEditor.SnapboxSDK
{
    [CustomEditor(typeof(EntityStateHandler), true)]
    public class EntityStateHandlerEditor : Editor
    {
        private static readonly string[] READ_ONLY_FIELD_NAMES =
        {
            "_isRegistered", "_isRestored", "_isInitialized"
        };


        public override void OnInspectorGUI()
        {
            EditorGuiUtility.DrawWithReadOnlyFields(serializedObject, READ_ONLY_FIELD_NAMES);
        }
    }
}