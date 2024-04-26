**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Tocseoj/StardewValleyMods**

----

# Tocseoj's Stardew Valley mods

[![Nexus Mods](https://img.shields.io/badge/Nexus-Mods-4DB7FF.svg)](https://www.nexusmods.com/users/165805258?tab=user+files)
[![CurseForge](https://img.shields.io/badge/CurseForge-Tocseoj-4DB7FF.svg)](https://www.curseforge.com/members/tocseoj/projects)
[![ModDrop](https://img.shields.io/badge/ModDrop-ModDrop-4DB7FF.svg)](https://www.moddrop.com/stardew-valley/profile/431108/mods)
[![GitHub](https://img.shields.io/badge/GitHub-Tocseoj-4DB7FF.svg)](https://github.com/Tocseoj)

This repository contains my SMAPI mods for Stardew Valley.

## Mods

Active mods:

- **Ladder Light** ([Nexus](https://www.nexusmods.com/stardewvalley/mods/22052) | [CurseForge](https://www.curseforge.com/stardewvalley/mods/ladder-light) | [ModDrop](https://www.moddrop.com/stardew-valley/mods/1549539-ladder-light) | [GitHub](https://github.com/Tocseoj/StardewValleyMods/releases?q=LadderLight&expanded=true) | [source](LadderLight))
  _Let there be light! Makes mine ladders and shafts have a slight glow to them, so you don't lose them on levels 30-40._

- **Big Crop Bonus** ([Nexus](https://www.nexusmods.com/stardewvalley/mods/22337) | [(todo)CurseForge]() | [ModDrop](https://www.moddrop.com/stardew-valley/mods/1553635-big-crop-bonus) | [GitHub](https://github.com/Tocseoj/StardewValleyMods/releases?q=BigCropBonus&expanded=true) | [source](BigCropBonus))
  _Adds a bonus to the selling price of related items when you have a giant crop of it grown. (This stacks!)_

- **Mossy Tree Bubble** ([Nexus](https://www.nexusmods.com/stardewvalley/mods/22818) | [(todo)CurseForge]() | [ModDrop](https://www.moddrop.com/stardew-valley/mods/1564905-mossy-tree-bubble) | [GitHub](https://github.com/Tocseoj/StardewValleyMods/releases?q=MossyTreeBubble&expanded=true) | [source](MossyTreeBubble))
  _Put reminders on your trees when they have harvest-able moss. Won't overlap with tapper bubbles._

## Compiling the mods

Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

These mods use the [crossplatform build config](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
so they can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
for troubleshooting.

### Compiling a mod for testing

To compile a mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio Code](https://code.visualstudio.com/) with the [C# DevKit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension (_I've heard this doesn't work with the Apple Silicon chips, try JetBrains Rider_), [Visual Studio](https://www.visualstudio.com/vs/community/), or [MonoDevelop](https://www.monodevelop.com/).
   - This will compile the code and package it into the mod directory.
2. Launch the project with debugging.
   - This will start the game through SMAPI and attach the Visual Studio debugger.
   - My `launch.json` config and `tasks.json` are below (using VS Code and running macOS with GOG version of the game).

#### launch.json

```json
{
  "name": ".NET Core Launch (console)",
  "type": "coreclr",
  "request": "launch",
  "preLaunchTask": "dotnet: build",
  "program": "/Applications/Stardew Valley.app/Contents/MacOS/StardewModdingAPI",
  "args": [],
  "cwd": "${workspaceFolder}",
  "stopAtEntry": false,
  "console": "internalConsole",
  "requireExactSource": false
}
```

#### tasks.json

```json
// build for debugging
{
  "type": "dotnet",
  "task": "build",
  "group": "build",
  "problemMatcher": [],
  "label": "dotnet: build"
},
// build for release
{
   "type": "shell",
   "label": "dotnet: release",
   "command": "dotnet",
   "args": [
     "build",
     "--configuration",
     "Release"
   ],
   "group": {
     "kind": "build",
     "isDefault": true
   },
   "problemMatcher": "$msCompile"
}
```

### Getting hot-reloading to work

- <https://github.com/arannya/macos-sdv-hotreload>

### Compiling a mod for release

To package a mod for release:

1. Switch to `Release` build configuration.
2. Recompile the mod per the previous section.
3. Upload the generated `bin/Release/<mod name>-<version>.zip` file from the project folder.

---

> Thanks to all the wonderful people in the Stardew Valley discord who have helped with my questions!
>
> - XxHarvzBackxX
> - atravita
> - luckypuppy96
> - Khloe Leclair
> - Pathoschild
> - Esca
> - ichortower
>
> _...and many more_

---

> README template is taken directly from [Pathoschild's repository](https://github.com/Pathoschild/StardewMods)
