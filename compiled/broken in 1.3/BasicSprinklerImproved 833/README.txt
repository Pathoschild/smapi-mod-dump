1. INTRODUCTION

Basic Sprinkler Improved changes the way the basic sprinkler works without making it cover any more area than it does in the base game. I always found the most annoying thing about it to be the shape, which was just useless. But while there are a number of fine mods out there that change the way the sprinkler works, they all make it cover more total area, which seemed a bit overpowered to me. So I made this for my personal use.

It's hoped that in addition to making the basic sprinkler more useful, the different patterns one can make with them will give them a niche so they can still be useful even when you get improved sprinklers.

2. CONFIGURATION

The mod works by watering tiles according to one of the following configurations. (The X represents the sprinkler and the Os the tiles it waters while each * represents an unwatered tile.)

horizontal: 

*****
*****
OOXOO
*****
*****

vertical:

**O**
**O**
**X**
**O**
**O**

north:

**O**
**O**
**O**
**O**
**X**

south:

**X**
**O**
**O**
**O**
**O**

east:

*****
*****
XOOOO
*****
*****

west:

*****
*****
OOOOX
*****
*****

The config.json file by default makes the mod use the "horizontal" configuration. To change that, change the "patternType" item. The valid types are: "horizontal", "vertical", "north", "south", "east", "west", and "custom", which deserves more explanation.

The four items in the config.json called "northArea", "southArea", "eastArea", and "westArea" define possible alternative configurations. You need to set the "patternType" to "custom" for this to work. Then you need to provide values. Whatever value you put in the appropriate direction is how many tiles it will water in that direction. However, it won't let you water more than 4 tiles total.

For example, with this configuration:

"patternType": "custom",
"northArea": 1,
"southArea": 1,
"eastArea": 2,
"westArea": 0
  
The sprinkler will produce this pattern:

*****
**O**
**XOO
**O**
*****

If you don't set the "patternType" to "custom" then the numbers will be ignored entirely.

Changes to config.json will only take effect after restarting the game.

3. KNOWN ISSUES

*Can't do anything with the sprinkler's animation to make it match how it actually works, I'm afraid.
*There is a possibility that a basic sprinkler may "unwater" parts of the pattern of another kind of sprinkler. I plan on fixing this eventually, but for now, as a workaround, don't put basic sprinklers within or adjacent to the watering area of an improved or iridium sprinkler.

4. FUTURE PLANS

*Depending on how crazy I get I might want to make in-game config menus or something. Probably not though.
*Something I would like to do is make it more dynamic in handling config changes. Currently, it only applies changes when you restart the game.

5. CREDITS

I'll admit that I studied closely the work of OrSpeeder (maker of Configurable Improved Sprinklers) and ADoby (maker of Simple Sprinkler). The method overall is my own but without their pioneering work, I probably wouldn't have been able to make heads or tales of anything. If you want a different take on changing up how sprinklers work, definitely check them out.

Configurable Improved Sprinklers: http://community.playstarbound.com/threads/configurable-improved-sprinklers-scarecrow-and-sprinklers-area-highlights.112443/
Simple Sprinkler: http://community.playstarbound.com/threads/smapi-simple-sprinkler.110326/

6. LICENSE

Basic Sprinkler Improved is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

For information on this license, see: <http://www.gnu.org/licenses/>

Basic Sprinkler Improved mod is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.













