# Better Butterfly Hutch

## Permissions

Better Butterfly Hutch is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

The Better Butterfly Hutch mod is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

I don't really care if you redistribute it or alter it or use it in compilations.
I'd ask that you give credit to myself, that's all.

Source code is available at
https://github.com/NormanPCN/StardewValleyMods/tree/main/BetterButterflyHutch

## Description

I like to put a Butterfly Hutch in the Greenhouse. The butterfly hutch in the game all too often spawns very few butterflies. I got tired of this so I wrote this mod.

This mod lets you specify a range for the number of butterflies to spawn in the location of the hutch.  
You can specify different ranges for indoor and outdoor locations.  
This mod also lets you set the number of Bat Wings required in trade for a hutch at the Desert Trader.  
The Vanilla value is quite high given that the Hutch really does nothing functional.

### Placing a butterfly hutch outdoors.
The game code for the Butterfly Hutch will spawn butterflies in the rain, snow or wind with debris. That is a bit silly.  
The Hutch game code will spawn butterflies after dark.  
Ambient butterflies do not spawn in those conditions.  
This mod will not spawn butterflies in those conditions and will remove hutch butterflies spawned in those conditions.

## Config

config.json is available to edit and configure the Mods functions.
 After the first time the Mod is started it will create a default config.json file in the Mod folder.
 config.json is a simple text file and easy to edit. It might be hard to find the Mods folder for some.

The mod also supports the Generic Mod Config Menu (GMCM) interface.
GMCM is a much easier way to configure the Longer Fence Life mod.
GMCM provides a graphical user interface to configure the Mod settings.
You can configure from the game title screen and/or during the game.
GMCM is available at https://www.nexusmods.com/stardewvalley/mods/5098
The NexusMods page shows the locations of the GMCM config button(s).

### Config options

- Minimum Indoors:  
The minimum number of butterflies to spawn indoors.  A value of zero disables the mod for indoors.

- Maximum Indoors:  
The maximum number of butterflies to spawn indoors. If minimum is greater than maximum, then minimum is assigned to maximum.  

- Minimum Outdoors:  
The minimum number of butterflies to spawn outdoors.  A value of zero disables the mod for outdoors.

- Maximum Outdoors:  
The maximum number of butterflies to spawn outdoors. If minimum is greater than maximum, then minimum is assigned to maximum.  

- Num Bat Wings:  
The number of Bat Wings used to trade for the Hutch at the Desert Trader. Vanilla default is 200.  

- Winter butterflies:  
Spawn butterflies outdoors in winter.  

- Island Butterflies:
Spawn Ginger Island butterflies indoors on the mainland.

- Shake Hutch:
Shake the hutch to spawn butterflies. You shake the hutch just like shaking a tree.

- Night butterflies:
When enabled, indoor butterfly hutches will spawn butterflies after the game "getting dark" time. The vanilla hutch spawns butterflies all day when indoors. Disabling this spawns butterflies as outdoors and butterflies can get the evening off and get a little rest. :-).

## Changlog

v1.0.1:  
 Initial release. 

 v1.1.0
 No longer spawn butterflies outdoors in winter or after dark.  
 Remove game spawned hutch butterflies in conditions this mod would not spawn butterflies. Rain/dark/season.  
 Added minimum/maximum normalization in GMCM interface.  
 Update to GMCM v1.8
 
 v1.2.0
 Added config option to spawn Island butterflies indoors on the mainland.
 Adjusted the "dark" time to stop spawning butterflies outdoors.

 v1.3.0
 You can now shake the hutch to instantly spawn butterflies.

 v1.3.1
 Butterflies spawned by shaking the hutch now stay inbounds on the map.

 v.1.3.2
 Fix for a bad upload. Version change to trigger updates.

 v.1.3.3
 Added Hungarian translation. Thanks martin66789 on Nexus Mods.

 v.1.3.4
 Added a night butterflies option. Let indoor butterflies have the evenings off, just like outdoor butterflies.
