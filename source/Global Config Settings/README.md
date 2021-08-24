**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Gaphodil/GlobalConfigSettings**

----

**Global Config Settings** is a [Stardew Valley](http://stardewvalley.net/) mod that lets
the user load a default configuration of settings after creating a save file, or optionally
on every loaded file or joined multiplayer game.

## What?
On loading into the game, the save file's settings are replaced with the ones assigned with this mod.
This effect is not permanent until the day has ended and the game is saved.

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/9340).
3. Run the game using SMAPI.

## Configure
The mod creates a `config.json` file in its mod folder the first time you run it. You can open that
file in a text editor to configure the mod. Alternatively, you can edit the settings in-game using
[Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (this is recommended).

These are some of the available settings:

| General settings			| what it does
| ---						| ---
| `ChangeOnEveryLoad`       | Default `false`. If disabled, only activates on ending the intro after creating a new farm.

| In-game settings with unclear options   | options
| ---                   | ---
| `GamepadMode`			| Default `auto`. `auto, force_on, force_off` as equivalent to `Auto-detect; Force On; Force Off`
| `ItemStowing`			| Default `off`. `both, gamepad, off` as equivalent to `On; Gamepad Only; Off`
| `SlingshotFireMode`	| Default `hold`. `hold, legacy` as equivalent to `Hold and release; Pull in opposite direction`
| `MusicVolume, SoundVolume, AmbientVolume, FootstepVolume`			| Defaults `75, 100, 75, 90`. Must be in the range [0, 100] (inclusive)
| `FishingBiteSound`	| Default `-1`. `-1, 0, 1, 2, 3` as equivalent to `Default; 1; 2; 3; 4`
| `UiScale`				| Default `100%`. Must be increments of 5% in the range [75%, 150%] (inclusive)
| `ZoomLevel`			| Default `100%`. Must be increments of 5% in the range [75%, 200%] (inclusive)
| `LightingQuality`		| Default `Med.`. Available options are `Low, Med., High`, though `Lowest` and `Ultra` appear in the code.
| `SnowTransparency`	| Default `100`. Actually represents snow *opacity*. Must be in the range [0, 100] (inclusive)

Setting options to invalid values, as well as doubling up on hotkeys, may result in unusual behaviour.

## See also
* [release notes](release-notes.md)
* [Nexus page](https://www.nexusmods.com/stardewvalley/mods/9340)
