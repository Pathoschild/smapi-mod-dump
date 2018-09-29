
# Multiplayer Emotes

This mod allows to play emotes and to other player see your emotes.
Adds a interface to play any Stardew Valley emote in any time.

The interface is a button to open and close the emote list, and the button can be dragged holding the right click and positioned in any place of the screen.

To know what changed between versions you can go to the [Release Notes](https://github.com/FerMod/StardewMods/tree/master/MultiplayerEmotes/release-notes.md).

>**Note:** In order to other players see the emotes they also need to have this mod.

## Instalation

To download the mod, go to the the page of [Multiplayer Emotes](https://www.nexusmods.com/stardewvalley/mods/2347) in NexusMods mod page.

This mod requires [SMAPI](https://smapi.io/). All the help to install SMAPI and the troubleshooting help can be found in the "Player guide".

Follow these spteps to install the mod:
1. Download the mod [here](https://www.nexusmods.com/stardewvalley/mods/2347)
2. Extract the `.zip` in the `Mods` folder


## Configuration File
The mod allows  some configuration, and can be changed in the `config.json` file if you want. 
The config file is generated once Stardew Valley is launched at least once with the mod installed.

Available settings:

| Setting                     | Effect
| --------------------------- | -------------------------------
| `AnimateEmoteButtonIcon` | Default `true`. Enable or disable the emote menu button animation
| `ShowTooltipOnHover`      | Default `true`. Enable or disable the tooltip when hovering the emote menu button

## Console Commands
This mod adds some console commands to use with the SMAPI console. This can be useful in case a emote gets stuck playing or to stop playing emotes.

The commands are the following:
  
| Command 				| Parameters 							   | Action
| --------------------- | -----------------------------------------|----------------
| `emote <value>`		| A integer representing the animation id. | Play the emote with the passed id
| `stop_emote` 		| None				                       | Stop any playing emote
| `stop_all_emotes` 	| None		                               | Stop any playing emote by other players
