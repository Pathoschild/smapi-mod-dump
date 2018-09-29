**Gate Opener** is a [Stardew Valley](http://stardewvalley.net/) mod which automatically opens
gates when you approach, and closes them when you move away.

## Installation
* Install the [latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
* [Download this mod](https://github.com/mralbobo/stardew-gate-opener/releases).
* Unzip it into your `Stardew Valley/Mods` folder.

## Compiling the mod
Installing a stable release is recommended for most users. If you really want to compile the mod
yourself, read on.

This mods uses the [crossplatform build config](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
so it can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
for troubleshooting.

### For testing
To compile the mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).  
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### For release
To package the mod for release:

1. Recompile the mod per the previous section.
2. Create a zip file of the mod's folder in the `Mods` folder. The zip name should include the
   mod name and version (like `GateOpener-1.0.zip`).
