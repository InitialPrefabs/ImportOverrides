using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

    public enum CookieLightType {
        Spot = 0,
        Directional = 1,
        Point = 2,
    }

    public partial class TextureImporterSettingsPropertyDrawer {

        private CookieLightType cookieLightType = CookieLightType.Spot;

        private static void SetCookieLightTypeDefaults(SerializedProperty root, CookieLightType cookieLightType) {
            var borderMipMap = root.FindPropertyRelative(Variables.m_BorderMipMap);
            var wrapU = root.FindPropertyRelative(Variables.m_WrapU);
            var wrapV = root.FindPropertyRelative(Variables.m_WrapV);
            var wrapW = root.FindPropertyRelative(Variables.m_WrapW);
            var generateCubeMap = root.FindPropertyRelative(Variables.m_GenerateCubemap);
            var textureShape = root.FindPropertyRelative(Variables.m_TextureShape);

            // Note that, out of all of these, only the TextureShape is truly strongly enforced.
            // The other settings are nothing more than recommended defaults and can be modified
            // by the user at any time.
            switch (cookieLightType) {
                case CookieLightType.Spot:
                    borderMipMap.intValue = 1;
                    wrapU.intValue = wrapV.intValue = wrapW.intValue = (int)TextureWrapMode.Clamp;
                    generateCubeMap.intValue = (int)TextureImporterGenerateCubemap.AutoCubemap;
                    textureShape.intValue = (int)TextureImporterShape.Texture2D;
                    break;
                case CookieLightType.Point:
                    borderMipMap.intValue = 0;
                    wrapU.intValue = wrapV.intValue = wrapW.intValue = (int)TextureWrapMode.Clamp;
                    generateCubeMap.intValue = (int)TextureImporterGenerateCubemap.Spheremap;
                    textureShape.intValue = (int)TextureImporterShape.TextureCube;
                    break;
                case CookieLightType.Directional:
                    borderMipMap.intValue = 0;
                    wrapU.intValue = wrapV.intValue = wrapW.intValue = (int)TextureWrapMode.Repeat;
                    generateCubeMap.intValue = (int)TextureImporterGenerateCubemap.AutoCubemap;
                    textureShape.intValue = (int)TextureImporterShape.Texture2D;
                    break;
            }
        }

        private static void DrawLightSettings(SerializedProperty root, ref CookieLightType type) {
            // TODO: The texture importer settings doesn't store the cookie light type
            // For now we store a local variable
            type = (CookieLightType)EditorGUILayout.Popup(
                TextureImporterSettingsStyles.CookieType,
                (int)type,
                TextureImporterSettingsStyles.CookieOptions);
            SetCookieLightTypeDefaults(root, type);
        }

        private static void HandleCookie(SerializedProperty root, ref CookieLightType type) {
            EditorGUILayout.Space();
            // Draw the lighting settings
            DrawLightSettings(root, ref type);

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
                            var useLimits = EditorGUILayout.Toggle(
                                TextureImporterSettingsStyles.MipmapLimits,
                                ignoreMipMapLimitsProp.intValue == 0);
                            ignoreMipMapLimitsProp.intValue = useLimits ? 0 : 1;
                            // TODO: Limits are not shown here, figure out where they are stored

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
                }
            }
        }
    }
}