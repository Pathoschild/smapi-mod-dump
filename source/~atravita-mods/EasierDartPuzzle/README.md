**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Easier Dart Puzzle
=================================

![Header image](docs/darts.gif)

This mod allows you to adjust the difficulty of the darts game.

## Install

1. Install the latest version of [SMAPI](https://smapi.io).
2. Download and install [AtraCore](https://www.nexusmods.com/stardewvalley/mods/12932).
2. Download this mod and unzip it into `Stardew Valley/Mods`.
3. Run the game using SMAPI.

## Configuration
Run SMAPI at least once with this mod installed to generate the `config.json`, or use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) to configure.

Options
* `MPPirateArrivalTime` - when the pirates show up and start partying, in multiplayer. This setting was put in because the darts game won't freeze time in multiplayer, which adds an element of time pressure.
* `MinDartCount` - the dart count for the hardest round of the game. 
* `MaxDartCount` - the dart count for the easiest round of the game.
* `ShowDartMarker` - draws a small dot at the center of the circle.
* `JitterMultiplier` - how fast the dart should move in its figure-8 pattern. Set higher for slower dart movement.
* `DartPrecision` - how much randomness the dart should have after it's released. Set higher for less dart randomness.

(note - if you set the `MinDartCount` greater than the `MaxDartCount`, the code will automatically swap the two.)

## Compatibility

* Works with Stardew Valley 1.5.6 on Linux/macOS/Windows.
* Works in single player, multiplayer, and split-screen mode. Probably fine for only one player to install in multiplayer, but I haven't tested this. Safer for both to install.
* Should be compatible with most other mods. 

## See also

[Changelog](docs/Changelog.MD)
