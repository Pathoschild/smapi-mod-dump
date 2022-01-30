**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/elizabethcd/MoreConversationTopics**

----

# MoreConversationTopics
A mod for Stardew Valley to add conversation topics that need code to add, so dialogue mods can reference them.

## What You Can Use This Mod For

This mod was created primarily for dialogue content packs added by Content Patcher, but since events can use conversation topics as preconditions and Content Patcher can use conversation topics as part of a condition, there is a broad range of possible uses. However, this mod does not implement any of the dialogue/content itself, just the conversation topics. When adding dialogue, it can be added via setting the dialogue key as the relevant conversation topic. 

Some good references when creating content packs that rely on this mod:
   * The wiki page on conversation topics: https://stardewvalleywiki.com/Modding:Dialogue#Conversation_topics
   * Lumisteria Villagers React To Marriage, which is a mod that relies on this mod: https://www.nexusmods.com/stardewvalley/mods/10009

## Current Conversation Topics Added By This Mod
   * `wedding` (triggered when a player gets married)
   * `luauBest` (triggered when the luau soup gets the best result)
   * `luauShorts` (triggered when the Mayor's shorts are added to the luau soup)
   * `luauPoisoned` (triggered when a poisonous item is added to the luau soup)
   * `divorce` (triggered when you get divorced)
   * `babyBoy` (triggered when you have a baby, and it's a boy)
   * `babyGirl` (triggered when you have a baby, and it's a girl)

These conversation topics have default lengths but are also configurable in the config file, with the following variables:
   * `WeddingDuration` (default of 7 days, controls length of `wedding` conversation topic)
   * `LuauDuration` (default of 7 days, controls length of all luau conversation topics)
   * `BirthDuration` (default of 7 days, controls length of `babyBoy` and `babyGirl` conversation topics)
   * `DivorceDuration` (default of 7 days, controls length of `divorce` conversation topic)

## Console Command

This mod also adds some new console commands:

* `current_conversation_topics`, which gives a list of the current active conversation topics. 
* `player_hasmailflag <flagName>`, which tells you whether or not you have that mail flag.
* `add_conversation_topic <topicName>`, which adds the specified conversation topic with a duration of 1 day. (coming soon)
* `remove_conversation_topic <topicName>`, which removes the specified conversation topic. (coming soon)

## Planned Conversation Topics To Be Added
   * `joja_Greenhouse` triggered when the player buys the greenhouse Joja improvement
   * `joja_Complete` triggered in the Joja warehouse celebration cutscene
   * `leoArrival` triggered when Leo moves to the valley
   * `meteor` triggered when a meteor hits the farm
   * `newHorse` triggered when the player names a horse
   * `newPet` triggered when the player names a cat/dog
   * `passedOut` triggered when the player passes out
   * `beachBridgeFixed` triggered when the player fixes the bridge on the beach
   * `playerWonEgghunt` triggered when the current player wins the egghunt
   * `otherPlayerWonEgghunt` triggered when a different player wins the egghunt
   * `AbigailWonEgghunt` triggered when Abigail wins the egghunt (note: needs compatibility check with mods that change who wins the egghunt)

## Some Examples of When It Might Be Advisable to Use Stats as Tokens Or Content Patcher Instead

### Achievements

Most of the achievements rely on stats that are tracked in [Stats as Tokens](https://www.nexusmods.com/stardewvalley/mods/9659). For example, you could trigger dialogue based on amount of money earned with a `When` in Content Patcher. Or you could use the `goodFriends` token to trigger dialogue as the farmer is getting more popular. In terms of the fishing ones, there is the `fishCaught` stat from Stats as Tokens, and also a specific mail item for the Master Angler achievement. 

### Weddings and Births

This mod adds a `wedding` conversation topic, but Stats as Tokens is also planning to track days married, so that you can add dialogue or other things based on anniversaries. This mod adds a `babyBoy` or `babyGirl` conversation topic right when your children get born, but you may also want to use the upcoming Stats as Tokens children tokens to trigger dialogue/events/changes based on how old your children are. 

### Events That Happen Once or Things with Mail Flags

You can use Content Patcher to append `addConversationTopic <ID> [length]` at the end of an event or append `%item conversationTopic <key> <days> %%` at the end of a piece of mail. The only downside of this approach is that you may need to do some tricky Content Patcher tokens work to target all languages, and that you may or may not be compatible with any mods that also edit these events. Alternatively, you can use the `$1` command in dialogue to trigger some dialogue exactly once. Because of this, some conversation topics that could be added with events/mail flags have been added by this mod, but they are generally going to be lower priority. 
