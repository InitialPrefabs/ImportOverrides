using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

    [CustomPropertyDrawer(typeof(TextureImporterPlatformSettings))]
    public partial class TextureImporterPlatformSettingsPropertyDrawer : PropertyDrawer {

        private static readonly int[] MaxTextureSizes = {
            32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384
        };

        private static readonly GUIContent[] MaxTextureSizesDisplay = MaxTextureSizes
            .Select(x => new GUIContent(x.ToString()))
            .ToArray();

        // TODO: Better to add this via Code Generator to make maintenace easier.
        private static GUIContent GetOverride(KnownBuildTargets target) =>
            target switch {
                KnownBuildTargets.Standalone => TextureImporterSettingsStyles.OverrideStandalone,
                KnownBuildTargets.Server => TextureImporterSettingsStyles.OverrideServer,
                KnownBuildTargets.iOS => TextureImporterSettingsStyles.OverrideIOS,
                KnownBuildTargets.Android => TextureImporterSettingsStyles.OverrideAndroid,
                KnownBuildTargets.WebGL => TextureImporterSettingsStyles.OverrideWebGL,
                KnownBuildTargets.WindowsStoreApps => TextureImporterSettingsStyles.OverrideWindowsStoreApp,
                KnownBuildTargets.PS4 => TextureImporterSettingsStyles.OverridePS4,
                KnownBuildTargets.XboxOne => TextureImporterSettingsStyles.OverrideXbox,
                KnownBuildTargets.tvOS => TextureImporterSettingsStyles.OverrideForTVOs,
                KnownBuildTargets.VisionOS => TextureImporterSettingsStyles.OverrideForVisionOS,
                KnownBuildTargets.NintendoSwitch => TextureImporterSettingsStyles.OverrideForNintendoSwitch,
                KnownBuildTargets.Stadia => TextureImporterSettingsStyles.OverrideForStadia,
                KnownBuildTargets.LinuxHeadlessSimulation => TextureImporterSettingsStyles.OverrideForLinuxHeadlessSimulation,
                KnownBuildTargets.EmbeddedLinux => TextureImporterSettingsStyles.OverrideForEmbeddedLinux,
                KnownBuildTargets.QNX => TextureImporterSettingsStyles.OverrideForQNX,
                _ => throw new ArgumentException($"Does not support {target} at this time")
            };

        private bool expanded;

        public override void OnGUI(Rect position, SerializedProperty root, GUIContent label) {
            expanded = EditorGUILayout.Foldout(expanded, label);
            if (!expanded) {
                return;
            }

            using (new IndentScope(1)) {
                EditorGUI.BeginChangeCheck();
                root.serializedObject.Update();

                // Draw the known build targets
                var nameProp = root.FindPropertyRelative(Variables.m_Name);
                var target = Enum.TryParse(nameProp.stringValue, out KnownBuildTargets known) ? known : KnownBuildTargets.Unknown;
                target = (KnownBuildTargets)EditorGUILayout.EnumPopup(new GUIContent(nameProp.displayName), target);
                nameProp.stringValue = target.ToString();

                if (target == KnownBuildTargets.Unknown) {
                    if (EditorGUI.EndChangeCheck()) {
                        _ = root.serializedObject.ApplyModifiedProperties();
                    }
                    return;
                }

                var overrideProp = root.FindPropertyRelative(Variables.m_Overridden);
                CommonEditorGUI.ToggleFromInt(overrideProp, GetOverride(target));

                using (new GUIScope(overrideProp.intValue != 0)) {
                    var ignorePlatformSupport = root.FindPropertyRelative(Variables.m_IgnorePlatformSupport);
                    CommonEditorGUI.ToggleFromInt(ignorePlatformSupport, TextureImporterSettingsStyles.IgnorePlatformSupport);

                    var maxSizeProp = root.FindPropertyRelative(Variables.m_MaxTextureSize);
                    maxSizeProp.intValue = EditorGUILayout.IntPopup(
                        TextureImporterSettingsStyles.MaxSize,
                        maxSizeProp.intValue,
                        MaxTextureSizesDisplay,
                        MaxTextureSizes);

                    var resizeAlgorithm = root.FindPropertyRelative(Variables.m_ResizeAlgorithm);
                    var resize = (TextureResizeAlgorithm)EditorGUILayout.EnumPopup(
                        TextureImporterSettingsStyles.ResizeAlgorithm,
                        (TextureResizeAlgorithm)resizeAlgorithm.intValue);
                    resizeAlgorithm.intValue = (int)resize;

                    var textureFormatProp = root.FindPropertyRelative(Variables.m_TextureFormat);
                    var format = (TextureFormat)EditorGUILayout.EnumPopup(
                        TextureImporterSettingsStyles.Format,
                        (TextureFormat)textureFormatProp.intValue);
                    textureFormatProp.intValue = (int)format;

                    var textureCompressionProp = root.FindPropertyRelative(Variables.m_TextureCompression);
                    textureCompressionProp.intValue = EditorGUILayout.IntPopup(
                        TextureImporterSettingsStyles.Compression,
                        textureCompressionProp.intValue,
                        TextureImporterSettingsStyles.TextureCompressionOptions,
                        TextureImporterSettingsStyles.TextureCompressionValues);

                    if (textureCompressionProp.intValue > 0) {
                        var useCrunchCompression = root.FindPropertyRelative(Variables.m_CrunchedCompression);
                        CommonEditorGUI.ToggleFromInt(useCrunchCompression, TextureImporterSettingsStyles.UseCrunchCompression);

                        if (useCrunchCompression.intValue > 0) {
                            var crunchCompressionQuality = root.FindPropertyRelative(Variables.m_CompressionQuality);
                            crunchCompressionQuality.intValue = EditorGUILayout.IntSlider(
                                TextureImporterSettingsStyles.CompressorQuality,
                                crunchCompressionQuality.intValue,
                                0,
                                100);
                        }
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(TextureImporterSettingsStyles.AdditionalOptions, EditorStyles.boldLabel);
                    var allowAlphaSplitting = root.FindPropertyRelative(Variables.m_AllowsAlphaSplitting);
                    CommonEditorGUI.ToggleFromInt(allowAlphaSplitting, TextureImporterSettingsStyles.AllowAlphaSplitting);

                    if (target == KnownBuildTargets.Android) {
                        var fallback = root.FindPropertyRelative(Variables.m_AndroidETC2FallbackOverride);
                        fallback.intValue = (int)(AndroidETC2FallbackOverride)EditorGUILayout.EnumPopup(
                            TextureImporterSettingsStyles.ETC2AndroidFallbackOverride,
                            (AndroidETC2FallbackOverride)fallback.intValue);
                    }
                }

                if (EditorGUI.EndChangeCheck()) {
                    _ = root.serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(TextureImporterSettingsStyles.HelpfulLinks, EditorStyles.boldLabel);
            if (EditorGUILayout.LinkButton(TextureImporterSettingsStyles.TextureFormatHelp)) {
                Application.OpenURL("https://docs.unity3d.com/Manual/class-TextureImporterOverride.html#recommended-formats");
            }
        }
    }
}