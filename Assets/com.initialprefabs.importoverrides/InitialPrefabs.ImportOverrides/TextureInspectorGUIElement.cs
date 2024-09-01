using System;

namespace InitialPrefabs.ImportOverrides
{
    public partial class TextureImporterSettingsPropertyDrawer {
        // TODO: Move this to a specialized file
        [Flags]
        private enum TextureInspectorGUIElement {
            None = 0,
            PowerOfTwo = 1 << 0,
            Readable = 1 << 1,
            AlphaHandling = 1 << 2,
            ColorSpace = 1 << 3,
            MipMaps = 1 << 4,
            NormalMap = 1 << 5,
            Sprite = 1 << 6,
            Cookie = 1 << 7,
            CubeMapConvolution = 1 << 8,
            CubeMapping = 1 << 9,
            SingleChannelComponent = 1 << 11,
            PngGamma = 1 << 12,
            VTOnly = 1 << 13,
            ElementsAtlas = 1 << 14,
            Swizzle = 1 << 15,
        }
    }
}

