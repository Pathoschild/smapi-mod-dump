[← back to readme](readme.md)

# Release notes
## 2.0.0
* Updated for StardewValley 1.3

## 1.0.3
* Updated for SMAPI 2.0

## 1.0.2
* Fixed quoting issue on smapi parameters
* Fixed issues with cleaning up old mail
* Cleanup mail now filters correctly to yesterday
* Fixed issue where some local mail wasn't being deleted
* Added debug logging for mail delivery and cleanup

## 1.0.1-Beta
* Fixed bug with returning to tile screen
* Fixed bug with friend command extra spaces (issue #27)
* Bumped version because of bad release

## 1.0-Beta
* **Renamed project from "SendLetters" to "Send Items" for clarity**
* Moved local storage into LiteDb (stores data as binary json in the \Mods\SendItems directory)
* Made all web requests run in the background so they don't interfere with the game
* Mail is now only delivered overnight
* Added more commands to the consolse to help manage friends
* Added ability to use a postbox to send items instead of the letterbox (future development to add a postbox to Pierre's shop)
* Fixed bug with file storage locking

## 0.1.4
* Added commands to manage online friends (sendletters_me, sendletters_friends, sendletters_addfriend, sendletters_removefriend)
* Fixed letter deliver end time from 4pm to 6pm to match advertised timeframes
* Fixed bug where player would see messages they sent to others
* Fixed bug where sending a letter with no item would crash the game

## 0.1.3
* Sending items to your other farms (saved games) works just fine. If you add your online friends to the json file, you can send to them too but right now that's a bit manual.
* Added UI notification when a letter is sent
* Filtered what you can send in a letter
* Mail is now only delivered between 8am and 6pm on the hour
* Stability fixes