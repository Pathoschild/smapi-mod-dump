This repository contains all SMAPI mods in the [SMAPI compatibility list](https://stardewvalleywiki.com/Modding:SMAPI_compatibility).
It's used to update that page, find mods using specific APIs, etc.

## Mod stats
* Last updated 2024-04-01 (SMAPI 4.0.4 and Stardew Valley 1.6.3).
* 2431 mods in the SMAPI compatibility list.
* 1633 mods (67%) have a source code repo, with 945 (58%) in a multi-mod repo and 688 (42%) in a single-mod repo.

## What's in this repository
### `compiled`
The `compiled` folder has the latest download for each mod, grouped into a few categories:

* `abandoned`  
  Mods which are obsolete, or have been abandoned by their authors and probably won't be updated unofficially. These
  will likely never be updated again.
* `broken in *`  
  Mods which broke in that game version.
* `okay`  
  Mods which work fine in the latest versions (and don't fit one of the next two categories).
* `okay (Harmony)`  
  Mods which work fine in the latest versions, and use Harmony to patch the game code. Using many Harmony mods together
  often causes conflicts, so these are separate for testing.
* `okay (special groups)`  
  Mods which work fine in the latest version, but need to be tested separately from other mods. That include:

  mod | reason
  --- | ------
  [Battle Royalley](https://www.nexusmods.com/stardewvalley/mods/9891)      | disables single-player and blocks other mods
  [Betwitched](https://www.nexusmods.com/stardewvalley/mods/10172)          | prevents loading a save without a Twitch username + auth token configured
  [Instant Load](https://www.nexusmods.com/stardewvalley/mods/16253)        | instantly loads a save
  [MobileUI Android](https://www.nexusmods.com/stardewvalley/mods/17652)    | only works on Android
  [Please Fix Error](https://www.nexusmods.com/stardewvalley/mods/6492)     | spams fake errors
  [In-Game SMAPI Log Uploader](https://www.nexusmods.com/stardewvalley/mods/13979) | opens a browser window
  [Pong](https://www.nexusmods.com/stardewvalley/mods/1994)                 | overrides the entire game
  [Stardew Access](https://www.nexusmods.com/stardewvalley/mods/10319)      | locks the mouse cursor by default
  [Stardew Archipelago](https://www.nexusmods.com/stardewvalley/mods/16087) | prevents loading a save if it can't connect to Archipelago
  [Stardew Debt](https://www.nexusmods.com/stardewvalley/mods/10005)        | exits to title if certain other mods are installed
  [Stardew Roguelike](https://www.nexusmods.com/stardewvalley/mods/13614)   | prevents loading a save file

### `source`
The `source` folder has the latest source code for each open-source SMAPI mod. This only has the _code_, it doesn't
mirror the Git history. Since many repositories contain multiple mods, mods in `compiled` don't necessarily map
directly to repositories in `source`. Although `compiled` may contain unofficial updates, `source` only has the
official repositories.

## See also
* [Pathoschild/stardew-mod-dump-utils](https://github.com/Pathoschild/stardew-mod-dump-utils) has
  utility scripts used to maintain this repo.
