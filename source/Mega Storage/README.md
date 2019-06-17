# Stardew-MegaStorage

Adds Large Chests and Magic Chests to Stardew Valley.

# Large Chest
Capacity: 72 items (6 rows).

Recipe:
* 100 Wood
* 1 Copper Bar
* 1 Iron Bar
 
# Magic Chest
Infinite scrollable capacity. Might add filters/categories and searching in the future.

Recipe:
* 200 Wood
* 10 Solar Essense
* 10 Void Essense

# Configuration
Recipes are configurable in config.json. The format is pairs of item IDs and how many of that item to use. Default:
```
{
  "LargeChestRecipe": "388 100 334 1 335 1",
  "MagicChestRecipe": "388 200 768 10 769 10"  
}
```

# Compatibility
* Requires [SMAPI](https://smapi.io/).
* Supports multiplayers and controllers.
* Compatible with Automate and mods for Json Assets and Content Patcher.
* Partially compatible with Chests Anywhere. It works, but opening Large Chest and Magic Chest through Chests Anywhere will show them as normal chests.

# Is this safe?
Before saving, all Large Chests and Magic Chests are converted to normal chests. After saving, they are converted back. This makes sure your items aren't lost, even if uninstalling this mod. Normal chests actually have infinite capacity, it's only when adding items one at a time they are limited to 36 capacity.

# Credits
* Inspired by [Magic Storage](https://forums.terraria.org/index.php?threads/magic-storage.56294/) for Terraria.
* Custom sprites: Revanius.

[Nexus page](https://www.nexusmods.com/stardewvalley/mods/4089)
