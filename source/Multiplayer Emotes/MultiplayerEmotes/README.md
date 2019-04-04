
# Multiplayer Emotes

This mod allows you to play emotes with an interface and allows you to see other player, npc and farm animal emotes.

This interface is a button, that by left clicking it, opens and closes all the available emotes list. By doing right click and dragging the mouse, this interface can be positioned in any place of the screen. This allows to have other mods that add HUDs and other GUI elements, without causing any conflicts.

This mod have been tested more heavily in Windows, and in less degree in Unix and Mac.
If some type of issue related to this mod arises, please report it, so I can fix it.

To know what changed between versions you can go to the [Release Notes](/MultiplayerEmotes/release-notes.md).

> **Note:** In order to see other player emotes they will also need to have this mod. Sorry, this is due to some limitations.

## Instalation

To download the mod, go to the the page of [Multiplayer Emotes](https://www.nexusmods.com/stardewvalley/mods/2347) in NexusMods mod page.

This mod requires [SMAPI](https://smapi.io/). All the help to install SMAPI and the troubleshooting help can be found in the "Player guide".

Follow these steps to install the mod:

1. Download the mod [here](https://www.nexusmods.com/stardewvalley/mods/2347)
2. Extract the `.zip` in the `Mods` folder

## Configuration File

The mod allows some configuration, and can be changed in the `config.json` file if you want.
The config file is generated once Stardew Valley is launched at least once with the mod installed.

Available settings:

| Setting                          | Effect                                                                                                                                  |
| -------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| `AnimateEmoteButtonIcon`         | Default `true`. Enable or disable the emote menu button animation.                                                                      |
| `ShowTooltipOnHover`             | Default `true`. Enable or disable the tooltip when hovering the emote menu button.                                                      |
| `AllowNonHostEmoteNpcCommand`    | Default `false`. Allow other players to use the command `emote_npc`.<br>This command allows to force to a NPC to play an emote.           |
| `AllowNonHostEmoteAnimalCommand` | Default `false`. Allow other players to use the command `emote_animal`.<br>This command allows to force to a FarmAnimal to play an emote. |

## Console Commands

This mod adds some console commands to use with the SMAPI console. This can be useful in case a emote gets stuck playing or to stop playing emotes.

The available commands are the following:

| Command                          | Parameters                                                                                                          | Action                                                            |
| -------------------------------- | ------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------- |
| `multiplayer_emotes`             | None                                                                                                                | List all the players that have this mod installed                 |
| `emote <value>`                  | `<value>`: A integer representing the animation id.                                                                 | Play the emote with the given id                                  |
| `emote_npc <name> <value>`       | `<name>`: A string representing the npc name.<br>`<value>`: A integer representing the animation id.                  | Force a npc to play the emote animation with the given id         |
| `emote_animal <name> <value>`    | `<name>`: A string representing the farm animal name.<br>`<value>`: A integer representing the animation id.          | Force a farm animal to play the emote animation with the given id |
| `stop_emote`                     | None                                                                                                                | Stop any playing emote                                            |
| `stop_all_emotes`                | None                                                                                                                | Stop any playing emote by other players                           |
