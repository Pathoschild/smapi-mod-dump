**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Hedgehog-Technologies/StardewMods**

----

# AllowBeachSprinklers
**AllowBeachSprinklers** is an open-source mod for [Stardew Valley](https://stardewvalley.net) that allows players to utilize sprinklers on the Beach Farm map that was introduced in version 1.5.

## Documentation
### Overview
This mod programmatically removes the restriction of not being able to place / use sprinklers on the Beach Farm sand tiles.
- *Note:* This mod works for both existing and newly created Beach Farms

### Compatibility
#### Requirements
- Stardew Valley 1.6 or later
- SMAPI 4.0.0 or later

#### Conflicts
- No known mod conflicts
    - If you find one please feel free to notify me here or on the [Nexus Mod](https://www.nexusmods.com/stardewvalley/mods/7629) site

### Limitations
#### Multiplayer
- Host must have this mod installed for this mod to work for any players in their game
    - If the host *does not* have it installed then the animations may still play, but plants will not be considered "watered"
- Any additional player that would like to be able to place sprinklers will also need to install this mod

#### Solo + Multiplayer
- Players will need to keep this mod installed for sprinklers to continue to water adjacent tiles as expected
    - If mod is uninstalled, placed sprinklers will remain and animations will continue to run, however the expected adjacent tiles will not be considered "watered"

### Install
1. Install the latest version of [SMAPI](https://smapi.io)
    - [Nexus Mirror](https://www.nexusmods.com/stardewvalley/mods/2400)
    - [GitHub Mirror](https://github.com/Pathoschild/SMAPI/releases)
2. Install this mod by unzipping the mod folder into "Stardew Valley/mods"
    - This is meant to be whereever your Stardew Valley game is installed
3. Launch the game via SMAPI

## Releases
Releases can be found [here on GitHub](https://github.com/hedgehog-technologies/StardewMods/releases) and on the [Nexus Mod](https://www.nexusmods.com/stardewvalley/mods/7629) site.
### 1.2.0
- Update to SDV 1.6 compatibility
- Update to SMAPI 4.0.0 compatibility
### 1.1.1
- Removed Github update channel from manifest
   - This is to prevent false update available notifications from SMAPI since the mods are under a unified repository now
### 1.1.0
- Moved to new repository
- Updated to use Khloe Leclair's [Mod Manifest Builder](https://github.com/KhloeLeclair/Stardew-ModManifestBuilder)
- Updated to be compatible with I18n translations
    - Feel free to contribute translations as a Pull Request or on the Nexus Mod site
- Verified working as of Stardew Valley 1.5.6

### 1.0.0
- Initial Release
- Allows players to place sprinklers on Beach Farms
- Working as of Stardew Valley 1.5.3
