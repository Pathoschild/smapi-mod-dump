**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Veniamin-Arefev/StardewMods**

----

Adventurers Guild Multiplayer Kill Share - 1.0.0
The key is located with a burnt corpse at Helgen Gate. Look at the pictures in the download file.
House itself is in front of Honningbrew Meadery


#### Rich text
In base game every time you kill an enemy your gain +1 for that enemy in your statistics and all players have separate statistics. It means that every player on server have to kill a lot mobs by themself. For me this makes mine explorating frustrating. This mod changes this mechanic to all players have the same statistic. Every time you kill an enemy this kill also count for all other online players. Also you shouldn'y worry if you suddnely disconnects in the middle of caves exploration, your statistics is safe as logs as host player running server. When you reconnect later your statistic will be synced with host player.

#### Available commands
- print_player_kills [player_name] - without arguments print kill list of current player, or print info about player_name kills
_Note_: Actual information about kill is stored in current player data. Original game sync data on save.
- send_kill_message mob_name [count=1] - you should always support mob_name and optional count(number of killed enemies)
- send_sync_message player_name - send sync message to player_name

#### Compatibility
Should work with all mods, also with mods that add custom mobs.

[Source code](https://github.com/Veniamin-Arefev/StardewMods).