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
            new GUIContent("Sharp"),
            new GUIContent("Smooth"),
        };
        public static readonly GUIContent FlipGreenChannel = new GUIContent("Flip Green Channel");

        public static readonly GUIContent SpriteMode = new GUIContent("Sprite Mode");
        public static readonly GUIContent[] SpriteModeOptions = {
            new GUIContent("Single"),
            new GUIContent("Multiple"),
            new GUIContent("Polygon"),
        };
        public static readonly GUIContent[] SpriteMeshTypeOptions = {
            new GUIContent("Full Rect"),
            new GUIContent("Tight"),
        };

        public static readonly GUIContent SpritePixelsPerUnit = new GUIContent(
            "Pixels Per Unit", "How many pixels in the sprite correspond to one unit in the world.");
        public static readonly GUIContent SpriteExtrude = new GUIContent(
            "Extrude Edges", "How much empty area to leave around the sprite in the generated mesh.");
        public static readonly GUIContent SpriteMeshType = new GUIContent(
            "Mesh Type", "Type of sprite mesh to generate.");
        public static readonly GUIContent SpriteAlignment = new GUIContent(
            "Pivot", "Sprite pivot point in its localspace. May be used for syncing animation frames of different sizes.");
        public static readonly GUIContent[] SpriteAlignmentOptions = {
            new GUIContent("Center"),
            new GUIContent("Top Left"),
            new GUIContent("Top"),
            new GUIContent("Top Right"),
            new GUIContent("Left"),
            new GUIContent("Right"),
            new GUIContent("Bottom Left"),
            new GUIContent("Bottom"),
            new GUIContent("Bottom Right"),
            new GUIContent("Custom"),
        };
        public static readonly GUIContent spriteGenerateFallbackPhysicsShape = new GUIContent(
            "Generate Physics Shape",
            "Generates a default physics shape from the outline of the Sprite/s when a physics shape " +
            "has not been set in the Sprite Editor.");
        public static readonly GUIContent applyAndContinueToSpriteEditor = new GUIContent(
            "Unapplied import settings for \'{0}\'.\n Apply changes and continue to Sprite Editor Window?");
        public static readonly GUIContent EmptyContent = new GUIContent(" ");

        public static readonly GUIContent CookieType = new GUIContent("Light Type");
        public static readonly GUIContent[] CookieOptions = {
            new GUIContent("Spot Light"),
            new GUIContent("Directional Light"),
            new GUIContent("Point Light"),
        };

        public static readonly GUIContent SingleChannelComponent = new GUIContent(
            "Channel", "As which color/alpha component the single channel texture is treated.");
        public static readonly GUIContent[] SingleChannelComponentOptions =
        {
            new GUIContent("Alpha", "Use the alpha channel (compression not supported)."),
            new GUIContent("Red", "Use the red color component."),
        };
        public static readonly int[] SingleChannelComponentValues =
        {
            (int)TextureImporterSingleChannelComponent.Alpha,
            (int)TextureImporterSingleChannelComponent.Red,
        };

        public static readonly GUIContent AlphaSource = new GUIContent(
            "Alpha Source",
            "How is the alpha generated for the imported texture.");
        public static readonly GUIContent[] AlphaSourceOptions = {
            new GUIContent("None", "No Alpha will be used."),
            new GUIContent("Input Texture Alpha", "Use Alpha from the input texture if one is provided."),
            new GUIContent("From Gray Scale", "Generate Alpha from image gray scale."),
        };
        public static readonly int[] AlphaSourceValues = {
            (int)TextureImporterAlphaSource.None,
            (int)TextureImporterAlphaSource.FromInput,
            (int)TextureImporterAlphaSource.FromGrayScale,
        };

        public static readonly GUIContent OverrideStandalone = new GUIContent("Override For Windows, Mac, Linux");
        public static readonly GUIContent OverrideServer = new GUIContent("Override for Server");
        public static readonly GUIContent OverrideIOS = new GUIContent("Override For iOS");
        public static readonly GUIContent OverrideAndroid = new GUIContent("Override For Android");
        public static readonly GUIContent OverrideWebGL = new GUIContent("Override For WebGL");
        public static readonly GUIContent OverrideWindowsStoreApp = new GUIContent("Override For Windows Store App");
        public static readonly GUIContent OverridePS4 = new GUIContent("Override For PlayStation");
        public static readonly GUIContent OverrideXbox = new GUIContent("Override For Xbox One");
        public static readonly GUIContent OverrideForTVOs = new GUIContent("Override for Tv OS");
        public static readonly GUIContent OverrideForVisionOS = new GUIContent("Override for Vision OS");
        public static readonly GUIContent OverrideForNintendoSwitch = new GUIContent("Override for Nintendo Switch");
        public static readonly GUIContent OverrideForStadia = new GUIContent("Override for Stadia");
        public static readonly GUIContent OverrideForLinuxHeadlessSimulation = new GUIContent("Override for Linux Headless Simulation");
        public static readonly GUIContent OverrideForEmbeddedLinux = new GUIContent("Override for Embedded Linux");
        public static readonly GUIContent OverrideForQNX = new GUIContent("Override for QNX");
        public static readonly GUIContent MaxSize = new GUIContent("Max Size", "Textures larger than this will be scaled down.");

        public static readonly GUIContent ResizeAlgorithm = new GUIContent(
            "Resize Algorithm",
            "Select algorithm to apply for textures when scaled down.");

        public static readonly GUIContent Format = new GUIContent(
            "Format",
            "Please refer to the docs: https://docs.unity3d.com/Manual/class-TextureImporterOverride.html");
        public static readonly GUIContent Compression = new GUIContent("Compression",
            "How will the textures be compressed?");
        public static readonly GUIContent UseCrunchCompression = new GUIContent("Use Crunch Compression",
            "Texture is crunched compressed to save space on the disk when available.");
        public static readonly GUIContent CompressorQuality = new GUIContent("Compressor Quality",
            "Use the slider to adjust compression quality from 0 (Fastest) to 100 (Best).");

        public static readonly GUIContent[] TextureCompressionOptions = {
            new GUIContent("None", "Texture is not compressed."),
            new GUIContent("Low Quality", "Texture compressed with low quality but high performance, high compression format."),
            new GUIContent("Normal Quality", "Texture is compressed with a standard format."),
            new GUIContent("High Quality", "Texture compressed with a high quality format."),
        };

        public static readonly int[] TextureCompressionValues = {
            (int)TextureImporterCompression.Uncompressed,
            (int)TextureImporterCompression.CompressedLQ,
            (int)TextureImporterCompression.Compressed,
            (int)TextureImporterCompression.CompressedHQ
        };

        public static readonly GUIContent TextureFormatHelp = new GUIContent(
            "Texture Format Info",
            "Follow the link to Unity's official LTS documentation on texture formats per platform.");

        public static readonly GUIContent IgnorePlatformSupport = new GUIContent(
            "Ignore Platform Support",
            "By default, `TextureGenerator` checks if the active build target supports the " +
            "selected texture format. If you enable this property, `TextureGenerator` ignores " +
            "that check, and allows you to generate texture data in any format regardless of the " +
            "currently active build target.");
    }
}