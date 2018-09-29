﻿This repository contains my SMAPI mods for Stardew Valley. See the individual mods for
documentation and release notes.

## Mods
Active mods:
* **[Auto Speed](http://www.nexusmods.com/stardewvalley/mods/443)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/AutoSpeed))</small>  
  _Lets you move faster without the need to enter commands in the console._

* **[Billboard Anywhere](http://www.nexusmods.com/stardewvalley/mods/492)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/BillboardAnywhere))</small>  
  _Lets you look at the billboard anywhere in-game for easy access to upcoming events and birthdays._

* **[Build Endurance](http://www.nexusmods.com/stardewvalley/mods/445)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/BuildEndurance))</small>  
  _Lets you level up your endurance to increase your max stamina as you play._

* **[Build Health](http://www.nexusmods.com/stardewvalley/mods/446)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/BuildHealth))</small>  
  _Lets you level up your endurance to increase your max health as you play._

* **[Buy Back Collectables](http://www.nexusmods.com/stardewvalley/mods/507)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/BuyBackCollectables))</small>  
  _Lets you buy items from the collectables menu by pressing a key, at a configurable markup._

* **[Daily Quest Anywhere](http://www.nexusmods.com/stardewvalley/mods/513)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/DailyQuestAnywhere))</small>  
  _Lets you open the daily quest menu from anywhere in-game._

* **[Fall 28 Snow Day](http://www.nexusmods.com/stardewvalley/mods/486)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/Fall28SnowDay))</small>  
  _Snow falls on the last day of fall._

* **[Happy Birthday](http://www.nexusmods.com/stardewvalley/mods/520)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/HappyBirthday))</small>  
  _Lets you pick a day for your birthday. On your birthday, you get letters from your parents, and
  villagers give you gifts and wish you happy birthday._

* **[More Rain](http://www.nexusmods.com/stardewvalley/mods/441)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/MoreRain))</small>  
  _Lets you adjust the probability of rain and storms for each season._

* **[Museum Rearranger](http://www.nexusmods.com/stardewvalley/mods/428)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/MuseumRearranger))</small>  
  _Lets you rearrange donated items in the museum by pressing a key, even if you don't have a new
  item to donate._

* **[Night Owl](http://www.nexusmods.com/stardewvalley/mods/433)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/NightOwl))</small>  
  _Lets you stay up a full 24 hours instead of collapsing at 2am, including a morning light
  transition as the sun rises._

* **[No More Pets](http://www.nexusmods.com/stardewvalley/mods/506)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/NoMorePets))</small>  
  _Removes all pets from the game._

* **[Save Anywhere](http://www.nexusmods.com/stardewvalley/mods/444)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/SaveAnywhere))</small>  
  _Lets you save your game anywhere by pressing a key._

* **[Save Backup](http://www.nexusmods.com/stardewvalley/mods/435)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/SaveBackup))</small>  
  _Automatically backs up your save files before you play and every in-game night._

* **[Stardew Symphony](http://www.nexusmods.com/stardewvalley/mods/425)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/StardewSymphony))</small>  
  _Lets you add music packs to Stardew Valley and play them without editing the game's default
  sound files. Music can be conditional on location, season, and weather._

* **[Time Freeze](http://www.nexusmods.com/stardewvalley/mods/973)** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/TimeFreeze))</small>  
  _Freezes time while indoors. Configurably lets time pass while bathing._

Inactive mods:
* **Custom Shops Redux GUI** <small>([source](https://github.com/janavarro95/Stardew_Valley_Mods/tree/master/GeneralMods/CustomShopsRedux))</small>  
  _In development. Lets you create custom shops by editing text files._

## Compiling the mods
Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

These mods use the [crossplatform build config](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
so they can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
for troubleshooting.

To compile a mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).  
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

To package a mod for release:

1. Rebuild the project in release mode.
2. The release zips will be generated in the `_releases` folder.
