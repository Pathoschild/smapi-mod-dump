**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/PurrplingMod**

----

# Getting started

## Install
- [Install the latest version of SMAPI](https://smapi.io).
- Download and install [Quest Framework](https://www.nexusmods.com/stardewvalley/mods/6414)
- Download this mod and unzip it into *Stardew Valley/Mods*.
- Run the game using SMAPI.

## Compatibility

- Works with Stardew Valley 1.4 on Linux/Mac/Windows.
- Works on Android 6 and newer (experimental support)
- Works in **single player** ONLY.

### Note for Android users

Beginning with version *0.11.0* there's an experimental support for Android SMAPI and Stardew Valley. There is list of known android issues:

- **HUD may be drawn at an incorrect position on some devices.** - If you have this problem, you can disable the HUD in configuration file. (Set `ShowHUD` to `false`)
- **The game may crash on Android 5.1 and older** - Can't fix it, because it's caused by SMAPI. Android SMAPI has an experimental harmony patching, you can report these crashes to @MartyrPher (developer of SMAPI for Android). Remember Android 5.1 and older is not officialy supported by this mod.

### Note for SVE players

If you play this mod together with StardewValley Expanded, it's recommended to enable `UseCheckForEventsPatch` in `config.json`. This feature fixes some problems with trig Marlon's invitation event in Adventurer's guild before or after SVE event played. This feature is enabled by default and not recommended to disable it when you use SVE.

## Experimental features

**NOTE:** Remember that these features are experimental and can negativelly affect gameplay and game stability and may cause bugs. All experimental features are disabled by default and must be explicitly enabled in [configuration](configuration.md). See the `Experimental` section in `config.json`. For more info read [about experimental features](experimental.md).

### Companion swimsuits

You can enable swimsuit support for companions if they enter the pool with you. Some companions not own swimsuits and they are still enter the pool in their daily clothes.

**Companions supports swimsuits**

- Abigail
- Alex
- Haley
- Emily
- Sam

You can enable this experimental feature be set `true` for `Experimental.UseSwimsuits` in `config.json`.

## Upgrading to 0.15.0 and newer

### Players

NPC Adventures doesn't load legacy content packs by default. If you have installed some legacy content packs for NA and do you want load them, you must allow it in config file. See [Configuration](configuration.md).

### Modders

NPC Adventures now supports custom weapons for each combat level. This change may affects your content pack. Also if you are an author of legacy content pack, please upgrade your content pack format to versiob `1.3`. See [Modder's upgrade guide](../modding/upgrading.md) for more details.

## Upgrading to 0.14.0 and newer (from 0.13.x and older)

NPC Adventures is now based on [Quest Framework](https://www.nexusmods.com/stardewvalley/mods/6414). From version 0.14.0 this framework is dependency and is required to run NPC Adventures. You must [download and install Quest Framework](https://www.nexusmods.com/stardewvalley/mods/6414).

## Upgrading to 0.13.0 and newer (from 0.12.0 and older)

Before you replace your old files with files from `0.13.0` and newer, it's recommended to delete `assets` folder and all files included in this directory and then copy new files.

## Upgrading from 0.11.0 beta to 0.12.0 and newer

### Modders

There are some breaking changes for 0.12.0 affects existing content packs. You must do some edits in your content pack for make it compatible with new version, see the [Modder's upgrade guide](../modding/upgrading.md).

### Players

For players has no any affections. You can use your mod in standard way without any additional changes. If you want to enable new optional features or experimental features, see [Configuration](configuration.md).

## Upgrading from alpha versions (from 0.9 and older)

### Players

From version *0.10.0* there is one big change. You can't recruit companions until you recive an invitation from Marlon and you don't see his event. You can get invitation when:

- You have access to Adventurer's guild
- Reached 20 level of mines
- You have a 66% of required hearts with any recruitable NPC

If you already match all of this conditions, you will get invitation letter morning.

### Modders

There are some changes in mod's code. You must do some edits in your content pack for make it compatible with new version. Follow [Modder's upgrade guide](../modding/upgrading.md).

## See also

- [Configuration](configuration.md)
- [Modder's upgrade guide](../modding/upgrading.md)
- [How to create a content pack](../modding/content-packs.md)
