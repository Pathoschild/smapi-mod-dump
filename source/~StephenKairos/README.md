**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/StephenKairos/Teban100-StardewMods**

----

﻿This repository contains my SMAPI mods for [Stardew Valley](http://stardewvalley.net/). See the
individual mods for documentation and release notes.

## Mods
Active mods:
* **AutoGate** <small>([Nexus](https://www.nexusmods.com/stardewvalley/mods/820) | [source](AutoGate))</small>  
  _Automatically opens gates when you walk up to them, and closes them when you walk away._

* **Rope Bridge** <small>([Nexus](https://www.nexusmods.com/stardewvalley/mods/824) | [source](RopeBridge))</small>  
  _Lets you walk over ladders in the mines and Skull Cavern._

Inactive mods:
* Inventory Cycle <small>([source](InventoryCycle))  
  _(unreleased) Lets you cycle your hotbar through the three inventory rows at the press of a
  button._

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
3. Upload the generated `bin/Release/<mod name>-<version>.zip` file from the project folder.
