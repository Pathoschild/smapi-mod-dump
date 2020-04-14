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

This mod also supports [spacechase0](https://github.com/spacechase0)'s [Generic Mod Config Menu](https://spacechase0.com/mods/stardew-valley/generic-mod-config-menu/) (GMCM). Users with that mod will be able to change Custom Tracker's config.json settings from Stardew's main menu.

The available settings are:

Name | Valid settings | Description
-----|----------------|------------
EnableTrackersWithoutProfession | true, **false** | When set to true, the player will always be able to see forage tracker icons, even without unlocking the Tracker profession.
ReplaceTrackersWithForageIcons | true, **false** | When set to true, the tracker icon will be replaced with an image of each forage item being tracked. A customizable background image will be drawn behind each icon for visibility.
DrawBehindInterface | true, **false** | When set to true, the tracker icon will be drawn *behind* the game's interface. This makes it easier to see the interface, but harder to see the trackers.
TrackerPixelScale | A positive number (default **4.0**) | The size of each pixel of the custom tracker icon is multiplied by this value. This includes forage icons and their background image. This is a [floating point number](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types), so decimals are allowed.
TrackDefaultForage | **true**, false | When set to true, this mod will track most forage normally spawned by Stardew. In technical terms, this tracks objects with the "isSpawnedObject" flag.
TrackArtifactSpots | **true**, false | When set to true, this mod will track artifact spots, a.k.a. buried artifacts or "worm tiles".
TrackPanningSpots | **true**, false | When set to true, this mod will track panning spots, i.e. the glowing spots in water where the Copper Pan tool can be used.
TrackSpringOnions | true, **false** | When set to true, this mod will track harvestable spring onions. This is false by default because the original tracker  doesn't target them.
TrackBerryBushes | true, **false** | When set to true, this mod will track harvestable salmonberry and blackberry bushes. This is false by default because the original tracker doesn't target them.
OtherTrackedObjects | A list of object IDs or names (default **[]**) | A list of additional object IDs (a.k.a. parent sheet index) and/or object names to track. These should be separated by commas, and names should be in quotation marks. Example: `[599, "quality sprinkler", 645]`


### Customizing the tracker icon

CustomTracker includes the `[CP] CustomTracker` folder, which is a content pack for the mod Content Patcher. It loads **tracker.png** and **forage_background.png** into the game, and replaces the game's original tracker icon with **blank_cursor.png** to hide it.

To modify the custom tracker icon, edit or replace **tracker.png** in the `[CP] CustomTracker\assets` folder.

To modify the background drawn behind forage icons, edit or replace **forage_background.png** in the `[CP] CustomTracker\assets` folder.

Note that the background image is expected to be 16 x 16 pixels. Backgrounds with different sizes will not center the forage icon correctly.
