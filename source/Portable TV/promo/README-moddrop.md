**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/kdau/portabletv**

----

![[icon]](https://www.kdau.com/PortableTV/icon.png) It's little! It's lovely! It shows Livin' Off The Land! The Portable TV is a craftable item carried in your inventory that lets you watch TV channels (standard and custom) from anywhere in Stardew Valley.

## ![[Compatibility]](https://www.kdau.com/headers/compatibility.png)

**Game:** Stardew Valley 1.5+

**Platform:** Linux, macOS or Windows (Android: use 1.4 version; no TV on/off sounds)

**Multiplayer:** works; every player must install

**Other mods:** no known conflicts; custom channels will be listed with standard ones

## ![[Installation]](https://www.kdau.com/headers/installation.png)

1. Install [SMAPI](https://smapi.io/)
1. Install [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720)
1. Install [Generic Mod Config Menu](https://www.moddrop.com/stardew-valley/mods/771692-generic-mod-config-menu) (optional, for easier configuration)
1. Download this mod from the link in the header above
1. Unzip the mod and place the `PortableTV` and `[JA]PortableTV` folders inside your `Mods` folder
1. Run the game using SMAPI

## ![[Use]](https://www.kdau.com/headers/use.png)

The Portable TV is a resource item that can be carried in your inventory. Its crafting recipe is available immediately and requires a Gold Bar, 2 Refined Quartz and a Battery Pack.

To use the TV on Linux, macOS or Windows, select it in your inventory and right-click in an open part of the world. You can press "R" on your keyboard (or another key that you configure; see below) to activate any Portable TV in your inventory.

To use the TV on Android, select it in your inventory and tap anywhere in the world. You can also configure the Virtual Keyboard mod to add an "R" button; tapping it will activate any Portable TV in your inventory.

The farmer will pick up the TV and turn it on, showing the familiar channel list. After you have viewed your chosen program, the farmer will turn off the TV and put it away. Just use it again to watch something else.

## ![[Configuration]](https://www.kdau.com/headers/configuration.png)

If you have installed Generic Mod Config Menu, you can access this mod's configuration by clicking the cogwheel button at the lower left corner of the Stardew Valley title screen and then choosing "Portable TV".

Otherwise, you can edit this mod's `config.json` file. It will be created in the mod's main folder (`Mods/PortableTV`) the first time you run the game with the mod installed. These options are available:

* `Animate`: Set this to `false` to have the Portable TV appear and disappear instantly from the screen.
* `Static`: Set this to `false` to remove the light static from the Portable TV screen.
* `Music`: Set this to `false` to prevent the Portable TV from playing music tracks over the standard channels. (Custom channels should be configured through their own mods.)
* `ActivateKey`: Set this to any valid keybinding that will then activate a Portable TV in your inventory. [See the list of keybindings here.](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings)

## ![[Translation]](https://www.kdau.com/headers/translation.png)

This mod can be translated into any language supported by Stardew Valley. It is currently available in English, Spanish, French and Hungarian, with further limited translations to German, Italian and Portuguese.

Your contribution would be welcome. Please see the [instructions on the wiki](https://stardewvalleywiki.com/Modding:Translations). You can send me your work in [a GitLab issue](https://gitlab.com/kdau/portabletv/-/issues) or in the Comments tab above.

## ![[Acknowledgments]](https://www.kdau.com/headers/acknowledgments.png)

* Like all mods, this one is indebted to ConcernedApe, Pathoschild and the various framework modders.
* This mod is an implementation of an [idea from HoustoCo](https://github.com/StardewModders/mod-ideas/issues/268).
* Coding of this mod relied on [PyTK](https://www.nexusmods.com/stardewvalley/mods/1726) by Platonymous as a key example.
* The #making-mods channel on the [Stardew Valley Discord](https://discordapp.com/invite/StardewValley) offered valuable guidance and feedback.
* The French translation was prepared by Inu'tile.
* The Hungarian translation was prepared by martin66789.
* The sound when the TV turns on is clipped from [Turning on an old CRT TV](https://freesound.org/people/pfranzen/sounds/328171/) by [pfranzen](https://freesound.org/people/pfranzen/), used under [CC BY 3.0](http://creativecommons.org/licenses/by/3.0/).
* The sound when the TV turns off is [Electric1.wav](https://freesound.org/people/xtrgamr/sounds/321420/) by [xtrgamr](https://freesound.org/people/xtrgamr/), used under [CC BY 3.0](http://creativecommons.org/licenses/by/3.0/).
* The visual static when choosing a channel is adapted from [Random static.gif](https://commons.wikimedia.org/wiki/File:Random_static.gif) by [Atomicdragon136](https://commons.wikimedia.org/wiki/User:Atomicdragon136), in the public domain.

## ![[See also]](https://www.kdau.com/headers/see-also.png)

* [Release notes](https://gitlab.com/kdau/portabletv/-/blob/main/doc/RELEASE-NOTES.md)
* [Source code](https://gitlab.com/kdau/portabletv)
* [Report bugs](https://gitlab.com/kdau/portabletv/-/issues)
* [My other Stardew stuff](https://www.kdau.com/stardew)
* Mirrors:
	[Nexus](https://www.nexusmods.com/stardewvalley/mods/5674),
	**ModDrop**,
	[forums](https://forums.stardewvalley.net/resources/portable-tv.52/)

Other TV mods you may enjoy:

* ![[icon]](https://www.kdau.com/PublicAccessTV/icon.png) [Public Access TV](https://www.moddrop.com/stardew-valley/mods/757967-public-access-tv) for the day's mining conditions, garbage loot, train schedules, rare events and more
* [Gardening with Hisame](https://www.nexusmods.com/stardewvalley/mods/5485) for farm beautification tips
