# Climates of Ferngill (Rebuild) You Can (Not) Have Weather

Current Version (19 November 2018): 1.4.1

## What's New

- Fog!
- Blizzards, Thundersnow
- A more customizable weather system
- More descriptive and varied weather reports
- Thundersnow!

## Multiplayer

Currently doesn't do any custom weathers in MP. Works only for the host player in MP. Will be changing shortly with the new MP API.

## Requirements

- SMAPI 2.8+
- Stardew Valley 1.3.32+
- PyTK: latest
 
## Overview

This mod does the following:

- Alters the weather via a custom method that can read custom files
- Adds in several custom weathers - Thundersnow! Blizzards! Dry Lightning! Fog! Thunder Freeenzy!!!
- Changes the rain totem to occasionally spawn storms as well.
- Adds a weather menu option, which will display information about the weather
- Changes the text for the TV weather channel 

## Notices

This mod uses harmony to patch functions to allow it to draw custom rain and snow colors.

## Upcoming Features

- Sandstorms
- Variable rain

## Acknowledgements
- Prismuth for the fog sprite
- Pathoschild for ideas
- ChefRude for testing and the night icon sprites (as well as better fog textures)
- FoggyWizard for the sandstorm sprite
- The SDV discord #modding channel for discussions and encouragement.


## Changelog
v1.4.1
 - Updated for SDV 1.3.32
 - Updated for newest LunarDisturbances API

v1.4
 - lighter fog implemented
 - option to disable the weather icon
 - red snow!
 - Now uses harmony for some various features

v1.4.0-beta13
 - blood weather during a blood moon
 - fixed a bug blocking many icons from being properly seen. Fixed now

v1.4.0-beta12
 - API expansion
 - Fix for thundersnow + fog with no frost
 - updated to be coompatible with SDV 1.3.20+

v1.4.0-beta11
 - updated to be compatible with SDV 1.3.16

v1.4.0-beta10
 - enabled dynamic night time integration

v1.4.0-beta9
- some fixes for MP. Well, a lot of fixes for MP
- readjusted fog. If you want the old foggy amounts in spring, add "foggy" to enhanced and normal.
- readjusted night fog calculations.

v1.4.0-beta4
- fixed console command issues

v1.4.0-beta3
- fixed some API integrations

v1.4.0-beta2
- Removed unneeded options, cleaned up the readme
- Added Thunder Frenzy weather
- Whiteouts will respect hazardous weather flags
- some refactoring

v1.4.0-beta1
- Mod split into three - moon is now Lunar Disturbances, stamina is now WeatherIllnesses.
- fix for the weather icon in Climates.

v1.3.4
- Blood Moon implementation (see README notes)
- Refactored the weather object to be easier to debug and extend
- Fixed issue with stamina clearer not requiring drinking the item.
- Updated and expanded some of the dialogue
- Dry lightning will no longer happen period when it's frost conditions
- After consideration, wind will still restrict fog from spawning.
- Flag to stop the light from changing (if the stutter bothers you)

v1.3.3
- Harvey's 8-heart scene gives the temp scale. C now is Kraggs, the in-game scale.
- Dual lightning strikes removed (was erroneously triggering additional lightning on stormy daysaw)
- Apparently I never actually wrote in the code to spawn ghosts. :| Due to engine limitations, occurs only on wilderness farms.

v1.3.2
- the moon also disappears now when the rest of the shipping menu fades
- hazardous moon events can be turned off.

v1.3.1
- fixed the evening fog. Forgot to remove the debug line.

v1.3
- custom popup text on first and last day of the year

v1.3beta8
 - updated descriptors for (sunny, at night)
 - updated evening fog descriptor to not show the fog beginning once started
 - added special descriptors for the easter egg.
 - some differnation done on current descriptors
 - fixed some issues lingering with fog lines not spacing correctly
 - refined some of the text to read more cleanly
 - added conversion chance for snow in spring if it's cold enough

v1.3beta7
 - Updated the console command code to be more.. proper.
 - Added a easter egg blizzard mode.

v1.3beta6
 - ISDVWeather now implements a EndWeather 
 - Console commands can now arbitarily trigger blizzards
 - Fog code cleaned up to remove old code
 - Sprites cleaned up to remove old sprite code.
 - Festival text fixes

v1.3beta5
- fog text fixes (remove duplicate fog text, evening fog properly display)

v1.3beta4
- text fixes (fog Time not appearing, temperature string incomplete)
- modified the menu screen to work properly over fog.

v1.3beta3
- .. somehow the text fixes didn't take?
- cleaned up various elements of the code - no longer attempts to IAssetEditor
- fixes the spring wind icon not appearing on the TV.

v1.3beta2
- text fixes: spring wind may not display properly on the TV
- fix for the mouse not properly redrawing

v1.3beta1
- merged in the solar eclipse mod.
- text fixes
- added in weather popup
- solar eclipse will now only trigger on full moons and will prohibit fog formation.
- updated manifest to require PyTK to prevent any issues with it not loading before this mod.

v1.3alpha41
- fixes tv icon
- text fixes

