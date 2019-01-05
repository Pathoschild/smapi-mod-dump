# StardewNewsFeed
Morning Notifications for Stardew Valley

This Stardew Valley Mod inspects various locations every morning for harvestable items. Any locations with harvestable items found will generate a notification to the player.

## Nexus Mods Page
https://www.nexusmods.com/stardewvalley/mods/3206/

## Currently Supported Locations (version >= 1.1)
* Farm Cave Notifications
* Greenhouse Notifications
* Cellar Notifications
* Shed Notifications

## Additional Features (version >= 1.5)
As you move around the world. Each time you move to a new location, it will check all NPC's in your new location for birthdays, and display a notification if one is found.

## How to configure this mod
The install directory for this mod will contain a configuration file. ~/Mods/StardewNewsFeed/config.json

By default, scanning for items in the Farm Cave and the Greenhouse will be turned on. The game will need to be restarted after making adjustments to the configuration.

|Config Property|Description|Default Value|
|-|-|-|
|DebugMode|Prints debug info to the console|false|
|GreenhouseNotificationsEnabled|Enables/Disables scanning and notifications for the greenhouse|true|
|CaveNotificationsEnabled|Enables/Disables scanning and notifications for the farm cave|true|
|CellarNotificationsEnabled|Enables/Disables scanning and notifications for the cellar|false|
|ShedNotificationsEnabled|Enables/Disabled scanning and notifications for sheds|false|
|BirthdayCheckEnabled|Enables/Disables roaming birthday checks|false|

## Plans for additional future updates
Notifications for npc birthdays
A persistent checklist containing areas with harvestable items
