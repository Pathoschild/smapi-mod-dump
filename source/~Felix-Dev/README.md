This repository contains my SMAPI mods for Stardew Valley. See the individual mods for documentation and release notes.

## Mods
* **[ArchaeologyHouseContentManagementHelper](http://www.nexusmods.com/stardewvalley/mods/2804)** <small>([source](ArchaeologyHouseContentManagementHelper))</small>  
  _This mod improves the management of the Library/Museum. Shows number of found books and contributed items. 
  Allows item rearrangement even if nothing to donate. Shows specific information of the selected item. 
  Items can be swapped to make rearrangement less tedious. Adds extended gamepad cursor support. 
  Largely fixes the in-game item-placement bug.
  Lost Books can be "grabbed & sent" to the library even if the player's inventory has no space.
  Lost Books can be read without having to go the library._
  
* **[ToolUpgradeDeliveryService](http://www.nexusmods.com/stardewvalley/mods/2938)** <small>([source](ToolUpgradeDeliveryService))</small>  
  _This mod makes retrieving upgraded farm tools less tedious. With this mod, Clint (the blacksmith) will simply send you a mail with the
  upgraded tool included as soon as the upgrade is finished. No more visiting the blacksmith simply to get your improved farm tool!_
  
* **[FeTK](https://www.nexusmods.com/stardewvalley/mods/4403)** <small>([source](FeTK))</small>  
   _This mod is a collection of helper functions and mod services. It simplifies common developer tasks building Stardew-Valley mods and empowers developers to build rich 
   and high-quality mod experiences!_
  
## Translating the mods
The mods can be translated into any language supported by the game, and SMAPI will automatically
use the right translations.

(❑ = untranslated, ↻ = partly translated, ✓ = fully translated)

&nbsp;     | Archaeology House Content Management Helper    | Tool-Upgrade Delivery Service    |
---------- | :--------------------------------------------- | :------------------------------- |
Chinese    | [✓](ArchaeologyHouseContentManagementHelper/i18n/zh.json) | [✓](ToolUpgradeDeliveryService/i18n/zh.json) 
French     | ❑ untranslated                                            | [✓](ToolUpgradeDeliveryService/i18n/fr.json)
German     | [✓](ArchaeologyHouseContentManagementHelper/i18n/de.json) | [✓](ToolUpgradeDeliveryService/i18n/de.json)
Hungarian  | ❑ untranslated                                            | ❑ untranslated 
Italian    | ❑ untranslated                                            | ❑ untranslated 
Japanese   | ❑ untranslated                                            | [✓](ToolUpgradeDeliveryService/i18n/ja.json) 
Korean     | [✓](ArchaeologyHouseContentManagementHelper/i18n/ko.json) | [✓](ToolUpgradeDeliveryService/i18n/ko.json) 
Portuguese | [✓](ArchaeologyHouseContentManagementHelper/i18n/pt.json) | [✓](ToolUpgradeDeliveryService/i18n/pt.json)      
Russian    | ❑ untranslated                                            | ❑ untranslated 
Spanish    | [✓](ArchaeologyHouseContentManagementHelper/i18n/es.json) | [✓](ToolUpgradeDeliveryService/i18n/es.json) 
Turkish    | ❑ untranslated                                            | ❑ untranslated 

Contributions are welcome! See [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations) on the wiki for help contributing translations.
If you don't want to create an account on Github, you can always send them to me via [e-mail](mailto:felixdev91@gmail.com) or [nexus mods](https://www.nexusmods.com/users/58769686).

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

