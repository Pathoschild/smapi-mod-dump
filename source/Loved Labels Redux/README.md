**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/thespbgamer/LovedLabelsRedux**

----

## Description and Features

**Loved Labels Redux** is a [Stardew Valley](http://stardewvalley.net/) mod based on [this mod](https://www.nexusmods.com/stardewvalley/mods/279) and it allows you to see if the animal was petted today or not.

## Contents

- [Description and Features](#description-and-features)
- [Contents](#contents)
- [Installation](#installation)
- [Configuration](#configuration)
  - [In-game settings](#in-game-settings)
  - [`config.json` file](#configjson-file)
- [Compatibility](#compatibility)
- [Changelog](#changelog)
- [See also](#see-also)

## Installation

1. [Install the latest version of SMAPI](https://smapi.io/).
2. Download the mod from [nexus mods](https://www.nexusmods.com/stardewvalley/mods/8880?tab=files) or from [github](https://github.com/thespbgamer/LovedLabelsRedux/releases/).
3. Unzip the mod folder into your Stardew Valley/Mods.
4. Run the game using SMAPI.

## Configuration

### In-game settings

If you have the [generic mod config menu](https://www.nexusmods.com/stardewvalley/mods/5098?tab=files) installed, the configuration process becomes much simpler, you can click the cog button (⚙) on the title screen or the "mod options" button at the bottom of
the in-game menu to configure the mod.

### `config.json` file

The mod creates a `config.json` file in its mod folder the first time you run it. You can open the file in a text editor like notepad to configure the mod.

Here's what you can change:

- Player controls:

  | Setting Name             | Default Value                                 | Description                              |
  | :----------------------- | :-------------------------------------------- | :--------------------------------------- |
  | `KeybindListToggleUIKey` | `LeftShift and OemPipe` aka `LeftShift and \` | The keybind that toggles the UI in-game. |

- String values:

  | Setting Name           | Default Value    | Description             |
  | :--------------------- | :--------------- | :---------------------- |
  | `AlreadyPettedMessage` | "Already Petted" | Message already petted. |
  | `NeedsPettingMessage`  | "Needs Petting"  | Message not petted.     |

- Other options:

  | Setting Name       | Default Value | Description                                                      |
  | :----------------- | :------------ | :--------------------------------------------------------------- |
  | `IsUIEnabled`      | `true`        | If true shows the UI, if false hides it.                         |
  | `IsPettingEnabled` | `false`       | If true then auto pets farm animals, if false does not auto pet. |

## Compatibility

Loved Labels Redux is compatible with Stardew Valley 1.5+ on Linux/Mac/Windows, both single-player, local co-op and multiplayer.

## Changelog

- [Full Changelog](https://github.com/thespbgamer/LovedLabelsRedux/blob/main/CHANGELOG.md#full-changelog)

## See also

- [Nexus mod](https://www.nexusmods.com/stardewvalley/mods/8880)
- [The mod that this mod was based by](https://www.nexusmods.com/stardewvalley/mods/279)
