**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/JessebotX/StardewValleyMods**

----

# RELEASE NOTES
Health & Stamina Regeneration: [(nexus download)](https://www.nexusmods.com/stardewvalley/mods/3207)

## v2.0.2 (May 1, 2021)
- Support for local multiplayer ([#10](https://github.com/JessebotX/StardewValleyMods/pull/10) - [diocloid](https://github.com/diocloid)

## v2.0.1 (August 28, 2020)
- Integration with [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)
- Added 1 new configuration option (affects both Health and Stamina unless you have one disabled)
  - `IgnorePause` (default: false) will let you regen health/stamina even if your game is paused (cutscene playing, menu opened etc.)
- Remove `2.0.0` ALERT message

## v2.0.0 (May 27, 2019)
- Config overhaul, many more long-awaited options have been added
  - Includes: Configuring the seconds it takes in between heals and being able to make it work like a hunger mod.
  - Due to the config being overhauled, you must run the game once in order to get a new config. All your custom values will be reset so you would need to reconfigure it again
  - See [configuration document in README](README.md#configure)

## v1.0.2 (Feb 11, 2019)
- Possibly fixes the ```StaminaRegenRate``` field

## v1.0.1 (Jan 7, 2019)
- More customizable with ```StaminaRegenRate``` in the config.json
  - accepts decimal values (up to 3 decimal places (eg. ```1.555```))

## v1.0 (Jan 6, 2019)
- Initial Release :D
