**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/desto-git/smapi-RegularQuality**

----

# Gift Decline

Inhabitants will like items less each time you gift them that same item. Stop littering them with low-effort gifts!

## Installation

- Download the mod zip ->
	[GitHub](https://github.com/desto-git/sdv-mods/releases),
	[Nexus](https://www.nexusmods.com/stardewvalley/mods/6944/)
- Download and install [SMAPI](https://smapi.io/)
- Unzip the mod into Stardew Valley's Mods folder

## Configuration

- `ResetEveryXDays: int` = Reset all gift tastes to their original value after this many days have passed,
starting from the very first day (Day 1, Year 1).  
If set to `0`, gift taste will never be reset. Defaults to `112` (= yearly)
- `MaximumDeviation: int` = Limit by how much the taste for a gift can drop.
E.g. if set to `2`, a loved gift can not fall below a neutral reaction.
- `ReduceAfterXGifts: int` = Reduce the gift taste only after the item has been gifted to the NPC this many times.

## CLI

- `reset_gift_tastes` = Reset all gift tastes to their original value.

## Multiplayer

The gift taste decline is shared across all players.
So if you gift an item to an NPC and then your friend gifts the same item to that same NPC,
your friend will already get the subpar reaction.

You could counteract that by setting `ReduceAfterXGifts` to the number of players.