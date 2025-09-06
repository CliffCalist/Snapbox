using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WhiteArrow.Snapbox;

namespace WhiteArrowEditor.SnapboxSDK
{
    [CustomEditor(typeof(SceneStateHandler))]
    public class SceneStateHandlerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Find Root Handlers"))
            {
                var handler = (SceneStateHandler)target;
                var so = new SerializedObject(handler);
                var property = so.FindProperty("_rootHandlers");

                var sceneRoots = handler.gameObject.scene.GetRootGameObjects();
                var found = new HashSet<EntityStateHandler>();

                foreach (var root in sceneRoots)
                    SearchRecursively(root.transform, found);

                foreach (var h in found)
                {
                    bool alreadyExists = false;

                    for (int i = 0; i < property.arraySize; i++)
                    {
                        var element = property.GetArrayElementAtIndex(i);
                        if (element.objectReferenceValue == h)
                        {
                            alreadyExists = true;
                            break;
                        }
                    }

                    if (!alreadyExists)
                    {
                        int newIndex = property.arraySize;
                        property.InsertArrayElementAtIndex(newIndex);
                        property.GetArrayElementAtIndex(newIndex).objectReferenceValue = h;
                    }
                }

                so.ApplyModifiedProperties();
            }
        }

        private void SearchRecursively(Transform current, HashSet<EntityStateHandler> result)
        {
            if (current.TryGetComponent(out EntityStateHandler handler))
            {
                result.Add(handler);
                return;
            }

            foreach (Transform child in current)
                SearchRecursively(child, result);
        }
    }
}