[Better Train Loot](https://github.com/AairTheGreat/StardewValleyMods/tree/master/Better%20Train%20Loot) is a [Stardew Valley](http://stardewvalley.net/) mod which changes the way the town's train loot cans work.
                                                                                                           
**This documentation is for modders and players. See the [Nexus page](https://www.nexusmods.com/stardewvalley/mods/4234) if you want the compiled mod.**
                                                                                                           
## Contents
* [Install](#install)
* [Introduction](#introduction)
* [Configuration Setting](#configuration-setting)
  * [Overview of Config json File](#overview-of-config-json-file)
    - [enableMod](#enableMod)
    - [useCustomTrainTreasure](#useCustomTrainTreasure)
    - [enableNoLimitTreasurePerTrain](#enableNoLimitTreasurePerTrain)
    - [baseChancePercent](#baseChancePercent)
    - [basePctChanceOfTrain](#basePctChanceOfTrain)    
    - [trainCreateDelay](#trainCreateDelay)
    - [maxNumberOfItemsPerTrain](#maxNumberOfItemsPerTrain)
	- [enableForceCreateTrain](#enableForceCreateTrain)
	- [enableMultiplayerChatMessage](#enableMultiplayerChatMessage)
  * [Overview of Trains json File](#overview-of-Trains-json-file)
    - [Train Data](#train-data)
    - [Treasure List Data](#Treasure-List)  
* [Troubleshooting](#troubleshooting)  
  * [Bad Edits to Config json File](#Bad-Edits-to-config-json-file)
  * [Bad Edits to Trains json File](#Bad-Edits-to-Trains-json-file)
* [Localization](#localization)
* [Notes](#notes)
* [Multiplayer Notes](#multiplayer-notes)
* [Thank You!](#thank-you)
* [See Also](#see-also)

## Install
1. If needed, [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/4234).
3. Run the game using SMAPI.

## Introduction
### What is Better Train Loot?
This mod changes behavior of the town's [train's loot](https://stardewvalleywiki.com/Railroad) and hopefully for the better. Note:  The mod still depends on the player to be on the railorad map for items to be created.  If a train comes thru and a player is not there, then don't expect much. 

Better Train Loot allows you to: 
* Fine-tune each train type item list.  Some items are only available by train type.
  Train Types are:
    - Random Train
    - Joja Train
    - Coal Train
    - Passenger Train
    - Prisoner Train
    - Present Train
* Each day now can have up multiple trains per day. (Up to 5 trains)
* A certain train type has a very small chance of dropping a [Stardrop](https://stardewvalleywiki.com/Stardrop)
* Present Trains can have up to 3 times more items and has more chance of dropping items.
* Various configurable settings

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
Maximum treasure from each train.  The amount is still random but there is not limit on the amount.
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
#### maxNumberOfItemsPerTrain
Limits the amount of items per train that the mod will create.  Note: You can still get more than the maximum since the game can still create items.    
- Default Value: 5 
#### enableForceCreateTrain
If value is true, allows the user to force a train to be created by pressing the button Y.  
Note: The train will be created on next game time update if there is no current train.
- Default Value: false 
#### enableMultiplayerChatMessage
If value is true, a chat message will appear to the farmhands letting them know a train is coming.  
Note: This message is currently only in English.
- Default Value: true 

### Overview of Trains json File
Everyone loves items flying off trains!  Therefore, it's time to talk about how the configuration file is put together.  Here is the general workflow:
1. Every new day, the train loot is chances are calculated.  This makes the tresure list be more dynamic.
2. Every game time change, the mod determines if a new train can be created.
3. If the player is on the railroad map, then a random number is generated. 
4. The mod compares that number to see if a the player is a lucky winner of some loot.
5. The games generates another random number.
6. The mod then compares that number with the treasure list of the train going thru Stardew Valley.
7. The train throws the selected treasure which the player can pickup. 

The trains.json file is what controls this and it is located in the "Better Train Loot\DataFiles" mod folder.

#### Train Data
The possible treasure is grouped by train type and each train type has a list of treasure and a corresponding chance. The mod has the following groups:
* RANDOM_TRAIN
* JOJA_TRAIN
* COAL_TRAIN
* PASSENGER_TRAIN
* PRISON_TRAIN
* PRESENT_TRAIN

Each group has the following properties:
#### TrainCarID
This is the internal id used with the mod.  This should not be changed within the trains.json file.  If it's changed, it will break the mod.  

#### Treasure List 
This is the treasure that can be selected if the train decides to give out an item.  More information on treasure data setting is below.

Field                  | Purpose
---------------------- | -------
`Id`                 |Game internal ID for the object.  You can change this if you know the item id you want.
`Name`                | What humans know the items as.  Not really used by the mod, just useful if you don't know the item ID.
`Enabled`          | If the treasure is enabled, then it can be selected to become the player's treasure.  
`Rarity`          | The treasure's rarity.  There are five groups.    

The rarity groups are:
Group                  | Purpose
---------------------- | -------
 `NONE`                | You don't want this treasure.
`COMMON`               | The most common treasure.  It is roughly 10 times more common than the Ultra Rare group.
 `UNCOMMON`            | Still pretty common, but only 6 times more common than the Ultra Rare group.
 `RARE`                | You'll get them but is 5 times less common than the common group
 `UTLRA_RARE`          | The rarest items.  

Here is an example of a train entry looks like:
```
"RANDOM_TRAIN": {
"TrainCarID": "RANDOM_TRAIN",
"treasureList": [
  {
    "Id": 16,
    "Name": "Wild Horseradish",
    "Enabled": true,
    "Rarity": "COMMON"
  },
  {
    "Id": 18,
    "Name": "Daffodil",
    "Enabled": true,
    "Rarity": "COMMON"
  },
  {
    "Id": 20,
    "Name": "Leek",
    "Enabled": true,
    "Rarity": "COMMON"
  }
  ]
}
```

## Troubleshooting

### Bad Edits to Config json File
It possible that you decided to edit the config file and now it's not working as expected.  To get back to the default config.json file:
1. Stop Stardew Valley, if running.
2. Delete the config.json file.
3. Start Stardew Valley the default json file will be recreated.

### Bad Edits to Trains json File
It possible that you decided to edit the train config file and now it's not working as expected.  To get back to the default trains.json file:
1. Stop Stardew Valley, if running.
2. Delete the trains.json file.
3. Start Stardew Valley the default json file will be recreated.
 
## Localization
There is a chat message that is created when a train is created.  This should be localized eventually.

## Notes
If any boots or rings are in the treasure list, they will be created as a normal item.  This allows you to sell them in the shipping bin but not wearable.

## Multiplayer Notes
Here are a couple things:
1. It appears Stardew Valley does not send a message to farmhand players.  If this mod creates a train a chat message is sent out.
2. Farmhand trains are lagging.  I think this a base game issue, but it could be mod related.  I'm still looking into this.


## Thank You!
* [Concerned Ape](https://twitter.com/concernedape) - Creator of Stardew Valley.
* [Pathoschild](https://smapi.io/) - Creator of the Stardew Modding API.
* [Stardew Wiki](https://stardewvalleywiki.com) - To the people maintaining this very useful site.
* [Stardew ID List](https://stardewids.com/) - To the people maintaining this very useful site.
* Balentay for giving me the idea for this mod.
* To my testers: My Better Half  -- Thank you!

## See Also
* [Stardew Valley](https://www.stardewvalley.net/) - Home page and blog
* [Stardew Valley Mods Nexus Page](https://www.nexusmods.com/stardewvalley/mods)
