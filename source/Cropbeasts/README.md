# ![[icon]](promo/icon.png) Cropbeasts

*a [Stardew Valley](http://stardewvalley.net/) mod by [kdau](https://kdau.gitlab.io)*

Why just harvest your crops when you could do battle with them? Cropbeasts are crops cursed by the witch that turn into monsters on your farm. After you defeat them, you get the original harvest and maybe something extra.

## Contents

* [Compatibility](#compatibility)
* [Installation](#installation)
* [Use](#use)
* [Configuration](#configuration)
* [Customization](#customization)
* [Translation](#translation)
* [Acknowledgments](#acknowledgments)
* [See also](#see-also)

## Compatibility

This version of Cropbeasts is compatible with **Stardew Valley 1.4+**.

This mod should work on **Linux, macOS or Windows**. Android is not yet supported.

There are no known problems with multiplayer use. Every player must install the mod, but only the farmer (host) will control the spawning of cropbeasts.

This mod is partially incompatible with the Save Anywhere mod because cropbeasts are not designed to be persisted in save files. Saving with that mod should still be safe while no cropbeasts are active in any location.

There are no other known conflicts with other mods. Custom crops added by other mods can be set up to become cropbeasts; see "Customization" below.

## Installation

1. Install the latest version of [SMAPI](https://smapi.io/).
1. To unlock the Beast Mask for this mod's achievement, optionally install the [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720) mod.
1. To configure this mod without editing a JSON file, optionally install the [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) mod.
1. Download this mod from its [Nexus page](https://www.nexusmods.com/stardewvalley/mods/6030?tab=files) or [ModDrop page](https://www.moddrop.com/stardew-valley/mods/784523-cropbeasts).
1. Unzip the mod and place the `Cropbeasts` folder inside your `Mods` folder.
1. Run the game using SMAPI.

## Use

Cropbeasts spawn on farms with the **Wilderness Farm** layout*. If you want cropbeasts to spawn on **other farms** as well, you can configure the mod to do so (see "Configuration" below).

<details><summary>* (spoiler)</summary>...or on any farm that has been set to have nighttime monsters at the Dark Shrine of Night Terrors.</details>

While you are outdoors **on the farm** or indoors **in the greenhouse**, cropbeasts can spawn at any time before sunset. They spawn randomly, but only up to a configurable limit per day. By default, new cropbeasts do not spawn while existing ones are still active.

Only "regular" **fruit and vegetable crops** and coffee plants can become cropbeasts by default. Flowers, wild/forage crops, fruit trees, Tea Bushes, Sweet Gem Berries and anything planted in a Garden Pot are exempt.

After its brief transformation, you can fight a cropbeast like any other monster. The challenge scales up with your Combat skill level. See the [**list of cropbeasts**](doc/BEASTS.md) for details about each type's special attributes.

When you slay a cropbeast, it will drop the **original crop** with at least its original quantity and quality. You will get the **experience points** that you would get for harvesting the crop, split between the Farming and Combat skills. You could also get any of the following bonuses:

* Skilful combat has a good chance of increasing the **crop quality** one level. You can even get iridium crops!
* The usual chance of a **double harvest** is slightly increased, particularly at higher combat levels.
* Most cropbeasts have a small chance of dropping a packet of **seeds** for the same crop (Wheat and Coffee Beans excepted).
* Certain cropbeasts have **special loot drops** as well; see the [list](doc/BEASTS.md) for details.

Slaying at least one of every type of cropbeast will attain the **Tamer of Beasts achievement** and unlock the Beast Mask for purchase from the Hat Mouse.

## Configuration

If you have installed Generic Mod Config Menu, you can configure this mod by clicking the cogwheel button at the lower left corner of the Stardew Valley title screen and then choosing "Cropbeasts". Otherwise, see [Configuring Cropbeasts](doc/CONFIGURING.md) for details.

## Customization

Additional crops, including custom crops added by Json Assets or otherwise, can be made eligible to become cropbeasts. See [Customizing Cropbeasts](doc/CUSTOMIZING.md) for details.

## Translation

This mod can be translated into any language supported by Stardew Valley. No translations are currently available.

Your contribution would be welcome. Please see the [details on the wiki](https://stardewvalleywiki.com/Modding:Translations) for help. You can send me your work in an issue [on GitLab](https://gitlab.com/kdau/cropbeasts/-/issues), [on Nexus](https://www.nexusmods.com/stardewvalley/mods/6030?tab=bugs) or by DM on Discord.

## Acknowledgments

* Like all mods, this one is indebted to ConcernedApe, particularly for the vanilla assets it adapts.
* This mod would not function without [SMAPI](https://smapi.io/) by Pathoschild.
* This mod works best with [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720) and [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) by spacechase0.
* The #making-mods channel on the [Stardew Valley Discord](https://discordapp.com/invite/StardewValley) offered valuable guidance and feedback.
* In particular, the Starbeast sprite was designed with the help of [EssGee](https://www.nexusmods.com/stardewvalley/users/83595503).

## See also

* [Release notes](doc/RELEASE-NOTES.md) from existing versions
* [Discord channel](https://discord.gg/SjjMuyR) for discussing this mod
* [Issue tracker](https://gitlab.com/kdau/cropbeasts/-/issues) for bug reports and feature plans
* [MIT license](LICENSE) (TLDR: do whatever, but credit me)
* [My other mods](https://kdau.gitlab.io)

Mirrors:

* [This mod on GitLab](https://gitlab.com/kdau/cropbeasts)
* [This mod on Nexus](https://www.nexusmods.com/stardewvalley/mods/6030)
* [This mod on ModDrop](https://www.moddrop.com/stardew-valley/mods/784523-cropbeasts)
* [This mod on the forums](https://forums.stardewvalley.net/index.php?resources/cropbeasts.57/)
