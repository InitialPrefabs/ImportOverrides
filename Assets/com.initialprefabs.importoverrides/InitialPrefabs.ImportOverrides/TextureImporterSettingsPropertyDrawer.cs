using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

    [CustomPropertyDrawer(typeof(TextureImporterSettings))]
    public class TextureImporterSettingsPropertyDrawer : PropertyDrawer {

        private const float kSpacingSubLabel = 4;
        private static readonly int Texture2DOnly =
            AsMask(TextureImporterType.GUI) |
            AsMask(TextureImporterType.Sprite) |
            AsMask(TextureImporterType.Cursor) |
            AsMask(TextureImporterType.Cookie) |
            AsMask(TextureImporterType.Lightmap) |
            AsMask(TextureImporterType.DirectionalLightmap) |
            AsMask(TextureImporterType.Shadowmask);

        private static int AsMask(TextureImporterType type) => 1 << (int)type;

        private static bool Folded = false;

        private static void HandleDefaultTexture(SerializedProperty root) {
            var srgbProp = root.FindPropertyRelative(Variables.m_sRGBTexture);
            var alphaSourceProp = root.FindPropertyRelative(Variables.m_AlphaSource);

            EditorGUILayout.Space();

            srgbProp.intValue = EditorGUILayout.Toggle(
                new GUIContent("sRGB (Color Texture)"), srgbProp.intValue != 0) ?
                1 : 0;

            TextureImporterAlphaSource alphaSource = (TextureImporterAlphaSource)EditorGUILayout.EnumPopup(
                new GUIContent("Alpha Source"),
                (TextureImporterAlphaSource)alphaSourceProp.intValue);
            alphaSourceProp.intValue = (int)alphaSource;

            var alphaIsTransProp = root.FindPropertyRelative(Variables.m_AlphaIsTransparency);

            switch (alphaSource) {
                case TextureImporterAlphaSource.None:
                    using (GUIScope.Disabled()) {
                        alphaIsTransProp.intValue = EditorGUILayout.Toggle(
                            new GUIContent("Alpha Is Transparency"),
                            alphaIsTransProp.intValue != 0) ? 1 : 0;
                    }
                    break;
                default:
                    alphaIsTransProp.intValue = EditorGUILayout.Toggle(
                        new GUIContent("Alpha Is Transparency"),
                        alphaIsTransProp.intValue != 0) ? 1 : 0;
                    break;
            }

            EditorGUILayout.Space();

            // Draw the advanced dropdown
            if (Folded = EditorGUILayout.Foldout(Folded, "Advance")) {
                using (new IndentScope(1)) {
                    var npotProp = root.FindPropertyRelative(Variables.m_NPOTScale);
                    var npot = (TextureImporterNPOTScale)EditorGUILayout.EnumPopup(
                        new GUIContent("Non-Power of 2", "How non power of twos are scaled on import."),
                        (TextureImporterNPOTScale)npotProp.intValue);
                    npotProp.intValue = (int)npot;

                    var isReadableProp = root.FindPropertyRelative(Variables.m_IsReadable);
                    isReadableProp.intValue = EditorGUILayout.Toggle(
                        new GUIContent("Read/Write", "Enable access to raw pixel from code."),
                        isReadableProp.intValue != 0) ? 1 : 0;

                    var isVirtualTxtProp = root.FindPropertyRelative(Variables.m_VTOnly);
                    isVirtualTxtProp.intValue = EditorGUILayout.Toggle(
                        new GUIContent("Virtual Texture Only",
                            "Texture is optimized for use as a virtual texture and can only be used as a Virtual Texture."),
                        isVirtualTxtProp.intValue != 0) ? 1 : 0;

                    var generateMipMapsProp = root.FindPropertyRelative(Variables.m_EnableMipMap);
                    generateMipMapsProp.intValue = EditorGUILayout.Toggle(
                        new GUIContent("Generate Mipmaps", "Create progressively smaller versions of the texture, " +
                            "for reduced texture shimmering and better GPU performance when the texture is viewed at a distance."),
                        generateMipMapsProp.intValue != 0) ? 1 : 0;

                    if (generateMipMapsProp.intValue != 0) {
                        using (new IndentScope(1)) {
                            var ignoreMipMapLimitsProp = root.FindPropertyRelative(Variables.m_IgnoreMipmapLimit);
                            bool useLimits = EditorGUILayout.Toggle(new GUIContent("Use Mipmap Limits",
                                "Disable this if the number of mips to to upload should not be limited by the " +
                                "quality settings. (effectively: always upload at full resolution, regardless of " +
                                "the global mipmap limit or mipmap limit group."),
                                ignoreMipMapLimitsProp.intValue == 0);
                            ignoreMipMapLimitsProp.intValue = useLimits ? 0 : 1;
                            // TODO: Limits are not shown here, figure out where they are stored

                            var mipStreamingProp = root.FindPropertyRelative(Variables.m_StreamingMipmaps);
                            var isStreaming = EditorGUILayout.Toggle(
                                new GUIContent("Mip Steaming",
                                "Only load larger mipmaps as needed to render the current game cameras. " +
                                "Required texture streaming to be enabled in quality settings."),
                                mipStreamingProp.intValue != 0);

                            mipStreamingProp.intValue = isStreaming ? 1 : 0;
                            if (isStreaming) {
                                using var _ = new IndentScope(1);
                                // Draw the priority for streaming
                                var priorityProp = root.FindPropertyRelative(Variables.m_StreamingMipmapsPriority);
                                priorityProp.intValue = Mathf.Clamp(EditorGUILayout.IntField(
                                    new GUIContent("Priority", "Mipmap streaming priority when there's contention for " +
                                    "resources. Positive numbers represent higher priority. Valid range is -128 to 127."),
                                    priorityProp.intValue), -128, 127);
                            }

                            var mipMapModeProp = root.FindPropertyRelative(Variables.m_MipMapMode);
                            mipMapModeProp.intValue = (int)(TextureImporterMipFilter)EditorGUILayout.EnumPopup(
                                new GUIContent("MipmapFiltering"), (TextureImporterMipFilter)mipMapModeProp.intValue);

                            var preserveCoverageProp = root.FindPropertyRelative(Variables.m_MipMapsPreserveCoverage);
                            bool coverage = EditorGUILayout.Toggle(new GUIContent(
                                "Preserve Coverage",
                                "The alpha channel of generated mipmaps will preserve coverage for the alpha test. Useful for foliage textures."),
                                preserveCoverageProp.intValue != 0);

                            preserveCoverageProp.intValue = coverage ? 1 : 0;

                            if (coverage) {
                                using var _ = new IndentScope(1);
                                EditorGUILayout.PropertyField(root.FindPropertyRelative(Variables.m_AlphaTestReferenceValue),
                                    new GUIContent(
                                        "Alpha Cutoff",
                                        "The reference value used during the alpha test. Controls mipmap coverage."));
                            }

                            var replicateBorderProp = root.FindPropertyRelative(Variables.m_BorderMipMap);
                            replicateBorderProp.intValue = EditorGUILayout.Toggle(new GUIContent(
                                "Replicate Border",
                                "Replicate pixel values from texture borders into smaller mipmap levels. Mostly used for Cookie texture types."),
                                replicateBorderProp.intValue != 0) ? 1 : 0;

                            var fadeoutToGrayProp = root.FindPropertyRelative(Variables.m_FadeOut);
                            bool fadedOut = EditorGUILayout.Toggle(new GUIContent("Fadeout to Gray"), fadeoutToGrayProp.intValue != 0);

                            fadeoutToGrayProp.intValue = fadedOut ? 1 : 0;
                            if (fadedOut) {
                                using var _ = new IndentScope(1);
                                EditorGUI.BeginChangeCheck();
                                var startProp = root.FindPropertyRelative(Variables.m_MipMapFadeDistanceStart);
                                var endProp = root.FindPropertyRelative(Variables.m_MipMapFadeDistanceEnd);

                                var min = (float)startProp.intValue;
                                var max = (float)endProp.intValue;
                                EditorGUILayout.MinMaxSlider(new GUIContent("Fade Range"), ref min, ref max, 0, 10);

                                if (EditorGUI.EndChangeCheck()) {
                                    startProp.intValue = Mathf.RoundToInt(min);
                                    endProp.intValue = Mathf.RoundToInt(max);
                                }
                            }

                            var ignorePngGammaProp = root.FindPropertyRelative(Variables.m_IgnorePngGamma);
                            ignorePngGammaProp.intValue = EditorGUILayout.Toggle(
                                new GUIContent("Ignore PNG Gamma", "Ignore the Gamma attribute in png"),
                                ignorePngGammaProp.intValue != 0) ? 1 : 0;

                            var swizzleProp = root.FindPropertyRelative(Variables.m_Swizzle);
                            var label = new GUIContent(
                                "Swizzle", 
                                "Reorder and invert texture color channels. For each of R, G, B, A " + 
                                "channel picks where the channel data comes from.");

                            EditorGUI.BeginProperty(
                                EditorGUILayout.BeginHorizontal(),
                                label,
                                swizzleProp);
                            EditorGUI.BeginChangeCheck();

                            EditorGUI.showMixedValue = swizzleProp.hasMultipleDifferentValues;
                            var modifier = new ParameterModifier(2);
                            for (int i = 0; i < 2; i++)
                            {
                                modifier[i] = false;
                            }

                            // TODO: Need to rewrite TextureImporterInspector's version manually.
                            var value = ReflectionUtils.Invoke(
                                TextureImporterInspectorType,
                                null,
                                "SwizzleField",
                                BindingFlags.NonPublic | BindingFlags.Static,
                                new [] { typeof(SerializedProperty), typeof(GUIContent) },
                                new object[] { swizzleProp, label },
                                new [] { modifier });
                            EditorGUI.showMixedValue = false;

                            if (EditorGUI.EndChangeCheck()) {
                                swizzleProp.uintValue = value != null ? (uint)value : 0;
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUI.EndProperty();
                        }
                    }
                }
            }
        }

        private static Type TextureImporterInspectorType;

        private void InitializeType() {
            if (TextureImporterInspectorType == null) {
                TextureImporterInspectorType = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(assembly => assembly.FullName.Contains("UnityEditor.CoreModule,"))
                    .GetTypes()
                    .FirstOrDefault(type => type.Name.Contains("TextureImporterInspector"));
            }
        }

        // TODO: For TextureImporterPlatformSettings use the BuildTargetGroup API
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            InitializeType();

            EditorGUI.BeginChangeCheck();
            property.serializedObject.Update();

            var textureTypeProp = property.FindPropertyRelative(Variables.m_TextureType);
            var textureType = (TextureImporterType)EditorGUILayout.EnumPopup(
                new GUIContent("Texture Type"),
                (TextureImporterType)textureTypeProp.intValue);
            textureTypeProp.intValue = (int)textureType;

            var textureShapeProp = property.FindPropertyRelative(Variables.m_TextureShape);

            var isTexture2D = (Texture2DOnly & AsMask(textureType)) > 0;
            using (isTexture2D ? GUIScope.Disabled() : GUIScope.Enabled()) {
                textureShapeProp.intValue = (int)(TextureImporterShape)EditorGUILayout.EnumPopup(
                    new GUIContent("Texture Shape"),
                    (TextureImporterShape)textureShapeProp.intValue);
            }

            switch (textureType) {
                case TextureImporterType.Default:
                    HandleDefaultTexture(property);
                    break;
                case TextureImporterType.GUI:
                case TextureImporterType.Sprite:
                case TextureImporterType.Cursor:
                case TextureImporterType.Cookie:
                case TextureImporterType.Lightmap:
                case TextureImporterType.DirectionalLightmap:
                case TextureImporterType.Shadowmask:
                    break;
                default:
                    break;
            }

            if (EditorGUI.EndChangeCheck()) {
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
