**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/SlivaStari/ManyEnchantments**

----

# ManyEnchantments
SDV mod to all enchanting tools with multiple enchantments.

Allows enchanting tools with more than one enchantment, and forging weapons with up to three of each gemstone.

For tools and weapons, they may each be enchanted with as many enchantments as are applicable. Enchantments are still added in a random order. Unforging a tool or weapon will remove ALL ENCHANTMENTS on it (but not Galaxy Souls added to weapons).

For weapon forging, each gemstone forging type may be applied up to three time - this takes an increasing amount of cinder shards depending on the total number of forgings previously applied. The formula for this is (previous_forgings * 5) + 10, so the first is 10, the next is 15, then 20, 25, 30, etc. As part of this, diamond usage has been rebalanced, so you get 3 random forgings that can be applied. However, the cost is 3 times the current cost (eg the first time would be 30) - this still saves 15 cinder shards, so is generally still worth it.

Should be widely compatible with additional most mods, such as those adding tools (such as Prismatic Tools) and weapons, including those added through Json Assets (there is no risk of shuffle for this mod). Compatible with Combine Many Rings.
