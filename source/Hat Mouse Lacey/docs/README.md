**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ichortower/HatMouseLacey**

----

# Hat Mouse Lacey

![The text 'Hat Mouse Lacey' and a pixel art portrait of an anthropomorphic
mouse character against a background of a cabin by a river](images/hero.jpg)

Adds a new, familiar face to Stardew Valley.

This mod turns the hat mouse from the far corner of Cindersap Forest (normally
a motionless map sprite with no name and only a few lines of dialogue) into a
full-fledged NPC.

Her name is Lacey, and she is single.

It's not required, but I think you'll have the best experience with her if you
start a new save, since she has some Year 1 dialogue.

I also recommend not snooping around in the mod files, since you may find
spoilers in there, and I think it's fun to discover things and be surprised.
Of course, if you like spoilers, be my guest; I can't stop you.

## Special Thanks and Acknowledgements
This mod was inspired by a [single Reddit post](https://old.reddit.com/r/StardewValley/comments/12crela/thought_i_knew_stardew_well_but_evidently_not_who/jf2sjk0/).

My heartfelt thanks to all the other weirdos (affectionate) in the
[Stardew Valley Discord server](https://discord.gg/stardewvalley) who like to
hang out and talk about mods and give help and advice to fellow modders.

Special thanks to **ElizaPhantomhive** on the Discord for doing a heroic
amount of work helping me find and fix a compatibility problem (with
[The Lorax](https://www.nexusmods.com/stardewvalley/mods/15012)).

Special thanks to Nexus user
[MriaMoonrose](https://www.nexusmods.com/stardewvalley/users/133194498)
for finding and reporting the bugs in multiplayer.

Thanks to the following contributors for providing translations:

* Turkish: [Rupurudu](https://github.com/Rupurudu)

My thanks as well to the following mod authors:

* **Gweniaczek** of [Way Back Pelican Town](https://www.nexusmods.com/stardewvalley/mods/7332)
* **Elle** of [Elle's Town Buildings](https://www.nexusmods.com/stardewvalley/mods/14524)
* **YriKururu** of [Project Yellog](https://www.nexusmods.com/stardewvalley/mods/14765)

... who have given me their gracious permission to, where necessary, include
derived versions of some of their mod assets in this mod. These files are
**NOT** covered by this mod's MIT license, since they are derivative works
used by permission.

For a full list of files which this mod's permissive license **DOES NOT** 
cover, see the file [nonlicensed.txt](nonlicensed.txt).


## Requirements

* Stardew Valley 1.6
* [SMAPI (4.0.0+)](https://smapi.io) (the mod framework for Stardew Valley)
* [Content Patcher (2.0.0+)](https://github.com/Pathoschild/StardewMods/tree/develop/ContentPatcher)

If you are using the legacy branch (1.5.6) of Stardew Valley, use the 1.1.3
release of this mod; that is the latest one compatible with the previous game
version.


## Installation
Download the [latest release](https://github.com/ichortower/HatMouseLacey/releases/latest)
and unzip it into your Mods folder. It contains two folders:

* `HatMouseLacey`
* `HatMouseLacey_Core`

(If you see folders called `CP`, `SMAPI`, `docs`, etc. instead, that's the
source code zip. Use the other one for installing to your game.)

`HatMouseLacey` is a Content Patcher content pack, which handles all of the
content: images, maps, text, music, event scripts, etc. `HatMouseLacey_Core`
is the C# mod which makes all the code changes necessary for Lacey's special
features.


## Configuration
In version 1.2, Hat Mouse Lacey supports seven config settings:

* `DTF`: true/false (default true). If true, enables some suggestive (but not
  explicit) dialogue. Set to false to keep it G-rated.
* `AlwaysAdopt`: true/false (default true). If true, Lacey will always adopt
  children when married to the farmer. If false, she can become pregnant with a
  male farmer.
* `RecolorPalette`: one of `Auto`, `Vanilla`, `Earthy`, `VPR`, `Starblue`, or
  `Wittily` (default Auto). If set to Auto, this mod will attempt to detect
  which recolor mod you are using and match it. If you get the wrong result, you
  can manually set it to the desired value.
* `InteriorPalette`: one of `Auto`, `Vanilla`, `Earthy`, `VPR`, or `Starblue`
  (default Auto). This is just like RecolorPalette, but attempts to detect and
  match enabled interior recolors. (Wittily does not recolor interiors, so it is
  not listed as an option)
* `MatchRetexture`: one of `Auto`, `Vanilla`, `WaybackPT`, `ElleTown`,
  `YriYellog`, or `FlowerValley` (default Auto). This is like RecolorPalette,
  but it matches active building retextures that apply to the mouse house. As
  with the others, set manually if Auto does not detect your situation.\
  **NOTE**: Elle's Town Buildings and Yri's Project Yellog will only be detected
  by `Auto` if you have set their respective config settings to retexture the
  mouse house (for Yellog, you will need `HatMouseHouseRestored`). Otherwise,
  you'll get the vanilla appearance.
* `SeasonalOutfits`: true/false (default false). Enables Lacey's optional
  alternate outfits for summer and fall. The winter outfit is automatically
  enabled, since those are vanilla.
* `WeddingAttire`: one of `Dress` or `Tuxedo` (default Dress). Choose which
  outfit Lacey will wear to her wedding.

**These config settings will be read from the HatMouseLacey_Core mod's
config.json.** The Core mod will appear in the Generic Mod Config Menu, if you
have that installed. The settings will apply to both mods.

More config settings may be added in future releases.


## Roadmap
* ~~`1.0`: Initial release.~~
* ~~`1.1`: The last SDV 1.5.6-compatible releases.~~
* `1.2`: Current release. Compatible with Stardew Valley 1.6.
* Beyond: I plan to continue supporting this mod for a while. Future content
  updates, feature additions, and compatibility improvements are expected.


## Compatibility
&check; Fully supported\
&rarrc; Partial or in-progress\
&cross; Expect breakage or major issues

Mods marked with EWONTFIX have conflicts I am not (currently) attempting to
resolve.

Even mods listed as incompatible and/or EWONTFIX probably won't break your
game, but please let me know if they do.

* &check; [Ridgeside Village](https://www.nexusmods.com/stardewvalley/mods/7286)\
    Lacey attends the RSV festivals.
* &check; [East Scarp](https://www.nexusmods.com/stardewvalley/mods/5787)\
    I have not tested all additional content, so some problems may remain.
* &check; [Stardew Valley Expanded](https://www.nexusmods.com/stardewvalley/mods/3753)\
    Now fully compatible. More tweaks and tie-ins may come later.
* &check; Multiple popular map recolors:\
    [DaisyNiko's Earthy Recolour](https://www.nexusmods.com/stardewvalley/mods/5255)\
    [Vibrant Pastoral Recolor](https://www.nexusmods.com/stardewvalley/mods/6367)\
    [Starblue Valley](https://www.nexusmods.com/stardewvalley/mods/1869)\
    [A Wittily Named Recolor](https://www.nexusmods.com/stardewvalley/mods/2995)\
    Additional recolors may be supported in the future.
* &check; Multiple popular building retextures:\
    [Way Back Pelican Town](https://www.nexusmods.com/stardewvalley/mods/7332)\
    [Elle's Town Buildings](https://www.nexusmods.com/stardewvalley/mods/14524)\
    [Yri's Project Yellog](https://www.nexusmods.com/stardewvalley/mods/14765)\
    [Flower Valley](https://kayainsdv.postype.com/post/10220280)\
    Additional retextures may be supported in the future.\
    **NOTE**: for Elle's Town Buildings and Project Yellog, you will need to
    set their config values for the mouse house. See **Configuration**, above.
* &check; [NPC Map Locations](https://www.nexusmods.com/stardewvalley/mods/239)
* &check; [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)
* &check; [Stardew Valley Reimagined 3](https://www.nexusmods.com/stardewvalley/mods/13497)
* &check; [Community Center Reimagined](https://www.nexusmods.com/stardewvalley/mods/6966)
* &cross; Other map mods which alter the terrain near the shack
* &cross; Mods which replace or redesign the vanilla hats
    (Lacey's comments are written for the unmodified versions)
* &cross; Mods which add an interior map to the shack\
    (incompatible vision; EWONTFIX)
* &cross; [Hat Shop Restoration](https://www.nexusmods.com/stardewvalley/mods/17291)\
    (incompatible vision; EWONTFIX)
* &cross; [Hat Mouse and Friends](https://www.nexusmods.com/stardewvalley/mods/17364)\
    (incompatible vision; EWONTFIX)
* &cross; [Fashion Mouse](https://www.nexusmods.com/stardewvalley/mods/17502)\
    (incompatible vision; EWONTFIX)
* &cross; Any other mods which <details><summary>Spoiler</summary>add other mouse characters (lore conflict; EWONTFIX)</details>
* &cross; [Unique Children](https://nexusmods.com/stardewvalley/mods/6278)<details><summary>Spoiler</summary>\
    This mod reimplements child sprites entirely, overriding my patch to provide custom sprites. EWONTFIX, manual compatibility only; see that mod's instructions.</details>


## Other Questions You May Have


### I need help with \<problem\>. What should I do/Can you help?
There is a [spoiler-filled help file](help-spoilers.md). (**WARNING**: spoilers!)
It has some tips and information in it.

If you have found a bug (including compatibility problems), please open a ticket.

I also frequent [the Stardew Valley Discord
server](https://discord.gg/stardewvalley). I'm probably in
`#making-mods-general` or `#modded-farmers`; you can ask me for help or report
problems to me there as well.

### I found something weird and/or broken in 1.2/Stardew 1.6!
Please open a ticket or find me on the Discord server, as above. I am playing
1.6 for the first time myself, so any bugs caused by new content or game
changes are also new to me! I will fix them when I can.

### Is this compatible with \<mod name here\>?
If it's not listed above, under **Compatibility**, the default answer is
"probably, technically". What that means is that I don't expect this mod to
crash your game or break much of it, but there may be asset conflicts, weird
behavior, or lore clashes.

This even applies to mods above which I have listed as incompatible and/or
EWONTFIX. Your game should still run and most things should still work. But it
may be jarring, especially if those kinds of errors greatly bother you.

If you find any compatibility problems with mods not listed above, I would be
much obliged to you if you let me know.

### What about multiplayer?
Everything works correctly, as far as I know. Special thanks to MriaMoonrose
for helping out.

### kind of a weird decision to make the mouse datable
That's not a question.
