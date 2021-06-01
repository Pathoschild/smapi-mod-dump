**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Hakej/Animal-Pet-Status**

----


# Animal Pet Status
This project is a mod for a game [Stardew Valley](https://www.stardewvalley.net/) which shows you the animals you haven't pet this day.

## Requirements
You have to have [SMAPI](https://smapi.io/) installed to run this mod.

## How to install it
You can download the mode via [latest release page](https://github.com/Hakej/Animal-Pet-Status/releases) or [nexus page](https://www.nexusmods.com/stardewvalley/mods/8725/). After you download the archive, go into your Stardew Valley installation folder and unpack the archive into **Mods** directory.

## How to use it
You can toggle the mod on/off by pressing a key (default **P**) and move it by holding down a key (default **L**) and moving your cursor to a desired place.


When the mod is on and you're on your farm, it shows you list of the animals you haven't pet yet on the screen. Animals that are in different location than you have their names greyed out.

After you pet an animal it will disappear from the list. 
When you'll pet all your animals it will show you a notification.

## Configuration
Built mod has a config.json file that you can freely modify to your needs. Mod will also remember your settings even when you modify them via game, like the position.
```json
{
  "ToggleButton": "P",
  "MoveButton": "L",
  "Position": "10, 10",
  "IsActive": true
}
```

## How it looks in action
![animal_pet_status](https://user-images.githubusercontent.com/25157378/117950728-c83b1d80-b313-11eb-9aaa-cd663a7a305f.gif)
