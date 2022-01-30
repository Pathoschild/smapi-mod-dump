**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/JohnsonNicholas/SDVMods**

----

## Changelog

### 1.3.9
- No longer runs DNT in areas installed by SpaceChase0's Moon Misadventures Mod

### 1.3.8
 - Island set to 30 degrees south (or north) of the valley. This difference is customizable
 - Fixes 1.5 issue introduced by parallax calculations assuming only sunset changes on the hour
 - CoF integration temporarily disabled

### 1.3.7
 - Updated for SDV 1.5

### 1.3.6
 - fix to dawn icon timing
 - less orange sunrise is now more orange sunrise

### 1.3.5
 - fixes an error-catch block that could cause slowdowns

### 1.3.4
 - fixes a null bug that seems to occur randomly when location or music is null
 - we've redone the curves. Again. Color changes, as well.

### 1.3.3
 - More bug fixing

### 1.3.2
 - Fixes a reversion bug

### 1.3.1
 - DNT should be more resilliant to Climates throwing stuff it doesn't understand
 - DNT now has a less-orange sunrise color option
 - Some smoother light curves when it rains
 
### 1.3
 - In the morning, the music will now update as the sun rises
 - Assorted fixes to various elements
 - The sun rising has smoother lighting changes.
 - Improved compatiblity with Lunar Disturbances
 - The sun rising has been overhauled, and works more properly now
 - More light levels have been created to allow even darker nights, and the formula will properly get darker in a more intuitive manner
 - Contains support for Generic Mod Config Menu, if installed.
 - Updated to be compatible with Climates of Ferngill 1.5

### 1.2.8
 - redid the formula to avoid overflow and underflow issues.

### 1.2.7
  - refined the night curve
  - added darker night option 

#### 1.2.6
 - further changes
 
#### 1.2.5
 - fixes to the transition at night

#### 1.2.1
 - Added in a lunar light
 - Updated for the newest lunar disturbances and stardew valley
 - fixed an edge case

#### 1.2 
 - restructured the change so it flows better in morning and night
 - fixed the flash issues
 - has a full day cycle

#### 1.2-beta.1
 - updated morning darkness to be more obvious
 - the spouse's room now properly responds to morning darkness
 - indoor locations now properly respond too.

#### 1.1-rc4
 - updated for SDV 1.3.16

#### 1.1-rc3
 - refined algoryhtmns for calcs
 - set night time to naval twilight
 - added API so other mods can get the times used

#### 1.1-rc2
 - some bug fixes
 - the morning will now properly appear

#### 1.1-rc1
- Some changes to resync the night progression so it doesn't get dark for hours before turning to 'night'
- Updated to Harmony 1.1.0

#### 1.0.5
 - sunset starts at -30 minutes now. This avoids some issues, but can be configured to turn off.
 - more agressive progression
 - now 1.3 compatible

#### 1.0.4
 - refactor check
 - fix in the underlying library for time managment that should properly estimate times

#### 1.0.3
 - Fixed NRE on critter check.

#### 1.0.2
 - Added update key