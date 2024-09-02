using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

    [CustomPropertyDrawer(typeof(TextureImporterSettings))]
    public partial class TextureImporterSettingsPropertyDrawer : PropertyDrawer {
        private const float kSpacingSubLabel = 4;
        private const float kSingleLineHeight = 18f;

        private static readonly int Texture2DOnly =
            AsMask(TextureImporterType.GUI) |
            AsMask(TextureImporterType.Sprite) |
            AsMask(TextureImporterType.Cursor) |
            AsMask(TextureImporterType.Cookie) |
            AsMask(TextureImporterType.Lightmap) |
            AsMask(TextureImporterType.DirectionalLightmap) |
            AsMask(TextureImporterType.Shadowmask);

        private static bool Expanded = true;

        private static readonly GUIContent[] FilterModeOptions = {
            EditorGUIUtility.TrTextContent("Point (no filter)"),
            EditorGUIUtility.TrTextContent("Bilinear"),
            EditorGUIUtility.TrTextContent("Trilinear")
        };

        private static readonly int[] FilterModeValues = Enum.GetValues(typeof(FilterMode)).Cast<int>().ToArray();

        private static int AsMask(TextureImporterType type) => 1 << (int)type;

        private static void HandleDefaultTexture(SerializedProperty root, TextureImporterShape shape) {
            var srgbProp = root.FindPropertyRelative(Variables.m_sRGBTexture);
            var alphaSourceProp = root.FindPropertyRelative(Variables.m_AlphaSource);

            EditorGUILayout.Space();

            srgbProp.intValue = EditorGUILayout.Toggle(
               TextureImporterSettingsStyles.ColorTexture, srgbProp.intValue != 0) ?
               1 : 0;

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
                default:
                    alphaIsTransProp.intValue = EditorGUILayout.Toggle(
                        TextureImporterSettingsStyles.AlphaIsTransparency,
                        alphaIsTransProp.intValue != 0) ? 1 : 0;
                    break;
            }

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

                    if (shape != TextureImporterShape.Texture2DArray) {
                        var isVirtualTxtProp = root.FindPropertyRelative(Variables.m_VTOnly);
                        isVirtualTxtProp.intValue = EditorGUILayout.Toggle(
                            TextureImporterSettingsStyles.VirtualTextureOnly,
                            isVirtualTxtProp.intValue != 0) ? 1 : 0;
                    }

                    var generateMipMapsProp = root.FindPropertyRelative(Variables.m_EnableMipMap);
                    generateMipMapsProp.intValue = EditorGUILayout.Toggle(
                        TextureImporterSettingsStyles.GenerateMipmaps,
                        generateMipMapsProp.intValue != 0) ? 1 : 0;

                    if (generateMipMapsProp.intValue != 0 && shape != TextureImporterShape.Texture3D) {
                        using (new IndentScope(1)) {
                            if (shape != TextureImporterShape.Texture2DArray) {
                                var ignoreMipMapLimitsProp = root.FindPropertyRelative(Variables.m_IgnoreMipmapLimit);
                                var useLimits = EditorGUILayout.Toggle(
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
                            }

                            var mipMapModeProp = root.FindPropertyRelative(Variables.m_MipMapMode);
                            mipMapModeProp.intValue = (int)(TextureImporterMipFilter)EditorGUILayout.EnumPopup(
                                TextureImporterSettingsStyles.MipmapFiltering, (TextureImporterMipFilter)mipMapModeProp.intValue);

                            var preserveCoverageProp = root.FindPropertyRelative(Variables.m_MipMapsPreserveCoverage);
                            var coverage = EditorGUILayout.Toggle(TextureImporterSettingsStyles.PreserveCoverage,
                                preserveCoverageProp.intValue != 0);

                            preserveCoverageProp.intValue = coverage ? 1 : 0;

                            if (coverage) {
                                using var _ = new IndentScope(1);
                                var _b = EditorGUILayout.PropertyField(root.FindPropertyRelative(Variables.m_AlphaTestReferenceValue),
                                    TextureImporterSettingsStyles.AlphaCutoff);
                            }

                            var replicateBorderProp = root.FindPropertyRelative(Variables.m_BorderMipMap);
                            replicateBorderProp.intValue = EditorGUILayout.Toggle(TextureImporterSettingsStyles.ReplicateBorder,
                                replicateBorderProp.intValue != 0) ? 1 : 0;

                            var fadeoutToGrayProp = root.FindPropertyRelative(Variables.m_FadeOut);
                            var fadedOut = EditorGUILayout.Toggle(
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
                    DrawGammaAndSwizzle(root);
                }
            }
        }

        private static void DrawGammaAndSwizzle(SerializedProperty root) {
            // Gamma
            var ignorePngGammaProp = root.FindPropertyRelative(Variables.m_IgnorePngGamma);
            ignorePngGammaProp.intValue = EditorGUILayout.Toggle(
                TextureImporterSettingsStyles.IgnorePNGGamma,
                ignorePngGammaProp.intValue != 0) ? 1 : 0;

            // Do the swizzle here
            using (new IndentScope(1)) {
                var swizzleProp = root.FindPropertyRelative(Variables.m_Swizzle);
                _ = EditorGUI.BeginProperty(
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

        private static void ToggleFromInt(SerializedProperty property, GUIContent label) {
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

        // TODO: For TextureImporterPlatformSettings use the BuildTargetGroup API
        public override void OnGUI(Rect position, SerializedProperty root, GUIContent label) {
            EditorGUI.BeginChangeCheck();
            root.serializedObject.Update();

            var textureTypeProp = root.FindPropertyRelative(Variables.m_TextureType);
            var textureType = (TextureImporterType)EditorGUILayout.EnumPopup(
                TextureImporterSettingsStyles.TextureType,
                (TextureImporterType)textureTypeProp.intValue);
            textureTypeProp.intValue = (int)textureType;

            var textureShapeProp = root.FindPropertyRelative(Variables.m_TextureShape);
            TextureImporterShape textureShape;

            var isTexture2D = (Texture2DOnly & AsMask(textureType)) > 0;

            // To support settings for Point Lights.
            if (textureType == TextureImporterType.Cookie) {
                isTexture2D = cookieLightType != CookieLightType.Point;
            }

            using (isTexture2D ? GUIScope.Disabled() : GUIScope.Enabled()) {
                textureShape = (TextureImporterShape)EditorGUILayout.EnumPopup(
                    TextureImporterSettingsStyles.TextureShape,
                    (TextureImporterShape)textureShapeProp.intValue);
                textureShapeProp.intValue = (int)textureShape;

                switch (textureShape) {
                    case TextureImporterShape.TextureCube:
                        var controlRect = EditorGUILayout.GetControlRect(true, kSingleLineHeight, EditorStyles.popup);

                        // TODO: Implement the function above
                        var cubemapProp = root.FindPropertyRelative(Variables.m_GenerateCubemap);
                        var seamlessMapProp = root.FindPropertyRelative(Variables.m_SeamlessCubemap);

                        var l = EditorGUI.BeginProperty(controlRect, TextureImporterSettingsStyles.Mapping, cubemapProp);

                        EditorGUI.showMixedValue = cubemapProp.hasMultipleDifferentValues || seamlessMapProp.hasMultipleDifferentValues;
                        EditorGUI.BeginChangeCheck();
                        var value = EditorGUI.IntPopup(controlRect, l, cubemapProp.intValue, CubemapOptions, CubemapValues2);

                        if (EditorGUI.EndChangeCheck()) {
                            cubemapProp.intValue = value;
                        }
                        EditorGUI.EndProperty();

                        var cubemapConvolutionProp = root.FindPropertyRelative(Variables.m_CubemapConvolution);

                        using (new IndentScope(1)) {
                            EditorGUILayout.IntPopup(cubemapConvolutionProp,
                                CubemapConvolutionOptions,
                                CubemapConvolutionValues,
                                CubemapConvolution);
                            ToggleFromInt(seamlessMapProp, TextureImporterSettingsStyles.FixupEdgeSeams);
                        }
                        EditorGUILayout.Space();
                        break;
                    case TextureImporterShape.Texture2DArray:
                    case TextureImporterShape.Texture3D:
                        using (new IndentScope(1)) {
                            var columnsProp = root.FindPropertyRelative(Variables.m_FlipbookColumns);
                            var rowsProp = root.FindPropertyRelative(Variables.m_FlipbookRows);
                            _ = EditorGUILayout.PropertyField(columnsProp, TextureImporterSettingsStyles.Columns);
                            _ = EditorGUILayout.PropertyField(rowsProp, TextureImporterSettingsStyles.Rows);
                        }
                        break;
                }
            }

            var showAniso = false;

            switch (textureType) {
                case TextureImporterType.Default:
                    HandleDefaultTexture(root, textureShape);
                    showAniso = true;
                    break;
                case TextureImporterType.NormalMap:
                    HandleNormalTexture(root);
                    break;
                case TextureImporterType.GUI:
                    HandleLegacyGUI(root);
                    break;
                case TextureImporterType.Sprite:
                    HandleSprite(root);
                    break;
                case TextureImporterType.Cursor:
                    HandleCursor(root);
                    break;
                case TextureImporterType.Cookie:
                    HandleCookie(root, ref cookieLightType);
                    break;
                case TextureImporterType.Lightmap:
                    HandleLightmap(root);
                    break;
                case TextureImporterType.DirectionalLightmap:
                    break;
                case TextureImporterType.Shadowmask:
                    break;
                default:
                    break;
            }

            var wrapUProp = root.FindPropertyRelative(Variables.m_WrapU);
            var wrapVProp = root.FindPropertyRelative(Variables.m_WrapV);
            var wrapWProp = root.FindPropertyRelative(Variables.m_WrapW);

            WrapModePopup(wrapVProp, wrapUProp, wrapWProp, false, ref showPerAxisWrapModes, false);

            var filterModeProp = root.FindPropertyRelative(Variables.m_FilterMode);

            // Draw the filter and aniso level
            EditorGUILayout.IntPopup(filterModeProp, FilterModeOptions, FilterModeValues, TextureImporterSettingsStyles.FilterMode);

            using (new GUIScope(showAniso && filterModeProp.intValue != (int)FilterMode.Point)) {
                var anisoProp = root.FindPropertyRelative(Variables.m_Aniso);
                anisoProp.intValue = EditorGUILayout.IntSlider(TextureImporterSettingsStyles.AnisoLevel, anisoProp.intValue, 0, 16);
            }

            if (EditorGUI.EndChangeCheck()) {
                _ = root.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}