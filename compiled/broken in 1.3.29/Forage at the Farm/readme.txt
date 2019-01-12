"Forage at the Farm" by Nishtra

Gather (not only) seasonal forages on your farm!
Spawns a number of forageable items on specified locations every day (if there are suitable and placeable tiles).



Installation
1. Install the latest version of SMAPI
2. Unpack the downloaded zip file and place it's contents into the Mods folder
3. Load a character to create a config file for that character.
4. Close the game and tweak configs to your liking. If you want to reset all settings to default, delete existing config file and the next time you load the game a new config will be created.
5. Play the game.



Console commands:
"export_object_info" - saves the ID, name and price of every object in ObjectInformation.xnb (plus those added by other mods) to a text file in the mod's folder.
"export_full_object_info" - saves ID and data string of every object to a text file in the mod's folder.



Config settings:

  > "Forage_Lists" - lists of ID's for items to spawn. Default values are set to seasonal forage. The list should contain all 4 seasons, but a season can have none ID (no items will be spawned at that season).

*Tip: You can raise a chance to spawn certain item by including it's ID several times

Format is
"Forage_Lists": {
  "locationName1": [
    "spring:id1,id2",
    "summer:id1,id2",
    "fall:id1,id2",
    "winter:id1,id2"
  ]
"locationName2": [
    "spring:",
    "summer:id2,id3",
    "fall:id1",
    "winter:"
  ]
}

----------------------------------------------------------------

> "Areas": a list of map names and coordinates defining rectangle areas where items can (or can't) spawn. 

Default settings
"Farm": [
  "+ 99999",       // include the whole Farm map
  "- 71,13/72,13", // exclude tiles behing shipping bin
  "- 62,10/66,10", // exclude tiles behind farmhouse
  "- 63,17/65,17", // exclude tiles in front of stairs to farmhouse
  "- 68,15/68,16"  // exclude tiles behind mailbox

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
  '%' if "DangerousSettings" is true, allows to use location specific values for "Number_PerDay", "Bonus_PerSkillLevel" and "Chance" settings. Better placed as the first entry in the list for a given location.
  Format is "% number:value / bonus:value / chance:value"
You can use different order or drop the settings, you don't need to be changed.

Note 3: 
It is possible to include the whole map (minus the excluded areas) without specifying coordinates. To do this 
use "+ 9999" entry (technically any entry with three or more nines will work. Like 999, 99999999, etc.)

----------------------------------------------------------------

  > "soilTypes": A type of tile where an item can spawn. Can be "Grass" (default value), "Dirt", "Stone" or "Diggable". There isn't uniform best answer for all the maps, but generally using "Diggable" will cover     the largest number of tiles, though on the forest map "Grass" is also a viable option. Can contain several values separated with comma.
Activating "DangerousSettings" option allows to add "All" soil type. It makes the mod skip all soil type checks and makes it possible to spawn items almost anywhere you can pass. It is discouraged to use "All", as it isn't well tested. And it is Very Much Discouraged to use "All" when "Areas" field has a "+ 9999" entry.

Format is
  "soilTypes": [ "Grass", "Dirt" ]

----------------------------------------------------------------

  > "Number_PerDay" - Default 1. A number of forageables to spawn per day at one location (maxes at 10). It is further affected by different factors, so the real number of spawned items will usually be higher.

----------------------------------------------------------------

  > "Bonus_PerSkillLevel" - Default 0.05. Bonus percent per foraging skill level to the number of items to spawn (0.05 = 5%). Maxes at 0.2 which will give three times the base number of items at foraging level 10.

  > "Chance" - Default 0.75. A chance for an item to spawn (counts separately for every item, maxes at 1).

----------------------------------------------------------------

  > "DangerousSettings" - FALSE by default. When active makes the mod skip checking for non-forage items and removes the upper limit on "Number_PerDay" and "Bonus_PerSkillLevel" settings. Also allows to use "All" soil type and redefine "Number_PerDay", "Bonus_PerSkillLevel" and "Chance" for a specific location.

----------------------------------------------------------------

  > "verbose": how many messages will the mod log to console. If FALSE - the number of messages is minimal, if TRUE - every item spawned will be logged.

----------------------------------------------------------------




Requires SMAPI!





*************ID's Help**************
16: Wild Horseradish
18: Daffodil
20: Leek
22: Dandelion
257: Morel
259: Fiddlehead Fern
281: Chanterelle
283: Holly
396: Spice Berry
398: Grape
402: Sweet Pea
404: Common Mushroom
406: Wild Plum
408: Hazelnut
410: Blackberry
412: Winter Root
414: Crystal Fruit
416: Snow Yam
418: Crocus
420: Red Mushroom
422: Purple Mushroom