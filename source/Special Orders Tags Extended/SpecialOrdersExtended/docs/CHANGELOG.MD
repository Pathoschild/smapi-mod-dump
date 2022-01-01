**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/SpecialOrdersExtended**

----

Changelog
============

#### Todo

1. Add condition to just randomly reduce the chance of a quest appearing. (May need integrate with that one mod that changes the day special orders appear?)
2. Add condition for total number of quests (both Qi and otherwise completed.)
3. Document tokens.
4. Document dialogue keys, their storage, and how to use console commands to check that.
5. Move loading of data models to first use and not when the save is loaded, perhaps.


##### In consideration

1. Somehow limit the kill objective to just monsters on a certain (few?) maps. (Perhaps a custom objective, perhaps a Special Rule).

#### Version 1.0.4 (upcoming)

* Added tokens (`Current`, `Available`, `Completed`, `RecentCompleted`). (Thanks, Lumina, for the suggestion!)
* Added token for the current special order rules (`CurrentRules`) (Thanks, shekurika, for the suggestion!)
* Added `achievement`, `walnutcount` to possible tags. De-hardcoded the stats section (and added a console command to list available stats). (Thanks, vert, for the suggestion!)
* Moved saving of previously-said dialogue lines to a custom data class instead of using mailflags.
* Alter dialogue conditions so `_Completed` is said for the rest of the day instead of being forgotten when the player dismisses the quest

#### Version 1.0.3

No real changes, just recompiled for NET 5.0/Stardew 1.5.5/SMAPI 3.13.0+

#### Version 1.0.2

Adds dialogue keys, adds the `dropboxRoom` tag.

#### Version 1.0.1 

Added tags for active conversation topics, total money earned, and crafting/cooking recipes.
