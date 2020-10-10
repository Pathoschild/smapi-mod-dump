**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Mikesnorth/StardewNewsFeed**

----

# StardewNewsFeed
Morning Notifications for Stardew Valley

This Stardew Valley Mod inspects various locations every morning for harvestable items. Any locations with harvestable items found will generate a notification to the player.

## Nexus Mods Page
https://www.nexusmods.com/stardewvalley/mods/3206/

## Currently Supported Notifications (versions >= 1.7)
* Farm Cave has Mushrooms/Fruit
* Greenhouse has harvestable items
* Cellar has harvestable items
* Sheds have harvestable items
* Barn Animals have produce available
* Barn Auto-Grabber has items
* Coops have animal products available
* Silo's are below 15% Capacity
* Someone nearby has a birthday today

## Currently Supported Languages (version >= 1.6)
* English
* Spanish
* German
* French -> Thanks to https://github.com/VincentRoth 
* Portugese -> Thanks to https://github.com/kelvindules
* Korean -> Thanks to https://github.com/S2SKY
* Italian -> Thanks to https://github.com/sfal

## How to configure this mod
The install directory for this mod will contain a configuration file. ~/Mods/StardewNewsFeed/config.json

By default, scanning for items in the Farm Cave and the Greenhouse will be turned on. The game will need to be restarted after making adjustments to the configuration.

|Config Property|Description|Default Value|
|-|-|-|
|DebugMode|Prints debug info to the console|false|
|GreenhouseNotificationsEnabled|Enables/Disables scanning and notifications for the greenhouse|true|
|CaveNotificationsEnabled|Enables/Disables scanning and notifications for the farm cave|true|
|CellarNotificationsEnabled|Enables/Disables scanning and notifications for the cellar|true|
|ShedNotificationsEnabled|Enables/Disabled scanning and notifications for sheds|true|
|BirthdayCheckEnabled|Enables/Disables roaming birthday checks|true|
|CoopCheckEnabled|Enables/Disables checking coops for items|true|
|BarnCheckEnabled|Enables/Disables checking barns and barn animals for produce|true|
|SiloCheckEnabled|Enables/Disables checking current amount of hay compared to max capacity|true|

## Plans for additional future updates
A persistent checklist containing areas with harvestable items.
Translations for all languages supported by the Stardew Valley core game.
