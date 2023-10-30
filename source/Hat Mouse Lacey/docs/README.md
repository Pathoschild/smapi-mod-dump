**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ichortower/HatMouseLacey**

----

# Hat Mouse Lacey
Adds a new, familiar face to Stardew Valley.

This mod turns the hat mouse from the far corner of Cindersap Forest (normally
a motionless map sprite with no name and only a few lines of dialogue) into a
full-fledged NPC.

![The farmer character is talking to an anthropomorphic mouse named Lacey
outside her house in the forest.](images/banner.png)

Her name is Lacey, and she is single.

It's not required, but I think you'll have the best experience with her if you
start a new save, since she has some Year 1 dialogue.

I also recommend not snooping around in the mod files, since you may find
spoilers in there, and I think it's fun to discover things and be surprised.
Of course, if you like spoilers, be my guest; I can't stop you.

Gracious acknowledgement goes to
[this Reddit post](https://old.reddit.com/r/StardewValley/comments/12crela/thought_i_knew_stardew_well_but_evidently_not_who/jf2sjk0/)
for inspiring me to make this mod.

## Requirements
This mod has only two dependencies:

* [SMAPI](https://smapi.io) (the mod framework for Stardew Valley)
* [Content Patcher](https://github.com/Pathoschild/StardewMods/tree/develop/ContentPatcher)

## Installation
Download the [latest release](https://github.com/ichortower/HatMouseLacey/releases/latest)
and unzip it into your Mods folder. It contains two folders:

* `HatMouseLacey`
* `HatMouseLacey_Core`

(If you see folders called `CP`, `SMAPI`, `docs`, etc. instead, that's the
source code zip. Use the other one for installing to your game.)

`HatMouseLacey` is a Content Patcher content pack, which contains all of the
images, map data, and text to be injected into the game. `HatMouseLacey_Core`
is the C# mod which makes all the code changes necessary for Lacey's special
features.

Because the Core mod loads the music I wrote for Lacey's events, that folder
contains the music files.

## Configuration
At this time, Hat Mouse Lacey supports four config settings:

* `DTF`: true/false (default true). If true, enables some suggestive dialogue
(nothing more so than Emily's sleeping bag). Set to false to keep it G-rated.
* `AlwaysAdopt`: true/false (default true). If true, Lacey will always adopt
children when married to the farmer. If false, she can become pregnant with a
male farmer.
* `RecolorPalette`: one of `Auto`, `Vanilla`, `Earthy`, `VPR`, `Starblue`, or
`Wittily`. (default Auto). If set to Auto, this mod will attempt to detect
which recolor mod you are using and match it. If you get the wrong result, you
can manually set it to the desired value.
* `InteriorPalette`: one of `Auto`, `Vanilla`, `Earthy`, `VPR`, or `Starblue`
(default Auto). This is just like RecolorPalette, but attempts to detect and
match enabled interior recolors. (Wittily does not recolor interiors, so it is
not listed as an option)

**These config settings will be read from the HatMouseLacey_Core mod's
config.json.** The Core mod will appear in the Generic Mod Config Menu, if you
have that installed. The settings will apply to both mods.

More config settings may be added in future releases.

## Roadmap
* ~~`1.0`: Initial release.~~
* `1.1`: Current release. Now on 1.1.0.
* `1.2`: The Content Update. Add extra dialogue, seasonal outfits, beach
visits, etc.
* `1.6`: Update the mod to work with Stardew Valley 1.6.

Subject to change, especially if 1.6's release date appears.

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
* &check; [NPC Map Locations](https://www.nexusmods.com/stardewvalley/mods/239)
* &check; [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)
* &check; [Stardew Valley Reimagined 3](https://www.nexusmods.com/stardewvalley/mods/13497)
* &check; [Community Center Reimagined](https://www.nexusmods.com/stardewvalley/mods/6966)
* &cross; Reskins\
    I am awaiting permission to provide full support for several town building
    retexture mods.
* &cross; Map mods which alter the terrain near the shack
* &cross; Mods which add an interior map to the shack\
    (incompatible vision; EWONTFIX)
* &cross; [Hat Shop Restoration](https://www.nexusmods.com/stardewvalley/mods/17291)\
    (incompatible vision; EWONTFIX)
* &cross; [Hat Mouse and Friends](https://www.nexusmods.com/stardewvalley/mods/17364)\
    (incompatible vision; EWONTFIX)
* &cross; [Fashion Mouse](https://www.nexusmods.com/stardewvalley/mods/17502)\
    (incompatible vision; EWONTFIX)
* &cross; Any other mods which <details><summary>Spoiler</summary>add other mouse characters (lore conflict; EWONTFIX)</details>

## Other Questions You May Have

### I need help with \<problem\>. What should I do/Can you help?
There is a [spoiler-filled help file](help-spoilers.md). (**WARNING**: spoilers!)
It has some tips and information in it.

If you have found a bug (including compatibility problems), please open a ticket.

I also frequent
[the Stardew Valley Discord server](https://discord.gg/stardewvalley). I'm
probably in `#making-mods` or `#modded-farmers`; you can ask me for help or
report problems to me there as well.

### Is this compatible with \<mod name here\>?
If it's not listed above, under **Compatibility**, the default answer is
"probably, technically". What that means is that I don't expect this mod to
crash your game or break much of it, but there may be asset conflicts, weird
behavior, or lore clashes.

This even applies to mods above which I have listed as incompatible and/or
EWONTFIX (for example, reskins). Your game should still run and most things
should still work. But it may be jarring, especially if those kinds of errors
greatly bother you.

If you find any compatibility problems with mods not listed above, I would be
much obliged to you if you let me know.

### What about multiplayer?
It should be compatible now. Special thanks to Nexus user
[MriaMoonrose](https://www.nexusmods.com/stardewvalley/users/133194498) for
being the multiplayer guinea pig who found (and reported!) the bugs.

### kind of a weird decision to make the mouse datable
That's not a question.
