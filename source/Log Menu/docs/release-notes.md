**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jaredtjahjadi/LogMenu**

----

# Release Notes

## 1.2.2
*Released on June 9, 2024. Compatible with Stardew Valley 1.6 or later, and SMAPI 4.0.0 or later.*

- Added French translation for configuration options. Thank you, Caranud!

## 1.2.1
*Released on June 4, 2024. Compatible with Stardew Valley 1.6 or later, and SMAPI 4.0.0 or later.*

- Added trace message logging for easier troubleshooting in the future.
- Fixed an issue in which the Log Menu was able to be opened in unexpected scenarios (e.g., during minigames, the end credits roll). However, the menu can no longer be opened during heart events nor story events.
- Fixed an issue in which longer messages (e.g., the Perfection Tracker) would not be logged and cause an error in the console.
- Fixed an issue in which the responses for some questions (e.g., during the Linus trash can rummaging event) caused a GameLoop.UpdateTicked event error in the console and the question looping in the Log Menu until the question is passed.

## 1.2.0
*Released on March 19, 2024. Compatible with Stardew Valley 1.6 or later, and SMAPI 4.0.0 or later.*

- Updated for Stardew Valley 1.6.
- Added toggle for HUD messages in the Log Menu.
- Added support for changing portrait backgrounds.
- Changed default maximum dialogue lines from 30 to 50.
- Made Log Menu slightly larger and fixed portrait scaling accordingly.
- Character portraits no longer render with questions, responses, nor mid-action dialogues.
- Fixed an issue in which a dialogue line or a set of responses with more than four line breaks would not be broken up properly, causing overlap with other dialogue lines.

## 1.1.1
*Released on January 22, 2024. Compatible with Stardew Valley 1.5 or later, and SMAPI 3.18.0 or later.*

- Fixed an issue in which all of a character's portraits in the Log Menu would be overwritten by their most recent portrait.
- Adjusted character portrait positioning and scaling to be more accurate with the vanilla game's dialogue boxes.

## 1.1.0
*Released on January 21, 2024. Compatible with Stardew Valley 1.5 or later, and SMAPI 3.18.0 or later.*

- Added an option to log messages from oldest to newest and an option to toggle logging non-NPC dialogue.
- Adjusted text width and changed Log Menu portrait display based on in-game Show Portraits option.

## 1.0.0
*Released on January 19, 2024. Compatible with Stardew Valley 1.5 or later, and SMAPI 3.18.0 or later.*

- Initial release.