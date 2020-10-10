**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/desto-git/sdv-mods**

----

# Regular Quality

Drop the quality level of all items to be regular quality. Less clutter and less profit!

Community Center bundle requirements are updated accordingly.
Compatible with [Minerva's Harder Community Center Bundles](https://www.moddrop.com/stardew-valley/mods/580704).

## Installation

- Download the mod zip ->
	[GitHub](https://github.com/desto-git/sdv-mods/releases),
	[Nexus](https://www.nexusmods.com/stardewvalley/mods/5090/)
- Download and install [SMAPI](https://smapi.io/)
- Unzip the mod into Stardew Valley's Mods folder

## Configuration

- `BundleIngredientQualityMultiplicator: int` = Increase the quantity of ingredients for community center bundles according to the following table:

Quality     | * | Multiplier | = | Result
----------- | - | ---------- | - | ------
0 (Regular) | * | 4          | = | 0 (don't change quantity)
1 (Silver)  | * | 4          | = | 4 (add 4x the amount on top, so basically *5)
2 (Gold)    | * | 4          | = | 8 (*9)
4 (Iridium) | * | 4          | = | 16 (*17)

## Existing save

You can use it on an existing save. However, the quality will not be dropped right away.
The quality will be removed once you add items to your inventory.
If you remove the mod, new items will have quality again. But already changed items will not be restored.

## Limitations:

Some changes would require [Harmony](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Harmony),
which I don't intent to use at the moment.

- Item quality is adjusted only AFTER it has been added to the inventory.
	Because of this, you should always keep an inventory slot open to be able to pick up items.
- The "Botanist" profession is useless, because the quality loot will be regular quality after pickup.
- You can't get the best response for the potluck soup, as it requires at least silver quality items.
- Winning first place for the grange display is really hard, if not impossible.

## TODO
- Option to keep the quality on aged items (Enabled by default).
	This way it is still possible to get the best rating for the potluck soup
- https://stardewvalleywiki.com/Cask#Aged_Values