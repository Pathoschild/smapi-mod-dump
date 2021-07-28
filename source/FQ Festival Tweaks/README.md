**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/FletcherGoss/FQTweaks**

----

This repository contains my SMAPI mods for [Stardew Valley](https://www.stardewvalley.net/). See the individual mods for documentation and release notes.

## Mods
- [FQFestivalTweaks](http://www.nexusmods.com/stardewvalley/mods/9062) ([Source](https://github.com/FletcherGoss/FQTweaks/tree/main/FQFestivalTweaks))

## Compiling the mods
Installing stable releases from Nexus Mods is recommended for most users. If you really want to compile the mod yourself, read on.

These mods use the [crossplatform build config](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig) so they can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig) for troubleshooting.

### Compiling a mod for testing
To compile a mod and add it to your game's `Mods` directory:

1. Rebuild the project using [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).  
	<small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
	<small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### Compiling a mod for release
To package a mod for release:

1. Rebuild the project in _Release_ mode using [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).
2. Find the `bin/Release/<mod name> <version>.zip` file in the mod's project folder.
3. Upload that file.
