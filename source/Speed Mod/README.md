**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Xebeth/StardewValley-SpeedMod**

----


## Overview
This mod adds a speed boost and the ability to teleport home at the cost of stamina. This could be considered cheating but I consider it more of QoL mod because of the built-in limitations.
#### Watch on YouTube
[![Speed Mod 1.1](https://img.youtube.com/vi/PtgfM0SvUS8/0.jpg)](https://www.youtube.com/watch?v=PtgfM0SvUS8 "Watch on YouTube")

## Changelog
#### Version 1.2
* Added an alternative method to detect configuration modifications if the [ModTabSettings](https://github.com/Xebeth/SVM/releases) integration fails
* Added support for interrupting the teleportation process (manually or through damage)
* The energy used by the teleportation is drained over time during the casting period
* The speed boost is disabled if the player is under the **tipsy** or **slimed** debuffs (both impact movement speed)
* Added a teleport effect matching the one from the totems
#### Version 1.01-beta 
* [ModTabSettings](https://github.com/Xebeth/SVM/releases) integration
#### Version 1.0 
* Initial release

## Configuration
Most aspects of the mod are configurable through the `config.json` file.  
  
___
#### SpeedModifier:
Value of the speed boost.  
*integer* [0-25] (default: **2**)  
___
#### TeleportHomeKey:
The key used to trigger the teleportation (displays a confirmation dialog).  
*keybind* [wiki reference](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings) (default: **H**)  
___
#### CanPlayerInterrupt: 💥NEW 1.2
Allows the player to cancel the teleportation if the activation key is pressed again.  
⚠ **Any spent energy is not recovered.**  
*boolean* **true** / **false** (default: **true**)  
___
#### CanDamageInterrupt: 💥NEW 1.2
Allows damage to interrupt the teleportation if the threshold is reached (See [**DamageThreshold**](#damagethreshold-new-12)).  
⚠ **Any spent energy is not recovered.**  
*boolean* **true** / **false** (default: **true**)  
___
#### DamageThresholdBasedOnTotalHealth: 💥NEW 1.2
Controls how the damage threshold is evaluated (See [**DamageThreshold**](#damagethreshold-new-12)):  
* if **true**, based on the *total health* (chance of interruption is linear: **easier**)
* if **false**, based on the *remaining health* (chance of interruption increases as health declines: **harder**).  

*boolean* **true** / **false** (default: **true**)  
___
#### DamageThreshold: 💥NEW 1.2
The amount of damage in % of the player's health that would interrupt the teleportation.  
*integer* [0-90] (default: **10**)  
___
#### StaminaCost:
The amount of stamina depleted when teleporting home.  
*integer* (default: **50**)  
___
#### RecastCooldown:
The time period for which the teleportation is on cooldown (also resets at the end of the day).  
*timespan* **HH:MM:SS** (default: **00:07:00**)  
___
#### CastCooldown: ⚠ Modified 1.2
The time in seconds it takes to perform the teleportation.  
**‼ You need to edit your `config.json` manually or delete it or the new version won't be able to load the configuration.**  
*integer* (default: **5**) ⚠ The type of the value was changed from *timespan* to *integer*.  
___
#### TeleportToBed: 💥NEW 1.2
Controls whether the player is teleported to their bed or outside the farm (same as the Return Scepter).  
*boolean* **true** / **false** (default: **true**)  
___
#### EnableTeleportationEffects: 💥NEW 1.2
Enables or disables the visual effects.  
*boolean* **true** / **false** (default: **true**)  
___
#### EnableTeleportationSounds: 💥NEW 1.2
Enables or disables the sound effects.  
*boolean* **true** / **false** (default: **true**)  
___
#### EnabledInMultiplayer:
Flag specifying if the teleportation feature is enabled in multiplayer.  
*boolean* **true** / **false** (default: **false**)  
___
## Example file:
```
{
  "SpeedModifier": 2,
  "TeleportHomeKey": "H",
  "CanPlayerInterrupt": true,
  "CanDamageInterrupt": true,
  "DamageThreshold": 15,
  "DamageThresholdBasedOnTotalHealth": true,
  "StaminaCost": 50,
  "RecastCooldown": "00:07:00",
  "CastCooldown": 3,
  "TeleportToBed": false,
  "EnableTeleportationEffects": true,
  "EnableTeleportationSounds": true,
  "EnabledInMultiplayer": true
}
```
