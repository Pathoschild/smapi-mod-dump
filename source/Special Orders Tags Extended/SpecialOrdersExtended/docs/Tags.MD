**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/SpecialOrdersExtended**

----

Tags
============

Tags are used to restrict the appearance of special orders. Tags can be negated with a `!` (ie `!season_winter` for any season not winter). Required tags cannot be combined with `/` (ie `season_spring/season_fall` will never be true). I am considering implementing this. Maybe later.

#### Vanilla tags:

1. `NOT_IMPLEMENTED`: Always false
2. `dropbox_<dropboxName>`: Whether or not the dropbox with the specific name is in use by a quest already.
3. `rule_<ruleName>`: Whether or not the specific special order rule is active.
4. `completed_<specialOrderKey>`: Whether or not the special order has been completed yet.
5. `season_<season>`: Restricts quest to only a specific season.
6. `mail_<mailKey>`: Does the master player have a specific mail key. Mail keys are used for far more than just for mail; they also keep track of a lot of game state.
7. `event_<eventID>`: Has the master player seen a specific event?
8. `island`: Has the island been unlocked?
9. `knows_<NPCname>`: Has *any* farmer met the NPC?


#### Mod tags:

1. `year_<num>`: Is it currently the specified year? Best used negated - ie `!year_1` for quests that should not appear the first year.
2. `atleastyear_<num>`: if it's at least the specified year.
3. `yearunder_<num>`: If the current year is strictly less than the specified year
4. `week_<num>`: If it's the specific week of the month. ie `week_1` for a quest you only want to appear in the first week of the month.
5. `daysplayed_<num>`: If a certain number of in-game days have been played. (use `daysplayed_under_<num>` for strictly less than).
6. `dropboxRoom_<MapName>`: Whether or not a dropbox is active (in use by a current quest) on the *map*. No negation key. Added in 1.0.2. This is to facilitate custom dropbox locations, as the map edits for custom dropboxes can override each other. To reduce the likelihood of that, you can use the token `{{atravita.SpecialOrdersExtended/Current|contains=questkey}}`: to only load your dropboxes when the quest is active, and use this tag to prevent your order from appearing on the board if someone else's mod has an active dropbox in that location. See tokens page for more.
7. `conversation_<conversationTopic>`: If *any* player has the conversation topic active. Conversation topics are regularly used in-game to delay events, and this is basically my intent with this one. Negate with `conversation_<conversationTopic>_not`.(Added in 1.0.1)
8. `haskilled_<Monster-Name>_<num>`: If any farmer has killed at least a specific number of the enemy. `Monster-Name` is the exact name of the monster with spaces traded for dashes - ie `haskilled_Green-Slime_1000`. Note that these are exact enemy names and the game doesn't combine similar monsters. `haskilled_Green-Slime_1000` will not count Frost Jellies. Use stats_slimesKilled if you want all slimes. haskilled_<Monster-Name>_under_<num> for strictly under.
9. `friendship_<NPCname>_<num>`: If any farmer has at least the required amount of friendship points with the NPC in question. ie `friendship_Robin_2500` = at least ten hearts with Robin. `friendship_<NPCname>_under_<num>` for strictly under.
10. `minelevel_<num>`: Whether or not any farmer has reached the mine level indicated. (Values > 120 refer to the Skull Cavern). `minelevel_under_<num>` for strictly under.
11. `houselevel_<num>`: Whether or not any farmer has the house upgrade referenced. Mostly useful for checking if anyone has a basement, I think. `houselevel_under_<num>` for strictly under.
12. `moneyearned_<num>`: Whether or not players have earned a certain amount of money.
13. `skillllevel_<skillname>_<num>`: Whether any farmer has the required skill level (note: three l's). `skilllevel_<skillname>_under_<num>` for strictly under.
14. `achievement_<key>`: Whether any farmer has the achievement in question, where `key` is the key in `Data/achievements`. Use `achievement_<key>_not` to negate.
14. `hasspecialitem_<item>`: If any farmer has the special item. Valid items: `clubCard`, `specialCharm`, `skullKey`, `rustyKey`, `translationGuide`, `townKey`. Use `hasspecialitem_<item>_not` for the inverse - if NO farmer has the `clubCard`, etc.
15. `craftingrecipe_<recipe>`: If any farmer has the recipe. Negate with `craftingrecipe_<recipe>_not`. (Added in 1.0.1)
16. `cookingrecipe_<recipe>`: Same as above, just for cooking. Also added in 1.0.1.
17. `stats_<statskey>_<num>`: If any farmer has any of the very long list of stats tracked by the game itself, minus `averageBedTime`, `totalMoneyGifted`, and `timesUnconscious`. `stats_<statskey>_under_<num>` for strictly less than. A list of stats is available from the console command `list_available_stats`. Stats provided by other mods will also be included. Hardcoded stats are not case-sensitive, but the ones in the stats dictionary are.
18. `walnutcount_<num>`: If the farmer team as at least that many walnuts. Use `walnutcount_under_<num>` for strictly under. Added in 1.0.4.