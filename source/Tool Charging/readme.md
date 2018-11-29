## Config
* IncreaseBy [40] - increase to speed up, decrease to slow down. For base-game set to 0. Effective range is: -15 to 599.

## Building it
* Presumably you should just be able to clone and run this... Open an issue if it doesn't work for some reason.

**Tool Charging** is a [Stardew Valley](http://stardewvalley.net/) mod which allows you to speed up charging time when using tools.

## How it works
Allows you to speed up charging time when using tools.

## Config
* IncreaseBy [40] - increase to speed up, decrease to slow down. For base-game set to 0. Effective range is: -15 to 599.

## Compiling the mods
Installing a stable release is recommended for most users. If you really want to compile the mod
yourself, read on.

This mods uses the [crossplatform build config](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
so it can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
for troubleshooting.

### Compiling a mod for testing
To compile a mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).  
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### Compiling a mod for release
To package a mod for release:

1. Recompile the mod per the previous section.
2. Create a zip file of the mod's folder in the `Mods` folder. The zip name should include the
   mod name and version (like `ToolCharging-1.3.zip`).
