**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Hedgehog-Technologies/StardewMods**

----

**Auto Trasher** is an open-source mod for [Stardew Valley](https://stardewvalley.net) that allows players to automatically send unwanted items to the trash.

![](https://i.imgur.com/TcdN9b9.gif)

## Documentation
### Overview
This mod allows users to specify items that they would like to automatically be sent to their trash bin as soon as they enter their inventory

### Config
- `ToggleTrasherKeybind`
  - Keybind(s) that are used to toggle the Auto Trasher functionality.
  - *Default:* `LeftAlt + R, RightAlt + R`
- `OpenTrashMenuKeybind`
  - Keybind(s) that are used open the Trash List management menu.
  - *Default:* `LeftAlt + L, RightAlt + L`
- `AddTrashKeybind`
  - Keybind(s) that are used to add an item to the Trash List
    - *Note:* You must be hovering over the item you wanted to add to the Trash List when using this functionality
  - *Default:* `LeftAlt + X, RightAlt + X`
- `TrashList`
  - List of Object Ids that will be automatically sent to the trash when entering your inventory
    - *Note:* The Auto Trasher only works with Object type items from the `Data\Objects` data file
  - *Default:* `[ "168", "169", "170", "171", "172", "747", "748" ]`
    - *Note:* This list equates to `[ Trash, Driftwood, Broken Glasses, Broken CD, Soggy Newspaper, Rotten Plant, Rotten Plant ]`

### Translation
&nbsp;     | No Translation  | Partial Translation  | Full Translation  | Translated By
:--------: | :-------------: | :------------------: | :---------------: | :------------:
Chinese    | ✔              | ✔                   | 🤖                | n/a
French     | ✔              | ✔                   | 🤖                | n/a
German     | ✔              | ✔                   | 🤖                | n/a
Hungarian  | ✔              | ✔                   | 🤖                | n/a
Italian    | ✔              | ✔                   | 🤖                | n/a
Japanese   | ✔              | ✔                   | 🤖                | n/a
Korean     | ✔              | ✔                   | 🤖                | n/a
Polish     | ✔              | ✔                   | 🤖                | n/a
Portuguese | ✔              | ✔                   | 🤖                | n/a
Russian    | ✔              | ✔                   | 🤖                | n/a
Spanish    | ✔              | ✔                   | 🤖                | n/a
Thai       | ✔              | ✔                   | 🤖                | n/a
Turkish    | ✔              | ✔                   | 🤖                | n/a
Ukrainian  | ✔              | ✔                   | 🤖                | n/a

### Install
1. Install the latest version of [SMAPI](https://smapi.io)
  1. [Nexus Mirror](https://www.nexusmods.com/stardewvalley/mods/2400)
  2. [CurseForge Mirror](https://www.curseforge.com/stardewvalley/utility/smapi)
  3. [GitHub Mirror](https://github.com/Pathoschild/SMAPI/releases)
2. *Optional* Install the latest version of [Generic Mod Config Menu](https://spacechase0.com/mods/stardew-valley/generic-mod-config-menu/)
  1. [Nexus Mirror](https://www.nexusmods.com/stardewvalley/mods/5098)
  2. [CurseForge Mirror](https://www.curseforge.com/stardewvalley/mods/generic-mod-config-menu)
3. Install **this mod** by unzipping the mod folder into `Stardew Valley/mods`
4. Launch the game use SMAPI

### Compatibility
- Compatible with...
  - Stardew Valley 1.6 or later
  - SMAPI 4.0.0 or later
- No known mod conflicts
  - If you find one, please feel free to notify me on Github, Nexus, or Curseforge

## Limitations
### Solo + Multiplayer
- This mod is player specific, each player that wants to utilize it must have it installed

## Releases
Releases can be found [Github](https://github.com/Hedgehog-Technologies/StardewMods/releases), on the [Nexus Mod](https://www.nexusmods.com/stardewvalley/mods/23663) site, and on the [CurseForge](https://www.curseforge.com/stardewvalley/mods/auto-trasher) site.
### 1.1.0
- Add Reclaim item functionality
- Items will not be trashed immediately from inventory when added to Trash List
- Fix issues with adding trash items when UI scaling not at 100%
### 1.0.0
- Initial Release
