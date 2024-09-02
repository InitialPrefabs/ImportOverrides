using UnityEditor;
using UnityEngine;

namespace InitialPrefabs.ImportOverrides.Demo {
    [CreateAssetMenu(menuName = "InitialPrefabs/Demo/TextureImporterSettings GUI Example")]
    public class Demo : ScriptableObject {
        [Tooltip("If you switch between debug and normal mode in the inspector you can see how " +
                "this GUI makes our TextureImporterSettings similar to the importer's UI")]
        public TextureImporterSettings settings;
    }
}