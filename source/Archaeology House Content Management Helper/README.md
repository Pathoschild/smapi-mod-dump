This repository contains my SMAPI mods for Stardew Valley. See the individual mods for
documentation and release notes.

## Mods
* **[ArchaeologyHouseContentManagementHelper](http://www.nexusmods.com/stardewvalley/mods/2804)** <small>([source](ArchaeologyHouseContentManagementHelper))</small>  
  _This mod improves the management of the Library/Museum. Shows number of found books and contributed items. 
  Allows item rearrangement even if nothing to donate. Shows specific information of the selected item. 
  Items can be swapped to make rearrangement less tedious. Adds extended gamepad cursor support. 
  Largely fixes the in-game item-placement bug.
  Lost Books can be "grabbed & sent" to the library even if the player's inventory has no space._
  
## Translating the mods
The mods can be translated into any language supported by the game, and SMAPI will automatically
use the right translations.

(❑ = untranslated, ↻ = partly translated, ✓ = fully translated)

&nbsp;     | Archaeology House Content Management Helper    | 
---------- | :--------------------------------------------- |
Chinese    | [✓](ArchaeologyHouseContentManagementHelper/i18n/zh.json)
German     | [✓](ArchaeologyHouseContentManagementHelper/i18n/de.json)
Japanese   | ❑ untranslated        
Portuguese | ❑ untranslated      
Russian    | ❑ untranslated  
Spanish    | [✓](ArchaeologyHouseContentManagementHelper/i18n/es.json)     

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

Create an issue or pull request here with your translations, or send them to me via [e-mail](mailto:felixdev91@gmail.com) or [nexus mods](https://www.nexusmods.com).

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
