**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Bpendragon/GreenhouseSprinklers**

----

# GreenhouseSprinklers

<!-- TOC -->

- [GreenhouseSprinklers](#greenhousesprinklers)
    - [Short Description](#short-description)
    - [Long Description](#long-description)
    - [Changelog](#changelog)
    - [Installation](#installation)
    - [Uninstallation](#uninstallation)
    - [Dependencies](#dependencies)
    - [Conflicts](#conflicts)
        - [Known Compatible Mods](#known-compatible-mods)
    - [Configuration](#configuration)
        - [Difficulty Settings](#difficulty-settings)
        - [Gameplay Settings](#gameplay-settings)
    - [Console Commands](#console-commands)
    - [Content Patcher Integration](#content-patcher-integration)
    - [Translations](#translations)

<!-- /TOC -->

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) 

## Short Description
Greenhouse Sprinklers [SMAPI](https://smapi.io) mod.

Become friends with the Wizard and he'll send Robin some blueprints for you to upgrade your Greenhouse to have automatic sprinklers installed.

## Long Description

The Junimos are pleased with your contributions to the valley! After they rebuilt your greenhouse, they realized that they could improve it further. They want to make sure you can use the whole thing without using up spots for Sprinklers. They've told the Wizard these plans, and as you grow friendlier with him, he'll see their merit. He'll eventually tell Robin the plans, and send you a letter telling you the upgrade is available. (It will also require that you have a Junimo Hut on your farm if you went the Joja route)

There are 3 levels of upgrade that will unlock as you become more friendly with the Wizard: 

1. Installs Sprinklers on the ceiling of the greenhouse, will run every morning.
2. Upgrades the sprinklers on the ceiling, will now run every morning and evening.
3. Installs sprinklers all over the farm, hidden from view that will water everything morning and night

## Changelog

*see [Changelog](CHANGELOG.md)*

## Installation

1. Install [SMAPI](https://smapi.io)
2. Download the latest release from the [Github Release Page](https://github.com/Bpendragon/GreenhouseSprinklers/releases) or [Nexus](https://www.nexusmods.com/stardewvalley/mods/7456?tab=files) (you can also use Nexus to download to their mod manager)
3. Unzip the Folder into your `StardewValley/Mods` folder
4. Run the game
5. After running the game once you can set the options config (see [Configuration](#configuration) section below)

## Uninstallation

1. Remove the folder from your mods folder.

This mod is set up to use the `ModData` Dictionary provided by ConcernedApe. It will leave a small token in your save file after uninstallation, however this won't affect anything.

## Dependencies

This mod has no explicit dependencies beyond SMAPI.

* Version 1.0 and 1.1 Will work with SDV 1.4, and possibly 1.3
* Version 1.2+ requires SDV 1.5+
* Version 1.3+ exposes a token for ContentPatcher mods to use if they so desire (See [Content Patcher Integration](#content-patcher-integration) section below)

## Conflicts

* This mod *may* conflict with mods that edit the interior of the greenhouse, especially if the new mod uses a multi-tilesheet interior. 
  * Most indications seem to be that it works with almost all mods that modify single tilesheet Greenhouses
* Will *probably* conflict with a mod that allows you to build multiple greenhouses (it might still run on the main greenhouse though)
* *Will* conflict with mods and Content Packs that modify the exterior of the Greenhouse ***EXCEPT*** if `ShowVisualUpdates` is disabled (see [Gameplay Settings](#gameplay-settings))

### Known Compatible Mods

* [Custom Greenhouse](https://www.nexusmods.com/stardewvalley/mods/3464) including the cellar.
* [Ellie's Ideal Greenhouse](https://www.nexusmods.com/stardewvalley/mods/7497)

## Configuration

Stored at `%modfolder%/GreenhouseSprinklers/config.json`

### Difficulty Settings

The majority of the file is taken up by the default difficulty settings

A difficulty settings consists of a `Difficulty` (name) and 3 settings, `FirstUpgrade`, `SecondUpgrade`, and `FinalUpgrade` each of the upgrades consists of:

| Field             | Type            | Acceptable Values               | Notes                                                    |
|-------------------|-----------------|---------------------------------|----------------------------------------------------------|
| `Sprinkler`       | `string` (enum) | `Basic` `Quality` `Iridium`     | The type of sprinkler required to pay for the upgrade    |
| `SprinklerCount`  | `int`           | 0 to 999 (inclusive)            | Number of sprinklers required for upgrade                |
| `Gold`            | `int`           | 0 to `int.MaxValue` (inclusive) | Cost in Gold for the upgrade                             |
| `Batteries`       | `int`           | 0 to 999 (inclusive)            | Number of batteries required for upgrade                 |
| `Hearts`          | `int`           | 0 to 10 (inclusive)             | Number of hearts of friendship with the Wizard           |

### Gameplay Settings

Located near the bottom of the file these chage some of the ingame behavior of the mod. SDV must be closed and reloaded for changes to take effect

| Name                   | Type            | Default Value | Acceptable Values                                            | Notes                                                                                                                                          |
|------------------------|-----------------|---------------|--------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------|
| `SelectedDifficulty`   | `string`  (enum)| `Medium`      |  `Easy`, `Medium`, `Hard`, `Custom`                          | Selects the difficulty/cost of the upgrades                                                                                                    |
| `ShowVisualUpgrades`   | `bool`          | `true`        | `true` or `false`                                            | Displays exterior changes to the greenhouse as upgrades are made. Set to `false` if using any mods that modify the exterior of the greenhouse. |
| `WaterSandOnBeachFarm` | `bool`          | `true`        | `true` or `false`                                            | Whether or not the final upgrade should water the sandy areas of the beach farm (which normally can't support sprinklers)                      |
| `MaxNumberOfUpgrades`  | `int`           | 3             | 1 to 3 (inclusive)                                           | Number of allowable upgrades, set this to 2 if you don't want the option to water the entire farm                                              |

## Console Commands

This mod exposes 3 SMAPI Console commands

|Name|Arguments|Notes|
|--|--|--|
| `ghs_setlevel` | level (`int`)| Sets the upgrade level |
| `ghs_getlevel` || Returns the current level of the greenhouse |
| `ghs_waternow` || Forces the game to water the greenhouse (and possible farm if level 3) in the same manner as it would at the start of the day |

## Content Patcher Integration

This mod exposes a single token `Bpendragon.GreenhouseSprinklers/GreenHouseLevel` which indicates the current upgrade level:

* `"0"` - No Upgrades applied
* `"1"` - First Upgrade applied, in my visual updates this adds a "controller" near the outside door, and some Quality Sprinklers in the rafters
* `"2"` - Second Upgrade applied, rafter sprinklers upgraded to Iridium
* `"3"` - All upgrades applied. Adds a pipe from the controller into the ground (indicating that it's also running teh underground sprinklers)

If you want to see visual updates in other mods/content packs, please ask those creators *nicely* to add the integration.

## Translations

* French - [Azurys](github.com/Azurys)
* Portuguese - [Caco-o-sapo](github.com/Caco-o-sapo)
* German - Kazel
* Italian - [Leecanit](https://github.com/LeecanIt)
* Chinese - [Cccchelsea226](https://github.com/Cccchelsea226)
* Spanish - [ManHeII](https://github.com/ManHeII)
* Korean - [Wally232](https://github.com/Wally232)
* Russian - [CatMartin](https://github.com/CatMartin)
* Hungarian - [Martin66789](https://www.nexusmods.com/stardewvalley/users/27323031)

To help with translating this mod please see [this issue](https://github.com/StardewModders/mod-translations/issues/38) at the `StardewModders/mod-translations` repo
