**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/stoobinator/Stardew-Valley-Grace-Period**

----

# Crop Grace Period
**Crop Grace Period** is a mod that allows a grace period for already-planted crops to grow into the next season. For example, you could allow Spring crops to grow until the 15th of Summer. I created it so my weekly Stardew Valley group wouldn't lose their crops if they couldn't make it to a session where the seasons changed.

## Contents
* [Install](#install)
* [Configure](#configure)
* [Compatibility](#compatibility)
* [Special thanks](#special-thanks)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. [Install this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/7203).
3. Run the game using SMAPI.

## Configure
The mod creates a `config.json` file in its mod folder the first time you run it. You can open that
file in a text editor to configure the mod.

Here's what you can change:

setting              | default     | what it affects
:------------------- | :---------- | :------------------
`Spring`    | 28 | The number of days to allow spring crops to grow after spring ends.
`Summer`    | 28 | The number of days to allow summer crops to grow after summer ends.
`Fall`      | 0 | The number of days to allow fall crops to grow after fall ends.
`Winter`    | 0 | The number of days to allow winter crops to grow after winter ends.

## Compatibility
Compatible with Stardew Valley 1.4+ on Linux/Mac/Windows, both single-player and multiplayer. In
multiplayer mode, it must be installed by the main player to work correctly.

## Special thanks
This mod (including the Readme) owes a lot to Pathoschild's [Crops Anytime Anywhere](https://www.nexusmods.com/stardewvalley/mods/3000) mod!

## See also
* [Release notes](release-notes.md)
* [Nexus mod](https://www.nexusmods.com/stardewvalley/mods/7203)
