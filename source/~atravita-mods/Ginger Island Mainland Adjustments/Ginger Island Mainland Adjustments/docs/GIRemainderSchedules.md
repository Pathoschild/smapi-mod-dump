**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Ginger Island Remainder Schedules
=================================

[← back to readme](../../README.md) <br/>
[← back to main guide](./README.MD)

GIRemainder schedules work very similarly to how normal schedule keys work and should be placed in the usual schedule file `Characters/schedules/<NPCname>`.

All GIRemainder keys must start with `GIRemainder`. If an NPC is married, they'll use `GIRemainder_marriage` as their base key instead, and the non-marriage keys are entirely ignored.

Next, keys are checked in the following order

1. `<base>_<season>_<day>` (ie `GIRemainder_winter_19`. Note that NPCs can't go to GI on their annual checkup day. They also can't go during the Night Market)
2. `<base>_<day>_<hearts>` (ie `GIRemainder_19_8`)
3. `<base>_<day>` (ie `GIRemainder_19`)
4. `<base>_rain` (for rainy days)
5. `<base>_<season>_<short day of week><hearts>` (ie `GIRemainder_winter_Fri6`)
6. `<base>_<season>_<short day of week>` (ie `GIRemainder_winter_Fri`)
7. `<base>_<short day of week><hearts>` (ie `GIRemainder_Fri6`)
8. `<base>_<short day of week>` (ie `GIRemainder_Fri`)
9. `<base><hearts>` (ie `GIRemainder6`)
10. `<base>_<season>` (ie `GIRemainder_winter`)
11. `<base>` (`GIRemainder`)

`<hearts>` is any valid integer - it'll handle even values above 14.

GI schedules are constructed similarly to normal schedules. You can use `MAIL`/`GOTO`/`NOT` friendship basically the same as vanilla keys; however, if the key is rejected due to error or whatever, GI schedules go to the next valid schedule and not just direct to `spring`. The construction of each element is also the same, `<time> <location> <x> <y> <facing direction> <animation> <dialogue>`.

For GI keys, the first time *must be* 1800. In general, it takes quite a while for NPCs to cross the map after getting home from GI, so I wouldn't recommend putting the next key any closer than 2100. (The schedule check command `av.gima.get_schedule <NPCname>` will give you an estimate of how long it will take for an NPC to traverse between schedule points.)

For examples, please check out the [example pack](https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments/blob/master/%5BCP%5D%20Ginger%20Island%20Mainland%20Adjustments/schedules.json).

See also: [the wiki's documentation on schedules](https://stardewvalleywiki.com/Modding:Schedule_data)!