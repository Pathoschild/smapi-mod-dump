**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Nerf Cave Puzzle
=================================

![Crystals gif](docs/crystals2.gif)

This mod makes that one cave puzzle easier.

By default, it simply requires the player interact with the statue again before the next round plays; this allows you to take a breather between rounds. But optionally, you can slow down the patterns (by increasing the `SpeedModifier`) or make the flashes last longer (by increasing `FlashScale`). You can also cap the maximum number of notes in eacn round with `MaxNotes`.

## Install

1. Install the latest version of [SMAPI](https://smapi.io).
2. Download and install [AtraCore](https://www.nexusmods.com/stardewvalley/mods/12932).
2. Download this mod and unzip it into `Stardew Valley/Mods`.
3. Run the game using SMAPI.

## Configuration

1. `SpeedModifer`: A multiplier that controls the gap between notes in the puzzle. Set higher to increase the gap between notes. **This can be set locally** (1 = same as vanilla.)
2. `FlashScale`: A multiplier that controls how long the flashes of light last. Set higher for them to last longer. **This can be set locally** (1 = same as vanilla.)
3. `MaxNotes`: Caps the maximum number of notes per round. (Default to 7, same as vanilla). **Host must set this - it is ignored for farmhands**.
4. `PauseBetweenRounds`: If set true, the puzzle will pause between rounds - you must talk to the statue again to continue. **Host must set this - it is ignored for farmhands**. (Much thanks to [unlitday](https://www.nexusmods.com/users/114778613/) for writing the dialogue for me!)
5. `AllowReAsks`: Whether or not the statue will repeat the pattern if asked to.

## Compatibility

* Works with Stardew Valley 1.5.6 on Linux/macOS/Windows.
* Works in single player, multiplayer, and split-screen mode. Probably best if everyone installs this mod in multipalyer.
* Should be compatible with most other mods. 

Technical note: this mod heavily relies on transpilers and may not be compatible with any other mod that edits the cave puzzle.

## See also

* [Changelog](docs/Changelog.md)
