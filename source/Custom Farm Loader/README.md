**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader**

----

<img src="Custom Farm Loader/Assets/Logo.png" alt="Logo" width="200" height="60">

<b>Custom Farm Loader</b> is a Stardew Valley Mod that gives Players an easy to use, gamepad friendly Farm Selection UI and Content Creators a powerful high level access to lower level farm related functionality enabling more diverse farm maps.

[Nexusmods page](https://www.nexusmods.com/stardewvalley/mods/13804)
<!-- TABLE OF CONTENTS -->
## Table of Contents

* [For Players](#for-players)
  * [Setup](#setup)
  * [Multiplayer](#multiplayer)
* [For Content Creators](#for-content-creators)
  * [Technical Summary](#technical-summary)
  * [Recommended Folder Structure](#recommended-folder-structure)
  * [Documentation](#documentation)
  * [Migrating existing maps to CFL](#migrating-existing-maps-to-cfl)
  * [Content Patcher Token](#content-patcher-token)
  * [Creating New Maps](#creating-new-maps)
# For Players

## Setup

1. Install [SMAPI](https://smapi.io/)
2. Unpack Custom Farm Loader in your Mods Folder
3. Unpack your chosen \[CFL\] Farms in your Mods Folder
4. Start the game using the SMAPI Launcher
5. Upon creating a new game press the new Custom Button <img src="Custom Farm Loader/Assets/CustomFarmIcon.png" alt="Logo" width="18" height="20"> and select your map

## Multiplayer

All Players need to have both Custom Farm Loader and the Farm you're intending to play on installed. <br>
Mismatching versions will cause issues. It is recommended to keep your mods updated to the latest version.

# For Content Creators

## Technical Summary

Custom Farm Loader will treat any cfl_map.json in your (loaded) Mods folder structure as a separate custom farm.<br>
The cfl_map.json contains Metadata that will be displayed to the player and describes game loop events including start furniture, daily resource spawns, caught fish, warp coordinates and building start positions using a variety of conditions.<br>
CFL adheres to the Modding Guidelines for [Custom Farm Types in Stardew Valley 1.5.5 and above](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.5.5#Custom_farm_types), as such the CFL User Interface can also be used alongside other Mods that append the Data/AdditionalFarms asset.
Older maps that overwrite vanilla maps should be updated to either Custom Farm Loader and/or a combination of [Content Patcher [CP]](https://www.nexusmods.com/stardewvalley/mods/1915), [Farm Type Manager [FTM]](https://www.nexusmods.com/stardewvalley/mods/3231) and [Custom Map Properties](https://stardewvalleywiki.com/Modding:Maps#Known_map_properties).<br>
CFL tries to streamline and ease the map creation process, but it is not the only possible choice.<br><br>
Please see the [migration page](#migrating-existing-maps-to-cfl) for templates to easily migrate existing maps to CFL and the [Content Patcher Token](#content-patcher-token) page on info on how to create even more creative farms.

## Recommended Folder Structure

The Mod folder should be prefixed with <b>[CFL]</b> to easily identify it as using the CFL Framework.<br>
Follow the [SMAPI Modding Guide](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) to assist you in creating the manifest.json<br>
To create your own cfl_map.json feel free to use a [template](#migrating-existing-maps-to-cfl) and follow the [documentation](#documentation) provided.<br>
If your mod has just a single farm map all map assets could be stored in the assets folder, but for map packs it is recommended to create a cfl_maps folder that seperates each map by its own separate folder.

<pre>
📂 Mods
  ┗📂 [CFL] &lt;your mod name&gt;
   ┣ 📜 manifest.json
   ┣ 📁 assets
   ┗ 📂 cfl_maps
      ┗ 📂 &lt;map name&gt;
         ┣ 📜 cfl_map.json
         ┣ 🌆 Preview.png
         ┣ 🌆 Icon.png (18x20 px recommended)
         ┣ 🌆 Overlay.png (131x61 px)
         ┗ 📄 &lt;map name&gt;.tbin/tmx/xnb
</pre>

While currently not necessary, please don't forget to set your mod as a CFL content pack in the manifest using:

```json
  "ContentPackFor": {
        "UniqueID": "DeLiXx.Custom_Farm_Loader"
  }
```

## Documentation

Check out the cfl_map.json documentation in [the gitlab wiki](https://gitlab.com/delixx/stardew-valley-custom-farm-loader/-/wikis/home#table-of-contents).

## Migrating existing Maps to CFL

CFL provides a set of templates for each vanilla farm available for download at the [nexusmods page](https://www.nexusmods.com/stardewvalley/mods/13804).
All that is needed to migrate an existing map to CFL is to create a [content pack folder](#recommended-folder-structure), copy & paste the template you'd like to use and change the Name, Author, Description and MapFile properties in the cfl_map.json
You can go the extra mile and create a custom icon, world map overlay and preview.
If you don't provide a preview, then the world map overlay will be used.

Why migrate?
Farms that replace vanilla maps instead of appending the Data/AdditionalFarms asset lack flexibility and mod compatibility.
Nothing beats being able to quickly swap between maps in terms of user experience.

## Content Patcher Token

Sometimes you might want try things that aren't (yet) supported by CFL directly like loading a custom Tilesheet.<br>
In cases where other mods like Content Patcher provide what you're looking for you will want to limit the changes you make to whenever your custom farm is played.<br>
You can usually do that by providing the farms unique ID in a condition.

Every CFL farm will have its own unique ID in the following format:
<pre>
&lt;Mod UniqueID&gt;/&lt;Map Name&gt;
eg. DeLiXx.Sunny_Farm/Sunny Farm
</pre>
The manifest.json Unique ID, a slash and your cfl_map.json name.

Use the command "set_farm_type list" to obtain a list of all currently installed modded farm types.

If you have Content Patcher installed you can use the [<b>FarmType</b> Token](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/tokens.md#world) in your WHEN conditions to change specific assets, like a tileset whenever your custom farm is loaded.

Alternatively use <b>Game1.whichModFarm.ID</b> in your code.

## Creating new Maps

Instead of creating a new farm from the ground up it is a lot easier and more reliable to instead edit an existing vanilla farm.<br>
CFL provides a set of templates for each vanilla farm available for download at the [nexusmods page](https://www.nexusmods.com/stardewvalley/mods/13804).<br>
You  can use those templates to create your own content pack mod, which will load the map into the game.<br>
Please follow the [Modding:Content packs Guide](https://stardewvalleywiki.com/Modding:Content_packs) to assist you in creating your own content pack.

Use the map editor "[Tiled](https://www.mapeditor.org/)" to edit your maps.<br>
Make sure to enable the Tbin plugin under 'Edit &gt; Preferences &gt; Plugins'.<br>
tbin.dll on Windows, libtbin.so on Linux, or libtbin.dylib on Mac.

Please check the wiki Modding:Maps page if you need any help creating your map:
https://stardewvalleywiki.com/Modding:Maps
