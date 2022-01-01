**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/cantorsdust/StardewMods**

----

This repository contains my mods for [Stardew Valley](http://stardewvalley.net/):

* [All Professions](AllProfessions) ([Nexus](http://www.nexusmods.com/stardewvalley/mods/174))
* [Instant Grow Trees](InstantGrowTrees) ([Nexus](http://www.nexusmods.com/stardewvalley/mods/173))
* [Recatch Legendary Fish](RecatchLegendaryFish) ([Nexus](http://www.nexusmods.com/stardewvalley/mods/172))
* [TimeSpeed](TimeSpeed) ([Nexus](http://www.nexusmods.com/stardewvalley/mods/169))

And these archived mods which are no longer actively maintained:
* ~~[All Crops All Seasons](archived/AllCropsAllSeasons) ([Nexus](http://www.nexusmods.com/stardewvalley/mods/170))~~ (see [alternatives](https://mods.smapi.io/#All_Crops_All_Seasons))
* ~~[Skull Cave Saver](_archived/SkullCaveSaver) ([Nexus](https://www.nexusmods.com/stardewvalley/mods/175))~~ (see [alternatives](https://mods.smapi.io/#Skull_Cave_Saver))

## Translating the mods
The mods can be translated into any language supported by the game, and SMAPI will automatically
use the right translations.

Contributions are welcome! See [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations)
on the wiki for help contributing translations.

(❑ = untranslated, ↻ = partly translated, ✓ = fully translated)

&nbsp;     | All Professions | Instant Grow Trees | Recatch Legendary Fish | TimeSpeed
---------- | :-------------- | :----------------- | :--------------------- | :--------------------------
Chinese    | ❑               | ❑                  | ❑                      | [↻](TimeSpeed/i18n/pt.json)
French     | ❑               | ❑                  | ❑                      | ❑
German     | ❑               | ❑                  | ❑                      | ❑
Hungarian  | ❑               | ❑                  | ❑                      | ❑
Italian    | ❑               | ❑                  | ❑                      | ❑
Japanese   | ❑               | ❑                  | ❑                      | ❑
Korean     | ❑               | ❑                  | ❑                      | ❑
Portuguese | ❑               | ❑                  | ❑                      | [↻](TimeSpeed/i18n/pt.json)
Russian    | ❑               | ❑                  | ❑                      | ❑
Spanish    | ❑               | ❑                  | ❑                      | [↻](TimeSpeed/i18n/es.json)
Turkish    | ❑               | ❑                  | ❑                      | ❑

## Compiling the mods
Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

These mods use the [crossplatform build config](https://smapi.io/package)
so they can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://smapi.io/package)
for troubleshooting.

### Compiling a mod for testing
To compile a mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).  
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### Compiling a mod for release
To package a mod for release:

1. Switch to `Release` build configuration.
2. Recompile the mod per the previous section.
3. Upload the generated `<mod name>-<version>.zip` file from the solution's `_releases` folder.
