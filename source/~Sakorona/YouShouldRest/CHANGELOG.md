## Changelog

### 1.3.2
 - fixes an issue with shop replacers

### 1.3.1
- API now also gives more details about location on the arc.
- Blue Moons and Harvest Moons also affect prices
- Blue Moons and Harvest Moons do not have to be up to affect prices
- The prices set by LD are now multipliers, allowing other mods to also act on the prices
- The moonrise and set times are set against August 2006 in Seattle, WA.

### 1.3
- API updates for updating compatibility

### 1.2.1
- Removes unncessary debug spam

### 1.2
- This mod now requires: SpaceCore
- API expanded for plain text moon phase not tied to loc.
- CP tokens provided. See API documentation for both changes
- Blood Moons now check for their chance at 6am, as well, they rise at 6am
- Blood Moon debug message removed
- Every second full moon in a month is now a blue moon
- Supermoons added - random, and may have no effect. When it does, it doubles effect
- Harvest Moon makes crops 3.5 times likely to grow, and also may be a double growth.
- The Blood Moon text no longer replaces in cut scenes.
- Text fixes (#93)
- Eclipses should now: sync to MP farmhands, clear for MP farmhands (#90, #91)
- Eclipses will no longer occur on festival days. (#92)
- The code will once again remove slimes. NOTE: This includes escaped slimes. The only place your slimes are safe from auto-removal is the slime hutch! (#99)

### 1.1

- Blood moons change the moon in the HUD.
- Beach spawning odds changed to remove the overwhelming possiblity of a trilobyte spawning.
- Default odds for crop growth and death are reset to 1.5%
- Full Moons used to have only 1 chance per moon, and now have 3, occuring at 930, 950 and 1010 rather than once at 1650.
- Ghosts now spawn starting at dusk, rather than night on combat farms.
- Some streamlining of log spam
- Eclipses only spawn according to if the game would already on farms
- The option to remove displaying the moon has been removed now that it renders behind the menu.
- Now has compatibility with PriceDrops
- Eclipses happen on a new moon now.
- Crop death should properly happen now.
- Fixed duplicate moon issue on the 27th of the month
- Fixed monster spawn issue. They should properly spawn now.
- Blood moons now don't "set" during the day via automated message.
- Supports GMCM if installed

### 1.0.11

- Updated SDVUtilities base code. Should now not cause random crop death in Crops Anywhere when the two mods are used together.

### 1.0.10

- Moves some configuration options to config. I cannot believe I forgot that
- Updating readme.
- I think I fixed the moon thing... I hope. I hope. Cleans up some issues with resetting

### 1.0.8

- Times after midnight are properly formatted
- Feature to turn off moon display added
- Updated for SMAPI 3.0
- Moon no longer watches you break down your financial report.
- Some debug lines removed
- Updated the integration logger text
- i18n fixes for solar eclipse and phase names
- Blood Moon notification added
- Blood Moon fix
- Chinese translation added thanks to FarAways!

### 1.0.7

- Updated for SDV 1.3.32
- API updated for moon rise and set times
- Notification feature added for moon rise and set

### 1.0.6

- Updated for SDV 1.3.20

### 1.0.5

- Updated for SDV 1.3.16

### 1.0.4

- crash bug since 1.3 removed some farm cleanup code
- tweak for small screens

### 1.0.3

- fix for beta 11 of 1.3

### 1.0.2

- SDV 1.3 compatible

### 1.0.1

- fixed the moon
