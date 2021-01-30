**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/AairTheGreat/StardewValleyMods**

----

[Fish Finder](https://github.com/AairTheGreat/StardewValleyMods/tree/master/Fish%20Finder) is a [Stardew Valley](http://stardewvalley.net/) mod which helps find the fishing spots.
                                                                                                           
**This documentation is for modders and player. See the [Nexus page](https://www.nexusmods.com/stardewvalley/mods/5011) if you want the compiled mod.**
                                                                                                           
## Contents
* [Install](#install)
* [Introduction](#introduction)
* [Configuration Setting](#configuration-setting)
  * [Overview of config json file](#Overview-of-config-json-file)
    - [showHudData](#showHudData)
    - [hudXPostion](#hudXPostion)
    - [hudYPostion](#hudYPostion)
    - [showDistance](#showdistance)
	- [configVersion](#configVersion)
* [Troubleshooting](#troubleshooting)
  * [Bad Edits to config json file](#Bad-Edits-to-config-json-file)  
* [Localization](#localization)
* [See Also](#see-also)

## Install
1. If needed, [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/4161).
3. Run the game using SMAPI.

## Introduction
### What is this Fish Finder Mod?
This is a simple mod that shows you the general direction and distance to a fishing spot (if one is found).
To show the information, just hold the fishing pole

## Configuration Setting
### Overview of config json file
Once this mod is installed and you have the fishing rod you can adjust some settings.  

If you don't have a config.json file, then the config file will be created when you first run Stardew Valley with this mod.

You should not need to adjust the configuration settings but if you do, here are what the setting are inside the config.json file:
#### showHudData
Shows the hud when player is holding the pan.   
- Default Value: true 
#### hudXPostion
The upper left corner X coordinate of the hud position.  The top left corner of the screen is 0,0.  If this value > 0 then it moves the hud right.    
- Default Value: 0 
#### hudYPostion
The upper left corner Y coordinate of the hud position.  The top left corner of the screen is 0,0.  If this value > 0 then it moves the hud down.    
Note: If you use the [UI Info Suite](https://www.nexusmods.com/stardewvalley/mods/1150), A good setting is 200 for this.
- Default Value: 0 
#### showDistance
Shows the distance to the panning spot in the hud when player is holding the pan.  
- Default Value: true 
#### configVersion
Don't modify.  This is used to update the config file if new settings are added.
- Default Value: 1

## Troubleshooting

### Bad Edits to config json file
It possible that you decided to edit the config file and now it's not working as expected.  To get back to the default config.json file:
1. Stop Stardew Valley, if running.
2. Delete the config.json file.
3. Start Stardew Valley the default json file will be recreated.
 
## Localization

Currently this mod does support localization.  Currently this mod has:
1. English
2. Simpilified Chinese
3. Spanish
4. Russian


## Thank You!
* [Concerned Ape](https://twitter.com/concernedape) - Creator of Stardew Valley.
* [Pathoschild](https://smapi.io/) - Creator of the Stardew Modding API.
* [Stardew Wiki](https://stardewvalleywiki.com) - To the people maintaining this very useful site.
* [Teh's Fishing Overhaul](https://www.nexusmods.com/stardewvalley/mods/866) - TehPers, helped inspire the HUD used in this mod
* Nexus user Nogohoho - For suggesting this mod idea.
* Github user asqwedcxz741 - For the Simplified Chinese translation
* Github user Dmitrey Kupcov - For the Russian translation
* Nexus user kaog1992 - For the Spanish translation
* To my testers: SparkyTheCat and My Better Half  -- Thank you!

## See Also
* [Stardew Valley](https://www.stardewvalley.net/) - Home page and blog
* [Stardew Valley Mods Nexus Page](https://www.nexusmods.com/stardewvalley/mods)
