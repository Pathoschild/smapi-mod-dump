**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/andyruwruw/stardew-valley-pet-bowl-sprinklers**

----

<p align="center">
  <img src="https://raw.githubusercontent.com/andyruwruw/stardew-valley-pet-bowl-sprinklers/main/documentation/cover.jpg">
</p>

<p align="center">
  <a href="https://www.nexusmods.com/stardewvalley/mods/8167">Download on Nexus</a>
  ·
  <a href="https://github.com/andyruwruw/stardew-valley-pet-bowl-sprinklers/issues/new/choose">Submit an Issue</a>
  ·
  <a href="https://www.nexusmods.com/stardewvalley/mods/2009/?tab=description">Inspiration</a>
</p>

<p align="center">
  A bunch of various controls for the pet bowl in Stardew Valley.
</p>

# PetBowlSprinklers

**PetBowlSprinklers** is a [Stardew Valley](https://www.stardewvalley.net/) mod that lets you change a few parameters around the Pet Bowl to make your life easier.

This has been created before with the mod [Pet Water Bowl](https://www.nexusmods.com/stardewvalley/mods/2009/?tab=description) by Mizzion. I just added a few more features I wanted like number of days the bowl stays filled.

Fully configurable, so you can make it cheaty, or just less of a pain.

## Features

- *Enable sprinklers* to fill the bowl automatically.
- Change the number of *days the bowl stays filled*.
- Allow *snow to fill the bowl*.

See [configuration](#configuration) for more details.

# Configuration

Here's a list of the configuration options available. Once run, a `config.json` will be created in the mod folder. If you have [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098), you'll be able to change any of the settings in game!

| Name | Description | Type | Default |
|------|-------------|------|---------|
| `SprinklersFillBowls` | Should sprinklers be able to fill pet bowls. | `boolean` | `true` |
| `ForceExactBowlTile` | Does the sprinkler need to fill the exact bowl tile? *(Pet Bowl building is 2x2 tiles, only one of the tiles is the actual bowl)* | `boolean` | `true` |
| `BowlFilledDuration` | How many days should the bowl stay filled? | `number` | `1` |
| `SnowFillsBowl` | Should snow fill the pet bowl? | `boolean` | `false` |
| `CheatyWatering` | Cheater cheater pumpkin eater, bowl will always be filled. | `boolean` | `false` |

# Install

1. Install the latest version of [SMAPI](https://smapi.io/).
2. Download this mod and unzip the contents.
3. Place the mod in your Mods folder.
4. Run the game using SMAPI.
