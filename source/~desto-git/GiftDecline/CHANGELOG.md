**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/desto-git/sdv-mods**

----

## 1.1.5

- Fix: Update to Stardew Valley 1.6

## 1.1.4

- Fix: Error when starting a new game (for real this time)

## 1.1.3

- Fix: NullReferenceException when either an NPC or item was not found in the game (in case a mod got removed)
- Fix: Error when starting a new game

## 1.1.2

- Fix: Recompile for compatibility with Stardew Valley 1.5
- Fix: Use Log.Trace if the target of a gift cannot be determined instead of throwing a full error

## 1.1.1

- Fix: Remove some debugging logs
- Fix: Remove error log when handing in a billboard quest item a day after accepting it

## 1.1.0

- Feature: [#2] Add option to cap gift taste reduction
- Feature: [#3] Add option to delay gift taste reduction
- Fix: Error when giving a quest item
- Fix: Use configuration of the host in multiplayer
- Fix: Clear gift taste changes when exiting to the title screen

## 1.0.1

- Fix: Multiplayer support
- Fix: error when gifting a bad gift to an NPC if friendship is already 0 [#1]

## 1.0.0

- Initial Release