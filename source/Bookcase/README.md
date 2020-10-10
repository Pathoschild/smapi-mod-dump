**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Stardew-Valley-Modding/Bookcase**

----

# Bookcase [![](http://cf.way2muchnoise.eu/297252.svg)](https://stardewvalley.curseforge.com/projects/297252) [![](https://img.shields.io/discord/464610060950306816.svg?logo=discord&colorB=7289DA)](https://discord.gg/qnZ96VU)

Bookcase is a core API mod for Stardew Valley. The aim of this mod is to provide other moders with helpful utilities and improve general compatability between mods.

## Features
- Log - This class allows you to create a logger for your mod with wrappers for different log levels.
- Utilities - There are many utility classes and helper methods. They can be found in Bookcase.Utils.
- Events - This mod gives mods event hooks which they can use. See Bookcase.Events.BookcaseEvents for a full list.

## Getting Started

Note: These steps are written with Visual Studio as the IDE. Other IDEs will also work, but the install steps may be different. 

### Prerequisites 
- You will need StardewValley and SMAPI installed on your computer.
- You should already know how to create a SMAPI mod. You can find more info about that [here](https://gist.github.com/darkhax/3acb02667bcaa450f8276c514c9dd82e).

### Add Bookcase using Nuget
1. Create a new project in Visual Studio.
2. Right click on your project in the Solution Explorer and select `Manage NuGet Packages`.
3. Click on Browse if it's not already selected, and search for Bookcase.
4. Install Bookcase for the version of the game you want. Usually this is the latest version.
5. Expand your project in the solution explorer, and then expand References.
6. Click on 0Harmony and then in the bottom properties set Copy Local to False.
7. Restart your Visual Studio.

### Adding Bookcase as a dependency
The SMAPI mod loader needs to know that your mod requires Bookcase. You can define that by adding the following property to the `manifest.json` of your mod.

```json
	"Dependencies": [{
		"UniqueID": "darkhax.bookcase",
		"MinimumVersion": "MIN_VERSION_HERE"
	}]
```

### Confirming
If you have sucessfully added Bookcase to your project, you should now be able to reference the mod and game classes. You can test this by adding these namespace references to one of your classes.

```cs
using Bookcase;
using StardewValley;
using StardewModdingAPI;
using Harmony;
```
