This file is to explain the configuration settings for the config.json file.
The most up to date information will be at:
https://github.com/AairTheGreat/StardewValleyMods/tree/master/Better%20Panning

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

#### chanceOfCreatingPanningSpot
This is the percent chance to have a panning spot be created when changing locations and the mod determines it needs to create a panning spot.  The range is between 0.0 and 1.0.  For example, a 0.50 would be a 50% chance.
- Default Value: 1.0

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

"useCustomFarmMaps": true,
"customMaps": {
    "0": "Immersive_Farm_2.json"
  }

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
"useCustomFarmMaps": true,
"customMaps": {
    "1": "MyCoolFishingFarmMapMod.json"
  }

When you first enter the farm, the mod will scan for valid panning spots and create the configured file but there could be unreachable spots.  This is where you can fine tune the file created "DataFiles\MyCoolFishingFarmMapMod.json" and remove unreachable spots.

#### configVersion
Don't modify.  This is used to update the config file if new settings are added.
- Default Value: 3 
