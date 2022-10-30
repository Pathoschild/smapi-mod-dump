**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/SpousesIsland**

----

(For a version in Spanish, [see here](https://github.com/misty-spring/SpousesIsland/blob/main/README-es.md). / Para una versión en Español, [ve aquí](https://github.com/misty-spring/SpousesIsland/blob/main/README-es.md).)

**Spouses' Island** is a Stardew Valley mod, which allows the player's spouse to visit the island. Comes with schedules for vanilla spouses and SVE+Child2NPC compatibility.
User-made are avaiable via ContentPatcher.

## Contents
* [For players](#for-players)
  * [Install](#install)
  * [Uninstall](#uninstall)
  * [Compatibility](#compatibility)
* [For custom spouses](#for-custom-spouses)
  * [How to use](#how-to-use)
  * [For more help](#for-more-help)
* [Translations](#translations)
* [Known issues](#known-issues)

## For players
### Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/11037).
3. Install [Non Destructive NPCs](https://www.nexusmods.com/stardewvalley/mods/5176) and [Custom NPC Exclusions](https://www.nexusmods.com/stardewvalley/mods/7089).
4. Run the game using SMAPI.

### Uninstall
Remove this mod from the game's 'Mods' folder (or, if placed in a sub-folder, remove it from that one).

### Compatibility
Works with Stardew Valley 1.5.5 on Linux/macOS/Windows. (Requires SMAPI 3.14)

The following mods are confirmed to be compatible:
* Child To NPC
* Free Love
* Stardew Valley Expanded
* Immersive spouses
* ContentPatcher mods

This mod only edits schedules on specific days (and uses unique dialogue keys), it should work with most mods.

## For custom spouses
### How to use
Edit the spouse's schedule via ContentPatcher, and add the following token: `mistyspring.spousesisland/IslandToday` as condition.

You don't need to worry about pathing them to a bed: the mod will do this starting from 10pm. 
(Just make sure their last schedule point is 'IslandFarmHouse').

The schedule can be however you want. For example:
```
{
  "Action": "EditData",
  "Target": "Characters/schedules/<your-spouse-name>",
  "Entries": {
     "marriage_Mon": "700 IslandFarmHouse 16 9 0/900 IslandFarmHouse 20 15 0/1200 IslandWest 39 41 0/1400 IslandWest 39 45 3/1500 IslandWest 85 39 2/1700 IslandSouth 12 27 2/a2300 IslandFarmHouse 16 9 0"
     "marriage_Tue": "GOTO marriage_Mon", 
     "marriage_Wed" = "GOTO marriage_Mon",
     "marriage_Thu" = "GOTO marriage_Mon",
     "marriage_Fri" = "GOTO marriage_Mon",
     "marriage_Sat" = "GOTO marriage_Mon",
     "marriage_Sun" = "GOTO marriage_Mon";
     },
  "When": {
     "mistyspring.spousesisland/IslandToday": "true"
    }
  },
```

### For more help

* If you're facing issues with the formatting
The [json parser](https://smapi.io/json) will tell you of any problem within the content pack. When asked for a JSON format, leave it as "None".

## Translations
If you'd like to contribute translating, you can send me the translation (either via nexusmods or as a pull request.) You can also post the translation on nexus as your own file.

## Known issues
Children can't use NPC warps. However, this is a bug on ChildToNPC's side (which i can't do much about).

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/G2G7CXX9P)
