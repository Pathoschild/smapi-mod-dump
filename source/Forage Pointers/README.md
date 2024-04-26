**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Bpendragon/ForagePointers**

----

# Forage Pointers
<!-- TOC -->

- [Forage Pointers](#forage-pointers)
    - [Short Description](#short-description)
    - [Long Description](#long-description)
    - [Installation](#installation)
    - [Dependencies](#dependencies)
    - [Conflicts](#conflicts)
    - [Configuration](#configuration)
    - [Changelog](#changelog)
    - [Special Thanks](#special-thanks)

<!-- /TOC -->

## Short Description

Adds little arrows over forage that is currently on screen.

## Long Description

Makes forage more visible over the course of the playthrough. When within a certain distance of the forage an arrow will appear over the top of it. As the player levels up their foraging skill the radius of visibility will increase. If the player has the "Tracker" skill forage will always have an arrow above it. Normally the arrow goes away as the forage enters the screen area.

## Installation

1. Install [SMAPI](https://smapi.io)
2. Download the latest release from the [Github Release Page](https://github.com/Bpendragon/GreenhouseSprinklers/releases) or [Nexus](https://www.nexusmods.com/stardewvalley/mods/7781) (you can also use Nexus to download to their mod manager)
3. Unzip the Folder into your `StardewValley/Mods` folder
4. Run the game
5. After running the game once you can set the options config (see [Configuration](#configuration) section below)

## Dependencies

None beyond SMAPI

## Conflicts

* ***NOT*** Compatible with Android at this time. This mod uses `Utility.ModifyCoordinatesForUIScale` which was added in SDV 1.5, and is thus unavailable on mobile ATM
* Will (somewhat ironically, given that Esca has been a huge help getting this going) conflict with [Custom Tracker](https://github.com/Esca-MMC/CustomTracker) due to that mod blanking out the default arrow. It might work if you set the config option `ReplaceTrackersWithForageIcons` in that mod to `false`
  * Will still only track standard SDV forage, does not track any custom items from that mod.

## Configuration

`config.json` contains several values, outlined below:

| Name | Type | Default Value | Acceptable Values | Notes |
|-|-|-|-|-|
| `ScaleEveryNLevels` | `int` | `2` | 1-10 (Inclusive) | How often the viewing distance increases, example `2` will increase the viewing distance after reaching levels 2, 4, 6, 8, and 10. Value is clamped to prevent divide by 0 errors. |
| `ScalingRadius` | `int` | `1` | 0-50 | How many tiles the viewing radius increases by when it scales, set this to `0` to never increase, values over about `10` are impractical if they start reaching outside the bounding box of the viewport. |
| `MinimumViewDistance` | `int` | `3` | 0-100 | The radius at which items will be marked at level 0. |
| `AlwaysShow` | `bool` | `false` | `true`, `false` | If set to `true` the mod will act as if you have the Tracker profession, even if you are level 0. Does not extend beyond the screen, but all forage on screen will be marked. |
| `BlinkPointers` | `bool` | `true` | `true`, `false` | If set to true, on screen pointers will blink occasionally as defined below. |
| `NumFramesArrowsOn` | `uint` | 50 | 1-int.MaxValue | Approx. number of frames the arrows should be on before blinking off. Actually based on ticks, there is about 1 tick per frame, and 60 frames per second |
| `NumFramesArrowsOff` | `uint` | 10 | 1-int.MaxValue | Approx. number of frames the arrows should be off before turning back on. |
| `ShowArtifactSpots` | `bool` | `true` | `true`, `false` | If set to true, on screen pointers will point at Artifact Spots (aka "Worms" or "Twigs"). |
| `ShowSeedSpots` | `bool` | `true` | `true`, `false` | If set to true, on screen pointers will point to the new Seed Spots (i.e. carrots, broccoli, etc.). |

View Distance is calculated as `MinimumViewDistance + ((player.ForagingLevel/ScaleEveryNLevels) * ScalingRadius)` this is done as integer math and will only increase in discrete steps.

All of these values can also be edited through use of [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)

## Changelog

*See [Changelog](CHANGELOG.md)*

## Special Thanks

* [Esca](https://github.com/Esca-MMC), for [Custom Tracker](https://github.com/Esca-MMC/CustomTracker) which helped with the sprite rendering for this project
* [itsbenter](https://github.com/itsbenter), for suggesting the idea in the first place.
