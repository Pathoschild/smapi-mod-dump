/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

This file is to explain the configuration settings for the config.json file.
The most up to date information will be at:
https://github.com/AairTheGreat/StardewValleyMods/tree/master/Better%20Train%20Loot

## Configuration Setting
### Overview of Config json File
Once this mod is installed and been started at least once, you can adjust some settings.  
If you don't have a config.json file, then the config file will be created when you first run Stardew Valley with this mod.
You should not need to adjust the configuration settings but if you do, here are what the setting are inside the config.json file:

#### enableMod
Sets the mod as enabled or disabled.  Normally should be set to true unless you are having issues and want to test something without removing the mod.  
- Default Value: true 

#### useCustomTrainTreasure
Uses the custom treasure list.  If set to false, then the base game item list is used.  
- Default Value: true 

#### enableNoLimitTreasurePerTrain
Maximum treasure from each train.  The amount is still random but there is not limited on the amount.
- Default Value: false 

#### showTrainIsComingMessage
Shows/hides the message when a train is passing thru the valley while you are not in the desert.  
- Default Value: true 

#### showDesertTrainIsComingMessage
Shows/hides the message when a train is passing thru the valley while you are in the desert.  
- Default Value: true 

#### showIslandTrainIsComingMessage
Shows/hides the message when a train is passing thru the valley while you are on the island.  
- Default Value: false 

#### enableTrainWhistle
When the train comes thru the valley and you are not in the desert, does it make a sound?  
- Default Value: true 

#### enableDesertTrainWhistle
When the train comes thru the valley and you are in the desert, does it make a sound?  
- Default Value: true

#### enableIslandTrainWhistle
When the train comes thru the valley and you are on the island, does it make a sound?  
- Default Value: false

#### baseChancePercent
What is the chance to get something from a train.  The player's daily luck does factor into this.
- Default Value: 0.20 

#### basePctChanceOfTrain
Every time the game time changes, this the percent chance of a new train, assuming the daily maximum has not been met.
- Default Value: 0.15 

#### trainCreateDelay
How many milliseconds from when the message about a train is going thru Stardew Valley and when the train shows up.
- Default Value: 10000 (10 seconds) 

#### maxTrainsPerDay
Sets the maximum possible number of trains.  
Note: You can still get more or less than the maximum since the game can still create trains and you might have a really bad luck day.    
- Default Value: 5 

#### maxNumberOfItemsPerTrain
Limits the amount of items per train that the mod will create.  
Note: You can still get more than the maximum since the game can still create items.    
- Default Value: 5 
 
#### enableForceCreateTrain
If value is true, allows the user to force a train to be created by pressing the button Y.  
Note: The train will be created on next game time update if there is no current train.
- Default Value: false 

#### enableMultiplayerChatMessage  -- DEAD Config setting in SDV version 1.4 and greater
If value is true, a chat message will appear to the farmhands letting them know a train is coming.  
Note: This message is currently only in English.
- Default Value: false 

#### configVersion
Don't modify.  This is used to update the config file if new settings are added.
- Default Value: 2 