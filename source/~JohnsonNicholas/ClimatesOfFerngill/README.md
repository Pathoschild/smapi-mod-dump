**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/JohnsonNicholas/SDVMods**

----

# Climates of Ferngill

Current Version: 1.5.12

## What's New
- Variable Rain.
- A more customizable weather system
- Weather Systems!
- Temperature variation chances

## NOTES
The German translation may be out of date. I apologize.

## Multiplayer
Supports MP, with most features locked out to main player. However, most weathers will render on farmhands screen.

## Requirements

- SMAPI 3.0+
- Stardew Valley 1.4+
- PyTK: latest
 
## Incompatibilities
 This mod is not compatible with More Rain or Weather Machine or Snow on Fall 28 (albeit, the last only on Fall 28). (i.e: weather mods do not work well together). 
 For people who may want to use Snow on Fall 28, there is a config option to set this mod to spawn snow on fall 28.

## Overview

This mod does the following:

- Alters the weather via a custom method that can read custom files
- Adds in several custom weathers - Thundersnow! Blizzards! Dry Lightning! Fog! Thunder Freeenzy!!!
- Changes the rain totem to occasionally spawn storms as well.
- Adds a weather menu option, which will display information about the weather
- Changes the text for the TV weather channel
- Adds Variable rain and custom debris sprites
- Adds weather systems!
- Adds current temperature displays where relevant

## MAJOR CHANGES
1.5 implements a full variable rain system. This means that on any day it rains, there are now chances for the rain to not remain constant throughout the day, or even chances for it to become overcast with no rain. 

In addition, it adds a new feature: weather systems.  Weather systems are written to be biased towards sunny days for the first 10 days of spring. 

## ENHANCED v NORMAL

Enhanced adds in debris in seasons not present in vanilla weather, as well as other factors. 

NOTE: 
- If you need to trigger a sunny day for the community center event (especially important if you use enhanced_monsoon or monsoon), I recommend using the command world_tmrwweather sun in the SMAPI console.

## IMPORTANT NOTE
If you have a custom climate, you'll need to update it to match the changes to climates in 1.5 (and move it to assets/climates/)

## Notices

This mod uses harmony to patch functions to allow it to draw custom rain and snow colors.

## Acknowledgements
- fog textures orginally by Prismuth, updated ones by ChefRude, and then again by Hadi
- Pathoschild for ideas
- jahangmar for the German translation
- Sky for the Korean translation
- ChefRude for testing and the night icon sprites (as well as newer better fog textures)
- Pan for the new base debris sprites
- FoggyWizard for the sandstorm sprite
- FarAways for the Chinese translation
- mouse and minerva for the testing
- Amu Tsukiyomi for all of the debris sprites, and several icon sprites
- jahangmar for a bug fix re sync
- floatingatoll for a bug fix re fog flashes
- kdau and Strobe for testing
- Corrin, Hadi and Mae for suggestions
- The SDV discord #modding channel for discussions and encouragement.
- Kaya for debuff/buff icon sprites


If I have left you out of acknowledgements, please contact me so I can get you added to the acknowledgement list


## Config Options

- Temperature is now controlled by the three settings
'ShowBothScales' (false/true, default: false),  'FirstTempScale' and 'SecondTempScale' 

The last two can be configured to be any of "Farenheit", "Kelvin", "Rankine", "Delisle", "Romer", "Reaumur", with the first defaulting to Kraggs (Which is functionally identical to Celsius.)

- `ShowLighterFog` - Changes the fog opacity to be lower. Default is `false`. Valid values: `false, true`

- 'EnableCustomWeatherIcon' - This enables the custom weather icons. Default is `true`. Valid values are `false` and `true`

