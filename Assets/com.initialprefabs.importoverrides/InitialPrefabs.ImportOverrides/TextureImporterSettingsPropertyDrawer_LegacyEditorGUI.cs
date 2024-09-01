using UnityEditor;

namespace InitialPrefabs.ImportOverrides {

    public partial class TextureImporterSettingsPropertyDrawer {

        private static void HandleLegacyGUI(SerializedProperty root) {
            EditorGUILayout.Space();

            // Draw the advanced dropdown
            if (Expanded = EditorGUILayout.Foldout(Expanded, "Advance")) {
                using (new IndentScope(1)) {
                    var alphaSourceProp = root.FindPropertyRelative(Variables.m_AlphaSource);

                    TextureImporterAlphaSource alphaSource = (TextureImporterAlphaSource)EditorGUILayout.EnumPopup(
                        TextureImporterSettingsStyles.AlphaSource,
                        (TextureImporterAlphaSource)alphaSourceProp.intValue);
                    alphaSourceProp.intValue = (int)alphaSource;

                    var alphaIsTransProp = root.FindPropertyRelative(Variables.m_AlphaIsTransparency);

                    switch (alphaSource) {
                        case TextureImporterAlphaSource.None:
                            using (GUIScope.Disabled()) {
                                alphaIsTransProp.intValue = EditorGUILayout.Toggle(
                                    TextureImporterSettingsStyles.AlphaIsTransparency,
                                    alphaIsTransProp.intValue != 0) ? 1 : 0;
                            }
                            break;
                        default:
                            alphaIsTransProp.intValue = EditorGUILayout.Toggle(
                                TextureImporterSettingsStyles.AlphaIsTransparency,
                                alphaIsTransProp.intValue != 0) ? 1 : 0;
                            break;
                    }

                    var npotProp = root.FindPropertyRelative(Variables.m_NPOTScale);
                    var npot = (TextureImporterNPOTScale)EditorGUILayout.EnumPopup(
                        TextureImporterSettingsStyles.NOP2,
                        (TextureImporterNPOTScale)npotProp.intValue);
                    npotProp.intValue = (int)npot;

                    var isReadableProp = root.FindPropertyRelative(Variables.m_IsReadable);
                    isReadableProp.intValue = EditorGUILayout.Toggle(
                        TextureImporterSettingsStyles.ReadWrite,
                        isReadableProp.intValue != 0) ? 1 : 0;

                    var generateMipMapsProp = root.FindPropertyRelative(Variables.m_EnableMipMap);
                    generateMipMapsProp.intValue = EditorGUILayout.Toggle(
                        TextureImporterSettingsStyles.GenerateMipmaps,
                        generateMipMapsProp.intValue != 0) ? 1 : 0;

                    // Gamma
                    var ignorePngGammaProp = root.FindPropertyRelative(Variables.m_IgnorePngGamma);
                    ignorePngGammaProp.intValue = EditorGUILayout.Toggle(
                        TextureImporterSettingsStyles.IgnorePNGGamma,
                        ignorePngGammaProp.intValue != 0) ? 1 : 0;

                    // Do the swizzle here
                    using (new IndentScope(1)) {
                        var swizzleProp = root.FindPropertyRelative(Variables.m_Swizzle);
                        EditorGUI.BeginProperty(
                            EditorGUILayout.BeginHorizontal(),
                            TextureImporterSettingsStyles.Swizzle,
                            swizzleProp);
                        EditorGUI.BeginChangeCheck();

                        EditorGUI.showMixedValue = swizzleProp.hasMultipleDifferentValues;
                        var value = SwizzleField(TextureImporterSettingsStyles.Swizzle, swizzleProp.uintValue);
                        EditorGUI.showMixedValue = false;

                        if (EditorGUI.EndChangeCheck()) {
                            swizzleProp.uintValue = value;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndProperty();
                    }
                }
            }
            EditorGUILayout.Space();
        }
    }
}

