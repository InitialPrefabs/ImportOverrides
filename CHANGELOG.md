# 1.1.0
* Adds a GUI for `TextureImporterPlatformSettings`
* Updates PrimitiveGenerator 
    * adds `#GENERATE` define guards to avoid users from regenerating
    * to generate `KnownBuildTargets` with mappings to `NamedBuildTargets`
    * appends `Variables` with all unique private variables from `TextureImporterSettings` and `TextureImporterPlatformSettings` 

# 1.0.0
* Adds a GUI similar to Unity's `TextureImporter` for `TextureImporterSettings`
* Adds a basic code generator to scan all private members for `TextureImporterSettings`