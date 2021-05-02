**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/rikai/Grandfathers-Heirlooms**

----

# Grandfather's Heirlooms

This mod is a continuation of [Grandfather's Gift Remade](https://www.nexusmods.com/stardewvalley/mods/4151) with direct permission from pepoluan.

This mod is directly inspired by [Grandfather's Gift 1.0 on NexusMods](https://www.nexusmods.com/stardewvalley/mods/985), but since that mod is not open source, pepoluan had to totally remake it from scratch.

Only the ideas remain; the code, the logic, even the words, everything came from pepoluan's diabolical mind (with great help from friends at [The Stardew Valley Forums](https://community.playstarbound.com/forums/mods.215/)).

* Grandfather's Gift Remade is available on NexusMods [**here**](https://www.nexusmods.com/stardewvalley/mods/4151).
* Grandfather's Heirlooms is available on NexusMods [**here**](https://www.nexusmods.com/stardewvalley/mods/8557).

## Description

The mod is meant for use on a fresh save.

It Gives you an Elf Dagger on the 2nd Day of Spring, in Year 1 and a chest that the dagger was stored in to help make early game combat a bit less stressful.

## Requirements

* Stardew Valley 1.5 (latest)
* SMAPI 3.0.0 or newer

## Configuration

Some `config.json` knobs to fine tune the experience:

| Knob        | Purpose |
|-------------|---------|
|`traceLogging` | Set to `false` if you think the TRACE logging is too chatty, making it difficult to troubleshoot other mods |
|`directToChest`| If `false` (default), the weapon will go into inventory, and only placed in the chest if the inventory is full. |
|`triggerDay`   | The day when the mod will trigger. Limited to `2<=day<=28`. Season & year are locked to "spring" and 1, respectively |
|`weaponStats`  | You can modify the Elf Dagger stats here. Won't affect other weapons and/or events because, by default, you can NOT get the Elf Dagger |

## Internationalization 

Yes, however files are not there, yet. Feel free to write a translation of the default.json in the i18n directory and submit a pull request.

Also happy to accept pull requests that flesh out the implementation.

## Compatibility

* No known confliects.

## To Do / Future Plans?

* Yeah, I do have some plans to put more polish & finish to this mod.
* Instead of directly giving a chest, put a "package" with custom interaction (similar to the "starter parsnip" gift Lewis gave)
* Colorificize the chest
* Custom sprite for the Elf Dagger
* And finally, if there's interest, additional gifts! (I'll need the When + Where + What)

## Discussion?

Ping me (@rikai#6969) on the [Stardew Valley Discord](https://discord.com/invite/stardewvalley) in one of the modding channels!

## Other Things...

This mod is open source and available [on Github](https://github.com/rikai/Grandfathers-Heirlooms/)
