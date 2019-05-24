# Hedge Fences

## Description:

**Hedge Fences** is a Content Patcher mod which replaces one of the fence items in the game with a hedge. The base hedge is based closely on the graphics of the the maze from the Spirit's Eve festival. There are a variety of Customization options which can be set in the `config.json` file; this file is not included with the download and will be automatically created the first time the game is run with the mod loaded.

## Installation:

1. Install the latest version of [SMAPI](https://smapi.io/).
2. Install the latest version of [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915).
3. Unzip the download and place the included `[CP] Hedge Fences` folder into your `Mods` folder.

## Configuration:

* **ReplaceFence** lets you select which fence item will be replaced. Choices are *hardwood* (default), *wood*, *stone*, or *iron*.
* **CraftingMaterial** lets you select what item is needed to craft the hedge. Choices are *hardwood* (default), *wood*, or *fiber*.
* **AddFlowers** lets you choose whether or not the hedge will have flowers. Choices are *true* (default) or *false*.
* **FlowerType** lets you choose what color flowers (if **AddFlowers** is true). The mixed option has blue, pink, and purple flowers; the others are various shades of the named color. Choices are *mixed* (default), *blue*, *pink*, and *yellow*. 
* **HedgeShade** lets you choose how dark the hedge should be. Choices are *dark* (default) and *light*.
* **SnowInWinter** lets you choose whether the hedge should be covered in snow during winter. Choices are *all* (default), *half* or *none*.
* **FlowersInWinter** lets you choose whether the flowers (if enabled) still show during winter. Choices are *true* or *false* (default).

## Additional Notes:

* The fence item will be renamed to "Hedge Fence" and will keep the durability amount of the fence which it replaces; so if you replace wood fences they will decay very quickly and if you replace hardwood fences they will decay very slowly. **_The name change will only apply to items that are newly crafted after the mod was installed; hedge fences currently in the save file will still retain their old name_**.
* The Hedge Fence will sell for 1g regardless of what they are replacing and what material was chosen. **_The value change will only apply to items that are newly crafted after the mod was installed; hedge fences currently in the save file will still retain their old sale price_**.
* The crafting recipe **_will not change names_** and will still refer to the fence that was replaced. This is done to prevent possible issues with the *Craft Master*  achievement.
 * Hedge Fences always craft at a 1:1 rate regardless of what they are replacing and what material was chosen.

## Compatibility:

**Hedge Fences** requires [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915) version 1.3 or higher (which means it also requires [SMAPI](https://smapi.io/) version 2.5.4 or higher) and should work with both version 1.2 and 1.3 of Stardew Valley.