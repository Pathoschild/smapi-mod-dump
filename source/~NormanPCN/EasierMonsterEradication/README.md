**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/NormanPCN/StardewValleyMods**

----

# Easier Monster Eradication

## Permissions

Easier Monster Eradication is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

The Easier Monster Eradication mod is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

I don't really care if you redistribute it or alter it or use it in compilations.
I'd ask that you give credit to myself, that's all.

Source code is available at https://github.com/NormanPCN/StardewValleyMods/tree/main/EasierMonsterEradication

## Description
Adjust the number of monsters slays needed for monster eradication goals.

Stardew Valley has a bit of a grinding factor to it. Parts can be excessive to some.  
Monster eradication is one of these as many goals are quite high.  
By the time you reach the goal and obtain the reward you might be somewhat close to end of the playthrough for a given save.  
Thus maybe not so much time to enjoy and play with your reward(s).  
Adjusting the eradication goal can ease this grind. So you have less time spent grinding monsters, just for the sake of grinding monsters, to get the goal/reward.

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

Monster Percentage
The percentage relative to vanilla monster defaults. 0.5 means you need to eradicate half as many monsters. You can raise the burden above 1.0 should you so desire.

If you lower the eradication goal in the middle of a gameplay save and the new goal puts a monster goal below the current number of that monster you have slayed, then you will never receive a notification of reaching the monster slayer goal for that monster. That goal notification is given at that one monster slay that surpasses the goal. That said, your reward at the Adventurers Guild will still be available when you talk to Gil.

Conversely if you raise the goal and have already received your reward then you will not receive the reward again, and again meeting the new eradication goal. You receive the reward once. If you have not yet collected the reward and raise the goal then the reward will no longer be available until you meet the eradication goal.

## Changlog

v1.0.0:  
 Initial release. 

 v1.0.1
 Fix issues with the Api for Mod authors

 v1.1.0
 Updates for Stardew Valley 1.6 and SMAPI 4.

 v1.1.1
 Updated the vanilla quest amount for Cave insects. From 125 to 80. v1.5 to v1.6.

 v.1.2.0
 Added Japanese translation. (Thanks mitekano23)
 The config dialog is no longer restricted to the game title screen menu. It can now be used inGame.
