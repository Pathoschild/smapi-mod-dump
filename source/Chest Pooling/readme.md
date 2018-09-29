**Chest Pooling** is a [Stardew Valley](http://stardewvalley.net/) mod which automatically moves
items into existing chests when you deposit items into a chest.

## How it works
The mod keys off of where your existing items are and adds any new items to existing stacks. You
can right-click to transfer a single item, which won't be moved so you can override the
auto-sorting at will.

For example, you can return from mining, walk up to a single chest, and throw in everything. The
mod will then for example move the minerals to the mineral chest, the ore and bars to their chest,
the food items to the food chest, etc.

Breaking this up into steps:
1. You add an item to a chest.
2. If that chest already has a stack of that item or no other chest does, it's added normally.
3. If another chest has a stack of that item and has room, the item gets moved to that chest. If
   the chest only has room for some of them, the overflow is left in the original chest.

## Installation
* Install the [latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
* [Download this mod](https://github.com/mralbobo/stardew-chest-pooling/releases).
* Unzip it into your `Stardew Valley/Mods` folder.

## Known bugs
* Not terribly fond of the behavior when a stack is filled in the currently open chest. But I'm
  fairly sure I need either a "chestUpdated" or "beforeInventoryChanged" event to fix it
  consistently.

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
   mod name and version (like `ChestPooling-1.3.zip`).
