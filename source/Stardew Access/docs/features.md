**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/stardew-access/stardew-access**

----

# Features

## Table Of Contents

- [Game Narration](#game-narration)
- [Tile Reader](#tile-reader)
    - [Tile Viewer](#tile-viewer)
    - [Tile Info Menu](#tile-info-menu)
- [Object Tracker](#object-tracker)
    - [Object Tracker Favorites](#object-tracker-favorites)
- [Grid Movement](#grid-movement)
- [Radar](#radar)
- [Other Pages](#other-pages)

## Game Narration

Stardew Access uses your screen reader to narrate the game as you play, this includes objects, menus, inventories, shops, character dialogue, and more. Some information is narrated on request: the player's map and coordinates, health and energy, currency, and more To convey all of your stats exactly when you need them.
Warnings are also included for full inventory, low health, low energy, and being out very late.

**Important Notes:**

1. Narration may be interrupted due to your screen reader's configuration
    - If using NVDA, ensure that "sleep mode" is turned on or that "Speech Interrupt for Typed Characters" in NVDA's keyboard settings section is turned off. It is recommended to set up a profile to do this automatically [NVDA user guide section 12.4: Profiles](https://www.nvaccess.org/files/nvda/documentation/userGuide.html#ConfigurationProfiles).
<!--todo: document procedures for other platforms-->

For more information, see the following pages:

- [Keys](keybindings.md#global-keys)
- [Narration & Verbosity Configs](config.md#narration--verbosity-configs)

## Tile Reader

This feature set makes up the core of Stardew Access. It reads the tile that the player is currently facing by getting the tile's info internally.
Extra tile information can be added via `tiles.json` located in `assets/TilesData` directory.
You may also create `tiles_user.json` in `assets/TilesData` directory to add new entries via the tile info menu in-game. these entries may be made dependent on certain mods, farm types, and other options.

<!--todo: add manual user.json guide-->

### Tile Viewer

This feature extends the tile reader by allowing you to move the tile cursor anywhere on the active map. This feature is handy for exploring, placing items, selecting tiles, and quickly adding and removing items to machines.
You may also walk to selected tiles with a keystroke, open the tile info menu to get more info about any objects, or add custom tile info to a selected tile.

**Important Notes:**

1. For better navigation and mouse button simulation, the cursor is automatically snapped to the tile that the player is looking at unless relative cursor lock has been enabled.
2. Minimizing or otherwise unfocusing the window without pausing will cause this feature to temporarily stop working when you return to the window. To prevent this, always pause the game before switching to another window.

### Tile Info Menu

This menu is opened with `left ctrl + enter` and allows you to mark the tile's coordinates for future reference, add the tile to custom tile data, and speak detailed tile info.

**Important notes:**

1. The third option, the one that should speak the details about the tile only speaks the tile's name.
2. We can only have one mod as a dependency although having multiple in the `tiles_user.json` is supported.
3. We can only set the currently ongoing event or the current farm type as dependency.
4. Festivals are not properly detected.

For more information about all tile reader features, including the tile info menu, see the following pages:

- [Tile Viewer Keys](keybindings.md#tile-viewer-keys)
- [Tile Viewer Configs](config.md#tile-viewer-configs)
- [Global & Tile Reader Keybindings](keybindings.md#global-keys)
- [Tile Reader Commands](commands.md#tile-reader-commands)

## Object Tracker

This feature set allows you to track individual objects on the map. from doorways to forageables to animals and slimes, the object tracker sorts everything into categories and provides a list of everything on the map, sorted by proximity.
You can get coordinates, distance, and auto-travel to the nearest instance of the selected object.

### Object Tracker Favorites

Set frequently-traveled spots as a favorite. With 10 favorites per stack and an effectively-limitless set of stacks of favorites per map, you can efficiently get anywhere you need to.

For more information about the object tracker feature set, including object tracker favorites, see the following pages:

- [Object Tracker Keys](keybindings.md#object-tracker-keys)
- [Object Tracker configs](config.md#object-tracker-configs)

## Grid Movement

When enabled, the player moves 1 tile at a time and makes a footstep sound on every movement. This feature is very handy when precise movement is required, such as when planting or harvesting crops.

**Important notes:**

1. In case you encounter the player moving more than one step or the speed being faster than usual,
try reducing the speed of grid movement from the config.

For more information, see the following pages:

- [Grid Movement Keys](keybindings.md#grid-movement-keys)
- [Grid Movement Configs](config.md#grid-movement-configs)

## Radar

This feature will play auditory beacons at the location of the nearest selected object.

**Important Notes:**

1. This feature is experimental and not well-developed and is not recommended for general gameplay.

For more information, see the following pages:

- [Radar Commands](commands.md#radar-commands)
- [Radar Configs](config.md#radar-configs)

## Other Pages

- [Readme](README.md)
- [Setup](setup.md)
- [Keybindings](keybindings.md)
- [Commands](commands.md)
- [Configs](config.md)
- [Guides](guides.md)
