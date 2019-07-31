[Better Panning](https://github.com/AairTheGreat/StardewValleyMods/tree/master/Better%20Panning) is a [Stardew Valley](http://stardewvalley.net/) mod which improves the panning gameplay.
                                                                                                           
**This documentation is for modders and player. See the [Nexus page](https://www.nexusmods.com/stardewvalley/mods/4161) if you want the compiled mod.**
                                                                                                           
## Contents
* [Install](#install)
* [Introduction](#introduction)
* [Configuration Setting](#configuration-setting)
  * [Overview of config json file](#Overview-of-config-json-file)
    - [useCustomPanningTreasure](#useCustomPanningTreasure)
	- [enableSplashSounds](#enableSplashSounds)
    - [enableGeodeMineralsTreasure](#enableGeodeMineralsTreasure)
    - [enablePanningTrash](#enablePanningTrash)
    - [enableArtifactTreasures](#enableArtifactTreasures)
    - [enableAllArtifactsAfterFoundThemAll](#enableAllArtifactsAfterFoundThemAll)
    - [enableSeedPanning](#enableSeedPanning)
    - [enableAllSeedsEverySeason](#enableAllSeedsEverySeason)
    - [enableAllSecondYearSeedsOnFirstYear](#enableAllSecondYearSeedsOnFirstYear)	
    - [sp_alwaysCreatePanningSpots](#spalwaysCreatePanningSpots)
    - [mp_alwaysCreatePanningSpots](#mpalwaysCreatePanningSpots)
    - [maxNumberOfOrePointsGathered](#maxNumberOfOrePointsGathered)
    - [showHudData](#showHudData)
    - [hudXPostion](#hudXPostion)
    - [hudYPostion](#hudYPostion)
    - [showDistance](#showdistance)
	- [additionalLootChance](#additionalLootChance)    
    - [useCustomFarmMaps](#useCustomFarmMaps)
    - [customMaps](#customMaps)
	- [configVersion](#configVersion)
  * [Overview of treasure json file](#Overview-of-treasure-json-file)
    - [Treasure Groups](#Treasure-Groups)
    - [Treasure Data](#Treasure-List)
  * [Overview of DataFiles Folder](#Overview-of-DataFiles-Folder)
* [Troubleshooting](#troubleshooting)
  * [Bad Panning Spots in a Game Location/Area](#Bad-Panning-Spots-in-a-Game-Location)
  * [Bad Edits to config json file](#Bad-Edits-to-config-json-file)
  * [Bad Edits to treasure json file](#Bad-Edits-to-treasure-json-file)
* [Localization](#localization)
* [See Also](#see-also)

## Install
1. If needed, [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/4161).
3. Run the game using SMAPI.

## Introduction
### What is Better Panning?
How many of you finish the fish tank and then place the [copper pan](https://stardewvalleywiki.com/Copper_Pan) into a chest to never be seen again?

If you want the pan to be useful, then this is the mod for you.

Better Panning allows you to: 
* Fine tuned game areas so the chance of getting a panning spot out of reach is greatly reduced.  In fact, sometimes the panning spots are on the ground next to the water.
* The Beach now can have panning spots
* Custom treasure list.  With Better Panning you don't just get ore.  You can get any item.  With that said, the following types of items are by default configured to be panned:
	* Ore and Ore Bars
	* Gems, Minerals, Geodes, and previously found geode minerals
	* Seed packets.  You can find seeds for the next season or if you're in winter you can find seeds from all the other seasons too.
	* Rings and boots
	* Rare items like pearls, prismatic shards, treasure chests
	* Unfound Artifacts. Once you find all the artifacts, all the artifacts can be panned. 
	* Fishing Tackle
	* Totems
	* Coal, Rice, and the occasional trash
* While holding the pan, see the general direction and distance to the panning spot.  Beware, sometimes the fish seem eat the panning spots so get to them quickly.
* Various configurable settings

## Configuration Setting
### Overview of config json file
Once this mod is installed and you have the pan you can adjust some settings.  

If you don't have a config.json file, then the config file will be created when you first run Stardew Valley with this mod.

You should not need to adjust the configuration settings but if you do, here are what the setting are inside the config.json file:
#### useCustomPanningTreasure
Use the custom treasure from the mod.  Setting to false will use the standard game item list.   
- Default Value: true 
#### enableSplashSounds
Maybe you don't want the splash sound of the panning spots... then set this to false.   
- Default Value: true 
#### enableGeodeMineralsTreasure
Maybe you don't want the [Geode Minerals](https://stardewvalleywiki.com/Minerals)... then set this to false.   
- Default Value: true 
#### enablePanningTrash
Maybe you don't want the even the chance of trash... then set this to false.   
- Default Value: true 
#### enableArtifactTreasures
Maybe you don't want to pan up missing [Artifacts](https://stardewvalleywiki.com/Artifacts)... then set this to false.   
- Default Value: true 
#### enableAllArtifactsAfterFoundThemAll
By default once the artifacts are all found, then the panning spots get all the artifacts again. If you don't what any more then set this to false.
Note: if enableArtifactTreasures is false, this setting does not matter.  
- Default Value: true 
#### enableSeedPanning
You get the chance to get next season seeds, except Winter.  
* Spring = Summer seeds
* Summer = Fall seeds
* Fall = Spring seeds
* Winter = All season seeds
- Default Value: true 
#### enableAllSeedsEverySeason
You think above is stupid and you want seeds, all the seeds then set this to true.
Note: if enableSeedPanning is false, this setting does not matter.     
- Default Value: false 
#### enableAllSecondYearSeedsOnFirstYear
Sorry you can't get second year seeds in the first year, unless you set this to true.    
Note: if enableSeedPanning is false, this setting does not matter.  
- Default Value: false 
#### sp_alwaysCreatePanningSpots
Single Player Only - If the mod detects a panning point that it did not create, it creates a new spot.  This helps reduce the potential of spots too far away.  
- Default Value: true 
#### mp_alwaysCreatePanningSpots
Multiplayer Player Only - If the mod detects a panning point that it did not create, it creates a new spot, but if a different player creates the spot, it will move 
the spot.  Set to true if you want panning spots to potentially move on your friends!
- Default Value: false 
#### maxNumberOfOrePointsGathered
Maximum times per day a player can pan spots before the mod stops creating them.  The game can/will create more spots but not the mod.
- Default Value: 50 
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
#### additionalLootChance
The chance of panning up more than one type of item
- Default Value: 0.4
#### useCustomFarmMaps
You using a custom farm?  Then you can enable it.  Currently this mod has a fine-tuned selection for the [Immersive Farm 2](https://www.nexusmods.com/stardewvalley/mods/3561) map.  But that's okay, setup the map with the next setting and the mod will create a un-optimized panning spot file... but still be better then the base game.    
- Default Value: false
#### customMaps
You have a custom farm map, then you add the file to use and what type of farm it replaces.  For example:
```
"useCustomFarmMaps": true,
"customMaps": {
    "0": "Immersive_Farm_2.json"
  }
```
Type of farm maps:
* 0 = Default Farm (Standard)
* 1 = Fishing Farm (Riverland)
* 2 = Forging Farm (Forrest)
* 3 = Mining Farm (Hill-Top)
* 4 = Combat (Wilderness Farm)   
- Default Value: blank {} 

Here are the steps to get a custom farm map to work:
1. Stop Stardew Valley, if running.
2. Edit the config.json file (like the example above or below):
    * Set useCustomFarmMaps = true
    * Setup the customMaps setting
3. Start Stardew Valley and exit the farm house.
4. (Optional) Stop the game and fine-tune the settings found in file you configured.  The file is in the DataFiles folder.

Here is an example where you have a riverland farm mod:
```
"useCustomFarmMaps": true,
"customMaps": {
    "1": "MyCoolFishingFarmMapMod.json"
  }
```

When you first enter the farm, the mod will scan for valid panning spots and create the configured file but there could be unreachable spots.  This is where you can fine tune the file created "DataFiles\MyCoolFishingFarmMapMod.json" and remove unreachable spots.

#### configVersion
Don't modify.  This is used to update the config file if new settings are added.
- Default Value: 2

### Overview of treasure json file
Everyone loves treasure!  Therefore, it's time to talk about how the configuration file is put together.  Here is the general workflow:
1. Player goes and finds a panning spot.
2. The game generates a random number.
3. The mod compares that number with the treasure group and determines what lucky group the treasure is in.
4. The games generates another random number.
5. The mod then compares that number with the treasure list within the choosen group.
6. Player gets selected treasure and mod looks to see if another item can be selected.  If it can, then the mod repeats steps 2-5.

The treasure.json file is what controls this and it is located in the "Better Panning\DataFiles" mod folder.

#### Treasure Groups
The possible treasure is grouped.  Each group has a list of treasure and a corresponding chance.  The mod has the following groups:
 * Artifacts
 * Bars
 * Boots
 * Custom
 * FallSeeds
 * Gems
 * Geodes
 * GeodeMinerals
 * Minerals
 * Ores
 * Other
 * Rares 
 * Rings
 * SpringSeeds
 * SummerSeeds
 * Tackle
 * Totems
 * WinterSeeds
 
Each group has the following properties:
#### GroupID
This is the internal id used with the mod.  This should not be changed within the treasure.json file.  If it's changed, it will break the mod.  
#### GroupChance
This is the chance the group will be selected.  Value is between 0 and 1.  
#### Enabled 
This setting will completely remove the group from being chosen.  Only the Custom group is disabled by default.
#### ManualOverride
If you want to play around with a group then you should set this to true.  There is some code (mainly the Artifacts/GeodeMineral groups) that the mod adjusts based on game status.  If you set this flag to true, the code will respect your changes and accept you are it's overlord.  For example, you are tird of rusty spoons, so you disable that treasure.  Set this flag so the mod never enables the rusty spoon again!
#### Treasure List 
This is the treasure that can be selected if the group is selected.  More information on treasure data setting is below.

Field                  | Purpose
---------------------- | -------
`Id`                 |Game internal ID for the object.  You can change this if you know the item id you want.
`Name`                | What humans know the items as.  Not really used by the mod, just useful if you don't know the item ID.
`Chance`          | The chance this treasure is selected if it's treasure group is selected.  Value range: (0.0 - 1.0)
`MinAmount`        | The minimum number of items you can pan per panning spot.
`MAxAmount`               | The maximum number of items you can pan per panning spot.
`AllowDuplicates`          | If the item can be selected again if the mod feels like giving you more stuff.
`Enabled`          | If the treasure is enabled, then it can be selected to become the player's treasure.  Note: If you change this setting, you should set the treasure group setting ManualOverride to true.

Here is an example of a treasure group entry looks like:
```
"Other": {
  "GroupID": "Other",
  "GroupChance": 0.14,
  "Enabled": true,
  "ManualOverride": false,
  "treasureList": [
    {
      "Id": 167,
      "Name": "Joja Cola",
      "Chance": 0.005,
      "MinAmount": 1,
      "MaxAmount": 1,
      "AllowDuplicates": false,
      "Enabled": true
    },
    {
      "Id": 168,
      "Name": "Trash",
      "Chance": 0.005,
      "MinAmount": 1,
      "MaxAmount": 1,
      "AllowDuplicates": false,
      "Enabled": true
    },
  ]
```

### Overview of DataFiles Folder
Each game location/area can be configured to setup where panning spots can show up.  There is a limit on where spots can should up, so you can't just have a spot in the middle of land.  The files are used to finetune each area to limit unreachable panning spots being created.  The file is formated as a list of X and Y coordinates like:
```
[
  "9, 36",
  "9, 37"
]
```

If a game location file is deleted then the next time a player goes to the that area the file will be recreated. 

For example, if you are using a custom area like a Big Coop with a fishing pond, then you will need to delete the Big Coop.json and next time Stardew Valley is started then panning spots might show up in the modded Big Coop.  It does depend on a few things like if the water is considered open water or not, but the mod scans for valid panning spots if there is no data file. 

## Troubleshooting

### Bad Panning Spots in a Game Location
If you are using a modded area and/or there are problems with panning spots not showing up or in the worng spots then there is an easy fix:  
1. Stop Stardew Valley, if running.
2. In the Better Panning mod folder there is a DataFiles folder.  Open that folder and find the file named <GameLocation>.json where the game location is the area with the problem.
3. Delete the that file found in step 2. 
4. Start Stardew Valley and go to the area.  It should work now and the file should be regenerated.

### Bad Edits to config json file
It possible that you decided to edit the config file and now it's not working as expected.  To get back to the default config.json file:
1. Stop Stardew Valley, if running.
2. Delete the config.json file.
3. Start Stardew Valley the default json file will be recreated.

### Bad Edits to treasure json file
It possible that you decided to edit the treasure config file and now it's not working as expected.  To get back to the default treasure.json file:
1. Stop Stardew Valley, if running.
2. Delete the treasure.json file.
3. Start Stardew Valley the default json file will be recreated.
 
## Localization

Currently this mod does not support localization but it is planned to support it in the future.  If you want to help with this effort, let me know.

## Thank You!
* [Concerned Ape](https://twitter.com/concernedape) - Creator of Stardew Valley.
* [Pathoschild](https://smapi.io/) - Creator of the Stardew Modding API.
* [Stardew Wiki](https://stardewvalleywiki.com) - To the people maintaining this very useful site.
* [Stardew ID List](https://stardewids.com/) - To the people maintaining this very useful site.
* [Teh's Fishing Overhaul](https://www.nexusmods.com/stardewvalley/mods/866) - TehPers, helped inspire the HUD used in this mod
* To my testers: SparkyTheCat and My Better Half  -- Thank you!

## See Also
* [Stardew Valley](https://www.stardewvalley.net/) - Home page and blog
* [Stardew Valley Mods Nexus Page](https://www.nexusmods.com/stardewvalley/mods)
