using System;
using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

    [CustomPropertyDrawer(typeof(TextureImporterPlatformSettings))]
    public partial class TextureImporterPlatformSettingsPropertyDrawer : PropertyDrawer {

        private bool expanded;

        public override void OnGUI(Rect position, SerializedProperty root, GUIContent label) {
            expanded = EditorGUILayout.Foldout(expanded, label);
            if (!expanded) {
                return;
            }

            EditorGUI.BeginChangeCheck();
            root.serializedObject.Update();

            // Draw the known build targets
            var nameProp = root.FindPropertyRelative(Variables.m_Name);
            var target = Enum.TryParse(nameProp.stringValue, out KnownBuildTargets known) ? known : KnownBuildTargets.Unknown;
            target = (KnownBuildTargets)EditorGUILayout.EnumPopup(new GUIContent(nameProp.displayName), target);
            nameProp.stringValue = target.ToString();

            if (EditorGUI.EndChangeCheck()) {
                _ = root.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}