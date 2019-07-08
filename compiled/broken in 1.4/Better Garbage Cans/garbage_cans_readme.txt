This file is to explain the configuration settings for the garbage_cans.json file located in the DataFiles folder.
The most up to date information will be at:
https://github.com/AairTheGreat/StardewValleyMods/tree/master/Better%20Garbage%20Cans

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
---------------------- | -----------------------------------------------------------------------------------------------
`Id`                   | Game internal ID for the object.  You can change this if you know the item id you want.
`Name`                 | What humans know the items as.  Not really used by the mod, just useful if you don't know the item ID.
`Enabled`              | If the treasure is enabled, then it can be selected to become the player's treasure.  
`Chance`               | The chance this treasure is selected if it's treasure group is selected.  Value range: (0.0 - 1.0)
`MinAmount`            | The minimum number of items you can get from the garbage can.
`MaxAmount`            | The maximum number of items you can get from the garbage can.
`AvailableStartTime`   | The start time for when the item is available to be selected.  
`AvailableEndTime`     | The end time is the last time for the item to be available to be selected.  
