**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Changelog
==============

#### ToDo
1. Catch input to place rug under things.
<!-- Figure out how to replace the reference to coffee in the night market dialogue?-->
<!-- Make it so notifications do not go away until dismissed? -->
<!-- Make farm pan faster for animal menu and do some sort of keybind mess to snap to building-->
<!-- do I need to override reading secret notes too? -->
<!-- fix the stupid integration issues with CM/SAAT -->

#### Version 1.1.2
* By popular request, defang the napalm ring in safe areas.
* New fixes: Journal scraps are now less silly, also removing a mod that adds secret notes won't lock you out of secret notes.
* Jukeboxes are now sanity-checked at save load, so authors changing audio cues no longer breaks things.

#### Version 1.1.1
* Some menus now accept arrow keys.
* Internal improvements (mostly with regard to enums).
* Jukebox now checks your current location, not the mainland, for the is-it-raining check.
* Adds a way to flip fruit trees and crops. Yes I am this petty.
* Leo no longer attends the Winter Star festival before he's moved to the mainland.
* Schedule fixer will now detect infinite recursion and prevent those crashes as well.
* Difficulty setting for the crane game.
* Hopefully escaped children will cause fewer crashes?
* Korean translation, thanks to [park4934](https://blog.naver.com/park971202/222878509680)!.

#### Version 1.1.0
* Move to using AtraCore.
* Spanish translation, thanks to [Mistyspring](https://github.com/misty-spring)!
* Prevent broken sound cues from breaking the game.
* Slingshot ammo now stacks instead of swapping.
* Schedule fixer now tries to resolve `GOTO spring` and other statements to use the actual schedule.
* Allows speeding up of phone calls.

#### Version 1.0.11
* Fix issues with the night market warp home.
* Some internal improvements.

#### Version 1.0.10
* Bugfixes
* Add code that prevents broken special orders from crashing the quest menu.

#### Version 1.0.9
* Splitscreen fixes.
* Reorder confirmation dialogue to more match eating dialogue
* Break Return Scepter into individual settings.
* Bugfixes.
* Volcano chests will now avoid giving you the same thing twice in a row.
* Russian translation, thanks to [Angel4Killer](https://github.com/angel4killer)!

#### Version 1.0.8
Compiled for SMAPI 3.14.0
* Removes damage to fruit trees from hoes and scythes.
* Option to save bombed forage.
* Hides crab pots during festivals and events.
* Add bet 1k and bet 10k to the casino
* Adds confirmation to warps.

#### Version 1.0.7

* Just-in-case compatability adjustments for Smart Building - disables this mod's ability to place grass under things whenever Smart Building's build mode is on, which IMO makes it easier to place things anyways....

#### Version 1.0.6

* Remove excess debugging statements accidentally left in. My apologies.

#### Version 1.0.5 : Rebrand to Atra's Interaction Tweaks

* Allows placement of grass under things.
* Bomb confirm: Bombs placed in "non dangerous" areas will ask for a confirmation first. Definitely safe areas: farm, greenhouse, town and Ginger Island farm. Definitely unsafe areas: mines, skull caverns, and volcano dungeon. Everywhere else is unsafe if there's a single monster on the map. (Configurable)
* Fix splitscreen issues.
* Clicking on a butterfly hutch spawns a butterfly.
* Jukeboxes can be placed anywhere.
* Config to allow coconuts to be dropped by palm trees off island.

#### Version 1.0.4

* Makes the gate message less spammy.

#### Version 1.0.3

* Allows outdoor rugs.
* Gates no longer pop off when right click is held near them.

#### Version 1.0.2

* Catches ill-formed config files and asks users to use GMCM instead. Adds GMCM version check.
* Prevents accidental removal of items from tables as well.