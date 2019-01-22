# Lunar Disturbances

## Current Version: 
1.0.8 (Last Update: 1 Jan 2019)

## Requirements
- SMAPI 2.10+
- Stardew Valley 1.3.32+

## This Mod Does:
- Adds a moon overhead, which will act on the world
- Adds a blood moon!

## Multiplayer
Works only for the host player.

## Blood Moon

A blood moon is a rare event in Pelican Town. It only appears to happen 1 in 67 times on a full moon, and 1 in 800 times on a waning or waxing gibbeous moon. During this event, you can spot it by the red fog and red water. In addition, shopkeepers are often more anxious and drive harder bargins. Outside, monsters roam.

Thankfully, they don't appear to spawn on wedding and festival days..

-- sale prices down 20%
-- buy prices up 85%
-- monsters spawn every 10 real-world seconds if you are outside.

## Changelog
1.0.8
- Times after midnight are properly formatted
- Feature to turn off moon display added
- Updated for SMAPI 3.0
- Moon no longer watches you break down your financial report.
- Some debug lines removed
- Updated the integration logger text
- i18n fixes for solar eclipse and phase names
- Blood Moon notification added
- Chinese translation added thanks to FarAways!

1.0.7
- Updated for SDV 1.3.32
- API updated for moon rise and set times
- Notification feature added for moon rise and set

1.0.6
 - Updated for SDV 1.3.20

1.0.5
 - Updated for SDV 1.3.16

1.0.4
- crash bug since 1.3 removed some farm cleanup code
- tweak for small screens

1.0.3
- fix for beta 11 of 1.3

1.0.2
- sdv 1.3 compatible

1.0.1
- fixed the moon

## Config Options
To configure, edit config.json in the mod folder.

 - `BadMoonRising` - Chance of a blood moon on a full moon. Default: .004 (.4%). Valid Range is 0 to 1.

 - `EclipseOn` - Whether or not the eclipse is enabled. Defaults to true. (NOTE: Will not trigger until at least Spring 2, and must be a full moon.) (valid: true, false)

 - `EclipseChance` - The chance of an eclipse every full moon. Defaults to .015 (1.5%) Valid Range is 0 to 1.

 - `SpawnMonsters` - Controls if monsters spawn on your wilderness farm. Default: true. Valid: true, false

 - `SpawnMonstersAllFarms` - Controls if monsters spawn on all farms. Default: false. Valid: true, false

  - `HazardousMoonEvents` - Turns on or off moon events that hinder the player. Default: false. Valid: true, false.

  -`ShowMoonPhase` - Provide HUD messages about the moon rising and setting. Default: true. Valid: true, false

  -`ShowMoonInEndOfNight` - Shows the moon in the end of night menus. Default: true. Valid: true, false

### Acknowledgements
- eemie for the moon sprites
- ChefRude for testing