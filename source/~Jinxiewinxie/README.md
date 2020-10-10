**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Jinxiewinxie/StardewValleyMods**

----

This repository contains my SMAPI mods for Stardew Valley. See the individual mods for
release notes.

All mods are compatible with Stardew Valley 1.2+ on Linux, Mac, and Windows.

## Mods
* **[Pond With Bridge](https://www.nexusmods.com/stardewvalley/mods/316)** <small>([source](PondWithBridge))</small>  
  _Adds a bridge over the lower pond on the farm map so you can quickly access the path leading south. No XNB replacements needed._

* **Wonderful Farm Life** <small>([source](WonderfulFarmLife))</small>  
  _Spruces up the farm with a telescope, picnic area, working radio, dog area, and more._

* **Tainted Cellar** <small>([source](TaintedCellar))</small>  
  _Adds an underground cellar (separate from the cask cellar) with all the amenities you'd want in a bomb shelter._

## Compiling the mods
Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mods yourself, read on.

These mods use the [crossplatform build config](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
so they can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
for troubleshooting.

### Compiling a mod for testing
To compile a mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).  
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### Compiling a mod for release
To package a mod for release:

1. Delete the mod's directory in `Mods`.  
   <small>(This ensures the package is clean and has default configuration.)</small>
2. Recompile the mod per the previous section.
3. Create a zip file of the mod's folder in the `Mods` folder. The zip name should include the
   mod name and version (like `PondWithBridge-1.0.zip`).
