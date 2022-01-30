# Longer Fence Life

## Permissions

Longer Fence Life is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

The Longer Fence Life mod is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

I don't really care if you redistribute it or alter it or use it in compilations.
I'd ask that you give credit to myself, that's all.

Source code is available at
https://github.com/NormanPCN/StardewValleyMods/tree/main/LongerFenceLife

## Description

### Fence Life
Sets a multiplier for the default game fence life. 1.0 gives the vanilla game fence life.
You can set the life anywhere from less than the game base life to many times the base life. Yes, you can make the life shorter should you so desire.

This mod detects when you drop/place a fence from your inventory. At that time, it adjusts the health to the modified value.
After this point, all game functions with respect to life/health function normally as in the vanilla game. This mod is hands off after placement.

## Config

config.json is available to edit and configure the Mods functions.
 After the first time the Mod is started it will create a default config.json file in the Mod folder.
 config.json is a simple text file and easy to edit. It might be hard to find the Mods folder for some.

Longer Fence Life also supports the Generic Mod Config Menu (GMCM) interface.
GMCM is a much easier way to configure the Longer Fence Life mod.
GMCM provides a graphical user interface to configure the Mod settings.
You can configure from the game title screen and/or during the game.
GMCM is available at https://www.nexusmods.com/stardewvalley/mods/5098
The NexusMods page shows the locations of the GMCM config button(s).

### Config options

- Wood Fence Life:  
Base life is approximately 1/2 year.  
A value of 2.0 will give a life of approximately 1 year.

- Stone Fence Life:  
Base life is approximately 1 year.  
A value of 2.0 will give a life of approximately 2 years.

- Iron Fence Life:  
Base life is approximately 2.25 years.  
A value of 1.3 will give a life of approximately 3 years.

- Hardwood Fence Life:  
Base life is approximately 5 years.

- Gate Life:  
Base life is approximately 4 years.

- Fence life keybind
The mod can also display the health/life status of your fences. 
You do this by holding down a specific key. User configurable. 
While the key is held down any fences visible on screen will have a Green, Yellow or Red overlay. 
Green means life is greater than 2 months. Yellow is greater than 1 month. Red is remaining life is less than one month. 
A tooltip is also shown if the mouse cursor is pointing at a fence post. 
This will show the approximate number of days remaining fence life.


## Fence Trivia
The game has what I would call a bug, where if a gate replaces an **existing** fence post, the gate keeps the fence post life. 
The gate does not get the life for gates. Gates have a longer life than all fence types except hardwood fences.

## Changlog

v1.0.0:  
 Initial release. 

 1.1.0
 Added a fence life display overlay and tooltip. Uses a keybind to activate.
 Spanish translation added. Thanks. Yllelder B.
