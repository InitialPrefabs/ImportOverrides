using UnityEditor;

namespace InitialPrefabs.ImportOverrides {

    public partial class TextureImporterSettingsPropertyDrawer {

        private static void HandleNormalTexture(SerializedProperty root) {
            EditorGUILayout.Space();
            BumpGUI(root);
            EditorGUILayout.Space();

            // Draw the advanced dropdown
            if (Expanded = EditorGUILayout.Foldout(Expanded, "Advance")) {
                using (new IndentScope(1)) {
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
        }

        private static void BumpGUI(SerializedProperty root) {
            var generateFromBump = root.FindPropertyRelative(Variables.m_ConvertToNormalMap);
            using var _ = ChangeCheckScope.Begin();
            ToggleFromInt(generateFromBump, TextureImporterSettingsStyles.GenerateFromBump);
            using (new IndentScope(1)) {
                if (generateFromBump.intValue == 0) {
                    return;
                }
                var heightScaleProp = root.FindPropertyRelative(Variables.m_HeightScale);
                var normalMapFilterProp = root.FindPropertyRelative(Variables.m_NormalMapFilter);
                EditorGUILayout.Slider(heightScaleProp, 0f, 0.3f, TextureImporterSettingsStyles.Bumpiness);
                normalMapFilterProp.intValue = EditorGUILayout.Popup(
                    TextureImporterSettingsStyles.BumpFilteringOption,
                    normalMapFilterProp.intValue,
                    TextureImporterSettingsStyles.BumpFilteringOptions);
            }

            var flipGreenChannelProp = root.FindPropertyRelative(Variables.m_FlipGreenChannel);
            ToggleFromInt(flipGreenChannelProp, TextureImporterSettingsStyles.FlipGreenChannel);
        }
    }
}
