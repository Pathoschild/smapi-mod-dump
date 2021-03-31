**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/QuestFramework**

----

# Console commands

There are list of available console commands in Quest Framework for debug your work.

Command name       | Description                     | Example of usage
------------------ | ------------------------------- | ----------------
quests_list        | List all managed quests         | `quests_list`
quests_log         | List all managed quests which are in player's quest log | `quests_log`
quests_stats       | Show quest statistics. Available stats: accepted, completed, removed, summary `<fullQuestName>` | `quests_stats accepted` or `quests_stats abigail_amethyst@purrplingcat.testquests`
quests_invalidate  | Invalidate quest assets cache   | `quests_invalidate`
quests_accept      | Accept managed quest and add it to questlog | `quests_accept abigail_amethyst@purrplingcat.testquests`
quests_complete    | Complete managed quest in questlog | `quests_complete abigail_amethyst@purrplingcat.testquests`
quests_remove      | Remove managed quest from questlog | `quests_remove abigail_amethyst@purrplingcat.testquests`
quests_customtypes | Show exposed custom quests ready to use in content packs | `quests_customtypes`
