**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/tylergibbs2/StardewValleyMods**

----

This repository contains my SMAPI mods for Stardew Valley. See the individual mods for documentation and release notes.

## Mods

* **Battle Royalley - Year 2** ([nexus](https://www.nexusmods.com/stardewvalley/mods/9891)/[source](https://github.com/tylergibbs2/StardewValleyMods/tree/master/BattleRoyale)) - Battle Royalley: Year 2 is an update to the original Battle Royalley mod for Stardew Valley.
* **Cold Pets** ([nexus](https://www.nexusmods.com/stardewvalley/mods/14379)/[source](https://github.com/tylergibbs2/StardewValleyMods/tree/master/ColdPets)) - Pets stay inside during winter.
* **Default Farmer** ([nexus](https://www.nexusmods.com/stardewvalley/mods/12421)/[source](https://github.com/tylergibbs2/StardewValleyMods/tree/master/DefaultFarmer)) - Adds a Load and Save button to the character creation menus.
* **Live Progress Bar** ([nexus](https://www.nexusmods.com/stardewvalley/mods/7330)/[source](https://github.com/tylergibbs2/StardewValleyMods/tree/master/LiveProgressBar)) - Shows live progress on Stardew Valley with a menu for advanced details.
* **Junimo Boy** ([nexus](https://www.nexusmods.com/stardewvalley/mods/14384)/[source](https://github.com/tylergibbs2/StardewValleyMods/tree/master/JunimoBoy)) - Adds a mobile game console to play your favorite arcade games.
* **Seasonal Save Slots** ([nexus](https://www.nexusmods.com/stardewvalley/mods/14382)/[source](https://github.com/tylergibbs2/StardewValleyMods/tree/master/SeasonalSaveSlots)) - Adds seasonal flair to save slots in the load game menu.
* **StardewHitboxes** ([nexus](https://www.nexusmods.com/stardewvalley/mods/12264)/[source](https://github.com/tylergibbs2/StardewValleyMods/tree/master/StardewHitboxes)) - Visualize the hitboxes of characters, farmers, and weapons!
* **Stardew Nametags** ([nexus](https://www.nexusmods.com/stardewvalley/mods/12158)/[source](https://github.com/tylergibbs2/StardewValleyMods/tree/master/StardewNametags)) - Add nametags above players in Stardew Valley!
* **Stardew Valley - Roguelike** ([nexus](https://www.nexusmods.com/stardewvalley/mods/13614)/[source](https://github.com/tylergibbs2/StardewValleyMods/tree/master/StardewRoguelike)) - Explore a new area of the mines with this unique dungeon crawling experience for Stardew Valley!

## Compiling the mods

Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

These mods use the [crossplatform build config](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
so they can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
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
3. Upload the generated `_releases/<mod name>-<version>.zip` file from the project folder.
