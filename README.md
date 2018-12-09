This repository contains all SMAPI mods in the [SMAPI compatibility list](https://stardewvalleywiki.com/Modding:SMAPI_compatibility).
It's used to update that page, find mods using specific APIs, etc.

## Mod stats
* Last updated 2018-12-08 (SMAPI 2.8.2 and Stardew Valley 1.3.32).
* 603 mods in the SMAPI compatibility list.
* 400 mods (67%) have a valid Git repository.
* 168 repositories (42%) contain multiple mods.

## What's in this repository
* `compiled` has the latest download for each mod, grouped into these categories:

  category       | description
  -------------- | -----------
  abandoned      | Mods which are obsolete, or have been abandoned by their authors and probably won't be updated unofficially. These will likely never be updated again.
  broken in \*   | Mods which broke in a specific game version.
  okay           | Mods which work fine in the latest versions (and don't fit one of the next two categories).
  okay (Harmony) | Mods which work fine in the latest versions, and use Harmony to patch the game code. Using many Harmony mods together often causes conflicts, so these are separate for testing.
  okay (Pong)    | The [Pong mod](https://www.nexusmods.com/stardewvalley/mods/1994). This overrides the entire game, so it's separate for testing.

* `source` has the latest source code for each open-source SMAPI mod. This only has the _code_, it
  doesn't mirror the Git history. Since many repositories contain multiple mods, mods in `compiled`
  don't necessarily map directly to repositories in `source`. Although `compiled` may contain
  unofficial updates, `source` only has the official repositories.

* `utilities` contains scripts used to update this repository.
