# Climates of Ferngill (Rebuild) You Can (Not) Have Weather

Current Version (4 Feburary 2018): 1.3

## What's New

- Fog!
- Blizzards, Thundersnow
- A more customizable weather system
- More descriptive and varied weather reports

## Requirements

- SMAPI 2.3+
- Stardew Valley 1.2.33+
- PyTK 0.7.0+
 
## Overview

This mod does the following:

- Alters the weather via a custom method that can read custom files
- Adds in several custom weathers - Thundersnow! Blizzards! Dry Lightning!
- Changes the rain totem to occasionally spawm storms as well.
- Adds a moon overhead, which will act on the world
- Adds a weather menu option, which will display information about the weather
- Changes the text for the TV weather channel
- Going out in storms, blizzards, frosts and heatwaves is now more perilous, as it drains your stamina. Thankfully, 
    a 'Muscle Remedy' has been found to cure even the hardiest flu

## Stamina System

__Very Important: Read this before you alter StaminaDrain in the config!__

Rather than a fixed penalty for certain conditions, this now calculates a multiplier based on the conditions prevailing.

Every ten minutes, the mod checks to see if you've been outside for a certain percentage of the last 10 minutes. By default, it's set to 65%. (This percentage is calculated by counting the ticks you've been outside and the total number in the span to account for time change mods). Then, it generates a random number and tests it against a chance to get sick. By default, this is 70%. In addition, this must be during certain weather conditions:
*Temperature: Frost or Heatwave (as defined in the config file, see the readme for more information)
*Weathers: Lightning
*Special Weathers: Thundersnow, Blizzard
(NB: While you incur a stamina penalty for being sick in fog, it deliberately does not trigger this.)

The penalties are **cumulative** - that is, they add up to the final multiplier.
*Lightning : +100% ( 1)
*Thundersnow: +100% (1)
*Thundersnow (nighttime): +50% (.5)
*Foggy: +50% (.5)
*Foggy (nightime) +25% (.25)
*Blizzard: +125% (1.25)
*Blizzard: **White Out**+225% (2.25)
*Blizzard (nighttime) +50% (.5)
*Frost (nightime): +125% (1.25) - this is not during the winter. During winter, the frost penalty is untriggered.
*Heatwave (daytime): +125% (1.25)

The calculated number is then rounded __down__

For example, therefore, if you're outduring a storm, with the base of 2, you only take a stamina penalty of 2. But if it's also a heatwave, your penatly is now (+1+1.25)=*2.25 or 4.5. So a penalty of 4.
If you're out in a blizzard during the day, it's *1.25 or 2.5 rounded down to 2. If you're out in that blizzard at night, another .5 (1.25+.5) is added making it 1.75 or 3.5 rounded down to 3.

(This does mean a foggy blizzard at night is (+.5+.25+1.25+.5 or *2.25), and if you somehow get this in fall, would be *3.5. And somehow, if you get a whiteout, it would be *3.25 and *4.5!)

## Known Issues

## To Do

## Wishlist

## Acknowledgements
- eemie for the moon sprites
- Prismuth for the fog sprite
- Pathoschild for ideas
- ChefRude for testing and the night icon sprites (as well as better fog textures)

## Changelog
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

- 'TooColdOutside' - This controls the temperature required (the *low* temperature required) to trigger the Frost event. Note this is a Spring and Fall event, and will potentially kill crops
  Defaults to '-3'. Values are in Celsius (1 C is 33.8 F). 
  NOTE: Frosts trigger at dark

- 'TooHotOutside' - This controls the temperature required (the *high* temperature required) to trigger the Heatwave event.
  Defaults to '39'. Values are in Celsius (39 C is 102.2 F)
  NOTE : Heatwaves taper off at night.

- `SnowOnFall28` - If set to true, this will force snow fall and appropriate temperatures on Fall 28. Default: `false`,
  Valid: `true` or `false`

- `StormTotemChange` - Usage of the rain totem will now spawn storms with the same odds as spawning storms on 
   rainy days. Default: `true`. Valid: `true` or `false`.

- `Verbose` - This makes the mod noisy to provide a lot of debug options. Default: `false`. Valid: `true` or `false`.

- `AllowStormsSpringYear1` - Default: `false`. Normally, you can't get storms in Spring Y1. This keeps that valid. Valid: `true` or 
 `false`

- `DisplayBothScales` - Default: `false`. This will display both known scales. Set to `true`, if you want to see Farenheit as well.

- `HazardousWeather` - Default: `false`. This turns on hazardous weather. It's normally turned off. Right now, it only turns on the heatwave and frost events
	IMPORTANT NOTE: This only enables the stamina drain on them, and the dewatering of the heatwave. Frost's crop death will remain disabled, 
	as well not watering the plants in time for a heatwave.

- `AllowCropDeath` - Default: `false`. Normally, hazardous weather won't kill crops, just stop them growing. This reenables crop death.'

- 'AffectedOutside' - The percentage outside you need to be within a 10 minute span to be affected by stamina events.
 Defaults to '.65', valid values are between 0 and 1. To turn stamina drains off entirely, set it to 0. 

 - 'SickMoreThanOnce' - By default, the false means you can only get sick once a day. Set it to true to be affected by multiple colds.

 - 'StaminaDrain' - This is an int containing the default stamina drain for hazardous events. See the writeup (soon) for more information. Default is '2'. Valid range is any number between foo and bar.'

 - 'DeadCropPercentage' - The amount of crops that a heatwave and frost can kill. (Note: Frost will kill more than heatwaves). Default: '.1' Valid range is 0 to 1.

 - 'CropResistance' - This represents the resistance an average crop has to heatwaves and frosts. Default: '.4' Valid Range is 0 to 1.

 - 'DarkFogChance' - This controls the chance of the darker fog appearing. Default is set to '.0875' (or a 1/8th chance if it's foggy it'll be dark fog.) Valid Range is 0 to 1.

 - 'ChanceOfGettingSick' - Controls the chance you'll get sick when conditions are matched. Default is set to '.7' for (70% chance). Valid Range is 0 to 1.

 - 'Use12HourTime' - Tells it whether or not to use 12hour time or not in displays. Defaults to false. Valid: true, false

 - 'BadMoonRising' - Chance of a blood moon on a full moon. Default: .004 (.4%). Valid Range is 0 to 1.

 - 'EclipseOn' - Whether or not the eclipse is enabled. Defaults to true. (NOTE: Will not trigger until at least Spring 2, and must be a full moon.) (valid: true, false)

 - 'EclipseChance' - The chance of an eclipse every full moon. Defaults to .015 (1.5%) Valid Range is 0 to 1.

 - 'SpawnMonsters' - Controls if monsters spawn on your wilderness farm. Default: true. Valid: true, false

 - 'SpawnMonstersAllFarms' - Controls if monsters spawn on all farms. Default: false. Valid: true, false