"Mining at the Farm" by Nishtra

Turn your farm into an extension of the mines!
Spawns a number of ores, gems, geode nodes on your farm every day (if there are available spawnable tiles).

*note: for better experience reccomended using together with "Mining with Explosives" mod



Installation
1. Install the latest version of SMAPI
2. Unpack the downloaded zip file and place it's contents into the Mods folder
3. Load a character to create a config file for that character.
4. Close the game and tweak configs to your liking. If you want to reset all settings to default, delete existing config file and the next time you load the game a new config will be created.
5. Play the game.



Config settings:

> "Areas": a list of map names and coordinates defining rectangle areas where items can (or can't) spawn. 

Default settings
"Farm": [
  "- 71,13/72,13", // exclude tiles behing shipping bin
  "- 62,10/66,10", // exclude tiles behind farmhouse
]

Format is
"Areas": {
  "locationName1": ["+ coordinates1", "+ coordinates2", "- coordinates3"],
  "locationName2": ["+ 9999"]

Note 1: 
One set of coordinates looks like this "+ 3,7/76,61" where:
    + - tells the mod that this area should be available for spawn.
    3 - X coordinate of the top left corner of the area
    7 - Y coordinate of the top left corner
    76 - X coordinate of the bottom right corner
    61 - Y coordinate of the bottom right corner
!Important: inside a set should be only a special symbol that (+/-/%) and 4 numbers separated by commas and a slash symbol. Any other symbol will cause an error when trying to parse coordinates (whitespaces are allowed). Different sets are separated with commas.

Note 2:
There are 3 special symbols that can be used
  '+' points that the area should be made available for spawn.
  Format is "+ coordinates1"
  '-' points that the area should be excluded from spawn.
  Format is "- coordinates2"
  '%' if "DangerousSettings" is true, allows to use location specific values for "Number", "Bonus_PerSkillLevel" and all chances settings. Better placed as the first entry in the list for a given location.
  Format is "% number:value / bonus:value / ores:value / iron:value / gold:value / iridium:value / geodes:value / gems:value / mystics:value"
You can drop the settings, you don't need changed (e.g "% number:2 / geodes:0 / gems:0").
There shouldn't be any symbols besides '%', ':'. '/', keywords pointing to the option to change and number values (whitespaces are allowed). Keywords and values should be separated by ':', and different settings should be separated by '/'.

Note 3: 
It is possible to include the whole map (minus the excluded areas) without specifying coordinates. To do this 
use "+ 9999" entry (technically any entry with three or more nines will work. Like 999, 99999999, etc.)

Note 4:
Hill-Top farm default quarry coordinates set: "Farm": ["+ 5,37/27,45"]
Mountain quarry coordinates are : "Mountain": ["+ 106,13/127,34"]

----------------------------------------------------------------

  > "soilTypes": A type of tile where an item can spawn. Can be "Dirt" (default value), "Grass", or "Diggable". There isn't uniform best answer for all the maps, but generally using "Diggable" will cover     the largest number of tiles, though on the forest map "Grass" is also a viable option. Can contain several values separated with comma.
Activating "dangerousSettings" option allows to add "All" soil type. It makes the mod skip all soil type checks and makes it possible to spawn items almost anywhere you can pass. It is discouraged to use "All", as it isn't well tested. And it is Very Much Discouraged to use "All" when "Areas" field has a "+ 9999" entry.

Format is
  "soilTypes": [ "Dirt", "Diggable" ]

----------------------------------------------------------------
    
  > "Number": How many items the mod will try to spawn daily. Keep in mind that this number is further affected by luck and available tiles. Default 3.

----------------------------------------------------------------

  > "Bonus_PerSkillLevel": Default 0.05. Bonus percent per mining skill level to the number of items to spawn (0.05 = 5%). Maxes at 0.1 which will give twice the base number of items at mining level 10.

----------------------------------------------------------------

  > "Skill_Level_Correlation": default TRUE. Certain items will spawn as regular rocks* until you reach required skill level.
    Level:
        2 - Amathyst and Topaz available
        3 - Iron ore available
        5 - Jade, Aquamarine and Frozen Geode available
        6 - Gold ore available
        8 - Ruby, Emerald, Diamond and Magma Geode available
        9 - Iridium ore available

  > "Mines_Level_Correlation": default FALSE. Certain items will spawn as regular rocks* until you reach required level in mines. Also the deepest reached level of mines affects spawn rates.
    Level:
        40 - Iron ore, Frozen Geode, Jade and Aquamarine available
        80 - Gold ore, Magma Geode, Ruby, Emerald and Diamond available
        120 - Iridium ore available

  > "Personal_Experience_Correlation": default FALSE. Certain items will spawn as regular rocks* until you find (gems)/ship (ores) them. Geodes are not affected. Also the number of items found/shipped affects the spawn rates.

*note: the justification for replacing valuable items with regular rocks is that the Farmer doesn't know how an item looks and can't distinguish it from rocks until the requirements are met.

----------------------------------------------------------------

  > "Chance_to_spawn_ore": the chance to spawn a node containing copper/iron/gold or (if you are very lucky) iridium ores. Note that this setting affects ore in general.
  
  > "Chance_to_spawn_[iron/gold/iridium/geode/gem/MysticStone]": the chance to spawn a certain type of object. If you don't want something to spawn, set it's chance to 0.  

----------------------------------------------------------------

  > "DangerousSettings" - FALSE by default. When active makes the mod skip some fail checks and removes the upper limit on "Bonus_PerSkillLevel". Also allows to use "All" soil type and location specific settings.

----------------------------------------------------------------

  > "verbose": how many messages will the mod log to console. If FALSE - the number of messages is minimal, if TRUE - every item spawned will be logged.

----------------------------------------------------------------


  

Requires SMAPI!