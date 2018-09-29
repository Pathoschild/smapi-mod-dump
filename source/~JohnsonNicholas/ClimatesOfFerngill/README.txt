# Climates of Ferngill (Rebuild) You Can (Not) Have Weather

Current Version (4 November 2017): v1.2 rc1

## What's New

- Fog!
- Blizzards, Thundersnow
- A more customizable weather system
 
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

The penalties are cumulative - that is, they add up to the final multiplier.
*Lightning : +100% ( 1)
*Thundersnow: +100% (1)
*Thundersnow (nighttime): +50% (.5)
*Foggy: +50% (.5)
*Foggy (nightime) +25% (.25)
*Blizzard: +125% (1.25)
*Blizzard (nighttime) +50% (.5)
*Frost (nightime): +125% (1.25) - this is not during the winter. During winter, the frost penalty is untriggered.
*Heatwave (daytime): +125% (1.25)

The calculated number is then rounded __down__

For example, therefore, if you're outduring a storm, with the base of 2, you only take a stamina penalty of 2. But if it's also a heatwave, your penatly is now (+1+1.25)=*2.25 or 4.5. So a penalty of 4.
If you're out in a blizzard during the day, it's *1.25 or 2.5 rounded down to 2. If you're out in that blizzard at night, another .5 (1.25+.5) is added making it 1.75 or 3.5 rounded down to 3.

(This does mean a foggy blizzard at night is (+.5+.25+1.25+.5 or *2.25), and if you somehow get this in fall, would be *3.5)

## Known Issues

## To Do

## Wishlist

## Changelog
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

##Requirements

- Stardew Valley: 1.2.33+
- SMAPI: 1.15.4+ 

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

 - 'CropResistance' - This represents the resistance an averagecrop has to heatwaves. Frosts have half this resistance. Default: '.4' Valid Range is 0 to 1.

 - 'DarkFogChance' - This controls the chance of the darker fog appearing. Default is set to '.0875' (or a 1/8th chance if it's foggy it'll be dark fog.) Valid Range is 0 to 1.

 - 'ChanceOfGettingSick' - Controls the chance you'll get sick when conditions are matched. Default is set to '.7' for (70% chance). Valid Range is 0 to 1.