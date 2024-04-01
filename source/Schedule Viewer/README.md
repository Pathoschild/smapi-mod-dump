**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/BinaryLip/ScheduleViewer**

----

# ScheduleViewer
Adds two new menus that show the schedules for NPCs. The main menu shows the previous, current, and next schedule entries for each NPC. On hover, it also shows which specific tile the NPC is on in that location, which direction they are facing, and which sprite animation will be playing. On click, a second menu opens showing the full schedule for that NPC as well as their current location and if they can receive a gift today.

## Install
Install the latest version of SMAPI.
Download this mod and unzip it into Stardew Valley/Mods.
Run the game using SMAPI.

## Config Options
| setting                           | Default                | what it does                                                                                           |
|-----------------------------------|------------------------|--------------------------------------------------------------------------------------------------------|
| `ShowSchedulesKey`                | `V`                    | the key to open the Schedule Viewer                                                                    |
| `DisableHover`                    | `false`                | don't show tile, facing direction, and animation info on hover                                         |
| `UseLargerFontForScheduleDetails` | `false`                | use a larger font size on the Schedule Details page (longer location names may get cut off if enabled) |
| `NPCSortOrder`                    | Alphabetical Ascending | the order the NPCs are sorted in the Schedule Viewer (Alphabetical/Heart Level & Ascending/Descending) |
| **Filters**                                                                                                                                                         |
| `OnlyShowMetNPCs`                 | `false`                | hides NPCs the farmer hasn't met yet                                                                   |
| `OnlyShowSocializableNPCs`        | `true`                 | hides NPCs you can't socialize with i.e. Gunther or Sandy before the bus is repaired                   |

## Compatibility
Works with Stardew Valley 1.6 on Windows/Linux/Mac

Works in single-player and multiplayer (mods need to match for it to be accurate).

Custom NPCs (like those from SVE) are fully supported

Custom Locations are fully supported.

## Specific Mod integrations
 * [Lookup Anything](https://www.nexusmods.com/stardewvalley/mods/541) - able to lookup NPC information from the Schedule Viewer and details pages

## Translations
<!--

    This section is auto-generated using a script, there's no need to edit it manually.
    https://github.com/Pathoschild/StardewScripts/tree/main/create-translation-summary

-->
Mods can be translated into any language supported by the game, and SMAPI will automatically
use the right translations.

Contributions are welcome! See [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations)
on the wiki for help contributing translations.

(❑ = untranslated, ↻ = partly translated, ✓ = fully translated)

Language   | Status                            | Contributors
-----------|-----------------------------------|--------------------------------------------------------
Chinese    | [↻](ScheduleViewer/i18n/zh.json) | [RegenLicht](https://www.nexusmods.com/users/102031818) 
French     | [✓](ScheduleViewer/i18n/fr.json) | YoshY
German     | [❑](ScheduleViewer/i18n) | &nbsp;
Hungarian  | [❑](ScheduleViewer/i18n) | &nbsp;
Italian    | [❑](ScheduleViewer/i18n) | &nbsp;
Japanese   | [❑](ScheduleViewer/i18n) | &nbsp;
Korean     | [❑](ScheduleViewer/i18n) | &nbsp;
Portuguese | [❑](ScheduleViewer/i18n) | &nbsp;
Russian    | [❑](ScheduleViewer/i18n) | &nbsp;
Spanish    | [❑](ScheduleViewer/i18n) | &nbsp;
Turkish    | [❑](ScheduleViewer/i18n) | &nbsp;

## See also
 * [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/19305)