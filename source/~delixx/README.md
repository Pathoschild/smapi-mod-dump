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
  * [Creating a Content Pack](#creating-a-content-pack)
  * [Documentation](#documentation)
    * [Custom Farm Type](#custom-farm-type)
    * [Custom Assets](#custom_assets)
    * [Content Patcher Token Conditions](#content-patcher-token-conditions)
    * [Custom Debug Commands](#custom-debug-commands)
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
CFL adheres to the Modding Guidelines for [Custom Farm Types](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.5.5#Custom_farm_types).

## Recommended Folder Structure

The Mod folder should be prefixed with <b>[CFL]</b> to easily identify it as using the CFL Framework.<br>
Follow the [SMAPI Modding Guide](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) to assist you in creating the manifest.json<br>
The manifest.json is required for SMAPI to load your mod.<br>
To create your own cfl_map.json feel free to use a [template](#migrating-existing-maps-to-cfl) and follow the [documentation](#documentation) provided.<br>

<pre>
📂 Mods
  ┗📂 [CFL] &lt;your mod name&gt;
    ┣ 📜 manifest.json
    ┗ 📁 assets
       ┣ 📜 cfl_map.json
       ┣ 🌆 Preview.png
       ┣ 🌆 Icon.png (18x20 px recommended)
       ┣ 🌆 Overlay.png (131x61 px)
       ┗ 📄 &lt;farm_map&gt;.tbin/tmx/xnb
</pre>

Please don't forget to set your mod as a CFL content pack in the manifest using:

```json
  "ContentPackFor": {
        "UniqueID": "DeLiXx.Custom_Farm_Loader"
  }
```

## Creating a Content Pack

If you're new to modding Stardew Valley you might want to read up on the [Modding:Content packs Guide](https://stardewvalleywiki.com/Modding:Content_packs), if you haven't already.

CFL requires a cfl_map.json file to load your custom farm map.<br>
It will tell CFL what your farm is called, who made it, where to respawn large stumps and so on.<br>
You can look up how to write the cfl_map.json at [the gitlab wiki](https://gitlab.com/delixx/stardew-valley-custom-farm-loader/-/wikis/home#table-of-contents)

Whether you're intending to use CFL to migrate an existing map, or to create a new farm from the ground up, it always makes sense to use previous solutions to your problem as a starting ground.
You'll find a set of cfl_map.json templates that'll help you create your own at the [nexusmods optional files](https://www.nexusmods.com/stardewvalley/mods/13804?tab=files).

Are you new to creating maps for Stardew Valley?<br>
The SDV Wiki provides a [great guide](https://stardewvalleywiki.com/Modding:Maps) to get you started.

## Documentation

Check out the cfl_map.json documentation in [the gitlab wiki](https://gitlab.com/delixx/stardew-valley-custom-farm-loader/-/wikis/home#table-of-contents).

### Custom Farm Type

Every CFL farm will have its own unique FarmType ID in the following format:
<pre>
<b>&lt;Mod UniqueID&gt;/&lt;Map Name&gt;</b>
eg. DeLiXx.Sunny_Farm/Sunny Farm
</pre>
The manifest.json Unique ID, a slash and your cfl_map.json name.

Use the command <b>set_farm_type list</b> to obtain a list of all currently installed modded farm types.

If you have Content Patcher installed you can use the [<b>FarmType</b> Token](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/tokens.md#world) in your WHEN conditions to change specific assets, like a custom FarmCave load, only when your custom farm is loaded to avoid interference with other Farms. 
```json
"When": {
  "FarmType": "DeLiXx.Sunny_Farm/Sunny Farm"
}
```

Alternatively use <b>Game1.GetFarmTypeID()</b> in your code.

### Custom Assets
CFL will create the following assets for each custom Farm.<br>
The map asset in particular is very useful as it allows [configurable edits to your map using Content Patcher](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/action-editmap.md).

<pre>
//Map asset
<b>Maps/CFL_Map/&lt;Farmtype&gt;</b>
eg. Maps/CFL_Map/DeLiXx.Sunny_Farm/Sunny Farm

//Farm Preview Icon
<b>CFL_Icon/&lt;Farmtype&gt;</b>
eg. CFL_Icon/DeLiXx.Sunny_Farm/Sunny Farm

//Worldmap Overlay
<b>CFL_WorldMap/&lt;Farmtype&gt;</b>
eg. CFL_WorldMap/DeLiXx.Sunny_Farm/Sunny Farm
</pre>

### Content Patcher Token Conditions

Amidst its own set of conditions Custom Farm Loader supports Content Patcher Token Conditions for any DailyUpdate or FishingRule.
<br>
Read more about it [here](https://gitlab.com/delixx/stardew-valley-custom-farm-loader/-/wikis/home#universal-conditions)

### Custom Debug Commands

These debug commands can be written directly into the SMAPI console.<br>They are intended to make mod creation easier.<br>

| <div style="width:600px">Command</div> |  |
| ------ | ------ |
| cfl reload       | Reloads all cached cfl_map.json data       |
| cfl dayupdate `num`,<br> cfl du `num`       | Performs all valid daily updates of the players location `num` times       |
| cfl furniture       | Prints out all furniture of the current location as json so it can be directly copied into StartFurniture        |
