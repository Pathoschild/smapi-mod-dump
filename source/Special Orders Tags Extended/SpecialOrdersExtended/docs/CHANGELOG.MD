**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/SpecialOrdersExtended**

----

Changelog
#### Todo

1. Add condition to just randomly reduce the chance of a quest appearing. (May need integrate with that one mod that changes the day special orders appear?)
2. Add condition for total number of quests completed, per board.
<!-- Better documentation for the dialogue tags? -->
3. Clear the inProgress dialogue log when a player *fails* a quest.

#### Stardew 1.6 todo

1. Register anything not included in Game State Queries as GSQ
2. Push people towards GSQ

##### In consideration

1. Somehow limit the kill objective to just monsters on a certain (few?) maps. (Perhaps a custom objective, perhaps a Special Rule).
6. Implement a temporary saving mechanism for Save Anywhere (look into their API?)
<!-- Handle WoL professions? -->

#### Version 1.0.7

* Adds tags for `anyplayermail` and `anyplayerseenevent`.
* Makes my (but **not** game tags) mostly case-insensitive.
* Fixes multiplayer logic.
* Hopefully removes Sunday night crashes that can happen if a special order is unparsable.

#### Version 1.0.6

* Add verbosity config option. Intended mostly for mod authors.
* Fixes splitscreen, but for real this time.
* Special Order Dialogue will now cause the NPC's original dialogue to be delayed by one in-game hour. (Previously, dialogue from this mod would just be added on top of the dialogue stack.)
* `week_X` now handles extended months.
* Add `profession` tag. Makes `skilllevel` tag take into account Spacecore skills, if that's installed (in theory. Memo to self: test that).
* For the `_IsAvailable` dialogue series, remove from possible pool if quest is already accepted.
* Adds condition for total number of Special Orders completed.
* Adds a `random` tag.

#### Version 1.0.5

* In theory: fixes splitscreen.
* `Available` token now correctly returns no entries if the special orders board is not unlocked. Also, adds `_IsAvailable` as a possible dialogue option.
* Adds dialogue key `_RepeatOrder`, which is cleared seven days after the completion of the order.
* Check heart levels for dialogue conditions above 14.
* Added tags: `married`.
* (Internal) Use delegates instead of `PropertyInfo`s.

#### Version 1.0.4

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
