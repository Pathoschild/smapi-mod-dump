# Release notes

[← back to readme](readme.md)

## 2.1.0

Released 27 November 2019.

* Update for Stardew Valley 1.4.

## 2.0.1

Released 25 June 2019.

* Updated for Stardew Valley 1.3.36 and SMAPI 3.0.

## 2.0.0

Released 05 October 2018.

* Updated for StardewValley 1.3.

## 1.0.3

Released 04 October 2017.

* Updated for SMAPI 2.0.

## 1.0.2

Released 07 May 2017.

* Fixed quoting issue on SMAPI parameters.
* Fixed issues with cleaning up old mail.
* Clean up mail now filters correctly to yesterday.
* Fixed issue where some local mail wasn't being deleted.
* Added debug logging for mail delivery and cleanup.

## 1.0.1

Released 29 April 2017.

* Fixed bug with returning to tile screen.
* Fixed bug with friend command extra spaces.
* Fixed version number.

## 1.0

Released 29 April 2017.

* **Renamed from "Send Letters" to "Send Items" for clarity.**
* Moved local storage into LiteDb (stores data as binary json in the mod folder).
* Made all web requests run in the background so they don't interfere with the game.
* Mail is now only delivered overnight.
* Added more commands to the console to manage friends.
* Added ability to use a postbox to send items instead of the letterbox (future development to add a postbox to Pierre's shop).
* Fixed bug with file storage locking.

## 0.2

Released 28 April 2017.

* Updated readme.

## 0.1.4

Released 21 April 2017.

* Added console commands to manage online friends.
* Fixed letter delivery end time from 4pm to 6pm to match advertised times.
* Fixed players seeing messages they sent to others.
* Fixed sending a letter with no item crashing the game.

## 0.1.3

Released 19 April 2017.

* Initial version.
* Added support for sending items to your other saves.
* Added support for sending items to online friends. (Currently requires adding them to the json file.)
* Added UI notification when a letter is sent.
* Filtered what you can send in a letter.
* Mail is now only delivered between 8am and 6pm on the hour.
* Stability fixes.
