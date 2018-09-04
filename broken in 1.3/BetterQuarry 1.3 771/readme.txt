"Better Quarry" by Nishtra

Improves the standard Stardew Valley quarry.
Spawns set number of ores, gems, geode nodes in the quarry every day (if there are available spawnable tiles). Chances for all items are customizable.

*note: for better experience reccomended using together with "Mining with Explosives" mod


Installation
0. Install the latest version of SMAPI
1. Unpack the downloaded zip file and place it's contents into the Mods folder
2. Tweak the configs to your liking. If there is no config file or you want to reset all settings to default, delete existing config file and the next time you load the game a new config will be created.
3. Play the game.



Config settings:
  > "verbose": how many messages will the mod log to console. If FLASE - the number of messages is minimal, if TRUE - every item spawned will be logged.

  > "baseNumber": How many items the mod will try to spawn daily. Keep in mind that this number is further affected by luck and available tiles. Default 3.

  > "bonusPerSkillLevel": Default 0.05. Bonus percent per mining skill level to the number of items to spawn (0.05 = 5%). Maxes at 0.1 which will give twice the base number of items at mining level 10. 

  > "replaceExistingObjects": If the tile is already occupied an existing object will be replaced with the new one. Default FALSE

  > "chanceToReplace": Chances to replace an existing object, Default 0.5

  > "skillLevelCorrelation": default TRUE. Certain items will spawn as regular rocks** until you reach required skill level.
    Level:
        2 - Amathyst and Topaz available
        3 - Iron ore available
        5 - Jade, Aquamarine and Frozen Geode available
        6 - Gold ore available
        8 - Ruby, Emerald, Diamond and Magma Geode available
        9 - Iridium ore available

  > "minesLevelCorrelation": default FALSE. Certain items will spawn as regular rocks** until you reach required level in mines. Also the deepest reached level of mines affects the spawn rates.
    Level:
        40 - Iron ore, Frozen Geode, Jade and Aquamarine available
        80 - Gold ore, Magma Geode, Ruby, Emerald and Diamond available
        120 - Iridium ore available
  
  > "personalExperienceCorrelation": default FALSE. Certain items will spawn as regular rocks** until you found (gems)/ship (ores) them. Geodes are not affected. Also the number of items found/shipped affects the spawn rates.

**note: the justification for replacing valuable items with regular rocks is that the Farmer doesn't know how an item looks and can't distinguish it from rocks until the requirements are met. Also, since v1.5 when requirments are met some of existing rocks will be transformed to ore/gem nodes.

  > "spawnOres": Spawn ore nodes on your farm? Default TRUE

  > "spawnGeodes": Spawn geode nodes on your farm? Default TRUE

  > "spawnMinerals": Spawn minerals that you can get after breaking a geode. Default FALSE  

  > "spawnGems": Spawn gem nodes on your farm? Default TRUE

  > "spawnMysticStones": Spawn mystic stones (those purplish with spiral marks) on your farm? Default TRUE

  > "[object]Chance": Chance to spawn [object] (ore/geode/gem/mineral/mystic stone)

Requires SMAPI!