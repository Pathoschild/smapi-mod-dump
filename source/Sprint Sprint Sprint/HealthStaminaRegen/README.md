# Health Stamina Regen
[(download from nexus)](https://www.nexusmods.com/stardewvalley/mods/3207)

Health and stamina regenerates overtime. Highly Configurable.

## Contents
- [Features](#features)
- [Compatibility](#compatibility)
- [Install](#install)
- [Configure](#configure)
  - [Notes](#notes)
  - [config.json](#configjson)
- [See Also](#see-also)

## Features
- Health and stamina regenerates overtime
- If you take damage/lose health, health regeneration goes on cooldown
- If you use stamina, stamina regeneration goes on cooldown
- Highly configurable. See [configuration documentation](#configure)
- 2 Console Commands:
  - ```healthstaminaregen_confighelp```: displays a ```config.json``` guide in the console
  - ```healthstaminaregen_debuglogging```: spams log with debug info such as health and stamina being regenerated
  - type ```help``` in the SMAPI console for more info

## Compatibility
- Works with SMAPI 3.6+
- Works in Singleplayer and Multiplayer
  - Only works for the person who has is installed it
- No known mod conflicts	

## Install
_**Installation**_
1. Download [SMAPI (3.6 or later)](https://www.nexusmods.com/stardewvalley/mods/2400)
2. Extract/Unzip this mod into your ```Stardew Valley/Mods``` folder

**Uninstallation**
- Move this mod out of the ```Stardew Valley/Mods``` folder

See [Player's Guide to Downloading and Using Mods](https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started) if you are new to using mods.

## Configure
##### Notes 
1. *if you don't see a ```config.json``` in the ```HealthStaminaRegen``` folder, run the game once with mod installed for it to generate.*
2. *since version 2.0.0, a config overhaul happened, all your previous custom values have been reset. You may go into the ```config.json``` and re add the values again.* 

##### config.json
- Categories: `Health` and `Stamina`
- Subcategories: see table below

| **Option** | **Accepted Values** | **Description** | **Default Value** |
| ---------- | ------------------- | --------------- | ----------------- |
| Enabled | ```true```, ```false``` | if health or stamina regen (depending on which category you are editing) is enabled or not | ```true``` |
| HealthPerRegenRate / StaminaPerRegenRate | a number (for stamina, you can use up to 3 decimal places, no decimal places for health ) | The amount of health/stamina you receive per specified RegenRateInSeconds | ```2``` |
| RegenRateInSeconds | a number (between 1 to 4,294,967,295 (0 may break it, not sure)) | how many seconds in between regenerating | ```1``` |
| SecondsUntilRegenWhenTakenDamage / SecondsUntilRegenWhenUsedStamina | a number (between 0 to 2,147,483,647 (not sure what will happen below 0)) | the duration of the cooldown for regen when consumed health/stamina, set it to 0 if you don't want a regen cooldown | ```3``` |
| DontCheckConditions | ```true```, ```false``` | Keep regenerating regardless if it goes past maxstamina/health, ignores SecondsUntilRegen... etc. (for the people who want to have it decrease health and stamina like a hunger mod) | ```false``` |

Global

| **Option** | **Accepted Values** | **Description** | **Default Value** |
| ---------- | ------------------- | --------------- | ----------------- |
| IgnorePause | ```true```, ```false``` | keep regenerating even if the player is paused (i.e. menu opened, cutscene playing etc.) | ```false``` |


## See Also
- [My other mods](https://www.nexusmods.com/users/55529772?tab=user+files)
- [Release Notes](changelog.md)