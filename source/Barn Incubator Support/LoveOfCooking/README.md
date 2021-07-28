**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/lshtech/StardewValleyMods**

----

![](https://i.imgur.com/WfFkBGI.png)

The Love of Cooking (or LOC) is a suite of optional and integrated changes bundled into a single mod.  
Broad in scope but with a single goal in mind, this is designed to be a fairly definitive collection of changes to enhance the presence cooking has in Stardew Valley. 
A basic overview of this mod and its features can be found on its linked Nexus Mods page.  


![](https://i.imgur.com/y5GeriD.png)

1. Download the latest versions of [SMAPI](https://smapi.io), [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720), and [PyTK](https://www.nexusmods.com/stardewvalley/mods/1726),
2. Download this mod from [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/6830),
3. Drop all of the mods above into your StardewValley/Mods folder,
4. Run the game using SMAPI

+  To change which features you have enabled or disabled, launch the game and edit this mod's config file.  
+  If you want to remove this mod and all of its effects, simply remove it from your Mods folder.

### ![](https://i.imgur.com/QyZQGVb.png) Editing mod assets

LOC's sprite and data assets are fully editable by other mods, including Pathoschild's [Content Patcher](https://github.com/Pathoschild/StardewMods/tree/stable/ContentPatcher).  

If your patches no longer work after an update, check whether the exposed asset paths have been renamed.  
You can find a list of all exposed asset paths in `LoveOfCooking/Core/AssetManager.cs:Load`.  
All exposed assets follow the naming pattern *GameContentAssetPath.*

#### To edit LOC assets:  
- With C#, reflect the asset path value, or check some loaded AssetName against the effective parsed asset path.  
- With Content Patcher, set your Target field to the effective parsed asset path.

Asset path value: `GameContentSpriteSheetPath = PathUtilities.NormalizePath(AssetPrefix + "Assets\\Sprites")`  
Effective parsed asset path: `"blueberry.LoveOfCooking.Assets/Sprites"`

### ![](https://i.imgur.com/QyZQGVb.png) Feature-complete deep dive

If you're interested in the inner workings of all the features in this mod, there's a guide in progress.  
Inside, you'll find (where complete) details on the exact purpose and mechanics behind the features added by LOC, as well as information you might find useful if you're planning on editing the definitions and configuration files.


https://gist.github.com/b-b-blueberry/a1cf06d1a4ecc4bb888119866920d57a
