**TimeSpeed** is a [Stardew Valley](http://stardewvalley.net/) mod which lets you control the flow
of time in the game: speed it up, slow it down, or freeze it altogether. This can happen
automatically or when you press a key in the game.

Compatible with Stardew Valley 1.2+ on Linux, Mac, and Windows.

## Contents
* [Install](#install)
* [Use](#use)
* [Versions](#versions)
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

The mod creates a `config.json` file the first time you run it. You can open the file in a text
editor to configure the mod:

setting | effect
:------ | :-----
`DefaultTickLength` | The default number of seconds per 10-game-minutes, or `null` to freeze time globally. The game uses 7 seconds by default.
`TickLengthsByLocation` | The number of seconds per 10-game-minutes (or `null` to freeze time) for each location. You can use location names or these values: `Mines`, `Indoors`, or `Outdoors`.
`EnableOnFestivalDays` | Whether to change tick length on festival days.
`FreezeTimeAt` | The time at which to freeze time everywhere (or `null` to disable this). This should be 24-hour military time (e.g. 800 for 8am, 1600 for 8pm, etc). (Be careful the number doesn't start with a zero, due to a bug in the underlying parser.)
`LocationNotify` | Whether to show a message about the time settings when you enter a location.
`Keys` | The keyboard/controller/mouse bindings used to control the flow of time. See [available keys](https://stardewvalleywiki.com/Modding:Key_bindings#Available_bindings). Set a key to `null` to disable it.

## Compatibility
* Works with Stardew Valley 1.3 on Linux/Mac/Windows.
* Works in single-player, and in multiplayer if you're the main player. (The mod will disable itself if you're not the main player, so it's safe to keep installed.)
* No known mod conflicts.

## See also
* [Release notes](release-notes.md)
* [Nexus mod](https://www.nexusmods.com/stardewvalley/mods/169)
* [Discussion thread](https://community.playstarbound.com/threads/storm-and-smapi-timespeed-mod-configurable-day-lengths.107876/)
