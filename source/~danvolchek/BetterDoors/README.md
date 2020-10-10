**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/danvolchek/StardewMods**

----

# Better Doors


See [This link](http://www.nexusmods.com/stardewvalley/mods/3797) for the NexusMods mod page, which has a description, screenshots, and a download of the built mod.

## How it works

The mod:
- Loads content packs and vanilla assets to find provided door sprites.
- Reads loaded maps to look for requested doors sprites.
- Dynamically generates door sprites based on loaded content packs/vanilla assets and requested sprites.
  - Sprite generation means users don't need to draw rotations/transformations.
- Attaches doors to the maps and handles user interaction.

See the wiki for more info about creating custom door sprites or adding custom doors to maps: https://github.com/danvolchek/StardewMods/wiki/Better-Doors.