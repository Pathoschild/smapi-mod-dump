**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TheMightyAmondee/CustomDeathPenaltyPlus**

----


Custom Death Penalty Plus allows the death penalty (and pass out penalty as of 1.1.0) to be fully customised in [Stardew Valley](https://www.stardewvalley.net/).

Now with translation support as of version 1.5.0. Feel free to translate the mod and send me the files for addition (best place to reach me is on the mod page). Full credit given, obviously!

Generic Mod Config Menu support added in 1.5.1 thanks to ayalfishey!

Current options allow configuration for:

**Death Penalty**
- Disabling item loss
- Amount of money lost, including max amount that can be lost
- Amount of health and energy to restore


**Pass Out Penalty**
- Amount of money lost, including max amount that can be lost
- Amount of energy to restore next day

**Other Penalties**
- A more realistic option for dying, disabled by default
- A friendship change with Harvey and/or Maru
- More realistic spawn locations after death, includes new events, disabled by default (this will NOT work in split-screen)
- Apply a debuff after death, chosen randomly normally or burnt on Ginger Island, disabled by default

### Installation:
1. Install the latest version of SMAPI
2. Install Custom Death Penalty Plus, available [here](https://www.nexusmods.com/stardewvalley/mods/7069).
3. Extract the contents into your Mods folder
4. Run the game through SMAPI

### Usage:
1. Run the game at least once to generate the config file
2. Change any desired values in the config. Percentage values are expressed in decimal form
3. Your changes should now be implemented in game

### Notes:
- WakeupNextDayinClinic and MoreRealisticWarps cannot both be true at the same time, they create conflicting changes. Mod will default to disabling MoreRealisticWarps if both are set to true
- Applied debuffs last for 60 seconds

### If changes are not implemented:
 - Check the mod page for accepted values
 - The mod should automatically use the default values if a config value is invalid. If your changes are not implemented check the SMAPI monitor for errors or messages.
 - Other mods that edit the PlayerKilled event can override the mod's edits. Penalties should still apply however (MoreRealisticWarps will also look really weird)
 - With GMCM, the mod will not allow MoreRealisticWarps and WakeUpNextDayInClinic to be true at the same time despite it being possible to do in the menu. The mod will disable MoreRealisticWarps when the menu is saved.

### Translations:
Available in:
- Italian, thanks to AlixNauts!
- Spanish, thanks to AlixNauts!
- French, thanks to AlixNauts!
- Chinese, thanks to KarnieSAMA!

### Versions:
1.0.0 - Initial release

1.1.0 - Passing out penalty is now fully customisable. Added config option for a more realistic death. Minor bug fixes

1.2.0 - Added config option for a friendship penalty with Harvey. Removed items lost menu if items will be restored. Bug fixes

1.2.1 - Fully compatible with multiplayer

1.2.2 - Fixed bug where money would not restore correctly when passing out

1.3.0 - Updated for Stardew Valley 1.5, added debug commands

1.4.0 - Mod data now saved to save file instead of the mod folder, added more config options, removed most debug commands (better to edit config manually, more error checking is in place) only configinfo remains

1.4.1 - Updated for new content api in SMAPI 3.14.0

1.5.0 - Added translation support, added italian, spanish and french translations thanks to AlixNauts on Nexus!

1.5.1 - Added GMCM support thanks to ayalfishey!

1.6.0 - Updated for Stardew Valley 1.6.0


