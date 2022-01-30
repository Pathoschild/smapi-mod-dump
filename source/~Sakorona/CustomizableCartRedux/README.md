**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Sakorona/SDVMods**

----

##  Customizable Traveling Cart Redux
Version 1.4

This mod is inspired by yyeahdude. It allows you to set the chances of the traveling cart appearing per day (by setting it from 0 to 1, for example, a 25% chance is .25), the items it contains, and how much it is. 

## Requires:
SMAPI 3.13+
SDV 1.5+

## INCOMPATIBLITES
- Ambient Light
- Casual Life

## WARNING UPDATING TO 1.4.5
***The API will not work - this is due to a temporary rewriting of the mod.


## Install Instructions
Unzip the archive into the Mods folder

## Multiplayer Behvaior
This mod will only work for the main player (i.e the host) in an MP game. 

## Current Config Options
To configure, open up `config.json` in your mod folder.

- Monday through SundayChance: sets the odds it appears that day of the week
- AppearOnlyAtStartOfSeason: Will appear only on day 1, regardless of any other settings.
- AppearOnlyatEndOfSeason: Will appear only on day 28, regardless of any other settings
- AppearOnlyatStartAndEndOfSeason: Will appear only day 1 and 28, regardless of any other settings
- AppearOnlyEveryOtherWeek: Will only appear on days 8-14 and 22-28 of the season
- Use Vanilla Max: The vanilla game stops looking for items at 790. Setting this to false allows PPJA assets
- Opening Time: The time the cart opens in the morning
- Closing Time: The time the cart closes in the evening. Use 2400 notation.
- AmountOfItems: default 12, but can be altered down or up to control how many items appear. Note: The mod will by default set any numbers less than 3 to 3.
- BlacklistedItems - These items will not appear in the cart. Note: This applies over any items added to AllowedItems.
- AllowedItems - These items will be permitted to appear in the cart. (This is primarily used to override the prohibited categories.)
- UseCheaperPricing: Uses a less expensive method of determining the value.


