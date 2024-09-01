using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {
    public partial class TextureImporterSettingsPropertyDrawer {
        private static readonly int[] SpriteValues = { 1, 2, 3 };
        private static readonly int[] SpriteMeshOptionValues = { 0, 1 };

        private static void HandleSprite(SerializedProperty root) {
            EditorGUILayout.Space();
            SpriteGUI(root);
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

                    if (generateMipMapsProp.intValue != 0) {
                        using (new IndentScope(1)) {
                            var ignoreMipMapLimitsProp = root.FindPropertyRelative(Variables.m_IgnoreMipmapLimit);
                            bool useLimits = EditorGUILayout.Toggle(
                                TextureImporterSettingsStyles.MipmapLimits,
                                ignoreMipMapLimitsProp.intValue == 0);
                            ignoreMipMapLimitsProp.intValue = useLimits ? 0 : 1;
                            // TODO: Limits are not shown here, figure out where they are stored
                            var mipStreamingProp = root.FindPropertyRelative(Variables.m_StreamingMipmaps);
                            var isStreaming = EditorGUILayout.Toggle(
                                TextureImporterSettingsStyles.MipStreaming,
                                mipStreamingProp.intValue != 0);

                            mipStreamingProp.intValue = isStreaming ? 1 : 0;
                            if (isStreaming) {
                                using var _ = new IndentScope(1);
                                // Draw the priority for streaming
                                var priorityProp = root.FindPropertyRelative(Variables.m_StreamingMipmapsPriority);
                                priorityProp.intValue = Mathf.Clamp(EditorGUILayout.IntField(
                                    TextureImporterSettingsStyles.MipmapPriority,
                                    priorityProp.intValue), -128, 127);
                            }

                            var mipMapModeProp = root.FindPropertyRelative(Variables.m_MipMapMode);
                            mipMapModeProp.intValue = (int)(TextureImporterMipFilter)EditorGUILayout.EnumPopup(
                                TextureImporterSettingsStyles.MipmapFiltering, (TextureImporterMipFilter)mipMapModeProp.intValue);

                            var preserveCoverageProp = root.FindPropertyRelative(Variables.m_MipMapsPreserveCoverage);
                            bool coverage = EditorGUILayout.Toggle(TextureImporterSettingsStyles.PreserveCoverage,
                                preserveCoverageProp.intValue != 0);

                            preserveCoverageProp.intValue = coverage ? 1 : 0;

                            if (coverage) {
                                using var _ = new IndentScope(1);
                                EditorGUILayout.PropertyField(root.FindPropertyRelative(Variables.m_AlphaTestReferenceValue),
                                    TextureImporterSettingsStyles.AlphaCutoff);
                            }

                            var replicateBorderProp = root.FindPropertyRelative(Variables.m_BorderMipMap);
                            replicateBorderProp.intValue = EditorGUILayout.Toggle(TextureImporterSettingsStyles.ReplicateBorder,
                                replicateBorderProp.intValue != 0) ? 1 : 0;

                            var fadeoutToGrayProp = root.FindPropertyRelative(Variables.m_FadeOut);
                            bool fadedOut = EditorGUILayout.Toggle(
                                TextureImporterSettingsStyles.FadeoutToGray,
                                fadeoutToGrayProp.intValue != 0);

                            fadeoutToGrayProp.intValue = fadedOut ? 1 : 0;
                            if (fadedOut) {
                                using var _ = new IndentScope(1);
                                EditorGUI.BeginChangeCheck();
                                var startProp = root.FindPropertyRelative(Variables.m_MipMapFadeDistanceStart);
                                var endProp = root.FindPropertyRelative(Variables.m_MipMapFadeDistanceEnd);

                                var min = (float)startProp.intValue;
                                var max = (float)endProp.intValue;
                                EditorGUILayout.MinMaxSlider(TextureImporterSettingsStyles.FadeRange, ref min, ref max, 0, 10);

                                if (EditorGUI.EndChangeCheck()) {
                                    startProp.intValue = Mathf.RoundToInt(min);
                                    endProp.intValue = Mathf.RoundToInt(max);
                                }
                            }
                        }
                    }
                }
            }
            EditorGUILayout.Space();
        }

        private static void SpriteGUI(SerializedProperty root) {
            var spriteMode = root.FindPropertyRelative(Variables.m_SpriteMode);
            var spritePixelsToUnits = root.FindPropertyRelative(Variables.m_SpritePixelsToUnits);
            var spriteMeshType = root.FindPropertyRelative(Variables.m_SpriteMeshType);
            var spriteExtrude = root.FindPropertyRelative(Variables.m_SpriteExtrude);
            var alignment = root.FindPropertyRelative(Variables.m_Alignment);
            var spritePivot = root.FindPropertyRelative(Variables.m_SpritePivot);
            var spriteGenerateFallbackPhysicsShape = root.FindPropertyRelative(Variables.m_SpriteGenerateFallbackPhysicsShape);

            // Sprite mode selection
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.IntPopup(
                spriteMode,
                TextureImporterSettingsStyles.SpriteModeOptions,
                SpriteValues, TextureImporterSettingsStyles.SpriteMode);

            // Ensure that PropertyField focus will be cleared when we change spriteMode.
            if (EditorGUI.EndChangeCheck()) {
                GUIUtility.keyboardControl = 0;
            }

            EditorGUI.indentLevel++;

            // Show generic attributes
            EditorGUILayout.PropertyField(spritePixelsToUnits, TextureImporterSettingsStyles.spritePixelsPerUnit);

            EditorGUILayout.IntPopup(spriteMeshType,
                TextureImporterSettingsStyles.SpriteMeshTypeOptions,
                SpriteMeshOptionValues, TextureImporterSettingsStyles.spriteMeshType);

            EditorGUILayout.IntSlider(spriteExtrude, 0, 32, TextureImporterSettingsStyles.spriteExtrude);

            if (spriteMode.intValue == (int)SpriteImportMode.Single) {
                alignment.intValue = EditorGUILayout.Popup(
                    TextureImporterSettingsStyles.spriteAlignment,
                    alignment.intValue, TextureImporterSettingsStyles.spriteAlignmentOptions);

                if (alignment.intValue == (int)SpriteAlignment.Custom) {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(spritePivot, TextureImporterSettingsStyles.EmptyContent);
                    GUILayout.EndHorizontal();
                }
            }

            if (spriteMode.intValue != (int)SpriteImportMode.Polygon) {
                ToggleFromInt(spriteGenerateFallbackPhysicsShape, TextureImporterSettingsStyles.spriteGenerateFallbackPhysicsShape);
            }

            EditorGUI.indentLevel--;
            // Do not draw the sprite window utility, that's leaved per importer
        }
    }
}

