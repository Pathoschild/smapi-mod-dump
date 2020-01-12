<h1 align="center">
    <img src="FeatureImage.png" alt="Feature Image">
</h1>

# Daily Screenshot v1.1.0

> A Stardew Valley mod that automatically takes a screenshot of your entire farm at the start of each day.

Releases can be found at the following websites:

- [NexusMods](https://www.nexusmods.com/stardewvalley/mods/4779)
- [Chucklefish](https://community.playstarbound.com/resources/daily-screenshot.5907/)
- [CurseForge](https://www.curseforge.com/stardewvalley/mods/daily-screenshot)
- [ModDrop](https://www.moddrop.com/stardew-valley/mods/677025-daily-screenshot)

## Features

- A screenshot is automatically taken of your entire farm every day as soon as you leave your house.
- Choose what time the screenshot gets taken.
- Choose how often a screenshot gets taken.
- Stores screenshots in the StardewValley/Screenshots folder, with each save file getting its own "FarmName-Screenshots" folder, so you can easily access the screenshots in-game under the "Options" tab in the menu and screenshots from multiple save files will not get mixed up. The file path to the screenshots folder is C:\Users\USERNAME\AppData\Roaming\StardewValley\Screenshots
- Makes it super easy to gather screenshots to create a GIF that shows how your farm changes over time! There are many GIF makers that can be found online, such as [ezgif GIF maker](https://ezgif.com/maker).
- Keeps the screenshots in the correct order to make creating the GIF easier. This is achieved by naming each screenshot with a "year-season-day.png" numerical format. For example, on Year 1, Spring, Day 3, the screenshot would be named "01-01-03.png". 
- Can enable a keyboard shortcut for taking screenshots (set to "None" by default, so you need to specify a key on your keyboard in the Config file for this to work)
- Custom configuration options! See below.

## Installation

1. [Install the latest version of SMAPI](https://smapi.io/).
3. Download this mod and unzip it into Stardew Valley/Mods.
4. Run the game using SMAPI.

## Compatibility

- Works with Stardew Valley 1.4 or later on Linux/Mac/Windows.
- Works in both singleplayer and multiplayer.
- No known mod conflicts.

## Config

| Configuration Description                                | Setting Options | Default Setting |
| -------------------------------------------------------- | -------- | -------- |
| TimeScreenshotGetsTakenAfter     | Any integer between 600 and 2600   | 600 (screenshot gets taken anytime after 6:00 AM, upon entering your farm) |
| TakeScreenshotKey | [List of Possible Key Bindings](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings) | None |
| TakeScreenshotKeyZoomLevel     | Any number from 0 to 1   | 0.25 (full-map screenshot) |
| FolderDestinationForDailyScreenshots | A path to a folder on your computer (if the path you specify does not exist, it will be created) Note: Needs to have two slashes in the file path. Example: C:\\\Users\\\Lauren\\\OneDrive\\\Pictures\\\Screenshots | default (located in StardewValley\Screenshots\Your-Farm-Name-Here folder) |
| FolderDestinationForKeypressScreenshots | A path to a folder on your computer (if the path you specify does not exist, it will be created) Note: Needs to have two slashes in the file path. Example: C:\\\Users\\\Lauren\\\OneDrive\\\Pictures\\\Screenshots | default (located in StardewValley\Screenshots\Your-Farm-Name-Here folder) |
| HowOftenToTakeScreenshot | Everything described for the rest of this table | -- |
| Daily | true, false | true |
| Mondays | true, false | true |
| Tuesdays | true, false | true |
| Wednesdays | true, false | true |
| Thursdays | true, false | true |
| Fridays | true, false | true |
| Saturdays | true, false | true |
| Sundays | true, false | true |
| First Day of Month | true, false| true |
| Last Day of Month | true, false | true |

## License

[![CC0](http://mirrors.creativecommons.org/presskit/buttons/88x31/svg/cc-zero.svg)](https://creativecommons.org/publicdomain/zero/1.0/)<br />This work is licensed under a <a rel="license" href="http://creativecommons.org/licenses/by/1.0/">Creative Commons Attribution 1.0 International License</a>.
