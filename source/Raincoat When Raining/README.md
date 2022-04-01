**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/bikinavisho/RainCoatWhenRaining**

----

# RainCoatWhenRaining
A Stardew Valley mod that automatically equips rain gear to your character on days that it is raining. 

## Overview
To utilize this mod, first you must acquire the rainy attire, and either have it in your inventory, a nearby dresser, or a nearby chest when you go to sleep.

At the start of each day, this mod checks to see if the weather is rainy/stormy, and if you have any of the rain gear available nearby. If both conditions are true, it will automatically change the hat/boots/shirt of your farmer into the raincoat equivalents. If you are currently wearing a hat or boots, it will unequip them, adding them to your dresser if you have one, otherwise adding it to your inventory. Your shirt will also be unequipped and added to your dresser if you have one, otherwise it will be added to your inventory. If your inventory is full, the items will be dropped beside you. 

At the end of the day, when your farmer goes to bed, it will change your farmer back into the outfit they were wearing (including hat/boots/shirt) before the rainy day. The rain attire you were wearing before will be placed into a nearby dresser if available, or placed into your inventory. If your inventory is full, the rain gear will drop beside you. 

Essentially it's replicating the natural behavior of you, once realizing it is rainy outside, deciding to put on your rain coat, hood, and rain boots before heading outside. 

## Installation

1. Install SMAPI
2. Install [IllogicalMoodSwing's Rainy Day Clothing](https://www.nexusmods.com/stardewvalley/mods/1825) & its dependencies
3. Install this mod
4. Run the game

## Configuration 

The `config.json` will show up after the first time you run the game with this mod. 

| Configuration Option  | Default | Explanation                                                                                                                                                     |
|-----------------------|---------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| RainHoodEnabled       | true    | Set to true if you want it to auto-equip the rain hood to your character on rainy days, false if you don't.                                                     |
| RainBootsEnabled      | true    | Set to true if you want it to auto-equip the rain boots to your character on rainy days, false if you don't.                                                    |
| RainCoatEnabled       | true    | Set to true if you want it to automatically change your character into a rain coat on rainy days, false if you don't.                                           |
| EnableDuringSnow      | false   | Enables automatically equipping the rain ensemble when it is snowing.                                                                                           |

