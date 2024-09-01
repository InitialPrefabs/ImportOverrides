using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

    public partial class TextureImporterSettingsPropertyDrawer {

        private static readonly int SwizzleFieldHash = "SwizzleField".GetHashCode();

        private static readonly string[] SwizzleOptions = { "R", "G", "B", "A", "1-R", "1-G", "1-B", "1-A", "0", "1" };

        private static readonly Type[] MultiFieldPrefixLabelTypeArgs = { typeof(Rect), typeof(int), typeof(GUIContent), typeof(int) };

        private static readonly ParameterModifier[] Modifiers = new ParameterModifier[1];

        private static uint SwizzleField(GUIContent label, uint swizzle) {
            const float singleLineHeight = 18f;

            var rect = EditorGUILayout.GetControlRect(true, 
                EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, label), EditorStyles.numberField);
            ReflectionUtils.SetField<EditorGUILayout>(null, rect, "s_LastRect", BindingFlags.NonPublic | BindingFlags.Static);
            var id = GUIUtility.GetControlID(SwizzleFieldHash, FocusType.Keyboard, rect);

            // Set the label
            Modifiers[0] = new ParameterModifier(4);
            for (int i = 0; i < 4; i++) {
                Modifiers[0][i] = false;
            }

            var newRect = ReflectionUtils.Invoke<EditorGUI>(
                null,
                "MultiFieldPrefixLabel",
                BindingFlags.NonPublic | BindingFlags.Static,
                MultiFieldPrefixLabelTypeArgs,
                new object[] { rect, id, label, 4 },
                Modifiers);

            rect = newRect != null ? (Rect)newRect : rect;

            rect.height = singleLineHeight; // The internal single line height
            var w = (rect.width - 3 * kSpacingSubLabel) / 4;
            var subRect = new Rect(rect) { width = w };
            var oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            for (int i = 0; i < 4; i++) {
                int shift = 8 * i;
                uint swz = (swizzle >> shift) & 0xFF;
                swz = (uint)EditorGUI.Popup(subRect, (int)swz, SwizzleOptions);
                swizzle &= ~(0xFFu << shift);
                swizzle |= swz << shift;
                subRect.x += w + kSpacingSubLabel;
            }
            EditorGUI.indentLevel = oldIndent;
            return swizzle;
        }
    }
}

