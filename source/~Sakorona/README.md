**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Sakorona/SDVMods**

----

This repository contains my SMAPI mods for Stardew Valley. See the individual mods for documentation, licenses, and release notes. May have other related projects that are here for tracking.

## Mods
 
* **[Customizable Cart Redux](https://www.nexusmods.com/stardewvalley/mods/1402)**<small> ([source](CustomizableCartRedux))</small>

 _Lets you customize when the cart appears, and how many items it has. Contains an API for other mods to hook into._
  
* **[LunarDisturbances](https://www.nexusmods.com/stardewvalley/mods/2208)**<small> ([source](LunarDisturbances))</small>

_This mod adds a moon, which adds events, such as the Blood Moon, to the world. Also includes a solar eclipse!_

* **[WeatherIllnesses](https://www.nexusmods.com/stardewvalley/mods/2210)**<small> ([source](WeatherIllnesses))</small>

_This adds a stamina system to the game. In hazardous weather, the player will drain stamina per 10 minutes._

* **[Climates of Ferngill (Rebuild)](http://www.nexusmods.com/stardewvalley/mods/604)** <small>([source](ClimatesOfFerngill))</small>

_Creates a different climate system, with custom weather, including fog and thunder frenzy. 
NOTE: After 1.4, the stamina parts were folded into Weather Illlnesses (and the moon bits into Lunar Disturbances) Follow the link for more details._

* **[Time Reminder](http://www.nexusmods.com/stardewvalley/mods/1000)** <small>([source](TimeReminder))</small> 

_Alerts you of system time after a configurable number of minutes._

* **[Dynamic Night Time](https://www.nexusmods.com/stardewvalley/mods/2072)** <small>([source](DynamicNightTime))</small>

_Want a dynamic day/night cycle? Look here!_

  
## Depricated Mods
* **[Solar Eclipse Event]** <small>([source](SolarEclipseEvent))</small>  

_This adds the possibility of a solar eclipse to your game! Configurable chance. Also will spawn monsters if you have a wilderness farm in line with the same percentages as the base game. Now implemented into Lunar Disturbances, and as of SDV 1.3, no longer maintained, and as of SDV 1.4: no longer available.
    
* **[StardewNotification](https://www.nexusmods.com/stardewvalley/mods/4713)** <small>([source](StardewNotification))</small>

_Source code for Stardew Notification, currently maintained by me. MIT License. Original author is monopandora_

## Translating My Mods

Here's how to translate one of my mods:

1. Copy `default.json` into a new file with the right name:

   language   | file name
   ---------- | ---------
   Chinese    | `zh.json`
   German     | `de.json`
   Japanese   | `ja.json`
   Portuguese | `pt.json`
   Spanish    | `es.json`

2. Translate the second part on each line:
   ```json
   "example-key": "some text here"
                   ^-- translate this
   ```
   Don't change the quote characters, and don't translate the text inside `{{these brackets}}`.
3. Launch the game to try your translations.  
   _You can edit translations without restarting the game; just type `reload_i18n` in the SMAPI console to reload the translation files._

Create an issue or pull request here with your translations, or send them to me privately. :)

## Compiling the mods
Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

These mods use the [crossplatform build config](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
so they can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
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
3. Upload the generated `bin/Release/<mod name>-<version>.zip` file from the project folder.

