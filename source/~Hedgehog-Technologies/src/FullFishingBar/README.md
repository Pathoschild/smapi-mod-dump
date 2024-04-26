**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Hedgehog-Technologies/StardewMods**

----

**Full Fishing Bar** is an open-source mod for [Stardew Valley](https://stardewvalley.net) that allows players to trivialize the fishing mini game.

![](https://i.imgur.com/03calCX.gif)

## Documentation
### Overview
When enabled, this mod will set the fishing mini game's "Bobber Bar" to take up the entire possible range.

You may be asking yourself, "why in the world would I want this when there are instant catch mods?"
Potential answers:
- Accessibility without totally breaking balance
  - Playing the fishing mini game can be very difficult for some players for reasons unintended by the original design
  - This mod helps to maintain some amount of balance without completely blowing out the fish economy
- Instant catch fishing takes a ridiculously small amount of time in-game
  - This mod allows you to take the normal amount of time that fishing would take out of a day
- Very obvious in multiplayer server if you are using instant catch mods
  - This mod helps you be a bit more covert about your fishing, allowing you to still appear to be fishing normally
  - You are still able to cancel the fishing mini game early to give the appearance of failing the mini game

### Config
- `IsEnabled`
  - Whether or not the mod will function and resize the fishing mini game's bobber bar
  - *Default:* `true`
- `OnlyCorkBobber`
  - Whether or not the Bobber Bar resizing only works when a Cork Bobber is equipped on the current fishing rod
  - *Default:* `false`
- `BarSizePercentage`
  - What percentage of the mini game play space do you want the Bobber Bar to take up?
  - *Default:* `1.0`
  - *Range:* `0.1` - `1.0`
- `ExceptBossFish`
  - Whether or not the mod will **skip** resizing the fishing mini game's bobber bar **for Boss Fish**
  - *Default:* `false`

### Translation
&nbsp;     | No Translation  | Partial Translation  | Full Translation  | Translated By
:--------: | :-------------: | :------------------: | :---------------: | :------------:
Chinese    | ✔              | ❌                   | ❌                | n/a
French     | ✔              | ❌                   | ❌                | n/a
German     | ✔              | ❌                   | ❌                | n/a
Hungarian  | ✔              | ❌                   | ❌                | n/a
Italian    | ✔              | ❌                   | ❌                | n/a
Japanese   | ✔              | ❌                   | ❌                | n/a
Korean     | ✔              | ❌                   | ❌                | n/a
Polish     | ✔              | ❌                   | ❌                | n/a
Portuguese | ✔              | ❌                   | ❌                | n/a
Russian    | ✔              | ❌                   | ❌                | n/a
Spanish    | ✔              | ❌                   | ❌                | n/a
Thai       | ✔              | ❌                   | ❌                | n/a
Turkish    | ✔              | ❌                   | ❌                | n/a
Ukrainian  | ✔              | ❌                   | ❌                | n/a

### Install
1. Install the latest version of [SMAPI](https://smapi.io)
  1. [Nexus Mirror](https://www.nexusmods.com/stardewvalley/mods/2400)
  2. [CurseForge Mirror](https://www.curseforge.com/stardewvalley/utility/smapi)
  3. [GitHub Mirror](https://github.com/Pathoschild/SMAPI/releases)
2. *Optional* Install the latest version of [Generic Mod Config Menu](https://spacechase0.com/mods/stardew-valley/generic-mod-config-menu/)
  1. [Nexus Mirror](https://www.nexusmods.com/stardewvalley/mods/5098)
  2. [CurseForge Mirror](https://www.curseforge.com/stardewvalley/mods/generic-mod-config-menu)
3. Install **this mod** by unzipping the mod folder into `Stardew Valley/mods`
4. Launch the game use SMAPI

### Compatibility
- Compatible with...
  - Stardew Valley 1.6 or later
  - SMAPI 4.0.0 or later
- No known mod conflicts
  - If you find one, please feel free to notify me on Github, Nexus, or Curseforge

## Limitations
### Solo + Multiplayer
- This mod is player specific, each player that wants to utilize it must have it installed

## Releases
Releases can be found on [GitHub](https://github.com/Hedgehog-Technologies/StardewMods/releases), on the [Nexus Mod](https://www.nexusmods.com/stardewvalley/mods/23006) site, and on the [CurseForge](https://www.curseforge.com/stardewvalley/mods/full-fishing-bar) site.
### 1.1.0
- Add "Only Cork Bobber" config option
### 1.0.0
- Initial Release
