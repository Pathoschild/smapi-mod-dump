**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/JohnsonNicholas/SDVMods**

----

##  Customizable Traveling Cart Redux
Version 1.4

This mod is inspired by yyeahdude. It allows you to set the chances of the traveling cart appearing per day (by setting it from 0 to 1, for example, a 25% chance is .25), the items it contains, and how much it is. 

## Requires:
SMAPI 3.0+
SDV 1.4+

## WARNING UPDATING TO 1.4
***The API has changed. Any mods using this should use the newer API***

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


## API documentation

This mod provides an API if people want to add items to the cart. 

The interface looks like:

    public interface ICustomizableCart
    {
        event EventHandler CartProcessingComplete;
        void AddItem(Item item, int price, int quality);
    }


***Important Note***: `AddItem` is meant for more advanced uses outside of the handler, but we strongly recommend you call it only with the handler.
The API defaults the last to 1, but it can be passed in if you want a different number. This interface is from 1.2.2+

In order to add items, subscribe to the CartProcessingComplete event, and put your logic for adding items there. 