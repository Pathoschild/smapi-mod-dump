**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/su226/StardewValleyMods**

----

# StayUp [![Download at Nexus Mods](https://img.shields.io/badge/download_from-nexus_mods-orange?style=for-the-badge)](https://www.nexusmods.com/stardewvalley/mods/7592)
StayUp mod let you work 24 hours per day without collapsing at 2:00 am.

I'm using Omegasis' [Night Owl](https://www.nexusmods.com/stardewvalley/mods/433) mod before, but it doesn't support 1.5 and multiplayer, so I made this mod. 

## Features
- Support Stardew Valley 1.5.
- Save and jump into a new day at 6:00 am.
- Keep your and your horse's position automatically.
- Keep stamina and health. (Disabled by default)
- Tested in multiplayer. (But not split screen)
- Supress time shake and tired emote.
- Morning light transition works with any weather.
- Smart fish editing for your fishing needs.
- Fully ﻿configurable.

## Config
```jsonc
{
  "editFishes": true, // Make fish catchable before dawn
  "keepHealth": false, // Keep health at 6:00 am
  "keepHorse": true, // Keep horse position at 6:00 am
  "keepFarmer": true, // Keep farmer position at 6:00 am
  "keepStamina": false, // Keep stamina at 6:00 am
  "morningLight": 400, // Morning time (430 means 4:30 am, etc. -1 for disable in case you are using Dynamic Night Time or other mods.)
  "morningLightPower": 2.0, // Morning light power, for realism (2 means square, etc. Can be float.)
  "newDayAt6Am": true, // Save game and jump into new day at 6:00 am
  "noTiredEmote": true, // Supress tired emote (Unless you are in bed)
  "noTimeShake": true, // Supress time shake
  "smoothSaving": true, // Fade in and out when saving game
  "stayUp": true // Enable this mod (Don't affect noTiredEmote and noTimeShake)
}
```
