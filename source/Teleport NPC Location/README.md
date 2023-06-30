**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/chencrstu/TeleportNPCLocation**

----

### About this mod

This repository is a SMAPI mod for Stardew Valley. The main function is to teleport the current player to the selected NPC location。
The UI part of the code was copied from **Lookup Anything** , and of course, some modifications were made.

https://www.nexusmods.com/stardewvalley/mods/17190

### Lookup Anything

* **Lookup Anything** <small>([ModDrop](https://www.moddrop.com/stardew-valley/mods/606605) | [Nexus](https://www.nexusmods.com/stardewvalley/mods/541) | [source](LookupAnything))</small>  
  _See live info about whatever's under your cursor when you press F1. Learn a villager's favorite
  gifts, when a crop will be ready to harvest, how long a fence will last, why your farm animals
  are unhappy, and more._

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
