This file is to explain the configuration settings for the trains.json file located in the DataFiles folder.
The most up to date information will be at:
https://github.com/AairTheGreat/StardewValleyMods/tree/master/Better%20Train%20Loot

### Overview of Trains json File
Everyone loves items flying off trains!  Therefore, it's time to talk about how the configuration file is put together.  
Here is the general workflow:
1. Every new day, the train loot is chances are calculated.  This makes the tresure list be more dynamic.
2. Every game time change, the mod determines if a new train can be created.
3. If the player is on the railroad map, then a random number is generated. 
4. The mod compares that number to see if a the player is a lucky winner of some loot.
5. The games generates another random number.
6. The mod then compares that number with the treasure list of the train going thru Stardew Valley.
7. The train throws the selected treasure which the player can pickup. 

The trains.json file is what controls this and it is located in the "Better Train Loot\DataFiles" mod folder.

#### Train Data
The possible treasure is grouped by train type and each train type has a list of treasure and a corresponding chance.
The mod has the following groups: 
* RANDOM_TRAIN
* JOJA_TRAIN
* COAL_TRAIN
* PASSENGER_TRAIN
* PRISON_TRAIN
* PRESENT_TRAIN

Each group has the following properties:
#### TrainCarID
This is the internal id used with the mod.  This should not be changed within the trains.json file.  
If it's changed, it will break the mod.  

#### Treasure List 
This is the treasure that can be selected if the train decides to give out an item.  
More information on treasure data setting is below.

Field               | Purpose
--------------------| -------
Id                  | Game internal ID for the object.  You can change this if you know the item id you want.
Name                | What humans know the items as.  Not really used by the mod, just useful if you don't know the item ID.
Enabled             | If the treasure is enabled, then it can be selected to become the player's treasure.  
Rarity          	| The treasure's rarity.  There are five groups.    

The rarity groups are:
Group               | Purpose
------------------- | -------
NONE                | You don't want this treasure.
COMMON              | The most common treasure.  It is roughly 10 times more common than the Ultra Rare group.
UNCOMMON            | Still pretty common, but only 6 times more common than the Ultra Rare group.
RARE                | You'll get them but is 5 times less common than the common group
UTLRA_RARE          | The rarest items.  