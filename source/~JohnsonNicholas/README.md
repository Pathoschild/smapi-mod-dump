This repository contains my SMAPI mods for Stardew Valley. See the individual mods for
documentation, licenses, and release notes. May have other related projects that are here for tracking.

## Mods
* **[Better Shipping Box](https://community.playstarbound.com/threads/better-shipping-box.126235/#post-3228667)**<small> ([source](BetterShippingBox))</small>
  _This is a SMAPI 2.0 port of Kthio's Better Shipping Box, which gives you a menu._
 
* **[Customizable Cart Redux](https://rd.nexusmods.com/stardewvalley/mods/1402)**<small> ([source](CustomizableCartRedux))</small>
  _Lets you customize when the cart appears, and how many items it has._

* **Quest Overhaul** <small>([source](QuestOverhaul))</small>
  _This will overhaul the quest system, with more to come._
  
* **Tree Overhaul**<small> ([source](TreeOverhaul))</small>
  _This is the source code for the unofficial patch I am maintaining to make it compatibile with 2.0_

* **[Climates of Ferngill (Rebuild)](http://www.nexusmods.com/stardewvalley/mods/604)** <small>([source](ClimatesOfFerngill))</small>  
  _Creates a different climate system. This is a relatively basic approach, and may have more features in the future. It works by creating a base chance and a value that can increase (or decrease) as the month passes for weather._

* **[Solar Eclipse Event](http://www.nexusmods.com/stardewvalley/mods/897)** <small>([source](SolarEclipseEvent))</small>  
  _This adds the possibility of a solar eclipse to your game! Configurable chance. Also will spawn monsters if you have a wilderness farm in line with the same percentages as the base game._

* **[Time Reminder](http://www.nexusmods.com/stardewvalley/mods/1000)** <small>([source](TimeReminder))</small>  
  _Alerts you of system time after a configurable number of minutes._

## Compiling the mods
Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

These mods use the [crossplatform build config](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
so they can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://github.com/Pathoschild/Stardew.ModBuildConfig#readme)
for troubleshooting.

### Compiling a mod for testing
To compile a mod and add it to your game's `Mods` directory:

1. Rebuild the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](http://www.monodevelop.com/).  
   <small>This will compile the code and package it into the mod directory.</small>
2. Launch the project with debugging.  
   <small>This will start the game through SMAPI and attach the Visual Studio debugger.</small>

### Compiling a mod for release
To package a mod for release:

1. Delete the mod's directory in `Mods`.  
   <small>(This ensures the package is clean and has default configuration.)</small>
2. Recompile the mod per the previous section.
3. Create a zip file of the mod's folder in the `Mods` folder. The zip name should include the
   mod name and version (like `ClimateOfFerngill-1.0.zip`).
