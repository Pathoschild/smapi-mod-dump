**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Hedgehog-Technologies/StardewMods**

----

**Auto Forager** (previously AutoShaker) is an open-source mod for [Stardew Valley](https://stardewvalley.net) that allows players to automatically forage items simply by moving near them.

![](https://i.imgur.com/beGLdhy.gif)

## Documentation
### Overview
This mod checks for:
- Bushes that are currently blooming with berries or tea leaves
  - NOTE: This includes Golden Walnut bushes
- Fruit trees that currently have fruit on them
- Trees that have a seed available to be shaken down
  - NOTE: This includes trees with hazelnuts, coconuts, and golden coconuts
- Forageables throughout Stardew Valley

### Config
You can find a breakdown of the config values [here](./docs/config.md)

### Extensibility
- Custom Trees and Fruit Trees should automatically get picked up and recognized by the AutoForager
- If you are a mod maker working on custom Forageable items, to have your item recognized by the AutoForager all you need to do is add the context tag `forage_item` to the item definition
  - Alternatively, you can create a [content pack](./docs/ContentPack.md) that can also help to categorize your custom forageables / trees

### Translation
&nbsp;     | No Translation  | Partial Translation  | Full Translation  | Translated By
:--------: | :-------------: | :------------------: | :---------------: | :------------:
Chinese    | ✔              | ✔                    | ❌                | [Krobus](https://www.nexusmods.com/users/127351118)
French     | ✔              | ❌                   | ❌                | n/a
German     | ✔              | ❌                   | ❌                | n/a
Hungarian  | ✔              | ❌                   | ❌                | n/a
Italian    | ✔              | ❌                   | ❌                | n/a
Japanese   | ✔              | ❌                   | ❌                | n/a
Korean     | ✔              | ❌                   | ❌                | n/a
Polish     | ✔              | ❌                   | ❌                | n/a
Portuguese | ✔              | ✔                    | ❌                | [NARCOAZAZAL](https://www.nexusmods.com/users/200703680)
Russian    | ✔              | ❌                   | ❌                | n/a
Spanish    | ✔              | ✔                    | ❌                | [ElviraCroft](https://github.com/ElviraCroft)
Thai       | ✔              | ❌                   | ❌                | n/a
Turkish    | ✔              | ❌                   | ❌                | n/a
Ukrainian  | ✔              | ❌                   | ❌                | n/a

### Install
1. Install the latest version of [SMAPI](https://smapi.io)
  1. [Nexus Mirror](https://www.nexusmods.com/stardewvalley/mods/2400)
  2. [CurseForge Mirror](https://www.curseforge.com/stardewvalley/utility/smapi)
  3. [GitHub Mirror](https://github.com/Pathoschild/SMAPI/releases)
2. *Optional but recommended* Install the latest version of [Generic Mod Config Menu](https://spacechase0.com/mods/stardew-valley/generic-mod-config-menu/)
  1. [Nexus Mirror](https://www.nexusmods.com/stardewvalley/mods/5098)
  2. [CurseForge Mirror](https://www.curseforge.com/stardewvalley/mods/generic-mod-config-menu)
3. Install this mod by unzipping the mod folder into 'Stardew Valley/Mods'
4. Launch the game using SMAPI

### Compatibility
- Compatible with...
  - Stardew Valley 1.6 or later
  - SMAPI 4.0.0 or later
- Automatic Integrations
  - [Bush Bloom Mod](https://www.nexusmods.com/stardewvalley/mods/15792)
    - Minimum version: **1.1.9**
  - [Custom Bush](https://www.nexusmods.com/stardewvalley/mods/20619)
    - Minimum version: **1.0.4**
  - [Farm Type Manager](https://www.nexusmods.com/stardewvalley/mods/3231)
    - Minimum version: **1.20.0**
- No known mod conflicts
  - If you find one, please feel free to notify me here on Github, on the [Nexus Mod](https://www.nexusmods.com/stardewvalley/mods/7736) site, or on the [CurseForge](https://www.curseforge.com/stardewvalley/mods/auto-forager) site.

## Limitations
### Solo + Multiplayer
- This mod is player specific, each player that wants to utilize it must have it installed

## Releases
Releases can be found on [GitHub](https://github.com/Hedgehog-Technologies/StardewMods/releases), on the [Nexus Mod](https://www.nexusmods.com/stardewvalley/mods/7736) site, and on the [CurseForge](https://www.curseforge.com/stardewvalley/mods/auto-forager) site.
### 3.5.2
- Fix tappers being delayed by a day when auto foraged from
### 3.5.1
- Fix picking up artifact and seed spots
### 3.5.0
- Add automatic integration with [Farm Type Manager](https://www.nexusmods.com/stardewvalley/mods/3231)
  - Content packs should no longer be needed going forward but will continue to work
  - Category names will likely be different as they are now pulled from the FTM content pack instead of the AF content packs
  - Minimum version: **1.20.0**
  - Shoutout to [Esca-MMC](https://github.com/Esca-MMC) for supplying the API
- Add options to forage Mushroom Boxes, Mushroom Logs, and Tappers
- Add Animal products as a category for vanilla animal products
- Add vanilla Fruit forageable toggles to help with the Fruit Bat cave option
### 3.4.3
- Add additional safeguards around players clicking through title menu quickly
### 3.4.2
- Fix items incorrectly getting increased quality where they normally wouldn't
### 3.4.1
- Prevent content packs from overriding known categories of vanilla forageables
- Lower the amount of warning log spew when multiple content packs add the same native item as a forageable
### 3.4.0
- Add ability to ignore moss-covered trees that are near enough to contribute to a Mushroom Log
- Fix optional tool requirement for harvesting moss to be less intrusive
- Lower the amount some logs were spamming the SMAPI console
### 3.3.0
- Wild Tree shake items are now supported
- Adds Spanish translations
  - Thanks to [ElviraCroft](https://github.com/ElviraCroft) for the contribution
### 3.2.2
- Fix exception on launch due to multiple Title Screen init events running simultaneously
- Update config strings to be a bit more explicit what they are looking for
- Add some trace logging for config values
### 3.2.1
- Fix crash on launch
- Move mod initialization to final init heartbeat to ensure everything is loaded
### 3.2.0
- Buried forageables are now handled via `Artifact Spot` and `Seed Spot` toggles instead of individually
- Final initialization heartbeat now waits for Title Menu to be interactable
- All forageable toggles are initialized to enabled
  - This is a mitigation until I can find time to figure out a 'Select / Deselect All' config button
- Fixed issue where some objects weren't properly seen as forageable
- Fixed exception thrown by utilizing tools when foraging moss and buried forageables
- Fixed toggling forager with keybinds sometimes not respecting "Is Forager Active?" config setting
  - Config setting was removed to prevent doubling up on sources of truth, forager will always be active on game start
- Add content packs for Wild Flowers and Kombucha of Ferngill
### 3.1.0
- Added field for content packs to ignore items that may not actually be forageable
- Added content pack for Atelier Wildflour Crops and Forage
### 3.0.3
- Prevent crash when integrated mods aren't ready within the timeout window
- Bump up wait time for integrated mods
### 3.0.2
- Bump Integration wait timer from 5s -> 30s to account for larger installed mod counts
### 3.0.1
- Fix Spring Onions and Ginger not being foraged
### 3.0.0
- Added [content pack format](./docs/ContentPack.md) to allow for easier extensibility for various content mods
  - Content Packs for 2.2.3 compatibility parity provided as optional files
    - Cornucopia
      - More Crops
      - More Flowers
    - Forage of Ferngill
      - Coastal Forage of Ferngill
      - Fruits and Nuts of Ferngill
      - Mushrooms of Ferngill
      - Roots of Ferngill
    - Lumisteria
      - Serene Meadow
      - Visit Mount Vapius
    - Ridgeside Village
    - Stardew Valley Expanded
- Added automatic integrations with the following mods
  - [Bush Bloom Mod](https://www.nexusmods.com/stardewvalley/mods/15792) - minimum version: 1.1.9
  - [Custom Bush](https://www.nexusmods.com/stardewvalley/mods/20619) - minimum version: 1.0.4
- Add Portuguese translations
  - Thanks to [NARCOAZAZAL](https://www.nexusmods.com/users/200703680) for the provided translation
- Fixed buried forageables not respecting config toggles until game was reopened
### 2.2.3
- Fix some translations not getting updated on locale change
- Added Chinese translations
  - Thanks to [Krobus](https://www.nexusmods.com/users/127351118) for the provided translation
### 2.2.2
- Additional error checks and fallbacks when parsing Fruit Trees
### 2.2.1
- Fixed errors when parsing trees
- Prevent possible future errors from completely halting mod functionality
### 2.2.0
- Added compatibility with Stardew Valley Expanded forageable items
- Added "Moss" as a forageable option
  - Added option to toggle off requirement of having a tool in inventory for Auto Forager to forage for Moss
- Added partial compatibility for the Cornucopia mod
  - Additional work is needed to added compatibility with the "Custom Bush" mod
- Custom Wild and Fruit trees are now properly recognized by the Auto Forager
- Fixed a potential crash when running alongside the "Marry Morris" mod
### 2.1.0
- Added ability to forage for truffles found by pigs
- Wild Trees added in 1.6 are now shaken as expected
- Config settings no longer reset on game launch when content patch mods are present
  - Special shoutout to DromedarySpitz, babayagah07, and galedekarios for reporting and helping me investigate this issue
### 2.0.0
- Rebranded to **AutoForager**
- Extended functionality to include options to forage seasonal items
- Update to SDV 1.6 compatibility
- Update to SMAPI 4.0.0 compatibiliy
### 1.6.0
- Moved to new repository
- Updated to use Khloe Leclair's [Mod Manifest Builder](https://github.com/KhloeLeclair/Stardew-ModManifestBuilder)
### 1.5.2
- Fix tea bushes from constantly shaking
### 1.5.1
- Add Chinese localization
    - Translation by: liky123131231 (NexusMods)
### 1.5.0
- Simplify calculations per game tick
- Add translation language support
- Back-end versioning updates
- Thanks to @atravita-mods for this update
### 1.4.0
- Added the ability to shake Tea Bushes for their Tea Leaves
### 1.3.2
- Fixes a NullReferenceException thrown when a second user is joining a split-screen instance
- Updated the way the End-Of-Day messages are built
- Minor backend changes
### 1.3.1
- Fix for not shaking bushes when current language isn't set to English
- Updated default ShakeDistance from 1 to 2
- Minor backend changes
### 1.3.0
- Added the ability to specify the number of fruits (1-3) available on Fruit Tree before attempting to auto-shake it
- Minor backend changes
### 1.2.0
- Swapped config to have separate toggles for regular and fruit trees
- Added a check to ensure a user isn't in a menu when the button(s) for toggling the autoshaker are pressed
- Added some additional "early outs" when checking whether or not a tree or bush should be shaken
### 1.1.0
- Upgrading MinimumApiVerison to SMAPI 3.9.0
- Swap from old single SButton to new KeybindList for ToggleShaker keybind
    - Anyone who has a config.json file will no longer have to press an alt button to toggle the AutoShaker (unless they change their config.json file manually OR delete it and let it get regenerated the next time they launch Stardew Valley via SMAPI)
### 1.0.0
- Initial release
- Allows players to automatically shake trees and bushes by moving nearby to them
- Working as of Stardew Valley 1.5.3
