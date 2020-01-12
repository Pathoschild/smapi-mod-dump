# Custom Tracker
A mod for the game Stardew Valley, allowing players to use a custom forage tracker icon of any size or shape. It includes options to display the tracker for players without the Tracker profession, display the specific forage being tracked by each icon, and more.

## Contents
* [Installation](#installation)
* [Customization](#customization)
  * [Changing settings](#changing-settings)
  * [Customizing the tracker icon](#customizing-the-tracker-icon)

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Install the latest version of [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915).**
3. **Download CustomTracker** from [the Releases page on GitHub](https://github.com/Esca-MMC/CustomTracker/releases), Nexus Mods, or ModDrop.
4. **Unzip CustomTracker** into the `Stardew Valley\Mods` folder.

## Customization

By default, CustomTracker will hide the original tracker icon and replace it with a large yellow arrow.

To replace the tracker icon or customize this mod's available settings, see the sections below.

### Changing settings

1. **Run the game** using SMAPI. This will generate the mod's **config.json** file in the `Stardew Valley\Mods\CustomTracker` folder.
2. **Exit the game** and open the **config.json** file with any text editing program.

The available options are:

Name | Valid settings | Description
-----|----------------|------------
EnableTrackersWithoutProfession | true, **false** | When set to true, the player will always be able to see forage tracker icons, even without unlocking the Tracker profession.
ReplaceTrackersWithForageIcons | true, **false** | When set to true, the tracker icon will be replaced with an image of each forage item being tracked.
DrawBehindInterface | true, **false** | When set to true, the tracker icon will be drawn *behind* the game's interface. This makes it easier to see the interface, but harder to see the trackers.

### Customizing the tracker icon

CustomTracker includes the `[CP] CustomTracker` folder, which is a content pack for the mod Content Patcher. It loads **tracker.png** into the game and replaces the game's original tracker icon with **blank_cursor.png** to hide it.

To use a different custom tracker icon, edit or replace **tracker.png** in the `[CP] CustomTracker\assets` folder.