- `ClimateType` - set to weather that has a corresponding file in `assets\weather\`. Packaged with the mod is
`normal`, `enhanced`, `arid`, `dry`, `wet`, `monsoon`, `enhanced_wet`, `enhanced_arid`, `enhanced_monsoon`, `enhanced_dry`. Enhanced and normal variants also have a 'foggy' and `lowfog` version Default: `enhanced`.

- `DryLightning` - This controls the odds of dry lightning (Custom weather available during any clear day, as long as the temperature is 
over a certain value.). Valid 0-1, but it's recommended that this is kept low. Default: `.1` (10%)

- `DryLightningMinTemp` - This controls the minimum temperature to trigger the DryLightning event. 
  Defaults to `34`. Values are in Celsius. (34 C is 93.2 F)

- `TooColdOutside` - This controls the temperature required (the *low* temperature required) to trigger the Frost event. Note this is a Spring and Fall event, and will potentially kill crops
  Defaults to '-3'. Values are in Celsius (1 C is 33.8 F). 
  NOTE: Frosts trigger at dark

- `TooHotOutside` - This controls the temperature required (the *high* temperature required) to trigger the Heatwave event.
  Defaults to '39'. Values are in Celsius (39 C is 102.2 F)
  NOTE : Heatwaves taper off at night.

- `DarkenLightInFog` - If set to true, will darken the window when foggy. Set it to false if you want to disable this behavior if the flashes are a problem.

- `SnowOnFall28` - If set to true, this will force snow fall and appropriate temperatures on Fall 28. Default: `false`,
  Valid: `true` or `false`

- `StormTotemChange` - Usage of the rain totem will now spawn storms with the same odds as spawning storms on 
   rainy days. Default: `true`. Valid: `true` or `false`.

- `Verbose` - This makes the mod noisy to provide a lot of debug options. Default: `false`. Valid: `true` or `false`.

- `AllowStormsSpringYear1` - Default: `false`. Normally, you can't get storms in Spring Y1. This keeps that valid. Valid: `true` or 
 `false`

- `DisplayBothScales` - Default: `false`. This will display both known scales. Set to `true`, if you want to see Farenheit as well.

- `HazardousWeather` - Default: `false`. This turns on hazardous weather. It's normally turned off. Right now, it only turns on the heatwave and frost events
	IMPORTANT NOTE: This only enables dewatering of the heatwave AND the thunder frenzy event. Frost's crop death will remain disabled, 
	as well not watering the plants in time for a heatwave.

- `AllowCropDeath` - Default: `false`. Normally, hazardous weather won't kill crops, just stop them growing. This reenables crop death.'

 - `DeadCropPercentage` - The amount of crops that a heatwave and frost can kill. (Note: Frost will kill more than heatwaves). Default: '.1' Valid range is 0 to 1.

 - `CropResistance` - This represents the resistance an average crop has to heatwaves, frosts and sandstorms. Default: '.4' Valid Range is 0 to 1.

 - 'ThunderFrenzyOdds' - This controls the chance of the special weather Thunder Frenzy appearing. It's a double, valid range is 0 to 1

 - `DynamicRain` - Allows for the mod to dynamically control rain during the day. Defaults to true. Valid: true, false

 - `DisableHighRainWind` - If your computer has issues with high rain or wind, set this to true. Defaults to false.

 - `SetDefaultScaleToF` - This sets the default scale used to Farenheit. NOTE: DisplayBothScales will override this. Defaults to false. This setting will also invalidate DisplayCelsisuInsteadOfKraggs
 
 - `DisplayCelsiusInsteadOfKraggs` - Will display Celsius/C instead of Kraggs. NOTE: SetDefaultScaleToF will override this setting.

 - 'VariableRainChance' - The chance the rain will be variable instead of constant. Default is 27.5% (.275). Valid range is 0 to 1.

 - 'OvercastChance' - The chance the rain will be overcast instead of variable. Default is 5%. (.05). Valid range is 0 to 1. NOTE: This triggers off it being variable. (So the default is .01735) (1.735%)

 - 'VRChangeChance' - The chance that the rain amount will change on the 30 minute step. 

 - 'VRStepPercent' - The step percentage it applies (may be up or down)
  
 - 'VRMassiveStepChance' - The chance of a massive change (category of rain)

 - 'SandstormsInDesertOnly' - As it says on the tin.

 - 'DisableAllFog' - As it says on the tin.

 - 'ShowSummitClouds' - Controls if rain clouds appear in the summit. NOTE: Will not trigger if the mod detects Summit Reborn is installed.

 - 'RainToSnowConversion' - If cold enough, and in fall or spring, this is the chance rain will become snow.

 - 'MoreSevereThunderFrenzyOdds' - longer storms. 
