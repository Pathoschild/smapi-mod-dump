# RainCoatWhenRaining
A Stardew Valley mod that automatically equips rain gear to your character on days that it is raining. 

## Overview
This mod, at the start of each day, checks to see if the weather is rainy/stormy. If it is, it will automatically change the hat/boots/shirt of your farmer into the raincoat equivalents. If you are currently wearing a hat or boots, it will unequip them, adding them to your inventory. If your inventory is full, it will drop them beside you. At the end of the day, when your farmer goes to bed, it will change your farmer back into the outfit they were wearing (including hat/boots/shirt) before the rainy day. The rain hood and rain boots will be removed from the player's inventory on days it is not raining. 
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
| RaincoatClothingIndex | 112     | This is the shirt number of the rain coat. If you have not modified  IllogicalMoodSwing﻿'s Rainy Day Clothing mod in any way, you shouldn't need to change this. |
