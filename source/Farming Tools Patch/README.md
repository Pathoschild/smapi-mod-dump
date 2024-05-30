**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Torsang/FarmingToolsPatch**

----

# Farming Tools Patch - Version 1.3.1
_Forked and updated for Stardew Valley 1.6 running SMAPI 4.0.8+ and Harmony 2.0+_

### Releases 
Available at [NexusMods.com][ModPage]

### Description
This project creates a mod that replaces the default behavior of Stardew Valley's farming tools to better match other game mechanics. Specifically it changes the tiles affected by the tool's "charge up" moves.

### Hotkey Map
_this assumes a US keyboard, please adjust accordingly if using another layout._

These keys can be remapped in Generic Mod Config Menu or in the config.json file.
|Button|Description|
|------|-----------|
|Pipe/Backslash|Cycles power levels for adjustment|
|Pipe(Hold)|Reset tool values to mod's default|
|Open Bracket|Increases tool AoE length|
|Close Bracket|Increases tool AoE width|
|Semicolon|Decreases tool AoE length|
|Quote|Decreases tool AoE width|

### Changelog
<details>
  <summary>Version 1.3.1 - Reset hotkey added</summary>

  - Added reset to default functionality: hold down the Cycle Charge Level keybind
  - Customize how much time needed to hold the button in the GMCM menu
  - Added relevant i18n fields
  - Refactored global reach/radius values and updated GMCM fields to reflect this
</details>
<details>
  <summary>Version 1.3.0 - i18n Support</summary>

  - Refactored all game-visible strings from hardcoded to i18n compatible references
</details>
<details>
  <summary>Version 1.2.1 - The HotKeys Cometh</summary>

  - Implemented SMAPI button detection to allow Hot Key functionality
  - Refactored GMCM API calls from ModEntry to separate class
  - Added additional config variables and GMCM methods to handle hot keys
  - Added new methods to handle button press logic
  - Fixed logic errors in button press methods to properly constrain tool area of effect
  - Ternary operators... everywhere...
</details>
<details>
  <summary>Version 1.1.2 - GMCM Unfettering</summary>

  - Removed Title Screen restriction for GMCM: Edit tool behavior during gameplay
</details>
<details>
  <summary>Version 1.1.1 - All Tool Ranks, Finished GMCM Integration</summary>

  - Added control of all upgraded tool qualities
  - Added GMCM fields to affect the above-mentioned feature
  - Refactored Harmony class/method for more reasonable efficiency with expanded functionality
</details>
<details>
  <summary>Version 1.0.3 - Initial GMCM Implementation</summary>

  - Added beginning of Generic Mod Config Menu functionality
</details>
<details>
  <summary>Version 1.0.2 - Initial Fork</summary>
  
  - Update to StardewModConfig 4.1.1 and Harmony 2.3.3
  - Update namespace to reflect new project
</details>

### Roadmap
- [x] Add GMCM Functionality
- [x] Implement Hot Keys
- [x] Gold Behavior Modification
- [x] Steel Behavior Modification
- [x] Copper Behavior Modification
- [x] i18n Support
- [ ] Ensure (realistically) maximum compatibility with other mods
- [ ] Accomodate base-game tool enchantments

### License
Licensed under the MIT License. See [here][License] for more information.

### Acknowledgments
- _[EKomperud] producer of the original mad and generally cool cat_

[ModPage]: <https://www.nexusmods.com/stardewvalley/mods/24066/>
[License]: <https://github.com/Torsang/FarmingToolsPatch/blob/main/LICENSE>
[EKomperud]: <https://github.com/EKomperud/StardewMods/tree/master/IridiumToolsPatch>
