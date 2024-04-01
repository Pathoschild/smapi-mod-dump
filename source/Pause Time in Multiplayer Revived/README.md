**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/mishagp/PauseInMultiplayer**

----

# Pause Time in Multiplayer (Stardew Valley SMAPI mod) 

> [!IMPORTANT]  
> This fork has been updated to be compatible with Stardew Valley 1.6 and SMAPI 4.0

This is a non-intrusive mod that pauses time in multiplayer games when all players meet the conditions that would pause time if they were in a single-player game. A few other mods exist on here with this functionality, but they had some bugs and lack of implementation that jorgamun wasn't happy with so jorgamun made this one.

[NexusMods](https://www.nexusmods.com/stardewvalley/mods/21327)

## Additional Features

* Stops food and drink buffs from ticking down while the game is paused.
* Locks monster positions and player health while paused.
* Slows down clock time if all players are in the Skull Cavern as it does in single-player.
* Displays a subtle X graphic over the game clock to better indicate when time is paused.
* Vote pause function, where if all players vote to toggle pause (default key: Pause/Break) then the game will pause until one of them votes to toggle pause again.
* Pause override hotkey (default: Scroll Lock), allowing the host to manually toggle pausing the game regardless of status or votes
* Optional setting to pause time when any player is in a cutscene (disabled by default)


Each player must have this mod installed for it to function properly.
If each player meets any of the following conditions then time is paused:

* has a menu open
* is engaged in dialogue
* is shopping
* is processing geodes
* is in the animation of pulling crops
* is changing map locations
* is in an event
* is reeling in fish
* is playing an arcade game
* is playing darts
* is playing the crane game
* is watching a movie
* is gambling
* is being asked to use an item (such as eating food)

Time will also pause while all players have voted to or while the host has used their pause override hotkey, in addition to these conditions. If the host has enabled it, time will also pause when any player is in a cutscene.

The game clock will stop advancing when time is paused. NPCs, farm animals, and monsters will lock positions.

Monsters and player hitpoints lock as of 1.4.0.
This feature is new and while I've tested it, if you have any bugs please let me know!

## Installation Instructions

1. Install the latest version of [SMAPI](https://smapi.io/) (4.0.0 or higher)
2. Download the [latest release](https://github.com/mishagp/PauseInMultiplayer/releases) and extract the contents of PauseInMultiplayer.zip to your `StardewValley/Mods` folder for every player (take care to use the same version for every player also)


Optional Configuration
Included with the mod is config.json, which allows you to set different values depending on your preference.
This may be configured by manually editing the file or by using Generic Mod Config Menu.
```json
{
  "ShowPauseX": true, //change to false if you don't want to display the X graphic when paused
  "FixSkullTime": true, //change to false to disable the Skull Cavern time fix (host only)
  "DisableSkullShaftFix": false, //change to true to disable hp fix for dropping down shafts
  "LockMonsters": true, //change to false to disable monster and hp locking (host only)
  "AnyCutscenePauses": false, //change to true to pause for all cutscenes (host only)
  "EnableVotePause": true, //change to false to disable pause voting (host only)
  "VotePauseHotkey": "Pause", //change to whatever hotkey you want to toggle vote pause
  "EnablePauseOverride": true, //change to false to disable pause override hotkey (host only)
  "PauseOverrideHotkey": "Scroll", //change to whatever hotkey to pause override (host only)
  "DisplayVotePauseMessages": true //change to false to disable showing vote messages
}
```

## Credits

Forked from original creator's **jorgamun**'s mod, which can be found on [GitHub](https://github.com/jorgamun/PauseInMultiplayer) and [NexusMods](https://www.nexusmods.com/stardewvalley/mods/10328).