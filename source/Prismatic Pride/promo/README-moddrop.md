**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/kdau/prismaticpride**

----

![[icon]](https://www.kdau.com/PrismaticPride/icon.png) Change prismatic items to rotate through the colors of the pride flag of your choice, then deck your farmer out in a wider selection of prismatic clothing to show it off. Let’s see your true colors shining through!

## ![Compatibility](https://www.kdau.com/headers/compatibility.png)

**Game:** Stardew Valley 1.5+

**Platform:** Linux, macOS or Windows

**Multiplayer:** works; every player must install

**Other mods:** Works with Get Glam and Change Shoe Color. Other clothing-related SMAPI mods (but not content packs) might cause lag or visual glitches.

## ![Installation](https://www.kdau.com/headers/installation.png)

1. Install [SMAPI](https://smapi.io/)
1. Install [Json Assets](https://www.moddrop.com/stardew-valley/mods/399895-json-assets)
1. Install [Shop Tile Framework](https://www.moddrop.com/stardew-valley/mods/716384-shop-tile-framework) (optional, for buying the clothing items)
1. Install [Generic Mod Config Menu](https://www.moddrop.com/stardew-valley/mods/771692-generic-mod-config-menu) (optional, for easier configuration)
1. Download this mod from the link in the header above
1. Unzip and put the `PrismaticPride`, `PrismaticPride.JA` and `PrismaticPride.STF` folders inside your `Mods` folder
1. Run the game using SMAPI

## ![Use](https://www.kdau.com/headers/use.png)

When this mod is installed, all prismatic things in the game - clothing, Junimo and more - will cycle through the colors of one of the pride flags. This starts out with the 11 colors of the **[Progress Pride Flag](https://quasar.digital/progress-initiative/)** by default (configurable; see below).

To **change the pride flag** whose colors are used, **press the `U` key** to bring up the menu (key configurable; see below). This menu works like the one for the Jukebox: use the arrows to cycle through the options; press the green play button to apply the option shown; and press the red "no" button to close the menu. See under Acknowledgments below for all the flags and their origins.

This mod adds **10 shirts and 5 pants** to the game. Together with the [prismatic clothing from the base game](https://stardewvalleywiki.com/Prismatic_Shard#Tailoring) and the [Prismatic Skirts and Dresses](https://www.nexusmods.com/stardewvalley/mods/4719) mod, this is a complete collection matching the base game's dyeable clothing collection. To **[tailor](https://stardewvalleywiki.com/Tailoring) a random one** of all these items, use one **Cloth and a Prismatic Shard** at the sewing machine.

This mod also adds **Prismatic Boots** which rotate through colors while worn, just like other prismatic items. To tailor an **existing pair** into Prismatic Boots, use the boots (on the left) and a **Prismatic Shard** (on the right) at the sewing machine. The machine will **preserve the stats** from the existing pair.

If you have installed Shop Tile Framework, you can also **purchase specific items** of prismatic clothing instead of leaving it up to chance. Interact with **Emily's fabric shelves** (just to the right of the sewing machine) to bring up the **shop menu**. On any given day, you will be able to choose from five prismatic shirts, five prismatic pants and the Prismatic Boots, including items from the base game, this mod and (if installed) Prismatic Skirts and Dresses. Each item **costs one Prismatic Shard**.

## ![[Configuration]](https://www.kdau.com/headers/configuration.png)

If you have installed Generic Mod Config Menu, you can access this mod's configuration by clicking the cogwheel button at the lower left corner of the Stardew Valley title screen and then choosing "Prismatic Pride".

Otherwise, you can edit this mod's `config.json` file. It will be created in the mod's main folder (`Mods/PrismaticPride`) the first time you run the game with the mod installed. These options are available:

* `ApplyColors`: Set this to `false` to leave the existing prismatic color rotation unaffected, whether for compatibility with another mod or to restore the base game colors.
* `ColorDuration`: Set this to the number of seconds (decimals OK) that each color should appear in rotation; the default is `1.5`.
* `DefaultColorSet`: Set this to the prismatic color set that should be chosen by default for new players. The available options are `"original"`, `"rainbow"`, `"philadelphia"`, `"progress"`, `"lesbian"`, `"genderfluid"`, `"genderqueer"`, `"bisexual"`, `"pansexual"`, `"polysexual"`, `"trans"`, `"nonbinary"`, `"intersex"`, `"agender"`, `"aromantic"` and `"asexual"`; the default is `"progress"`.
* `ColorSetMenuKey`: Set this to any valid keybinding that will open the menu of prismatic color sets; the default is the `U` key. [See the list of keybindings here.](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings)

## ![Translation](https://www.kdau.com/headers/translation.png)

No translations are available yet.

This mod can be translated into any language supported by the game. Your contribution would be welcome. Please see the [instructions on the wiki](https://stardewvalleywiki.com/Modding:Translations). You can send me your work in the Bugs tab above or in [a GitLab issue](https://gitlab.com/kdau/prismaticpride/-/issues) or the Comments tab above.

## ![Acknowledgments](https://www.kdau.com/headers/acknowledgments.png)

* Like all mods, this one is indebted to ConcernedApe, Pathoschild and the various framework modders.
* The #making-mods channel on the [Stardew Valley Discord](https://discord.gg/StardewValley) offered valuable guidance and feedback.
* The original eight-color rainbow flag was designed by Gilbert Baker in 1978.
* The rainbow flag was amended to its more common six-color version by Baker and others in 1979.
* The Philadelphia pride flag was designed for the City of Philadelphia in 2017.
* The “Progress” Pride Flag was designed by [Daniel Quasar](https://quasar.digital/) in 2018 and is used under [CC BY-NC-SA 4.0](http://creativecommons.org/licenses/by-nc-sa/4.0/).
* The lesbian flag in its current iteration was adopted in 2018.
* The genderfluid flag was designed by JJ Poole in 2012.
* The genderqueer flag was designed by Marilyn Roxie in 2011.
* The bisexual flag was designed by Michael Page in 1998.
* The pansexual flag was adopted in 2010.
* The polysexual flag was designed by Samlin in 2012.
* The transgender flag was designed by Monica Helms in 1999.
* The nonbinary flag was designed by Kye Rowan in 2014.
* The intersex flag was designed by Morgan Carpenter in 2013.
* The agender flag was designed by Salem X/"Ska" in 2014.
* The aromantic flag was designed by Cameron in 2014.
* The asexual flag was adopted in 2010.

## ![See also](https://www.kdau.com/headers/see-also.png)

* [Release notes](https://gitlab.com/kdau/prismaticpride/-/blob/main/doc/RELEASE-NOTES.md)
* [Source code](https://gitlab.com/kdau/prismaticpride)
* [Report bugs](https://gitlab.com/kdau/prismaticpride/-/issues)
* [My other Stardew stuff](https://www.kdau.com/stardew)
* Mirrors:
	[Nexus](https://www.nexusmods.com/stardewvalley/mods/9019),
	**ModDrop**,
	[forums](https://forums.stardewvalley.net/resources/prismatic-pride.81/)

Other mods to consider:

* [Prismatic Skirts and Dresses](https://www.nexusmods.com/stardewvalley/mods/4719) for a complete set of prismatic clothing choices
* [Prismatic Hair Bow](https://www.nexusmods.com/stardewvalley/mods/8916) for an alternate hat option
