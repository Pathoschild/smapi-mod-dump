/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

This file is to explain the configuration settings for the treasure.json file located in the DataFiles folder.

The most up to date information will be at:
https://github.com/AairTheGreat/StardewValleyMods/tree/master/Better%20Panning

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

Field                  	| Purpose
---------------------- 	| -------
Id     		            | Game internal ID for the object.  You can change this if you know the item id you want.
Name                	| What humans know the items as.  Not really used by the mod, just useful if you don't know the item ID.
Chance          		| The chance this treasure is selected if it's treasure group is selected.  Value range: (0.0 - 1.0)
MinAmount        		| The minimum number of items you can pan per panning spot.
MAxAmount               | The maximum number of items you can pan per panning spot.
AllowDuplicates         | If the item can be selected again if the mod feels like giving you more stuff.
Enabled          		| If the treasure is enabled, then it can be selected to become the player's treasure.  
						| Note: If you change this setting, you should set the treasure group setting ManualOverride to true.

Here is an example of a treasure group entry looks like:
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
