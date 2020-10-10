**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Sakorona/SDVMods**

----

## Changelog
### 1.5.12
 - some api improvements, blinding fog!!
 - weather now renders in screenshot mode.

### 1.5.11
 - fix for seasonal limits. (patch-fix)

### 1.5.10
 - fix on typo of key
 - added key for radio for languages to force a new line. This key is '[NLK]'

### 1.5.8
 - Updated for SMAPI 3.4.1 (reverts a fix)

### 1.5.7
- Fixes to a bad descriptor (dry thunder)
- A lot of experimental work (temporarily disabled)
- Some console command fixes
- It can apparently rain and snow at the same time. Fixed. (#100)
- Fix for Linux/Mac users with fog loading (#104)

### 1.5.6
- Resolve improperly set categories causing errors on displaying descriptions (#85)
- Spacing issues are fixed when retrieving rain category descriptors
- Resolve Night Market being the wrong days (#88)
- Thanks to floatingatoll, fix for white flashes (#89)

### 1.5.5
- The system will *properly* respect the config option for max rain
- Default max rain is now 11k. 
- TV forecast updated to look a bit better and flow more. In addition, the hazardous warning works properly now (Resolves issue 77)
- The feature blocking egress in sufficiently heavy rain has been disabled (Resolves issue 76, in one way..)
- Hazardous Weather will now block Torrential rain as well properly. (Resolves issue 75)
- NRE fixed when farmhands attempt to perform checks on non loaded materials. (Resolves issue 64.)
- System is readjusted to make high amounts of rain rarer... again. (Resolves issue 73.)
- Resolved a typo in the description for weather (#83)
- Rain amount increases only appear when the category changes (#80)
- Rain amount popup now uses units properly (#79)
- Some config settings that were unused were removed (#31, cont.)
- internal tweaks to make the code base more legiible

### 1.5.4
 - Fixes to rainfall amount display to be more proper.
 - The weather menu shouldn't display text off the edge of the screen.
 - Improved integration messages that should be easier to read, and makes it clear they're optional features
 - A TV translation was missing.
 - The variable rain checks will more aggressively try to decrease rain amounts in torrential rainfall
 - The check stopping you transitioning will not trigger in events.
 - Rainfall blocking egress will no longer occur if the HazardousWeather flag is off.

### 1.5.3
- added German translation thanks to jahangmar 
- Fixes for the violent rain feature not letting you go from outside area to outside area to get inside
- Fixes for the failsafe not properly triggering at 11pm.
- Fixes for the extremely violent rain happening too often.
- Some internal refactoring
- It will now properly tell you it is going to storm, instead of snow, when it is going to storm. 

### 1.5.2
- some fixes for a NRE issue

### 1.5.1
 - some fixes for the climate tracker not being created properly if null
 - some fixes for unset weather still cropping up when it really shouldn't
 
### 1.5
- Updated for current SMAPI/SDV
- API updated to add new functionality
- Translation file updated to no longer have unused strings
- Some internal tweaks and refactors
- Snow is now generated for the next day in advance, allowing it to be predictible.
- Fixes to write temperature to the save and read, allowing for consistency over reloading games.
- Fixes to make temperature generation less prone to error
- Climates now displays a "current temperature"
- Weather popup menu updated for new features
- Severe rain is now.. considered severe rainfall. No, I don't know why I did this either.
- Weather Systems now properly clear day count so that there might be more than 2 a save file.
- TV forecast revamped. Now will advice if fog is likely (>60% chance)
- API updated to provide climate name
- White out odds are no longer hard coded
- Blizzards odds are now set per climate. Heat/chill waves are configerable per span and no longer hard coded. 
- Lighter fog option should make fog less dark
- If the community center hasn't been opened, the chances of rain and debris are decreased.
- Thundersnow works differently now - it's with the storm multiplier. Note that the default chances are .0006% times snow. This is delibverate.
- Climate files overhauled
- Weather changes update music playing
- Variable rain doesn't display messages in the desert
- Odds more properly favor not going up to high numbers as easily
- Fixes to the icons not updating under certain circumstances when weather changes
- Weather systems don't run forever now.
- Variable rain doesn't display messages on saving.
- Variable Rain will show popups when it changes (currently set to 20%) or a category changes.
- Fixes a potential issue with internal rain calculation
- Variable Rain - highly severe rain blocks egress to outside areas
- New Feature: Weather Systems!
- Fix for why rain to snow conversions weren't working
- Variable Rain (multiple fixes since last beta, including rain accumulating when you sleep like it should.)
- Rain heavier in storms by default.
- Variable rain more biased to starting with heavier rain in storms.
- The window no longer darkens in fog
- Overcast added and fixed some issues
- Fixed an issue with wet climates (wind chance 200%!!!)
- Added in enhanced versions of arid,dry,wet and monsoon
- Dry already has nonvanilla weathers, as such, the new dry is.. purely vanilla weathers only in regards to wind.
- Climate files moved to assets/climates/
- temperature display simplified in an effort to allow for a better variety of options.
- The default weather for Climates is now 'enhanced'.
- Reorganized and refactored some code
- Adjusted chances of rain adjusting
- messages no longer show up in the desert
- Variable rain won't display messages indicating rain fall when you sleep
- Weather systems no longer have a +1400% chance to run. :|
- Variable Rain will show popups when it changes (currently set to 20%) or a category changes.
- Fixes a potential issue with internal rain calculation
- Severe rain blocks egrees to going outside
- Removed all watering code.
- Variable rain fixes some more (for watering issues)
- overcast fixes to work properly 
- fix for a badly coded check of rain
- Variable Rain may now start with a lower level of rain to start with.
- Bug fix added to the sync that should finally kill the get low error.
- Fix for Farenheit display
- Manifest name fix for better log parsing
- Lighter Fog option now further lightens the fog in rain
- An option was included to display fog in the desert
- Frosts now apply in Winter except for crops that are meant to grow in winter, or if the crop is a winter forgable. This can be turned off in the config
- Variable Rain now will alter if crops are watered until a certain threshold (the vanilla amount) is reached. This can be turned off in the config
- Overcast code will not show up as precipitation.
- updated bundled SDVUtilities to use code that should work better in some locations
- removes some debug code that made it silent hill, all the time

v1.5-beta6

- fixes to fog
- moon text restored if Lunar Disturbances is installed

v1.5-beta3

- Fixes to white outs and blizzards
- Letting the days go by, living underwater~

v1.5-beta2

- Variable Rain will no longer have a 1/3 in chance of becoming torrential every 30 minutes, so less invisible flash flooding.
  -- The chance of ramping up massively is now halved every step above 200 raindrops (vanilla is 70)
- adds in a config option to disable all fog
- Moves snow check for spring (and extends it to fall) in front of other special weathers
- Chance of rain -> snow conversion is now a config option
- Sandstorms will now kill crops if the option is enabled
- Crop death warnings will say how many crops are affected
- If Summit Reborn isn't installed, it will enable code to allow rain rendering on the summit map.
- Fixed MP sync issues with fog
- Some small fixes to all weathers
- added more thunder frenzy options
- fixed a thundersnow and wedding issue

v1.5

- MP sync added in. 
- Custom sprites added, adding in summer wind to the game (21%(-8,+6), (19%(-8,+6), 23%(-4%+8%))
- Thunder Frenzy weather improperly not clearing out on day reset causing issues with weather display has been fixed.
- Variable Rain! Overcast weather and variable rain fall has been added.
- Sandstorms now spawn in windy weather when it hasn't rained for a while
- New config option for Farenheit: "SetScaletoF"
- New config option to control cursor redrawing: "RedrawCursor"

v1.4.2

- removed extra Harmony debug line
- Corrected issue with checking the TV before the wedding tomorrow (badly formatted line)

v1.4.1

- Updated for SDV 1.3.32
- Updated for newest LunarDisturbances API
- Updated TV lines.

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
