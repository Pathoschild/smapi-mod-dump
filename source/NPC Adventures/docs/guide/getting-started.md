# Getting started

## Install
- [Install the latest version of SMAPI](https://smapi.io).
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

If you play this mod together with StardewValley Expanded, it's recommended to enable `Experimental.UseCheckForEventsPatch` in `config.json`. This feature is experimental, but fixes some problems with trig Marlon's invitation event in Adventurer's guild before or after SVE event played.

## Experimental features

**NOTE:** Remember that these features are experimental and can negativelly affect gameplay and game stability and may cause bugs. All experimental features are disabled by default and must be explicitly enabled in [configuration](configuration.md). See the `Experimental` section in `config.json`. For more info read [about experimental features](experimental.md).

### Allow fight through companion (without annyoing dialogue)

Fight togehther with your companion with monsters without showing annoying companion dialogue. You can enable this experimental feature in configuration by switch option `Experimental.FightThruCompanion` to `true`. When this feature is enabled and you want to show companion dialogue, you must do right-click on companion.

### Check NPC Adventures events by patched SDV check for events method.

This feature enables event checking of mod's event cutscenes by SDV's method `GameLocation.checkForEvents()` instead of SMAPI's player warped method. This feature fixes some problems with playing NA's events (like player must re-enter to play NA event when vanilla or other event played and finished on the location before NA event cutscene). It's recommended to enable this experimental function if you are playing NPC Adventures together with StardewValley Expanded.

You can enable this feature by set `Experimental.UseCheckForEventsPatch` to `true` in `config.json`.

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
