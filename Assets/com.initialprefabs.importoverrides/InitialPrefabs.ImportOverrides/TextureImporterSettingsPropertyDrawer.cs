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

        private static bool Folded = false;

        private static readonly int SwizzleFieldHash = "SwizzleField".GetHashCode();

        private static readonly string[] s_SwizzleOptions = { "R", "G", "B", "A", "1-R", "1-G", "1-B", "1-A", "0", "1" };

        private static readonly Type[] MultiFieldPrefixLabelTypeArgs = { typeof(Rect), typeof(int), typeof(GUIContent), typeof(int) };

        private static readonly ParameterModifier[] Modifiers = new ParameterModifier[1];

        private static readonly GUIContent[] FilterModeOptions = {
            EditorGUIUtility.TrTextContent("Point (no filter)"),
            EditorGUIUtility.TrTextContent("Bilinear"),
            EditorGUIUtility.TrTextContent("Trilinear")
        };

        private static readonly GUIContent[] wrapModeContents = {
            EditorGUIUtility.TrTextContent("Repeat"),
            EditorGUIUtility.TrTextContent("Clamp"),
            EditorGUIUtility.TrTextContent("Mirror"),
            EditorGUIUtility.TrTextContent("Mirror Once"),
            EditorGUIUtility.TrTextContent("Per-axis")
        };

        private static readonly int[] wrapModeValues = {
            (int)TextureWrapMode.Repeat,
            (int)TextureWrapMode.Clamp,
            (int)TextureWrapMode.Mirror,
            (int)TextureWrapMode.MirrorOnce,
            -1
        };

        private static readonly int[] FilterModeValues = Enum.GetValues(typeof(FilterMode)).Cast<int>().ToArray();

        private static int AsMask(TextureImporterType type) => 1 << (int)type;

        internal static void WrapModePopup(SerializedProperty wrapU, SerializedProperty wrapV, SerializedProperty wrapW, bool isVolumeTexture, ref bool showPerAxisWrapModes, bool enforcePerAxis) {
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
            }
            else {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = !showPerAxisWrapModes && (wrapU.hasMultipleDifferentValues || wrapV.hasMultipleDifferentValues || (isVolumeTexture && wrapW.hasMultipleDifferentValues));
                value = EditorGUILayout.IntPopup(wrapModeLabel, value, wrapModeContents, wrapModeValues);
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

        private static uint SwizzleField(GUIContent label, uint swizzle) {
            const float singleLineHeight = 18f;

            var rect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, label), EditorStyles.numberField);
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
                swz = (uint)EditorGUI.Popup(subRect, (int)swz, s_SwizzleOptions);
                swizzle &= ~(0xFFu << shift);
                swizzle |= swz << shift;
                subRect.x += w + kSpacingSubLabel;
            }
            EditorGUI.indentLevel = oldIndent;
            return swizzle;
        }

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
                        }
                    }

                    // Gamma
                    var ignorePngGammaProp = root.FindPropertyRelative(Variables.m_IgnorePngGamma);
                    ignorePngGammaProp.intValue = EditorGUILayout.Toggle(
                        new GUIContent("Ignore PNG Gamma", "Ignore the Gamma attribute in png"),
                        ignorePngGammaProp.intValue != 0) ? 1 : 0;

                    // Do the swizzle here
                    using (new IndentScope(1)) {
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
                        var value = SwizzleField(label, swizzleProp.uintValue);
                        EditorGUI.showMixedValue = false;

                        if (EditorGUI.EndChangeCheck()) {
                            swizzleProp.uintValue = value;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndProperty();
                    }
                }
            }
        }

        private bool showPerAxisWrapModes = false;

        /*
        void CubemapMappingGUI(TextureInspectorGUIElement guiElements)
        {
            m_ShowCubeMapSettings.target = (TextureImporterShape)m_TextureShape.intValue == TextureImporterShape.TextureCube;
            if (EditorGUILayout.BeginFadeGroup(m_ShowCubeMapSettings.faded))
            {
                if ((TextureImporterShape)m_TextureShape.intValue == TextureImporterShape.TextureCube)
                {
                    Rect controlRect = EditorGUILayout.GetControlRect(true, EditorGUI.kSingleLineHeight, EditorStyles.popup);
                    GUIContent label = EditorGUI.BeginProperty(controlRect, s_Styles.cubemap, m_GenerateCubemap);

                    EditorGUI.showMixedValue = m_GenerateCubemap.hasMultipleDifferentValues || m_SeamlessCubemap.hasMultipleDifferentValues;

                    EditorGUI.BeginChangeCheck();

                    int value = EditorGUI.IntPopup(controlRect, label, m_GenerateCubemap.intValue, s_Styles.cubemapOptions, s_Styles.cubemapValues2);
                    if (EditorGUI.EndChangeCheck())
                        m_GenerateCubemap.intValue = value;

                    EditorGUI.EndProperty();
                    EditorGUI.indentLevel++;

                    // Convolution
                    if (ShouldDisplayGUIElement(guiElements, TextureInspectorGUIElement.CubeMapConvolution))
                    {
                        EditorGUILayout.IntPopup(m_CubemapConvolution,
                            s_Styles.cubemapConvolutionOptions,
                            s_Styles.cubemapConvolutionValues,
                            s_Styles.cubemapConvolution);
                    }

                    ToggleFromInt(m_SeamlessCubemap, s_Styles.seamlessCubemap);

                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndFadeGroup();
        }
        */

        // TODO: For TextureImporterPlatformSettings use the BuildTargetGroup API
        public override void OnGUI(Rect position, SerializedProperty root, GUIContent label) {
            EditorGUI.BeginChangeCheck();
            root.serializedObject.Update();

            var textureTypeProp = root.FindPropertyRelative(Variables.m_TextureType);
            var textureType = (TextureImporterType)EditorGUILayout.EnumPopup(
                new GUIContent("Texture Type"),
                (TextureImporterType)textureTypeProp.intValue);
            textureTypeProp.intValue = (int)textureType;

            var textureShapeProp = root.FindPropertyRelative(Variables.m_TextureShape);

            var isTexture2D = (Texture2DOnly & AsMask(textureType)) > 0;
            using (isTexture2D ? GUIScope.Disabled() : GUIScope.Enabled()) {
                var textureShape =(TextureImporterShape)EditorGUILayout.EnumPopup(
                    new GUIContent("Texture Shape"),
                    (TextureImporterShape)textureShapeProp.intValue);
                textureShapeProp.intValue = (int)(textureShape);

                switch (textureShape) {
                    case TextureImporterShape.TextureCube:
                        // TODO: Implement the function above
                        var cubemapProp = root.FindPropertyRelative(Variables.m_GenerateCubemap);
                        cubemapProp.intValue = (int)(TextureImporterGenerateCubemap)EditorGUILayout.EnumPopup(
                            new GUIContent("Mapping"),
                            (TextureImporterGenerateCubemap)cubemapProp.intValue);
                        break;
                    case TextureImporterShape.Texture2DArray:
                        break;
                }
            }

            switch (textureType) {
                case TextureImporterType.Default:
                    HandleDefaultTexture(root);
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

            // Wrap Mode
            // TODO: Add a per axis component with the same logic
            var wrapUProp = root.FindPropertyRelative(Variables.m_WrapU);
            var wrapVProp = root.FindPropertyRelative(Variables.m_WrapV);
            var wrapWProp = root.FindPropertyRelative(Variables.m_WrapW);
            // TextureWrapMode mode = (TextureWrapMode)EditorGUILayout.EnumPopup(new GUIContent("Wrap Mode"), (TextureWrapMode)wrapUProp.intValue);
            // wrapUProp.intValue = (int)mode;
            
            WrapModePopup(wrapVProp, wrapUProp, wrapWProp, false, ref showPerAxisWrapModes, false);

            var filterModeProp = root.FindPropertyRelative(Variables.m_FilterMode);

            // Draw the filter and aniso level
            EditorGUILayout.IntPopup(filterModeProp, FilterModeOptions, FilterModeValues, new GUIContent("Filter Mode"));

            using (new GUIScope(filterModeProp.intValue != (int)FilterMode.Point)) {
                var anisoProp = root.FindPropertyRelative(Variables.m_Aniso);
                anisoProp.intValue = EditorGUILayout.IntSlider(new GUIContent("Aniso Level"), anisoProp.intValue, 0, 16);
            }

            if (EditorGUI.EndChangeCheck()) {
                root.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
