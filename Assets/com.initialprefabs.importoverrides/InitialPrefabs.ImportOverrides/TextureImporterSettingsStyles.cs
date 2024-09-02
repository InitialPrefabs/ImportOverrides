using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

    internal static class TextureImporterSettingsStyles {

        public static readonly GUIContent ColorTexture = new GUIContent("sRGB (Color Texture)");
        public static readonly GUIContent AlphaIsTransparency = new GUIContent("Alpha Is Transparency");
        public static readonly GUIContent NOP2 = new GUIContent("Non-Power of 2", "How no power of twos are scaled on import.");
        public static readonly GUIContent ReadWrite = new GUIContent("Read/Write", "Enable access to raw pixel from code.");
        public static readonly GUIContent VirtualTextureOnly = new GUIContent(
            "Virtual Texture Only",
            "Texture is optimized for use as a virtual texture and can only be used as a Virtual Texture");
        public static readonly GUIContent GenerateMipmaps = new GUIContent("Generate Mipmaps",
            "Create progressively smaller versions of the texture, " +
            "for reduced texture shimmering and better GPU performance when the texture is viewed at a distance.");
        public static readonly GUIContent MipmapLimits = new GUIContent("Use Mipmap Limits",
            "Disable this if the number of mips to to upload should not be limited by the " +
            "quality settings. (effectively: always upload at full resolution, regardless of " +
            "the global mipmap limit or mipmap limit group.");
        public static readonly GUIContent MipStreaming = new GUIContent("Mip Steaming",
            "Only load larger mipmaps as needed to render the current game cameras. " +
            "Required texture streaming to be enabled in quality settings.");
        public static readonly GUIContent MipmapPriority = new GUIContent("Priority",
            "Mipmap streaming priority when there's contention for " +
            "resources. Positive numbers represent higher priority. Valid range is -128 to 127.");
        public static readonly GUIContent MipmapFiltering = new GUIContent("Mipmap Filtering");
        public static readonly GUIContent PreserveCoverage = new GUIContent(
            "Preserve Coverage",
            "The alpha channel of generated mipmaps will preserve coverage for the alpha test. Useful for foliage textures.");
        public static readonly GUIContent AlphaCutoff = new GUIContent(
            "Alpha Cutoff",
            "The reference value used during the alpha test. Controls mipmap coverage.");
        public static readonly GUIContent ReplicateBorder = new GUIContent(
            "Replicate Border",
            "Replicate pixel values from texture borders into smaller mipmap levels. Mostly used for Cookie texture types.");
        public static readonly GUIContent FadeoutToGray = new GUIContent("Fadeout to Gray");
        public static readonly GUIContent FadeRange = new GUIContent("Fade Range");
        public static readonly GUIContent IgnorePNGGamma = new GUIContent("Ignore PNG Gamma", "Ignore the Gamma attribute in png");
        public static readonly GUIContent Swizzle = new GUIContent(
            "Swizzle",
            "Reorder and invert texture color channels. For each of R, G, B, A " +
            "channel picks where the channel data comes from.");
        public static readonly GUIContent TextureType = new GUIContent("Texture Type");
        public static readonly GUIContent TextureShape = new GUIContent("Texture Shape");
        public static readonly GUIContent Mapping = new GUIContent("Mapping");
        public static readonly GUIContent FixupEdgeSeams = new GUIContent("Fixup Edge Seams", "Enable if this texture is used for glossy reflections.");
        public static readonly GUIContent FilterMode = new GUIContent("Filter Mode");
        public static readonly GUIContent AnisoLevel = new GUIContent("Aniso Level");
        public static readonly GUIContent Columns = new GUIContent("Columns");
        public static readonly GUIContent Rows = new GUIContent("Rows");
        public static readonly GUIContent GenerateFromBump = new GUIContent("Create from Grayscale",
            "The grayscale of the image is used as the heightmap for generating the normal map.");
        public static readonly GUIContent Bumpiness = new GUIContent("Bumpiness");
        public static readonly GUIContent BumpFilteringOption = new GUIContent("Filtering");
        public static readonly GUIContent[] BumpFilteringOptions = {
            EditorGUIUtility.TrTextContent("Sharp"),
            EditorGUIUtility.TrTextContent("Smooth"),
        };
        public static readonly GUIContent FlipGreenChannel = new GUIContent("Flip Green Channel");

        public static readonly GUIContent SpriteMode = EditorGUIUtility.TrTextContent("Sprite Mode");
        public static readonly GUIContent[] SpriteModeOptions = {
            EditorGUIUtility.TrTextContent("Single"),
            EditorGUIUtility.TrTextContent("Multiple"),
            EditorGUIUtility.TrTextContent("Polygon"),
        };
        public static readonly GUIContent[] SpriteMeshTypeOptions = {
            EditorGUIUtility.TrTextContent("Full Rect"),
            EditorGUIUtility.TrTextContent("Tight"),
        };

        public static readonly GUIContent SpritePixelsPerUnit = EditorGUIUtility.TrTextContent(
            "Pixels Per Unit", "How many pixels in the sprite correspond to one unit in the world.");
        public static readonly GUIContent SpriteExtrude = EditorGUIUtility.TrTextContent(
            "Extrude Edges", "How much empty area to leave around the sprite in the generated mesh.");
        public static readonly GUIContent SpriteMeshType = EditorGUIUtility.TrTextContent(
            "Mesh Type", "Type of sprite mesh to generate.");
        public static readonly GUIContent SpriteAlignment = EditorGUIUtility.TrTextContent(
            "Pivot", "Sprite pivot point in its localspace. May be used for syncing animation frames of different sizes.");
        public static readonly GUIContent[] SpriteAlignmentOptions = {
            EditorGUIUtility.TrTextContent("Center"),
            EditorGUIUtility.TrTextContent("Top Left"),
            EditorGUIUtility.TrTextContent("Top"),
            EditorGUIUtility.TrTextContent("Top Right"),
            EditorGUIUtility.TrTextContent("Left"),
            EditorGUIUtility.TrTextContent("Right"),
            EditorGUIUtility.TrTextContent("Bottom Left"),
            EditorGUIUtility.TrTextContent("Bottom"),
            EditorGUIUtility.TrTextContent("Bottom Right"),
            EditorGUIUtility.TrTextContent("Custom"),
        };
        public static readonly GUIContent spriteGenerateFallbackPhysicsShape = EditorGUIUtility.TrTextContent(
            "Generate Physics Shape",
            "Generates a default physics shape from the outline of the Sprite/s when a physics shape " +
            "has not been set in the Sprite Editor.");
        public static readonly GUIContent applyAndContinueToSpriteEditor = EditorGUIUtility.TrTextContent(
            "Unapplied import settings for \'{0}\'.\n Apply changes and continue to Sprite Editor Window?");
        public static readonly GUIContent EmptyContent = new GUIContent(" ");

        public static readonly GUIContent CookieType = EditorGUIUtility.TrTextContent("Light Type");
        public static readonly GUIContent[] CookieOptions = {
            EditorGUIUtility.TrTextContent("Spot Light"),
            EditorGUIUtility.TrTextContent("Directional Light"),
            EditorGUIUtility.TrTextContent("Point Light"),
        };

        public static readonly GUIContent SingleChannelComponent = EditorGUIUtility.TrTextContent(
            "Channel", "As which color/alpha component the single channel texture is treated.");
        public static readonly GUIContent[] SingleChannelComponentOptions =
        {
            EditorGUIUtility.TrTextContent("Alpha", "Use the alpha channel (compression not supported)."),
            EditorGUIUtility.TrTextContent("Red", "Use the red color component."),
        };
        public static readonly int[] SingleChannelComponentValues =
        {
            (int)TextureImporterSingleChannelComponent.Alpha,
            (int)TextureImporterSingleChannelComponent.Red,
        };

        public static readonly GUIContent AlphaSource = EditorGUIUtility.TrTextContent(
            "Alpha Source",
            "How is the alpha generated for the imported texture.");
        public static readonly GUIContent[] AlphaSourceOptions = {
            EditorGUIUtility.TrTextContent("None", "No Alpha will be used."),
            EditorGUIUtility.TrTextContent("Input Texture Alpha", "Use Alpha from the input texture if one is provided."),
            EditorGUIUtility.TrTextContent("From Gray Scale", "Generate Alpha from image gray scale."),
        };
        public static readonly int[] AlphaSourceValues = {
            (int)TextureImporterAlphaSource.None,
            (int)TextureImporterAlphaSource.FromInput,
            (int)TextureImporterAlphaSource.FromGrayScale,
        };
    }
}
