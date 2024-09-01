using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides {

    public partial class TextureImporterSettingsPropertyDrawer {

        private static readonly GUIContent CubemapConvolution = EditorGUIUtility.TrTextContent("Convolution Type");

        private static readonly GUIContent[] CubemapConvolutionOptions = {
            EditorGUIUtility.TrTextContent("None"),
            EditorGUIUtility.TrTextContent("Specular (Glossy Reflection)",
                "Convolve cubemap for specular reflections with varying smoothness (Glossy Reflections)."),
            EditorGUIUtility.TrTextContent("Diffuse (Irradiance)",
                "Convolve cubemap for diffuse-only reflection (Irradiance Cubemap).")
        };

        private static readonly int[] CubemapConvolutionValues = {
            (int)TextureImporterCubemapConvolution.None,
            (int)TextureImporterCubemapConvolution.Specular,
            (int)TextureImporterCubemapConvolution.Diffuse
        };

        private static readonly GUIContent[] CubemapOptions = {
            EditorGUIUtility.TrTextContent("Auto"),
            EditorGUIUtility.TrTextContent("6 Frames Layout (Cubic Environment)",
                "Texture contains 6 images arranged in one of the standard cubemap layouts - " +
                "cross or sequence (+x, -x, +y, -y, +z, -z). Texture can be in vertical or horizontal orientation."),
            EditorGUIUtility.TrTextContent("Latitude-Longitude Layout (Cylindrical)",
                "Texture contains an image of a ball unwrapped such that latitude and longitude " +
                "are mapped to horizontal and vertical dimensions (as on a globe)."),
            EditorGUIUtility.TrTextContent("Mirrored Ball (Spheremap)",
                "Texture contains an image of a mirrored ball.")
        };

        private static readonly int[] CubemapValues2 = {
            (int)TextureImporterGenerateCubemap.AutoCubemap,
            (int)TextureImporterGenerateCubemap.FullCubemap,
            (int)TextureImporterGenerateCubemap.Cylindrical,
            (int)TextureImporterGenerateCubemap.Spheremap
        };
    }
}

