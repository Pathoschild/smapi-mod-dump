This repository contains all SMAPI mods in the [SMAPI compatibility list](https://stardewvalleywiki.com/Modding:SMAPI_compatibility).
It's used to update that page, find mods using specific APIs, etc.

## Mod stats
* Last updated 2023-02-28 (SMAPI 3.18.2 and Stardew Valley 1.5.6).
* 2014 mods in the SMAPI compatibility list.
* 1391 mods (69%) have a valid Git repository.
* 703 repositories (51%) contain multiple mods.

## What's in this repository
* `compiled` has the latest download for each mod, grouped into these categories:

  category       | description
  -------------- | -----------
  abandoned      | Mods which are obsolete, or have been abandoned by their authors and probably won't be updated unofficially. These will likely never be updated again.
  broken in \*   | Mods which broke in that game version.
  okay           | Mods which work fine in the latest versions (and don't fit one of the next two categories).
  okay (Harmony) | Mods which work fine in the latest versions, and use Harmony to patch the game code. Using many Harmony mods together often causes conflicts, so these are separate for testing.
  okay (special groups) | Mods which work fine in the latest version, but need to be tested separately from other mods. That include [Battle Royalley](https://www.nexusmods.com/stardewvalley/mods/9891) (which disables single-player and blocks other mods), [Betwitched](https://www.nexusmods.com/stardewvalley/mods/10172) (which prevents loading a save without a Twitch username + auth token configured), [Please Fix Error](https://www.nexusmods.com/stardewvalley/mods/6492) (which spams fake errors), [Pong](https://www.nexusmods.com/stardewvalley/mods/1994) (which overrides the entire game), [Stardew Access](https://www.nexusmods.com/stardewvalley/mods/10319) (which locks the mouse cursor by default), and [Stardew Roguelike](https://www.nexusmods.com/stardewvalley/mods/13614) (prevents loading a save file).

* `source` has the latest source code for each open-source SMAPI mod. This only has the _code_, it
  doesn't mirror the Git history. Since many repositories contain multiple mods, mods in `compiled`
  don't necessarily map directly to repositories in `source`. Although `compiled` may contain
  unofficial updates, `source` only has the official repositories.

## See also
* [Pathoschild/stardew-mod-dump-utils](https://github.com/Pathoschild/stardew-mod-dump-utils) has
  utility scripts used to maintain this repo.
