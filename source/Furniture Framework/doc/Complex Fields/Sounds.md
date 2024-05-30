**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/FurnitureFramework**

----

# How to define Custom Sounds?

Sounds are defined by only 2 fields:

## Mode

The Mode of a Sound tells the game when to play the sound, it can have 3 values:
- "on_turn_on"
- "on_turn_off"
- "on_click"

The "on_turn_on" and "on_turn_off" modes can only work if the Furniture is [Toggleable](https://github.com/Leroymilo/FurnitureFramework/blob/main/doc/Furniture.md#toggle), while "on_click" will work every time the Furniture is right-clicked (some actions like custom slots might block the sound).

## Name

The name of a Sound tells the game which sound to play when it is triggered, you can find a list of all the sound names that you can use [here](https://www.stardewvalleywiki.com/Modding:Audio#Sound).

# Add new Sounds

It is possible to use a Sound that is not already in the game by adding it with Content Patcher, more information about this [here](https://www.stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Custom_audio).