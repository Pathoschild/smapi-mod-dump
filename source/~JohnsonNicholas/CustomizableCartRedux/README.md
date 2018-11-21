##  Customizable Traveling Cart Redux

This mod is inspired by yyeahdude. It allows you to set the chances of the traveling cart appearing per day (by setting it from 0 to 1, for example, a 25% chance is .25), the items it contains, and how much it is. 

## Install Instructions
Unzip the archive into the Mods folder

## Multiplayer Behvaior
This mod will only work for the main player (i.e the host) in an MP game. 

## Changelog
1.3-beta.3
 - Fixed a duplicate bug
1.3-beta2
- Updated for SDV 1.3.16
1.3-beta1
- Updated the manifest
- small fix for the number of items not adding in the force spawns
- fix for issue with update preventing mod from updating the list
1.3
- Now includes 1.3 item conditions
- Will actually use SellItem extractions
- Updated for SDV 1.3 beta
- Adds a flag to use the vanilla max instead of modded items
1.2.2 
- Added API for allowing people to add items
1.2
- Added several config options to more control what inventory appears, improved handling of errors. In addition, it will now add items added via JsonAssets.
1.1.1
- fixes

## Current Config Options
To configure, open up `config.json` in your mod folder.

- Monday through SundayChance: sets the odds it appears that day of the week
- AppearOnlyAtStartOfSeason: Will appear only on day 1, regardless of any other settings.
- AppearOnlyatEndOfSeason: Will appear only on day 28, regardless of any other settings
- AppearOnlyatStartAndEndOfSeason: Will appear only day 1 and 28, regardless of any other settings
- AppearOnlyEveryOtherWeek: Will only appear on days 8-14 and 22-28 of the season
- AmountOfItems: default 12, but can be altered down or up to control how many items appear. Note: The mod will by default set any numbers less than 3 to 3.
- DisableDuplicates: This will prevent the mod from selecting duplicates.
- BlacklistedItems - These items will not appear in the cart. Note: This applies over any items added to AllowedItems.
- AllowedItems - These items will be permitted to appear in the cart. (This is primarily used to override the prohibited categories.)
- UseCheaperPricing: Uses a less expensive method of determining the value.

## Requires:
SMAPI 2.6-beta15+
SDV 1.3.16+

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