# StardewValleyEasyPrairieKing

This is a mod for Stardew Valley which adds cheat options to the minigame *Journey of the Prairie King*.

You can find the original mod source code [here](https://github.com/mucchan/sv-mod-prairie-king)



I just updated it to work with Stardew Valley 1.4

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/5816).
3. Run the game using SMAPI.

## Compatibility
* Should work with Stardew Valley 1.4 on Linux/Mac/Windows. (Only tested on Linux)

### Requirements

Stardew Modding API (SMAPI) 3.3.2 or above.


### Configuration

A *config.json* file will be generated the first time you run this mod. This file will look like this:

```
{
  "always_invincible_": false,
  "lives_": 98,
  "coins_": 100,
  "ammo_level_": 0,
  "bullet_damage_": 0,
  "fire_speed_level_": 5,
  "run_speed_level_": 2,
  "spread_pistol_": false
}
```

To enable always_invincible or spread_pistol(shoot 3 bullets at once) set it to true.
To disable always_invincible or spread_pistol(shoot 3 bullets at once) set it to true.

To enable the feature(s) you want, just set those values bigger than 0.
To disable the feature(s) you don't want, just set those features to 0.

To set your **lives** or **coins** to **infinite** set it to **100**.