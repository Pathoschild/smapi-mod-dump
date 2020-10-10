**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ZaneYork/SDV_Mods**

----

# Custom Crops Decay #

Makes crops be able to decay after several days of harvest, decay crop will transform into trash.

## Description ##
Mod's config file instructions:

| Config Name | Description   |
| ------------ | ------------ |
| FridgeEffect | Define the effect of fridge, 0.7 means 70% slower than origin decay speed |

Mod's content pack instructions:

| Config Name | Description   |
| ------------ | ------------ |
| crops | The list of crop rules |
| crops.id | Locate by id, the crop with this id will take this rule |
| crops.name | Locate by name, the crop with this name will take this rule |
| crops.category | Locate by category, the crop with this category will take this rule |
| crops.decayDays | The decay days of crop, key value pair, the key is quality(0 Normal/1 Silver/2 Golden/4 Iridium) |

# Dynamic Price #

Makes items have dynamic sell price, associate with date and decay percentage.

## Description ##
Mod's config file instructions:

| Config Name | Description   |
| ------------ | ------------ |
| DynamicWithDate | Should price associate with date |
| DynamicWithDecay | Should price associate with decay percentage, needs CustomCropsDecay mod to work |
| Seed | Seed for randomizer |
| ChangeRateMultiplier | The factor for change rate |
| Discount | Discount rule for crop decay |
| Discount.Min | Min discount for crop  |
| Discount.Max | Max discount for crop |
