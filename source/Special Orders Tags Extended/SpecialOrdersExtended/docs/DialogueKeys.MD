**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/SpecialOrdersExtended**

----

Dialogue Keys
========

New in 1.0.2, there's dialogue keys. Special Order Dialogue Keys will only ever be said once. They use the format `<preface><questkey>_<queststatus><predicate>`, and should be added to a character's normal dialogue file using Content Patcher (Characters/Dialogue/<charactername>)

Quest key is the key for the quest (as it is in the `SpecialOrders` file). Quest status is either `Completed` or `InProgress`. (The status `Failed` is technically also available, but I don't think you can actually talk to a NPC in that time.) Note that completion dialogue goes away when the quest reward is collected out of the journal.

Preface/predicate limits when the key can be used, and follows a similar structure as to vanilla's location keys. <Preface> can be any season, and season-specific keys will always take precedence. Predicate can be any of the following values, in priority order:

1. `_<short day of week>`
2. `<heartlevel>` (with valid heart levels 14,12,10,8,6,4,2, checked in that order)
3. No predicate.


ie: if I have a quest key that's `atravita.ValleyFlowers` and I'm writing completion dialogue, the dialogue keys would be `<preface>atravita.ValleyFlowers_Completed<predicate>`. If I wanted the dialogue to only appear on Friday's in spring, I'd use `springatravita.ValleyFlowers_Completed_Fri` . If I want the text to appear any time, I'd use `atravita.ValleyFlowers_Completed` 