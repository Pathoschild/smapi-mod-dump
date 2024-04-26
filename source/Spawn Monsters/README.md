**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Berisan/SpawnMonsters**

----

# Spawn Monsters

A [Stardew Valley](https://www.stardewvalley.net/) mod - Spawn any monster you want, anywhere you want!

## Installation

1. Install the latest version of [SMAPI](https://smapi.io/).
2. [Download this mod](https://www.nexusmods.com/stardewvalley/mods/3201) and unzip it into `Stardew Valley/Mods`.
3. Run the game using SMAPI.

## Mod Usage

Use the configurable hotkey (default: `P`) to open the monster selection menu.

The mod adds some commands:

| command | description |
| - | - |
| monster_menu | Opens the monster selection menu. |
| monster_list | Prints a list of available monsters. |
| monster_spawn `"Name"` `[positionX]` `[positionY]` `[amount]` | Spawns `<amount>` monsters at given (or at the Farmer's) coordinates. Exclude the brackets. |
| farmer_position | Prints your current position for use with the `monster_spawn`  command. |
| remove_prismatic_jelly | Removes all Prismatic Jelly from your inventory. |

## Compiling the mod

<!-- This text taken from Pathoschild's StardewMods: https://github.com/Pathoschild/StardewMods -->

Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

This mod uses the [crossplatform build config](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
so it can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
for troubleshooting.

### Compiling a mod for testing
To compile a mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](https://www.monodevelop.com/).
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### Compiling a mod for release
To package a mod for release:

1. Switch to `Release` build configuration.
2. Recompile the mod per the previous section.
3. Upload the generated `bin/Release/<mod name>-<version>.zip` file from the project folder.

## See also

- [Nexus Mods Page](https://www.nexusmods.com/stardewvalley/mods/3201)
