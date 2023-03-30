**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Growable Bushes
===========================
![Header image](docs/scroll.gif)

Lets you plop down bushes. Buy them from Caroline's sunroom (the planter on the left), or grab them from the Furniture Catalogue.

## Install

1. Install the latest version of [SMAPI](https://smapi.io).
2. Download and install [AtraCore](https://www.nexusmods.com/stardewvalley/mods/12932) and [SpaceCore](https://www.nexusmods.com/stardewvalley/mods/1348).
2. Download this mod and unzip it into `Stardew Valley/Mods`.
3. Run the game using SMAPI.

## Uninstall
Remove any bush items you have in your inventory, and cut down any bushes you don't want anymore, and then delete from your Mods directory.

A special note for SolidFoundations: **absolutely** make sure to remove every instance of the inventory version of these bushes from Solid Foundations buildings before removing this mod.

## Configuration
Run SMAPI at least once with this mod installed to generate the `config.json`, or use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) to configure.
* `CanAxeAllBushes`: By default you're only allowed to axe bushes that you've placed. Turn this on to axe all bushes.
* `ShopLocation`: The tile that the bush shop is on, in Caroline's Sunroom.
* `ShowBushShopGraphic`: Whether or not to display a little overlay to mark the bush shop's location.
* `ShouldNPCsTrampleBushes`: If turned on, will allow NPCs to remove bushes in their path.
* `RelaxedPlacement`: If enabled, will relax the placement restrictions on bushes. Strongly recommend having [NoClip](https://www.nexusmods.com/stardewvalley/mods/3900) installed if you want to use this, it's possible to get yourself stuck in a bush by placing it on your head.
* `PreserveModData`: mod data is basically something mods can use to put small amounts of data on individual instances of something. If this is enabled, mod data will be copied from the world version of something to the inventory version. This will mean that the inventory versions will stack less nicely, but data added by other mods will persist. For an example: More Fertilizers uses mod data for non-crop fertilizers, so disabling this will remove any bush fertilizer on a bush you move.
* `AllowBushStacking`: whether or not the inventory versions of bushes should stack.
* `ShopCostScale`: I couldn't really decide how much of a gold sink I wanted this shop to be, so this lets you control that! Set it higher to make the bushes more expensive.

## Technical Notes:
* The bushes in your inventory are a custom subclass and I use SpaceCore's serializer for them. Once placed, however, they're the exact same bushes as every other bush. If you remove this mod, those bushes will remain (and, uh, the walnut bushes you put down will become collect-able.)
* You should be able to target the bushes, after placement, with [Alternative Textures](https://www.nexusmods.com/stardewvalley/mods/9246).
* [Destroyable Bushes](https://www.nexusmods.com/stardewvalley/mods/6304) will try to regrow bushes you cut down (if you have that mod set to regrow).
* You can fertilize the medium-sized bushes with the fertilizers in [More Fertilizers](../MoreFertilizers/MoreFertilizers).
* Some of the bushes are simply alternative forms of other bushes in the game and probably won't be recognized by other mods. If you use Destroyable bushes and cut down an Alternative Small Bush, for example, Destroyable Bushes will regrow a Small Bush. The Town Bush and the Medium Bush are also alternative forms, as are the Large Town Bush and the Large Bush.
* If you use bushes to block of NPC pathing, it'll definitely break things. Don't do that.
* **A performance note**: The game is NOT optimized for having a lot of bushes. If you place many bushes on a map where there are also a lot of NPCs/monsters trying to path, expect slowdown.

## Compatibility

* Works with Stardew Valley 1.5.6 on Linux/macOS/Windows.
* Works in single player, multiplayer, and split-screen mode. **Absolutely** has to be installed by everyone in multiplayer.
* Should be compatible with most other mods, including [Destroyable Bushes](https://www.nexusmods.com/stardewvalley/mods/6304). Will work with recolors (I really like [Simple Foliage](https://www.nexusmods.com/stardewvalley/mods/8164), as an example)! Works with [Smart Building](https://www.nexusmods.com/stardewvalley/mods/11158?tab=description). Thank you so much, DecidedlyHuman!
* If Growable Giant Crops is installed, the shovel will be able to lift bushes.
* Specific compatibility notes: This mod uses SpaceCore's serializer.
    - I did not test this mod with Save Anywhere (either version), use at your own risk.
    - It should work fine with SolidFoundations, just remember to remove all instances of the inventory version of the bushes from all SolidFoundations buildings before removing this mod. (Placed bushes should be fine to leave.)

## Much thanks to:
* [Casey](https://www.nexusmods.com/stardewvalley/users/34250790) for...a lot at this point. This mod uses her serializer magic and would not be possible without it.

## See also

[Changelog](docs/changelog.md)
