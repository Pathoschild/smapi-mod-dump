**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/NormanPCN/StardewValleyMods**

----

# Combat Controls Redux

## Credits
This Mod is an adaption of the work of DjStln (DJ-STLN), who created the original Combat Controls Mod.
That mod is available at https://www.nexusmods.com/stardewvalley/mods/2590

## Permissions

Combat Controls Redux is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

The Combat Controls Redux mod is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

I don't really care if you redistribute it or alter it or use it in compilations.
I'd ask that you give credit to myself and DJ-STLN, that's all.

Source code is available at
https://github.com/NormanPCN/StardewValleyMods/tree/main/CombatControlsRedux

## Config
config.json is available to edit and configure the Mods functions.  
After the first time the Mod is started it will create a default config.json file in the Mods folder.  
config.json is a simple text file and easy to edit. It might be hard to find the Mods folder for some.  

Combat Controls Redux also supports the Generic Mod Config Menu (GMCM) interface.
GMCM is a much easier way to configure Combat Controls Redux.
GMCM provides a graphical user interface to configure the Mod settings.
You can configure from the game title screen and/or during the game.
GMCM is available at https://www.nexusmods.com/stardewvalley/mods/5098
The NexusMods page shows the locations of the GMCM config button(s).

## Description

### Fixed Mouseclick. Character facing direction  
Config setting = MouseFix.  
Turns the character in the direction of your mouse click. This functions with Swords, Clubs, Daggers and Scythe. 
It also operates with the dagger special attack (right click).
Tools like the pickaxe operate as they normally do in the game.
The game's normal functions for turning a character with mouse clicks check what specific tile is clicked,
requiring that the exact tile next to the character is clicked in order to turn the character towards it.
This mod makes it so that clicking in the general direction of a tile is enough to turn the character.

### Regular Tools fix
Config Setting = RegularToolsFix.  
If Mouse is enabled then regular tools will get the facing direction fix. You may not be able to hit diagonal tiles with this enabled.

### Auto Swing  
Config setting = AutoSwing.  
Holding the Use Tool button down (left click) will continuously swing the weapon. For swords and clubs.

### Auto Swing Dagger  
Config setting = AutoSwingDagger.  
Holding the Use Tool button down (left click) will continuously attack with a dagger. Daggers attack very fast and auto swing can be very effective.

### Club Ground Slam Spam Attack
Config Setting = ClubSpecialSpamAttack
This option will perform a spam attack of LeftClick/UseTool on the Club special ground slam attack. You must do the LeftClick/UseTool once to start the spam attack.

### Slick Moves!  
Config setting = SlickMoves.  
When running and attacking to the sides, the character will slide along the ground while swinging the weapon.
For swords and clubs.

> ### Sword Secondary Attack Dash  
> Config setting = SwordSpecialSlickMove.  
> A quick dash while using the secondary attack (right click). Use by hitting the use tool button (left click) immediately after the secondary attack and moving in the direction you want to dash. You can use this to close distance while blocking, or dash across a gap during the block.
> Enabled for swords.

> ### Club Special Slick Move  
> Config setting = ClubSpecialSlickMove.  
> This allows the club special attack to do the sliding/dashing slick move. Use by hitting the use tool button (left click) immediately after the secondary attack (right click).
> One may not want the club special area attack to move the player during the attack. This can help you keep your distance from enemies. 

> ### Slide Velocity  
> Config setting = SlideVelocity  
> The movement velocity of the normal attack slick moves slide. Swords and clubs. Controls the speed and distance of the slide.

> ### Special Slide Velocity  
> Config setting = SpecialSlideVelocity  
> The movement velocity for special attack slick moves. Swords and clubs. Controls the speed and distance of the slide.

## Changlog
v1.4.0
* Updates for Stardew Valley 1.6 and SMAPI 4.

v1.3.0
* The controller fix option has been removed. This mod no longer looks at, uses, the controller cursor for facing direction changes during controller use.
* Some features connected to the mouse cursor fix are now independent of that fix/feature being enabled. e.g. Slick moves.
* With auto swing enabled, the character facing direction will now change with mouse movement during the auto swing.

v.1.2.5
* Fix for REALLY fast daggers and auto swing.
* Added an auto spam attack for the Club special ground slam attack. After the ground slam, you immediately click LeftClick and/or UseTool button once and this mod will spam UseTool a number of additional times for you. Be quick the allowed timeframe is narrow.

v1.2.4
* Fixes typos and syntax of the config option text. Adds a German translation.

v1.2.3
* Fixed a bug when the regular tools direction fix was enabled and the fishing rod was in use. Facing direction change is now disabled after the fishing line is in the water.

v1.2.2
* Fixed a bug where clicking a/the weapon with a supported tool selected/active while swimming or in swim clothes gave you the velocity of the slick moves slide.

v1.2.1
* Fixed a bug where clicking a/the weapon in the hotbar gave you the velocity of the slick moves slide.

v1.2.0
* Added a config option to allow the facing direction fix on regular tools. Pickaxe, Hoe, etc.

v1.1.1
* Fixed auto swing bug with fast weapons (+4) and one or more speed buffs.

v1.1.0
* Split screen support.  
* Added a config option for controller facing direction fix.  

v1.0.0:  
* Initial release. 
