**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Veniamin-Arefev/StardewMods**

----

Adventurers Guild Multiplayer Shared Kills

#### Rich text
In a vanilla game, every time you kill a monster, you gain +1 for that monster in your statistics, and all players have separate statistics. This means that every player on the server has to kill a lot of mobs by themselves. For me, this makes mine exploration frustrating. This mod changes it so that all players share the kill statistics, and every time you kill a monster, this kill also counts for all other online players. Also, you no longer need to worry if you suddenly disconnect in the middle of cave exploration: your statistics are safe as long as the host player is running the server. When you reconnect later, your statistics will be synced with the host player.

For players who already have started farm with friends and want shared statistics this mod is also safe to install. On players connect to server mod will syncronize kills among all online players. Note, that mobs in [Monster Eradication Goals](https://stardewvalleywiki.com/Adventurer%27s_Guild#Monster_Eradication_Goals) actually are groups of mobs: if first player killed 5 green slimes and 1 red, second player killed 3 green and 7 red slimes their total kills os 5 green and 7 red slimes. But in guild you will see for first player 6 slimes, for second 10 slimes, but after syncing kills a total of 12 slimes. Dont be surprized by suddenly growing numbers.

#### Available commands:

- print_player_kills [player_name]: Without arguments, print the kill list of the current player or print information about player_name kills.  
   _Note_: Actual information about kills is stored in the current player's data. The original game syncs data on save.
- send_kill_message mob_id [count=1]: You should always support mob_id and optionally include the count (number of killed enemies). Monster ID's can be found [there.](https://stardewvalleywiki.com/Modding:Monster_data)
- send_sync_message player_name: Send a sync message to player_name.

#### Compatibility

This mod should work with all other mods, including those that add custom mobs.


[Source code](https://github.com/Veniamin-Arefev/StardewMods).