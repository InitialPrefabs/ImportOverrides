# ImportOverrides

Unity's texture importer provide a list of default settings for _manual_ editing. However, if you 
are trying to automate those importers, you need to go through

* [TextureImporterSettings](https://docs.unity3d.com/ScriptReference/TextureImporterSettings.html)
* [TextureImporterPlatformSettings](https://docs.unity3d.com/ScriptReference/TextureImporterPlatformSettings.html)

With those 2 structs, you would typically set them on the texture importer through an [AssetImporter](https://docs.unity3d.com/ScriptReference/AssetImporter.html).

## The issue

Setting up a **scriptable import pipeline** requires you know which settings to enable/disable. The
struct just has a bunch of getter and setters and you'll need to know which settings to place. Without 
a visual GUI, this makes it cumbersome to visualize.

## The solution
Provide a GUI similar to Unity's `TextureImporter`. This allows the `TextureImporterSettings` to 
feel more familiar and allows developers to set up their pipeline visually. **That is what this 
repository aims to do**.

## What this plugin does not do
It is still the **developer's** responsibility to set up their own Scriptable Import Pipeline. This 
is **_not an architecture_**, it is **only a GUI frontend** to the `TextureImporterSettings`.

You can use this GUI frontend for all instances of serialized `TextureImporterSettings`. In the 
demo folder, you can see a Scriptable Object called `Demo`.

* The Demo Scriptable Object only has the `TextureImporterSettings` variable. Instead of showing the
default struct, it shows a GUI similar to `TextureImporterSettings`.

## Reference Video
https://github.com/user-attachments/assets/4beae39d-5f3d-450e-85bd-25ef40803f25
