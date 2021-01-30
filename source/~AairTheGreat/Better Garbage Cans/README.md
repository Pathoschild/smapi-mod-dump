**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/AairTheGreat/StardewValleyMods**

----

[Better Garbage Cans](https://github.com/AairTheGreat/StardewValleyMods/tree/master/Better%20Garbage%20Cans) is a [Stardew Valley](http://stardewvalley.net/) mod which changes the way the town's garbage cans work.
                                                                                                           
**This documentation is for modders and player. See the [Nexus page](https://www.nexusmods.com/stardewvalley/mods/4171) if you want the compiled mod.**
                                                                                                           
## Contents
* [Install](#install)
* [Introduction](#introduction)
* [Configuration Setting](#configuration-setting)
  * [Overview of Config json File](#overview-of-config-json-file)
    - [enableMod](#enableMod)
    - [useCustomGarbageCanTreasure](#useCustomGarbageCanTreasure)
    - [allowMultipleItemsPerDay](#allowMultipleItemsPerDay)
    - [allowGarbageCanRecheck](#allowGarbageCanRecheck)
    - [baseChancePercent](#baseChancePercent)
	- [baseTrashChancePercent](#baseTrashChancePercent)
	- [enableBirthdayGiftTrash](#enableBirthdayGiftTrash)
    - [birthdayGiftChancePercent](#birthdayGiftChancePercent)
    - [FriendshipPoints](#FriendshipPoints)
    - [LinusFriendshipPoints](#LinusFriendshipPoints)
    - [WaitTimeIfFoundNothing](#WaitTimeIfFoundNothing)
    - [WaitTimeIfFoundSomething](#WaitTimeIfFoundSomething)
  * [Overview of Garbage Cans json File](#overview-of-garbage-cans-json-file)
    - [Garbage Can Data](#garbage-can-data)
    - [Treasure List Data](#Treasure-List)  
* [Troubleshooting](#troubleshooting)  
  * [Bad Edits to Config json File](#Bad-Edits-to-config-json-file)
  * [Bad Edits to Garbage Cans json File](#Bad-Edits-to-Garbage-Cans-json-file)
* [Localization](#localization)
* [See Also](#see-also)

## Install
1. If needed, [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/4171).
3. Run the game using SMAPI.

## Introduction
### What is Better Garbage Cans?
This mod changes behavior of the town's [garbage cans](https://stardewvalleywiki.com/Garbage_Can) and hopefully for the better.  

Better Garbage Cans allows you to: 
* Fine-tune each garbage can item list.  Some items are only available at certain times.
* Allow user to check the garbage cans multiple times a day.
* While in a multiplayer game, each farmer/farmhand can get items from the cans.
* Depending on where someone lives, their favorite item might be found in a garbage can the day before and on their birthday.
* Various configurable settings

## Configuration Setting
### Overview of Config json File
Once this mod is installed and been started at least once, you can adjust some settings.  

If you don't have a config.json file, then the config file will be created when you first run Stardew Valley with this mod.

You should not need to adjust the configuration settings but if you do, here are what the setting are inside the config.json file:
#### enableMod
Sets the mod as enabled or disabled.  Normally should be set to true unless you are having issues and want to test something without removing the mod.  
- Default Value: true 
#### useCustomGarbageCanTreasure
Uses the custom treasure list.  If set to false, then the base game item list is used.  
- Default Value: true 
#### allowMultipleItemsPerDay
Allows to get multiple items from the same garbage can.
- Default Value: true 
#### allowGarbageCanRecheck
Allows you check the same garbage can multiple times.
- Default Value: true 
#### baseChancePercent
What is the chance to get something from a garbage can.  The player's daily luck does factor into this.
- Default Value: 0.25 (5% better than the base game value)
#### baseTrashChancePercent
What is the chance to get trash instead of the good stuff from a garbage can.  
- Default Value: 0.25 
#### enableBirthdayGiftTrash
Enables or disables birthday gifts on the day before and on a birthday.
- Default Value: true 
#### birthdayGiftChancePercent
The increased percent chance of the birthday gift in the respective garbage can.    
- Default Value: 0.75 
#### FriendshipPoints
The amount of friendship points lost (or gained) if someone sees you, other than Linus.    
- Default Value: -25 (Base Game Value) 
#### LinusFriendshipPoints
The amount of friendship points lost (or gained) if Linus sees you.    
- Default Value: 5 (Base Game Value) 
#### WaitTimeIfFoundNothing
Per garbage can, The amount of time in minutes that you have to wait to try again if you have not found anything.
- Default Value: 60
#### WaitTimeIfFoundSomething
Per garbage can, the amount of time in minutes that you have to wait to try again if you did found something.  
- Default Value: 240 

### Overview of Garbage Cans json File
Everyone loves garbage can diving!  Therefore, it's time to talk about how the configuration file is put together.  Here is the general workflow:
1. Player goes and finds a garbage can.
2. The game generates a random number.
3. The mod compares that number to see if a the player is a lucky winner of some loot.
4. The games generates another random number.
5. The mod then compares that number with the treasure list of the garbage can.
6. Player gets selected treasure. 

The garbage_cans.json file is what controls this and it is located in the "Better Garbage Cans\DataFiles" mod folder.

#### Garbage Can Data
The possible treasure is grouped by garbage can and each garbage can has a list of treasure and a corresponding chance.  The mod has the following groups:
  
Each group has the following properties:
#### GarbageCanID
This is the internal id used with the mod.  This should not be changed within the garbage_cans.json file.  If it's changed, it will break the mod.  
#### LastTimeChecked
When the mod is running this is the last game time the garbage can was checked. Resets each day to -1.
#### LastTimeFoundItem 
When the mod is running this is the last game time the garbage can gave an item. Resets each day to -1.
#### Treasure List 
This is the treasure that can be selected if the garbage can decides to give out an item.  More information on treasure data setting is below.

Field                  | Purpose
---------------------- | -------
`Id`                 |Game internal ID for the object.  You can change this if you know the item id you want.
`Name`                | What humans know the items as.  Not really used by the mod, just useful if you don't know the item ID.
`Enabled`          | If the treasure is enabled, then it can be selected to become the player's treasure.  
`Chance`          | The chance this treasure is selected if it's treasure group is selected.  Value range: (0.0 - 1.0)
`MinAmount`        | The minimum number of items you can get from the garbage can.
`MAxAmount`               | The maximum number of items you can get from the garbage can.
`AvailableStartTime`          | The start time for when the item is available to be selected.  
`AvailableEndTime`          | The end time is the last time for the item to be available to be selected.  

Here is an example of a garbage can entry looks like:
```
  "JODI_SAM": {
    "GarbageCanID": "JODI_SAM",
    "LastTimeChecked": -1,
    "LastTimeFoundItem": -1,
    "treasureList": [
      {
        "Id": 72,
        "Name": "Diamond ",
        "Chance": 0.005,
        "Enabled": true,
        "MinAmount": 1,
        "MaxAmount": 1,
        "AvailableStartTime": 2100,
        "AvailableEndTime": 2600
      },
      {
        "Id": 88,
        "Name": "Coconut",
        "Chance": 0.0075,
        "Enabled": true,
        "MinAmount": 1,
        "MaxAmount": 1,
        "AvailableStartTime": 600,
        "AvailableEndTime": 2600
      }
	]
```

## Troubleshooting

### Bad Edits to Config json File
It possible that you decided to edit the config file and now it's not working as expected.  To get back to the default config.json file:
1. Stop Stardew Valley, if running.
2. Delete the config.json file.
3. Start Stardew Valley the default json file will be recreated.

### Bad Edits to Garbage Cans json File
It possible that you decided to edit the garbage can config file and now it's not working as expected.  To get back to the default garbagecans.json file:
1. Stop Stardew Valley, if running.
2. Delete the garbage_cans.json file.
3. Start Stardew Valley the default json file will be recreated.
 
## Localization
No real need to localize this mod.

## Other Mod Conflicts
There is a minor conflict with the [Automate](https://www.nexusmods.com/stardewvalley/mods/1063) mod.  It will only use it's copy of the game logic item list.
Depending on your config settings, you will be able to still check the garbage cans manually and will get the mod items.  

## Thank You!
* [Concerned Ape](https://twitter.com/concernedape) - Creator of Stardew Valley.
* [Pathoschild](https://smapi.io/) - Creator of the Stardew Modding API.
* [Stardew Wiki](https://stardewvalleywiki.com) - To the people maintaining this very useful site.
* [Stardew ID List](https://stardewids.com/) - To the people maintaining this very useful site.
* Sylverlyf for giving me the idea for this mod.
* To my testers: SparkyTheCat and My Better Half  -- Thank you!

## See Also
* [Stardew Valley](https://www.stardewvalley.net/) - Home page and blog
* [Stardew Valley Mods Nexus Page](https://www.nexusmods.com/stardewvalley/mods)
