**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

([versión en español aquí](https://github.com/misty-spring/StardewMods/blob/main/FarmhouseVisits/docs/CHANGELOG_es.md))

## 1.8.0
### 1.6-alpha only

- A lot of refactoring.
- Unique retiring dialogue.
- Retiring/In-law dialogue can be customized.
- Files are reloaded if any changes are made (e.g via ContentPatcher).
- NPCs can now go into sheds and greenhouse.
- NPCs can now stay for a sleepover, if the time matches.
- Changed how custom schedules work: removed "Force", but added "Extras" instead. (see more [here](https://github.com/misty-spring/StardewMods/blob/main/FarmhouseVisits/docs/Custom_visits.md))
- You can use GSQ conditions for scheduled visits.
- Added bool MustBeExact for scheduled visits (see more [here](https://github.com/misty-spring/StardewMods/blob/main/FarmhouseVisits/docs/Custom_visits.md)).

## 1.7.2
- Bug fixes

## 1.7.1
- Updated portuguese

## 1.7.0
- Visits will comment on your animals and crops, along with unique dialogue during winter (and dialogue if there's neither animals nor crops).
- Tweaked the "In-Law dialogue" explanation in translations/i18n
- Changed bugs with scheduled visits
- Removed the `force_visit` command
- More information is avaiable via console: the command has been renamed to `print`

## 1.6.2
- Added Thai translation (by watchakorn-18k)

## 1.6.1
- Updated portuguese translation

## 1.6.0
- Changed the way translations (and logs) are called.
- NPCs can now walk on the farm.
- Minor fixes

## 1.5.3
- Added portuguese (by BrasileiroTop)

## 1.5.2
- Turkish translation updated.

## 1.5.1
- Russian translation updated.

## 1.5.0
- Gifts can now be toggled off.
- Visits can now ask for permission to enter (and have optional rejection dialogue).
- Fixed bugs in custom visits.
- Renamed "NPC" to its equivalent "PNJ" in spanish.
- A lot of refactoring.
- New dialogues when NPCs leave.

## 1.4.0
- Added chinese translation (by SHIZHI01420142)

## 1.3.1
- Bug fixes.
- Mod will recheck friendship lvl every weekend
  - (rechecking every day would be expensive, so this makes sure values will update if the user goes through many days in a single play)

## 1.3.0
- Fixed typos.
- NPCs in island won't visit.
- Visits won't occur in festival nor hospital days.
- If a NPC falls asleep in your house, they immediately retire.
- Added in-law comments 
  - This includes commenting on your spouse and kids.
  - it is also configurable (mod-friendly and replacer-friendly).
- Added debug commands to config screen.
- Turkish translation (thanks to BURAKMESE!)
- Russian translation (thanks to crybaby00000!)
- Misc performance fixes.

## 1.2.2
- Fixed indexing errors when returning to title / ending the day. 
- Still trying to get NPCs to return their pre-visit dialogue and facing direction. (Doesn't seem to work so far)

## 1.2.1
- The characters `, . / ;` won't cause problems in blacklist. (However, spaces (' ') are still mandatory).

## 1.2.0
- Bug fixes, NPCs will return to what they were doing pre-visit.

## 1.1.0
- Added customizable schedules (see [here](https://github.com/misty-spring/FarmhouseVisits/blob/main/README.md)).

## 1.0.1
- Fixed typo that caused error in-game.

## 1.0.0
- Initial release. 
