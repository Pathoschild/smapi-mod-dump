**Skull Cave Saver** is a [Stardew Valley](http://stardewvalley.net/) mod which saves your progress
in the Skull Cave every 5th floor (configurable). When you return, it will warp you to the saved
level.

## Contents
* [Install](#install)
* [Use](#use)
* [Compatibility](#compatibility)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://smapi.io).
2. Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/175).
3. Run the game using SMAPI.

## Use
Just install the mod and play the game. It will save your Skull Cave level and warp you back to it
automatically.

The mod creates a `config/<save id>.json` file the first time you run it. You can open the file in a text
editor to configure the mod:

setting | effect
:------ | :-----
`SaveLevelEveryXFloors` | The multiple of floors at which to save (e.g. `5` to save every fifth floor).
`LastMineLevel` | The last saved Skull Cave floor. This is updated automatically, but you can set it manually to skip to that floor.

## Compatibility
* Works with Stardew Valley 1.3 beta on Linux/Mac/Windows.
* Works in single-player and multiplayer.
* No known mod conflicts.

## See also
* [Release notes](release-notes.md)
* [Nexus mod](https://www.nexusmods.com/stardewvalley/mods/175)
* [Discussion thread](https://community.playstarbound.com/threads/smapi-skullcavesaver.111429/)
