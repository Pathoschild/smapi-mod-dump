**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/dantheman999301/StardewMods**

----

This is a collection of Stardew Valley mods by dantheman999. Currently there's only one mod available.

**Variable Grass** adds a randomised chance for more or less plant growth per day.

Compatible with SMAPI 2.11.2+ on Linux, Mac, and Windows.

## Contents
* [Installation](#installation)
* [Configuration](#configuration)
* [Versions](#versions)
* [Compiling the mod](#compiling-the-mod)

## Installation
1. [Install the latest version of SMAPI](https://smapi.io).
2. Install this mod from the releases tab.
3. Run the game using SMAPI.

## Configuration
The mod will work fine out of the box, but you can tweak its settings by editing the `config.json`
file if you want. These are the available settings:

| setting           | what it affects
| ----------------- | -------------------
| `MinIterations`   | The minimum iterations of grass growing function to perform.
| `MaxIterations`   | The maximum iterations of grass growing function to perform.

## Compiling the mod
Installing stable releases is recommended for most users. If you really want to compile the mod
yourself, read on.

These mods use the [crossplatform build config](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
so they can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
for troubleshooting.

### Compiling the mod for testing
To compile the mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).  
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### Compiling the mod for release
To package the mod for release:

1. Switch to `Release` build configuration.
2. Recompile the mod per the previous section.
3. Upload the generated `bin/x86/Release/<mod name>-<version>.zip` file from the project folder.

## See also
* [release notes](release-notes.md)
