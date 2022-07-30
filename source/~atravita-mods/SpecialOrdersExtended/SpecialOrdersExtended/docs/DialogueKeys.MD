**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Dialogue Keys
========

New in 1.0.2, there's dialogue keys. Special Order Dialogue Keys will only ever be said once. They use the format `<preface><questkey>_<queststatus><predicate>`, and should be added to a character's normal dialogue file using Content Patcher (`Characters/Dialogue/<charactername>`)

Quest key is the key for the quest (as it is in the `SpecialOrders` file). Quest status is either `Completed` or `InProgress`. (The status `Failed` is technically also available, but I don't think you can actually talk to a NPC in that time.) `Completed` dialogue will persist to the end of the day after quest completion. Additionally, `IsAvailable` will reference quests currently available on the Quests board, and `RepeatOrder` can be used when the quest has been completed at least once before. `RepeatOrder` keys will also be cleared seven days after the successful completion of a quest.

Preface/predicate limits when the key can be used, and follows a similar structure as to vanilla's location keys. `<Preface>` can be any season, and season-specific keys will always take precedence. Predicate can be any of the following values, in priority order:

1. `_<short day of week>`
2. `<heartlevel>` (where valid heart levels are even numbers, checked counting down - ie 14,12,10,8,6,4,2 in vanilla)
3. No predicate.

For example: if I have a quest key that's `atravita.ValleyFlowers` and I'm writing completion dialogue, the dialogue keys would be `<preface>atravita.ValleyFlowers_Completed<predicate>`. If I wanted the dialogue to only appear on Friday's in spring, I'd use `springatravita.ValleyFlowers_Completed_Fri` . If I want the text to appear any time, I'd use `atravita.ValleyFlowers_Completed` .

If I wanted a bit of dialogue to be said while the quest is in progress, I would use `atravita.ValleyFlowers_InProgress`, which will only ever be said once while the quest is in progress. On the other hand, `atravita.ValleyFlowers_RepeatOrder` can only be said while the quest is in progress AND has been completed at least once before.

The NPC's original dialogue is held in a queue and re-added after an in-game hour.

### Storage

To prevent dialogue keys from this mod from becoming too spammy, they're only ever said once (except for `RepeatOrder` keys, but those have a cooldown period of seven days after the completion of the Special Order). As of 1.0.4+, previously seen keys are stored in the global mod data, locally (`.smapi/mod-data/atravita.specialordersextended`). The console command `special_orders_dialogue` is included to look at and and/remove from this data.

Call with: `special_orders_dialogue <add/remove/hasseen> <dialogue key> <one or more character names>`.

Note: Save Anywhere will probably cause keys to fail to be saved. I'm looking into it, but it's not particularly high on my list.