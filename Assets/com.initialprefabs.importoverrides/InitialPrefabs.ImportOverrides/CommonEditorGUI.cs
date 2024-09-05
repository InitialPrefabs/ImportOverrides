using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

    internal static class CommonEditorGUI {
        public static void ToggleFromInt(SerializedProperty property, GUIContent label) {
            var content = EditorGUI.BeginProperty(EditorGUILayout.BeginHorizontal(), label, property);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            var value = EditorGUILayout.Toggle(content, property.intValue > 0) ? 1 : 0;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck()) {
                property.intValue = value;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndProperty();
        }
    }
}
