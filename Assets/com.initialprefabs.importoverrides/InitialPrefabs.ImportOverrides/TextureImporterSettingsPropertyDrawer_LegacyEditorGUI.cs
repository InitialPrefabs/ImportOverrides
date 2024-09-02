using UnityEditor;

namespace InitialPrefabs.ImportOverrides {

    public partial class TextureImporterSettingsPropertyDrawer {

        private static void HandleLegacyGUI(SerializedProperty root) {
            EditorGUILayout.Space();

            // Draw the advanced dropdown
            if (Expanded = EditorGUILayout.Foldout(Expanded, "Advance")) {
                using (new IndentScope(1)) {
                    var alphaSourceProp = root.FindPropertyRelative(Variables.m_AlphaSource);

                    var alphaSource = (TextureImporterAlphaSource)EditorGUILayout.EnumPopup(
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
                        case TextureImporterAlphaSource.FromInput:
                            break;
                        case TextureImporterAlphaSource.FromGrayScale:
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
                }
            }
            EditorGUILayout.Space();
        }
    }
}