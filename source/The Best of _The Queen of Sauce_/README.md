**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Bpendragon/Best-of-Queen-of-Sauce**

----

# The Best of "The Queen of Sauce"

<!-- TOC -->

- [The Best of "The Queen of Sauce"](#the-best-of-the-queen-of-sauce)
    - [Short Description](#short-description)
    - [Long Description](#long-description)
    - [Changelog](#changelog)
    - [Installation](#installation)
    - [Dependencies](#dependencies)
    - [Conflicts](#conflicts)
    - [Configuration](#configuration)
    - [Translations](#translations)

<!-- /TOC -->

## Short Description
Best of Queen of Sauce [SMAPI](https://smapi.io) mod.

Recipes normally only available from the Queen of Sauce become available after some time to purchase from Gus at a markup.

## Long Description

The Stardrop Saloon has been asked to be part of a pilot program for "The Queen of Sauce" TV show. Some time after first airing (default 28 days/1 month) recipes that are only available from "The Queen of Sauce" are available to purchase at the saloon from Gus. However, due to licensing fees they aren't exactly cheap (default 3500 gold).

Note: This does not change recipes that are available from other sources. It only allows for recipes that are only attainable from watching the TV to be available. 

## Changelog

*see [Changelog](CHANGELOG.md)*

## Installation

1. Install [SMAPI](https://smapi.io)
2. Download the latest release from the [Github Release Page](https://github.com/Bpendragon/Best-of-Queen-of-Sauce/releases/) or [Nexus](https://www.nexusmods.com/stardewvalley/mods/7985?tab=files) (you can also use Nexus to download to their mod manager)
3. Unzip the Folder into your `StardewValley/Mods` folder
4. Run the game
5. After running the game once you can set the options config (see [Configuration](#configuration) section below)

## Dependencies

* None beyond SMAPI

This mod was originally written for SMAPI 3.9.x and SDV 1.5.x, however it may be backwards compatible since I don't believe I used any features that would lock it to these and later versions, there are no gauruntees of that though.

## Conflicts

* Will not add any recipes from other mods that may modify The Queen of Sauce, or that changes the airing order.
  * This will likely be addressed in the future through an API system so other mods can register changes

## Configuration

There are 2 values in the Config file, both are `int` and can take any positive value:

* `DaysAfterAiring` - The number of days after a recipe first airs that it will take to appear in the Saloon, default `28`
* `Price` - The cost in Gold of each recipe, default `3500`

## Translations

* Spanish - [vlcoo](https://github.com/vlcoo)
* Turkish - [KediDili](https://github.com/KediDili)
* French  - [Schneitizel](https://github.com/Schneitizel) 

Please contribute, see which languages are still needed [here](https://github.com/StardewModders/mod-translations/issues/39)
