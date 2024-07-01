**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# How to make a Furniture Pack

This is a tutorial on how to create a Content Pack for the [Furniture Framework](https://www.nexusmods.com/stardewvalley/mods/23458) mod, not to be confused with a Content Pack for [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915) (shortened to "CP" in this tutorial) or [Custom Furniture](https://www.nexusmods.com/stardewvalley/mods/1254) (which is not updated for SV 1.6).

If you don't know how to format a json data file, please read a tutorial about it so that the rest of this tutorial makes sense.

This tutorial and documentation uses the [Example Pack](https://github.com/Leroymilo/FurnitureFramework/tree/main/%5BFF%5D%20Example%20Pack) as an example, sometimes without a link.

## Contents

* [Manifest](#manifest)
* [Content](#content)
	* [Format](#format)
	* [Furniture](#furniture)
	* [Included](#included)
* [Commands](#commands)
* [Mixed Content Pack](#mixed-content-pack)

## Manifest

Like any other content pack, you will need a `manifest.json` file to make your Furniture Pack mod work with SMAPI. Here's the one provided in the Example Pack:

```json
{
	"Name": "Furniture Example Pack",
	"Author": "leroymilo",
	"Version": "1.0",
	"MinimumApiVersion": "4.0",
	"MinimumGameVersion": "1.6",
	"Description": "An example pack for the Furniture Framework",
	"UniqueID": "leroymilo.FurnitureExample.FF",
	"ContentPackFor": {
		"UniqueID": "leroymilo.FurnitureFramework",
		"MinimumVersion": "2.0.0"
	},
	"UpdateKeys": [ "Nexus:23458" ]
}
```

You need to make sure that the `UniqueID` for your Furniture Pack is unique, a good way to ensure this is too use your username as a part of it.  
The number in the `UpdateKeys` field points to the page of your mod, if you post your mod on [Nexus](https://www.nexusmods.com/stardewvalley/), this number appears in the url of your mod's page.

## Content

This is the file where you define all your custom Furniture. Please keep in mind that all file paths that you will write in it have to be relative to your mod's directory (where `content.json` and `manifest.json` are), it is strongly recommended to put all images in the `assets` folder of your mod.

:warning: <span style="color:red">**WARNING**</span>: Unlike a CP Content Pack, names in this field are <span style="color:red">**CASE SENSITIVE**</span>, make sure you don't forget capital letters when writting field names or ids.

It is highly recommended to use the Json Schema made for Furniture Packs (see at the very top of the content.json of the Example Pack), to setup your json validator with this Schema, you can start [here](https://json-schema.org/learn/getting-started-step-by-step#validate), but it mostly depends on what IDE/text editor you're using.

The `content.json` file is a model with only 2 fields:

### Format

The format you're using for your Furniture Pack, it matches the leftmost number in the mod's version. See the [Format changelogs](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Format%20changelogs.md). The current Format is 2.

### Furniture

This is a dictionary where you'll define your custom Furniture.</br>
Each entry in this dictionary has the Id of the Furniture for key and the Furniture definition for value like this:
```json
"Furniture": {
	"my_furniture_id": {
		// My furniture definition
	},
	"my_other_furniture_id": {
		// My other furniture definition
	}
}
```

Note: you can have as many custom Furniture as you want but their IDs must be all differents from each other.  
It is strongly recommended to add `{{ModID}}.` at the start of every new Furniture ID to avoid conflicts with other mods or the game itself. For example, the first custom furniture of the Example Pack would be added to the game as `leroymilo.FurnitureExample.FF.simple_test`.  
You should also be able to modify vanilla Furniture by using their vanilla ID, doing so might be tricky for some Furniture because some properties are hardcoded (like the catalogues and the cauldron) so be carefull of what you modify.

Note 2: when using [Furniture Variants](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#variants), the variation name or number will be automatically added to the original ID so that each variation can exist in the game.

The next part of the tutorial, with details about the Furniture definition is [here](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md).

You can also check the [Templates](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Templates.md) to make relatively simple Furniture.

### Included

This is a dictionary where you can give the path of other json files that should be included in this Furniture Pack. Here's the structure to define an Included file:
```json
"Included": {
	"Name": {
		"Path": "my/included/file/path.json",
		"Description": "A Part of the Furniture Pack",
		"Enabled": true
	}
}
```

Each entry in the "Included" dictionary will create a config options to enable/disable this part of the Furniture Pack that will be shown in the Generic Mod Config Menu.  
An included file has the same structure as the [content.json file](#content), but the `Format` field will be ignored.  
You can include files in included files, this will make config options nested in the Generic Mod Config Menu.

The "Name" can be anything you want, it will be used as the config option name in the Generic Mod Config Menu.  
The `Path` is the path of the included file, **relative to the mod folder**.  
The `Description` is also up to you (and optional), it will show in the tooltip of the config option.  
`Enabled` defines if this file is included by default. It is optional and its default value is `true`.

## Commands

Here's details about console commands:

### reload_furniture_pack

Reloads the furniture pack with the given UniqueID. It will add new Furniture and edit changed Furniture but not remove Furniture that was removed from the pack.  
There are some known limits to this command:
- Changing a Furniture's ID will create a new Furniture without removing the one with the old ID
- Changing a Furniture's rotations will break already placed Furniture of this type
- Removing slots and seats will remove anything that was in it, or cause errors
- Particles might break?

## Mixed Content Pack

There are multiple cases where you might want your mod to be both a CP content pack and a Furniture pack, for exemple if your pack has Furniture and other items or buildings.  
This is very easy: you basically have to make 2 separate mods, but since SMAPI is smart when reading mods, you can upload them together on Nexus (or other mod website) as a single file structured like this:
```
My Mod
└───[CP] My Mod
│	└───assets
│	│	└───(your assets for the CP pack)
│	└───manifest.json
│	│	{
│	│		...
│	│		"UniqueID": "MyName.MyMod.CP"
│	│		...
│	│	}
│	└───content.json
└───[FF] My Mod
	└───assets
	│	└───(your assets for the FF pack)
	└───manifest.json
	│	{
	│		...
	│		"UniqueID": "MyName.MyMod.FF"
	│		...
	│	}
	└───content.json
```

It is VERY IMPORTANT that the UniqueIDs of the 2 mods are different so that SMAPI correctly loads both.  
However, you can use the same update key for both, just make sure that both mods have the same version.