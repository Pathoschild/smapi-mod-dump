**Save Anywhere** is a [Stardew Valley](http://stardewvalley.net/) mod which lets you save your
game anywhere by pressing a key.

Compatible with Stardew Valley 1.2+ on Linux, Mac, and Windows.

## Installation
1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
2. Install [this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/444).
3. Run the game using SMAPI.

## Usage
Press `K` to save anywhere. Edit `Save_Anywhere_Config.txt` to configure the key.

## Versions
1.0:
* Initial release.

2.0:
* Major rewrite.
* Time is no longer simulated at high-speed to reset to the same time. This caused too many NPC pathfinding issues and players felt it was a bit of an exploit.
* NPCs should now properly pathfind themselves when loading after saving anywhere. Thanks to @Entoarox for the help!
* Save data is now better organised.
* Fixed various load crashes.
* Fixed the game transitioning to the next day when you save after putting items in your shipping bin.

2.0.1:
* Fixed crash if you don't have a horse.

2.0.2:
* Fixed config file.

2.1:
* Fixed mod crashing in some cases.

2.4:
* Updated to Stardew Valley 1.2 and SMAPI 1.12.

2.5:
* Updated for SMAPI 2.0.
* Overhauled save format.
* Switched to standard JSON config file.
* Fixed crash when saving in the community center.
* Fixed crash during cutscenes.
* Fixed load warp only happening after you move.
* Fixed load not working after you exit to title.
* Fixed some old data being restored if you reload after a normal save.
* Fixed player/NPC facing directions not being restored.
* Internal refactoring.

2.6:
* ?

2.6.1:
* Enabled update checks in SMAPI 2.0+.
