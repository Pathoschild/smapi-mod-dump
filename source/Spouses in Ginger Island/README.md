**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/SpousesIsland**

----

(For a version in Spanish, [see here](https://github.com/misty-spring/SpousesIsland/blob/main/README-es.md). / Para una versión en Español, [ve aquí](https://github.com/misty-spring/SpousesIsland/blob/main/README-es.md).)

**Spouses' Island** is a Stardew Valley mod, which allows the player's spouse to visit the island. Comes with integrated schedules for the vanilla spouses, and offers support for custom schedules (including mod NPCs).

To choose if it's an island day, this mod uses a randomized number- and compares it with the chance(%) set by the user. This number changes every in-game day.

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
The mod has a framework, but i suggest learning to edit schedules instead (the framework does the same thing).
If you know how schedules work, just add the token `"": "true"` to your patch.

If you'd still rather use the framework instead, [see here.]()

### For more help

* If you're facing issues with the formatting
The [json parser](https://smapi.io/json) will tell you of any problem within the content pack. When asked for a JSON format, leave it as "None".

## Translations
If you'd like to contribute translating, (or post the translation on your own)
## Known issues
Children can't use NPC warps. However, this is a bug on ChildToNPC's side (which i can't do much about).

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/G2G7CXX9P)
