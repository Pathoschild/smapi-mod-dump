**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/DecidedlyHuman/StardewValleyMods**

----

# Overarching TODO List
## High-priority TODOs should go in the code where the change will occur.

* Correct spacing restrictions for fruit trees.
* Consider making toolbar buttons data-driven via a config JSON.
* Fix the negative tile coordinate cursor offset bug.
* Figure out how to transplant fish correctly to allow fishtank furniture to be placed.
* Modularise `HasAdjacentNonWaterTile` to not be only for crab pots.
* Tidy up DemolishOnTile, perhaps relying upon a few external methods with appropriate parameters instead of repeated logic within many if statements.
* Investigate whether there's a less terrible way to go about identifying the correct floor/path (as in `GetFlooringIdFromName` and `GetFlooringNameFromId`).
  * I suppose a `Dictionary<string, int>` would do quite nicely as a lookup table here...
* Investigate whether there's a less terible way to get the type of chest, as in `GetChestType`.

# MEASURING TAPE TOOL.
## Duh.