-v1.3alpha40
- reimplemented the text for the TV.
- fixed the heatwave icons
- fixed stamina issue with getting sick more than once
- fixed a bug where frost kills didn't respect allow crop death

v1.3alpha29
- moved to PyTK 
- thanks to ChefRude's new neight icons, we now have night icons!
- refined the mod's definition of night to be TrulyDarkTime
- made the stamina drain mesages more descriptive to the reason induced
- fixed some weather status icons to properly appear

v1.3alpha28
 - evening fog will now respect the fact it's windy and stay away
 - some of the debug output updated to be more useful

v1.3alpha27
- The weather icon is working, and the pointer properly draws over it now
- fixed the cold repeating itself
- fog fade in/out is slowed down
- easter egg removed

v1.3alpha3
- easter egg added - the desert has snow in real time Dec 15 til Jan 4.
- a new fade in/out method of fog has been added, and the lighting system should work normally

v1.3alpha2
- Architecture change - now uses an interface to streamline adding new weathers to draw mechanics
- Fog can now also be at night also. 

v1.2rc2-2 (v1.3-beta1)
- refactoring
- began push to improve TV text
- new fog texture
- now reuses CustomTV to make sure it's compatible with stuff like DailyNews
- #%@%@%@@@@%@%@@%@%@

v1.2rc2 (v1.3-beta1)
- fog is now properly dark and either way, the fog fades over time.
- fall climates in normal and enhanced from Fall 19 to Fall 28 produced way too much fog
- frosts now get full resistance on crops
- the frost death message will now tell you how many died
- stamina system overhauled that you won't get sick from something you can't suffer from events at the time.
- bad moon rising: blood moons will appear.
- new sprites, thanks to eemie, that more closely match the vanilla ones!
- lunar events now have a 20% chance to not trigger
- stamina system now factors in luck and a few other things.

v1.1.12p5
 - overhauled stamina system to correct an odd error. See writeup for more details.
 - updated the console command to actually update the internal tracker. :v
 - possiblity of all day fog (.1%) added to fog time

v1.1.12p4
 - the hud overdraw code is now disabled on festival days
 - the hud overdraw code that was unneccesary was removed
 - it should properly not darken on inventory menu (and others, by using the right event)
 - error fixed on normal and enhanced climates during the second week of spring
 - corrected an incorrect filter causing wind to never be added

v1.1.12p3 
- Text tweaks to make it flow properly
- The TV and popup will have some lines about fog now
- The popup will scroll.
- The probability of dark fog will be lowered to 8.75% and configurable in the settings option. It will also now default to having day 1 not having dark fog (probably hard coded to prevent option bloat)
- Cleaned up some of the code, removed some debug spam
- Rain Totems override chance isn't just the first one now, although that means if you get it to set a Storm totem, the next use might override it..
- fixed issue where festival name would never appear in the popup, and the wrong text was called for the TV.
- fixed issue where summer drylightning tried to call for thundersnow (!)
- added fog icons to the weather hud, as well as a blizzard one
- the window will be dark while it is foggy outside
- fixed a path display error in Linux
- fixed the path being capitlization inspecific
- the mouse will draw over the icon again, and the weather icon darkens properly
  when menus draw.
 - No longer darkens for dialogue when it shouldn't.
- fixed fog and snow drawing over hud code.

v1.1.12p2
- removed fog testing code.

v1.1.12p1
- replaced the totem detection code to make it a bit more tolerant of fault

v1.1.12 beta
- fixed an issue with festivals
- added stamina drains back in
- heatwaves and frosts are back in, and now it triggers during certain times.

## Config Options

- `ShowLighterFog` - Changes the fog opacity to be lower. Default is `false`. Valid values: `false, true`

- 'EnableCustomWeatherIcon' - This enables the custom weather icons. Default is `true`. Valid values are `false` and `true`

- `ClimateType` - set to weather that has a corresponding file in `data\weather\`. Packaged with the mod is
`normal`, `extended`, `arid`, `dry`, `wet`, `monsoon`. Default: `normal`.

- `ThundersnowOdds` - This controls the odds of thundersnow. (Custom weather available during the snow.) Valid: 0-1, but it's 
recommended that this is kept low. Default: `.001` (.1%)

- `BlizzardOdds` - This controls the odds of blizzards (Custom weather available during the snow.). Valid 0-1, but it's 
recommended that this is kept low. Default: `.08` (8%)

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

 - `CropResistance` - This represents the resistance an average crop has to heatwaves and frosts. Default: '.4' Valid Range is 0 to 1.

 - `DarkFogChance` - This controls the chance of the darker fog appearing. Default is set to '.0875' (or a 1/8th chance if it's foggy it'll be dark fog.) Valid Range is 0 to 1.

 - 'ThunderFrenzyOdds' - This controls the chance of the special weather Thunder Frenzy appearing. It's a double, valid range is 0 to 1

 - `Use12HourTime` - Tells it whether or not to use 12hour time or not in displays. Defaults to false. Valid: true, false

 - `DynamicRain` - Allows for the mod to dynamically control rain during the day. Defaults to true. Valid: true, false

 - `DisableHighRainWind` - If your computer has issues with high rain or wind, set this to true. Defaults to false.