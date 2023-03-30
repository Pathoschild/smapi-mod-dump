**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

## 4.2.1
- Fixed version error.

## 4.2.0
- Bug fixes.
- Edited NPCWarp coordinates in Island_N, Island_S, etc.
- Giving Willy a ticket will bring you to the island (Only in visit days).
- Fixed path to NPC text in schedule (`Schedules` to `Dialogue`).
- Fixes to harmony patch in `TryToReceiveActiveObject` (dialogue that wasn't shown, etc).
- ModStatus will be saved every night (regardless of changes (or lack of)).
- Moved schedule-loading to ContentPatcher.
- Now uses the key "IslandVisit" (and patches internally).
- Added a "template" map for randomization: `Custom_Random`.
- Fixes to `OnUpdateTicked` (now successfully warps characters to IslandFarmHouse).

## 4.1.0
- Added compatibility with LittleNPCs.
- Bug fixes.

## 4.0.1
- Bugfix: Visits won't happen every day.
- Other minor fixes.

## 4.0.0
- Overhauled the code. 
- Removed the mod framework; ContentPatcher must be used now.

## 3.3.0
- Fixed the Willy bug.
- Optimization of asset load.
- More configuration options.
- Spouses can now visit random locations.

## 3.2.0
- Changed how some assets are loaded. 
- More debug commands. 
- Fixed bug of game freezing.

## 3.1.0
- Added translation support.

## 3.0.0
- Fully ported the mod to SMAPI (C#).
- Created a framework to add mod spouses' schedules.

## 2.2.0
- Added schedules for SVE spouses.
- New events and dialogues for Devan.
- Advanced configuration.
- Devan can now be added regardles of marital/familiar state.
- Removed spouse rooms (Custom Spouse Rooms added support for these).

## 2.1.0
- Bug fixes
- Added new content
- Removed the bed "patched" onto the map.

## 2.0.1
- Bugfix: Devan won't appear if you don't have children.

## 2.0.0
- Many important changes in the code
- Added Devan (babysitter NPC)
- Spouses sleep in the bed now.

## 1.1.0
- Added compatibility with Child2NPC and FreeLove.
- Bug fixes.

## 1.0.0
- Initial release.
