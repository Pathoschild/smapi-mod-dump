**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/cantorsdust/StardewMods**

----

**TimeSpeed** is a [Stardew Valley](http://stardewvalley.net/) mod which lets you control the flow
of time in the game: speed it up, slow it down, or freeze it altogether. This can happen
automatically or when you press a key in the game.

Compatible with Stardew Valley 1.2+ on Linux, Mac, and Windows.

## Contents
* [Install](#install)
* [Use](#use)
* [Configure](#configure)
* [Compatibility](#compatibility)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://smapi.io).
2. Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/169).
3. Run the game using SMAPI.

## Use
You can press these keys in-game (configurable):

key | effect
:-- | :-----
`N` | Freeze or unfreeze time. Freezing time will stay in effect until you unfreeze it; unfreezing time will stay in effect until you enter a new location with time settings.
`,` | Speed up time by one second per 10-game-minutes. Combine with Control to decrease by 100 seconds, Shift to decrease by 10 seconds, or Alt to decrease by 0.1 seconds.
`.` | Slow down time by one second per 10-game-minutes. Combine with `Control` to increase by 100 seconds, `Shift` to increase by 10 seconds, or `Alt` to increase by 0.1 seconds.
`B` | Reload the config settings from file.

## Configure
The mod creates a `config.json` file the first time you run it. You can open the file in a text
editor to configure the mod:

setting | effect
:------ | :-----
`DefaultTickLength` | The default number of seconds per 10-game-minutes, or `null` to freeze time globally. The game uses 7 seconds by default.
`TickLengthsByLocation` | The number of seconds per 10-game-minutes (or `null` to freeze time) for each location. You can use location names, predefined areas (`DeepWoods`, `Mine`, `SkullCavern`, `VolcanoDungeon`), or location types (`Indoors` or `Outdoors`).
`EnableOnFestivalDays` | Whether to change tick length on festival days. Default true.
`FreezeTimeAt` | The time at which to freeze time everywhere (or `null` to disable this). The game uses 26-hour time internally, from 600 (6am) to 2600 (2am after midnight). Be careful the number doesn't start with a zero, due to a bug in the underlying parser. Defaults to disabled.
`LocationNotify` | Whether to show a message about the time settings when you enter a location. Default false.
`Keys` | The keyboard/controller/mouse bindings used to control the flow of time. See [available keys](https://stardewvalleywiki.com/Modding:Key_bindings#Available_bindings). Set a key to `"None"` (including the quotes) to disable it.

## Compatibility
* Works with Stardew Valley 1.5 on Linux/Mac/Windows.
* Works in single-player, multiplayer, and split-screen mode. In multiplayer you must be the main player. (The mod will disable itself if you're not the main player, so it's safe to keep installed.)
* No known mod conflicts.

## See also
* [Release notes](release-notes.md)
* [Nexus mod](https://www.nexusmods.com/stardewvalley/mods/169)
