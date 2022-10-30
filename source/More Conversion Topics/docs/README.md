**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/elizabethcd/MoreConversationTopics**

----

# MoreConversationTopics
A mod for Stardew Valley to add conversation topics that need code to add, so dialogue mods can reference them.

## What You Can Use This Mod For

This mod was created primarily for dialogue content packs added by Content Patcher, but since events can use conversation topics as preconditions and Content Patcher can use conversation topics as part of a condition, there is a broad range of possible uses. However, this mod does not implement any of the dialogue/content itself, just the conversation topics. When adding dialogue, it can be added via setting the dialogue key as the relevant conversation topic. This mod can also be used to make conversation topics added by other mods repeatable.

Some good references when creating content packs that rely on this mod:
   * The wiki page on conversation topics: https://stardewvalleywiki.com/Modding:Dialogue#Conversation_topics
   * Lumisteria Villagers React To Marriage, which is a mod that relies on this mod: https://www.nexusmods.com/stardewvalley/mods/10009

## Current Conversation Topics Added By This Mod
Since there are a lot of conversation topics added by this mod, I've divided them up into a few sections. **Spoilers for endgame and Easter-egg type content here!**

### Social Conversation Topics
These are related to various social events that happen in the game.
   * `wedding` (triggered when a player gets married, repeatable)
   * `divorce` (triggered when you get divorced, repeatable)
   * `babyBoy` (triggered when you have a baby, and it's a boy, repeatable)
   * `babyGirl` (triggered when you have a baby, and it's a girl, repeatable)
   * `luauBest` (triggered when the luau soup gets the best result, repeatable)
   * `luauShorts` (triggered when the Mayor's shorts are added to the luau soup, repeatable)
   * `luauPoisoned` (triggered when a poisonous item is added to the luau soup, repeatable)

### Joja-Route Missing Conversation Topics
There are a few conversation topics missing when you go the Joja route, as compared to the Community Center route.
   * `joja_Greenhouse` (triggered when you buy the greenhouse repair from Joja)
   * `joja_Complete` (triggered during the Joja Community Development Program completion ceremony)

### Events Around Town
Some of the events that happen around town never get commented on by the townspeople—now they can! 
   * `jojaMartStruckByLightning` (triggered after Community Center completion on a rainy day)
   * `willyBoatRepaired` (triggered when Willy's boat in the back room is repaired)
   * `leoValleyArrival` (triggered when Leo moves to the valley)
   * `islandResortUnlocked` (triggered when the island resort is repaired)

### Overnight Events on Your Farm
Some of these might go unnoticed by most townspeople, but the more magically-inclined ones (or your spouse) might take an interest.
   * `UFOLandedOnFarm` (triggered when the Strange Capsule lands on your farm)
   * `meteoriteLandedOnFarm` (triggered when a meteorite lands on your farm, repeatable)
   * `owlStatueLandedOnFarm` (triggered when an owl statue lands on your farm, repeatable)
   * `railroadEarthquake` (triggered when the railroad is opened by an earthquake)
   * `witchSlimeHutVisit` (triggered when the witch visits your slime hutch, repeatable)
   * `witchCoopVisit` (triggered when the witch visits your coop, repeatable)
   * `goldenWitchCoopVisit` (triggered when the witch visits your coop post-perfection, repeatable)
   * `fairyFarmVisit` (triggered when the crop fairy visits your farm, repeatable)

## Configuration
In general, players can ignore the config options, although GMCM support is included in case they do want to change them. If they would like any conversation topics to be skipped, they can try setting the length to 0—depending on the conversation topic this will minimize the number of time it gets commented on or eliminate it altogether.

All the conversation topics have default lengths but are also configurable in the config file, with the following values:
   * `WeddingDuration` (default of 7 days, controls length of `wedding` conversation topic)
   * `BirthDuration` (default of 7 days, controls length of `babyBoy` and `babyGirl` conversation topics)
   * `DivorceDuration` (default of 7 days, controls length of `divorce` conversation topic)
   * `LuauDuration` (default of 7 days, controls length of all luau conversation topics)
   * `JojaGreenhouseDuration` (default of 3 days to match ``cc_Greenhouse`` in the game, controls length of ``joja_Greenhouse`` conversation topic)
   * `JojaCompletionDuration` (default of 4 days to match ``cc_Complete`` in the game, controls length of ``joja_Complete`` conversation topic)
   * `JojaLightningDuration` (default of 7 days, controls length of `jojaMartStruckByLightning` conversation topic)
   * `WillyBoatRepairDuration` (default of 7 days, controls length of `willyBoatRepaired` conversation topic)
   * `LeoArrivalDuration` (default of 7 days, controls length of `leoValleyArrival` conversation topic)
   * `UFOLandedDuration` (default of 7 days, controls length of `UFOLandedOnFarm` conversation topic)
   * `MeteoriteLandedDuration` (default of 7 days, controls length of `meteoriteLandedOnFarm` conversation topic)
   * `OwlStatueDuration` (default of 7 days, controls length of `owlStatueLandedOnFarm` conversation topic)
   * `RailroadEarthquakeDuration` (default of 7 days, controls length of `railroadEarthquake` conversation topic)
   * `WitchVisitDuration` (default of 7 days, controls length of all witch visit conversation topics)
   * `FairyVisitDuration` (default of 7 days, controls length of `fairyFarmVisit` conversation topic)
   * `IslandResortDuration` (default of 7 days, controls length of `islandResortUnlocked` conversation topic)

## Adding Repeatable Conversation Topics
This mod allows other mods to add repeatable conversation topics by using Content Patcher to edit the file `Mods/vl.mct/RepeatableTopics` with EditData. You can add new repeatable conversation topics by adding them as new entries, with any string as the value. An example CP pack will be provided. 

It is possible to remove the conversation topics added by this mod from the list of repeatable conversation topics. Please don't do this in a released mod unless you have a good reason to do so, as it could cause compatibility issues with any other mods that depend on this one or unnecessary bug reports.

## Console Commands
This mod also adds some new console commands:
   * `vl.mct.current_CTs`, which prints a list of the current active conversation topics and their durations to the console. 
   * `vl.mct.has_flag <flagName>`, which tells you whether or not you have that mail flag.
   * `vl.mct.add_CT <topicName> <duration>`, which adds the specified conversation topic with a duration of 1 day by default, or can specify duration.
   * `vl.mct.remove_CT <topicName>`, which removes the specified conversation topic.
   * `vl.mct.is_repeatable_CT <topicName>`, which tells you whether or not the specified conversation topic is repeatable.
   * `vl.mct.repeatable_CTs`, which prints a list of the repeatable conversation topics to the console.

## Planned Conversation Topics To Be Added

**Spoilers for endgame and Easter-egg type content here!**
   * `newHorse` triggered when the player gets a horse
   * `newCat` triggered when the player adopts a cat
   * `newDog` triggered when the player adopts a dog
   * `childrenDoved` triggered when the player turns their children into doves
   * `newBarn` triggered when the player builds a barn
   * `newCoop` triggered when the player builds a coop
   * other `newBuildingName` triggered when the player builds a building
   * `UFOEscaped` triggered when the Strange Capsule cracks open
   * `passedOutExhausted` triggered when the player passes out due to low energy
   * `passedOutInjured` triggered when the player passes out due to low health
   * `beachBridgeFixed` triggered when the player fixes the bridge on the beach
   * `playerWonEgghunt` triggered when the current player wins the egghunt
   * `otherPlayerWonEgghunt` triggered when a different player wins the egghunt
   * `AbigailWonEgghunt` triggered when Abigail wins the egghunt (note: will need compatibility check for mods that change who wins the egghunt)
   * `playerSkippedFestival` triggered when the player skips a festival (note: this is a fairly complex idea and may not be implemented)

## Some Examples of When It Might Be Advisable to Use Stats as Tokens, Custom Tokens, Or Content Patcher Instead

### Achievements

Most of the achievements rely on stats that are tracked in [Stats as Tokens](https://github.com/dtomlinson-ga/StatsAsTokens#readme). For example, you could trigger dialogue based on amount of money earned with a `When` in Content Patcher. Or you could use the `goodFriends` token to trigger dialogue as the farmer is getting more popular. In terms of the fishing ones, there is the `fishCaught` stat from Stats as Tokens, and also a specific mail item for the Master Angler achievement. 

### Weddings and Births

This mod adds a `wedding` conversation topic, but [Custom Tokens](https://github.com/TheMightyAmondee/CustomTokens#readme) adds an anniversary token and tracks number of years married, so that you can add dialogue or other things based on anniversaries. This mod adds a `babyBoy` or `babyGirl` conversation topic right when your children get born, but Custom Tokens also tracks various information about your children, including age and birthday.

### Events That Happen Once or Things with Mail Flags

You can use Content Patcher to append `addConversationTopic <ID> [length]` at the end of an event or append `%item conversationTopic <key> <days> %%` at the end of a piece of mail. The only downside of this approach is that you may or may not be compatible with any mods that also edit these events. Alternatively, you can use the `$1` command in dialogue to trigger some dialogue exactly once. Because of this, some conversation topics that could be added with events/mail flags have been added by this mod, but they are generally going to be lower priority. 
