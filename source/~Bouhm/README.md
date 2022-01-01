**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Bouhm/stardew-valley-mods**

----

﻿This repository contains my SMAPI mods for Stardew Valley. See the individual mods for
documentation and release notes.

## Mods
* **NPC Map Locations** ([Nexus](https://www.nexusmods.com/stardewvalley/mods/239) | [source](NPCMapLocations))  
  Shows character locations on the map. Uses a modified map page much more accurate to the game.
* **Location Compass** ([Nexus](https://www.nexusmods.com/stardewvalley/mods/3045) | [source](LocationCompass))  
  Locates characters on the screen indicating direction and distance from player's current position.
* **Pet Dogs Mod** ([Nexus](https://www.nexusmods.com/stardewvalley/mods/570) | [source](PerDogMod))  
  Replaces the pet dog with a Shiba Inu, Shepherd, or Husky.

## Translating the mods
The mods can be translated into any language supported by the game, and SMAPI will automatically
use the right translations.

Contributions are welcome! See [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations)
on the wiki for help contributing translations.

(❑ = untranslated, ↻ = partly translated, ✓ = fully translated)

&nbsp;     | NPC Map Locations
---------- | :----------------------------------------
Chinese    | [✓](NPCMapLocations/i18n/zh.json)
French     | [✓](NPCMapLocations/i18n/fr.json)
German     | [✓](NPCMapLocations/i18n/de.json)
Hungarian  | [✓](NPCMapLocations/i18n/hu.json)
Italian    | [✓](NPCMapLocations/i18n/it.json)
Japanese   | [✓](NPCMapLocations/i18n/ja.json)
Korean     | [✓](NPCMapLocations/i18n/ko.json)
Polish¹    | [✓](NPCMapLocations/i18n/pl.json)
Portuguese | [✓](NPCMapLocations/i18n/pt.json)
Russian    | [✓](NPCMapLocations/i18n/ru.json)
Spanish    | [✓](NPCMapLocations/i18n/es.json)
Thai¹      | [✓](NPCMapLocations/i18n/th.json)
Turkish    | [✓](NPCMapLocations/i18n/tr.json)

¹ This is a custom language provided by a mod (see [Polish](https://www.nexusmods.com/stardewvalley/mods/3616)
and [Thai](https://www.nexusmods.com/stardewvalley/mods/7052)).

## Compiling the mods
Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

These mods use the [crossplatform build config](https://smapi.io/package) so they can be built on
Linux, Mac, and Windows without changes. See [the build config documentation](https://smapi.io/package)
for troubleshooting.

### Compiling a mod for testing
To compile a mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or
   [MonoDevelop](https://www.monodevelop.com/).  
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### Compiling a mod for release
To package a mod for release:

1. Switch to `Release` build configuration.
2. Recompile the mod per the previous section.
3. Upload the generated `bin/Release/<mod name>-<version>.zip` file from the project folder.
