**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/BinaryLip/TrainInfo**

----

# TrainInfo
Adds a notification the day before and the morning of a train passing through Stardew Valley. Also displays a popup of what will be in the train when it starts passing through the valley.

Note: The train will NOT appear if you quit and reload the day it's supposed to appear.

## Install
Install the latest version of SMAPI.
Download this mod and unzip it into Stardew Valley/Mods.
Run the game using SMAPI.

## Config Options
| setting                 | default            | what it does                                                               |
|-------------------------|--------------------|----------------------------------------------------------------------------|
| `NotificationTime`     | `1800` (6 PM)       | sets what time to display the notification that a train is coming tomorrow |

## Compatibility
Works with Stardew Valley 1.6+ on Windows/Linux/Mac

Works in single-player and multiplayer (host needs to have the mod for the train contents to be displayed).

No known mod conflicts

## Specific Mod integrations
 * [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) - UI for editing config options

## Limitations
The game doesn't actually determine the number/type of the train cars until it starts passing through the valley (~20 in-game minutes after the global "A train is passing through Stardew Valley" notification) so that's the earliest that I can display the contents of the train.

## Translating the mods
<!--

    This section is auto-generated using a script, there's no need to edit it manually.
    https://github.com/Pathoschild/StardewScripts/tree/main/create-translation-summary

-->
The mods can be translated into any language supported by the game, and SMAPI will automatically
use the right translations.

Contributions are welcome! See [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations)
on the wiki for help contributing translations.

(❑ = untranslated, ↻ = partly translated, ✓ = fully translated)

&nbsp;     | Train Info                   | Contributors
---------- | ---------------------------- | ------------
Chinese    | [❑](TrainInfo/i18n)
French     | [✓](TrainInfo/i18n/fr.json) | [Caranud](https://www.nexusmods.com/stardewvalley/users/745980)
German     | [✓](TrainInfo/i18n/de.json) | [Th3Tob1](https://www.nexusmods.com/users/58067946) 
Hungarian  | [❑](TrainInfo/i18n)
Italian    | [❑](TrainInfo/i18n)
Japanese   | [❑](TrainInfo/i18n)
Korean     | [❑](TrainInfo/i18n)
Portuguese | [❑](TrainInfo/i18n)
Russian    | [❑](TrainInfo/i18n)
Spanish    | [❑](TrainInfo/i18n)
Turkish    | [❑](TrainInfo/i18n)


## See also
 * [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/19305)
 * [CurseForge](https://www.curseforge.com/stardewvalley/mods/train-info)