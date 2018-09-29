CleanFarm
=========

On each new day this mod will remove all debris and other items from the farm that you specify in the config (see below).

**Warning: It may be a good idea to back up your save game as your farm is saved after the items have been removed.**

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. [Install this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/794/).
3. Run the game using SMAPI.

To uninstall this mod you can simply delete the "CleanFarm" folder from your Mods directory.

## Configuration

In this mod's `config.json` file you will find options to enable/disable which objects are 'cleaned' from your farm. The options are:

```
{
  "RemoveGrass": false,
  "RemoveWeeds": true,
  "RemoveStones": true,
  "RemoveTwigs": true,
  "RemoveStumps": true,
  "RemoveSaplings": true,
  "MaxTreeGrowthStageToAllow": 5, // min 1, max 5
  "RemoveLargeLogs": true,
  "RemoveLargeRocks": true,
  "RemoveBushes": false // Removes the bushes near the shipping container.
  "ReportRemovedItemsToConsole": true // Report extra info about what was removed to the console.
}
```

The `MaxTreeGrowthStageToAllow` is only used if `RemoveSaplings` is true. It represents the max growth stage allowed, so any trees with a lower growth stage will be removed. This allows you to remove all seeds but keep existing saplings etc.
Only values between 1 and 5 are valid.

The growth stages are:
* 0 - Seed
* 1 - Sprout
* 2 - Sapling
* 3 - Bush
* 4 - Small Tree
* 5 - Tree

For more information on how to modify the config see [this page](http://canimod.com/guides/using-mods#configuring-mods).

## Console Commands
* cf_clean - Runs the clean command manually.
* cf_restore - Restores the items removed from the last clean command that occured this play session.
* cf_rload - Reloads the mod config so you can change what to remove on the fly.