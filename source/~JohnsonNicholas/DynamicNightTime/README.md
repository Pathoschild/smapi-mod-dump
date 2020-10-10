**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/JohnsonNicholas/SDVMods**

----

# Dynamic Night Time

## Requirements
- SMAPI 3.0+
- Stardew Valley 1.4+

## This Mod Does:
- Creates a more dynamic night time and sunrise lighting system. Does not alter wakeup time.

## IMPORTANT NOTE

Make sure that if you have problems with other mods that you are running their latest version.  Also, this mod does not alter wake up time. 

NOTE: DNT does some assumption work lighting after sunset is consistent. This also may not match your tick timing if you use time speed mods.
NOTE2: This mod is incomaptible with Deep Woods prior to 1.2-beta.18 / 1.3-beta.15

## Multiplayer Compatibility
This mod is MP compatible, and all clients will need it for it to work.

## Installation Instructions
- Unzip the folder into your Mods/ folder after making sure SMAPI is installed

## Troubleshooting Notes

* This mod dynamically generates your cycle based on latitude, and then clamps it to the nearest 10  hours. This does means that changes that are less than 3 minutes will not be visible. This is largely due to the nature of the 10 minute clock in Stardew Valley.

* This mod uses Harmony, a tool that allows for rewriting of game functions during runtime: This mod alters three functions in Game1 - isNightOut, GameClock, getStartingToGetDarkTime, getTrulyDarkTime. They will still return the same variables and generally the same intended use.

## Light Level Notes
The higher the number, the darker the night. Defaults to 1.

## Options Config
To configure, open up `config.json` in your mod folder.

-`SunsetTimesAreMinusThirty` - Valid is true or false. This applies a half hour corrective to the generated sunset.

- `latitude` - Valid is anything from -64 to 64. (- for lats S, + for lats N.). Sets the latitude that will determine sunrise and sunset times. Out of range latitudes will be reset. To find the latitude for your city, please consult google. Note that if you are in south latitude, you will get your sunrise/sunset times but not your climate.

- `NightDarknessLevel` - Set values from 1-4, as you go higher, it gets darker.

- `MoreOrangeSunrise` - Uses a more orange sunrise in the morning
