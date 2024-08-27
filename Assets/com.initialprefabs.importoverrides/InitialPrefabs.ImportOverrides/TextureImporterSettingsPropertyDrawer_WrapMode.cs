using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

    public partial class TextureImporterSettingsPropertyDrawer {

        private bool showPerAxisWrapModes = false;

        private static readonly GUIContent[] WrapModeContents = {
            EditorGUIUtility.TrTextContent("Repeat"),
            EditorGUIUtility.TrTextContent("Clamp"),
            EditorGUIUtility.TrTextContent("Mirror"),
            EditorGUIUtility.TrTextContent("Mirror Once"),
            EditorGUIUtility.TrTextContent("Per-axis")
        };

        private static readonly int[] WrapModeValues = {
            (int)TextureWrapMode.Repeat,
            (int)TextureWrapMode.Clamp,
            (int)TextureWrapMode.Mirror,
            (int)TextureWrapMode.MirrorOnce,
            -1
        };

        private static void WrapModeAxisPopup(GUIContent label, SerializedProperty wrapProperty) {
            // In texture importer settings, serialized properties for wrap modes can contain -1, which means "use default".
            var wrap = (TextureWrapMode)Mathf.Max(wrapProperty.intValue, 0);
            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(rect, label, wrapProperty);
            wrap = (TextureWrapMode)EditorGUI.EnumPopup(rect, label, wrap);
            EditorGUI.EndProperty();
            if (EditorGUI.EndChangeCheck()) {
                wrapProperty.intValue = (int)wrap;
            }
        }

        private static void WrapModePopup(
            SerializedProperty wrapU,
            SerializedProperty wrapV,
            SerializedProperty wrapW,
            bool isVolumeTexture,
            ref bool showPerAxisWrapModes,
            bool enforcePerAxis) {

            // In texture importer settings, serialized properties for things like wrap modes can contain -1;
            // that seems to indicate "use defaults, user has not changed them to anything" but not totally sure.
            // Show them as Repeat wrap modes in the popups.
            var wu = (TextureWrapMode)Mathf.Max(wrapU.intValue, 0);
            var wv = (TextureWrapMode)Mathf.Max(wrapV.intValue, 0);
            var ww = (TextureWrapMode)Mathf.Max(wrapW.intValue, 0);

            // automatically go into per-axis mode if values are already different
            if (wu != wv) showPerAxisWrapModes = true;
            if (isVolumeTexture) {
                if (wu != ww || wv != ww) showPerAxisWrapModes = true;
            }

            // It's not possible to determine whether any single texture in the whole selection is using per-axis wrap modes
            // just from SerializedProperty values. They can only tell if "some values in whole selection are different" (e.g.
            // wrap value on U axis is not the same among all textures), and can return value of "some" object in the selection
            // (typically based on object loading order). So in order for more intuitive behavior with multi-selection,
            // we go over the actual objects when there's >1 object selected and some wrap modes are different.
            if (!showPerAxisWrapModes) {
                if (wrapU.hasMultipleDifferentValues || wrapV.hasMultipleDifferentValues || (isVolumeTexture && wrapW.hasMultipleDifferentValues)) {
                    if (IsAnyTextureObjectUsingPerAxisWrapMode(wrapU.serializedObject.targetObjects, isVolumeTexture)) {
                        showPerAxisWrapModes = true;
                    }
                }
            }

            int value = showPerAxisWrapModes || enforcePerAxis ? -1 : (int)wu;
            var wrapModeLabel = new GUIContent("Wrap Mode");

            // main wrap mode popup
            if (enforcePerAxis) {
                EditorGUILayout.LabelField(wrapModeLabel);
            } else {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = !showPerAxisWrapModes &&
                    (wrapU.hasMultipleDifferentValues ||
                     wrapV.hasMultipleDifferentValues ||
                     (isVolumeTexture && wrapW.hasMultipleDifferentValues));
                value = EditorGUILayout.IntPopup(wrapModeLabel, value, WrapModeContents, WrapModeValues);
                if (EditorGUI.EndChangeCheck() && value != -1) {
                    // assign the same wrap mode to all axes, and hide per-axis popups
                    wrapU.intValue = value;
                    wrapV.intValue = value;
                    wrapW.intValue = value;
                    showPerAxisWrapModes = false;
                }
                EditorGUI.showMixedValue = false;
            }

            // show per-axis popups if needed
            if (value == -1) {
                showPerAxisWrapModes = true;
                EditorGUI.indentLevel++;
                WrapModeAxisPopup(new GUIContent("U axis"), wrapU);
                WrapModeAxisPopup(new GUIContent("V axis"), wrapV);
                if (isVolumeTexture) {
                    WrapModeAxisPopup(new GUIContent("W axis"), wrapW);
                }
                EditorGUI.indentLevel--;
            }
        }

        private static bool IsAnyTextureObjectUsingPerAxisWrapMode(UnityEngine.Object[] objects, bool isVolumeTexture) {
            foreach (var o in objects) {
                int u = 0, v = 0, w = 0;
                // the objects can be Textures themselves, or texture-related importers
                if (o is Texture) {
                    var ti = (Texture)o;
                    u = (int)ti.wrapModeU;
                    v = (int)ti.wrapModeV;
                    w = (int)ti.wrapModeW;
                }
                if (o is TextureImporter) {
                    var ti = (TextureImporter)o;
                    u = (int)ti.wrapModeU;
                    v = (int)ti.wrapModeV;
                    w = (int)ti.wrapModeW;
                }
                if (o is IHVImageFormatImporter) {
                    var ti = (IHVImageFormatImporter)o;
                    u = (int)ti.wrapModeU;
                    v = (int)ti.wrapModeV;
                    w = (int)ti.wrapModeW;
                }
                u = Mathf.Max(0, u);
                v = Mathf.Max(0, v);
                w = Mathf.Max(0, w);
                if (u != v) {
                    return true;
                }
                if (isVolumeTexture) {
                    if (u != w || v != w) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

