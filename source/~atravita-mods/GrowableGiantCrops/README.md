**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Growable Giant Crops (Shovel Mod)
===========================
![Header image](docs/showcase.gif)

Lets you pick up and put down a lot of things. With a shovel you can buy from the Witch's hut. Also lets you buy certain things from either Robin or the Witch.

## Install

1. Install the latest version of [SMAPI](https://smapi.io).
2. Download and install [AtraCore](https://www.nexusmods.com/stardewvalley/mods/12932) and [SpaceCore](https://www.nexusmods.com/stardewvalley/mods/1348).
2. Download this mod and unzip it into `Stardew Valley/Mods`.
3. Run the game using SMAPI.

## Uninstall
Remove any large inventory items you have in your inventory or any storage, and then delete from your Mods directory.

A special note for SolidFoundations: **absolutely** make sure to remove every instance of the inventory version of fruit trees, trees, giant crops, and resource clumps from Solid Foundations buildings before removing this mod.

## Configuration
Run SMAPI at least once with this mod installed to generate the `config.json`, or use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) to configure.

#### General options

* `RelaxedPlacement`: Relaxes placement rules for the giant crops, resource clumps, fruit trees, and trees. This will let you place fruit trees closer together than the game would normally allow, for example.
* `PlacedOnly`: If enabled, the shovel will only pick up large items you've placed.
* `ShovelEnergy`: How much energy should each use of the shovel on a large item take? Defaults to 7.
* `ShovelDoesDamage`: If enabled, the shovel will do a little damage when it comes into contact with monsters, and also the items thrown into the air by the shovel do damage.

#### Shop options

* `ShowShopGraphics`: Whether or not to show the overlay graphics that indicate where a shop is. 
* `GiantCropShopLocation` and `ResourceShopLocation` control where the shops are in the Witch's hut and Robin's desk, respectively. Use this to move the shops around if another mod edits those maps.

#### Large item options

* `ShouldNPCsTrampleResourcesClumps` and `ShouldNPCsTrampleGiantCrops` will let NPCs remove giant crops and resource clumps in their way. 
* `PreserveModData`: mod data is basically something mods can use to put small amounts of data on individual instances of something. If this is enabled, mod data will be copied from the world version of something to the inventory version. This will mean that the inventory versions will stack less nicely, but data added by other mods will persist. For an example: More Fertilizers uses mod data for non-crop fertilizers, so disabling this will remove any fruit tree fertilizer.
* `AllowLargeItemStacking`: Controls whether or not the inventory versions of large items stack.
* `PlacedTreesSpread`: Controls whether or not trees placed on farm can spread.
* `MaxTreeStage` and `MaxFruitTreeStage` define the largest trees the shovel can pick up, where the stage numbers match the ones on the wiki: [fruit trees](https://stardewvalleywiki.com/Fruit_Trees), [normal trees](https://stardewvalleywiki.com/Trees#Growth_Cycle). Set to 0 to prevent.
* `PalmTreeBehavior`: What palm trees should behave like, outside the desert. `Default` is desert-like behavior. `Stump` is mushroom-tree like behavior. And `Seasonal` makes palm trees use seasonal skins.

#### Grass options
* `ShouldAnimalsEatPlacedGrass`: If disabled, animals will skip over and avoid eating grass placed by this mod. (Animals will never eat cobwebs.)
* `ShouldPlacedGrassSpread`: If disabled, placed grass will never spread to a neighboring tile.
* `ShouldPlacedGrassIgnoreScythe`: If enabled, placed grass will be immune to the scythe and all weapons.

#### Misc options
* `CanSquishPlacedSlimeBalls`: If disabled, slime balls you've picked up and moved are not squish-able.
* `PreservePlacedWeeds`: If enabled, placed weeds will not die in winter.

## Items
* **Grass starters**: After getting a single heart from Robin and getting her letter about the first house expansion, Robin will sell grass starters corresponding to every type of grass you can see in game. You can also get these grass starters by using the shovel on any clump of grass in the game (including that in the mines). For balance reasons, the grass starters will only put down a single tuft of grass, but the grass will grow normally and fill out over time. Additionally, you can choose if animals will eat placed grass or if placed grass will spread at all. Grass from these starters will not die in winter.
   - Note that internally, those cobwebs are actually grass.
* **Resource clumps**: These include the big rocks you can find in the mines, the large stumps and hollow logs, and the meteorite. They are purchasable from Robin after you've gotten the skull key, except for the meteorite, which is locked behind getting level 25 in the skull cavern. You can also use the shovel to lift them.
* **Small terrain items**: These include the little nodes, twigs, weeds, etc. The little crystals in the frost levels of the mines are actually internally weeds. Placed weeds won't spread, and optionally won't die in winter. A small selection is sold at the witch hut shop every day. These are also lift-able from the mines with the shovel. (Custom Ore Nodes/Custom Resource Clumps isn't currently supported, though.)
* **Giant Crops**: A small selection of giant crops are available every day through the shop in the Witch's hut, or you can lift them with the shovel if they happen to grow for you.
* **Fruit trees**: Can be transplanted with the shovel. Note that stumps of fruit trees cannot be moved - you can use [this mod](https://www.nexusmods.com/stardewvalley/mods/15005) to revive them though.
* **Trees**: Can be transplanted with the shovel. Small versions will be sold in the Witch's hut, and unlike fruit trees, stumps of trees can be moved.

A note on bushes: this mod doesn't deal with bushes, but if you have [Growable Bushes](https://www.nexusmods.com/stardewvalley/mods/15111) installed, the shovel can lift bushes.

## Shops
* Robin's shop opens up after you get a single heart from her and you've gotten her letter on the first house expansion. She'll carry grass starters, and also will carry resource clumps after you've reached the bottom of the mines. These grass starters will not die in winter, and you can control if they spread or not.
* The Witch Hut shop will open when you reach that location. It'll carry the shovel (if you're not currently carrying one in your inventory), a selection of giant crops, nodes, twigs, weeds, and some wild trees. After perfection, the limits on this store are removed.

## Technical Notes:
* The large items (fruit trees, trees, giant crops, and resource clumps) in your inventory are a custom subclass and I use SpaceCore's serializer for them. Once placed, however, they're the exact same as every other resource clump and/or tree. If you remove this mod, those will remain and probably be fine. Resource clumps may disappear if you don't have another mod persisting them.
* Small items like the rocks and twigs are actually in the game normally and are fine, although you won't be able to place them if you remove this mod. The various grass starters from this mod will revert to being normal grass starters.
* This mod prevents decor weeds from spreading, etc. If you remove it, your decorative weeds will start to spread.
* You should be able to target everything, after placement, with [Alternative Textures](https://www.nexusmods.com/stardewvalley/mods/9246).
* If you use giant crops or resource clumps to block of NPC pathing, it'll definitely break things. Don't do that.
* **A performance note**: The game is NOT optimized for having a lot of resource clumps everywhere. If you place many giant crops or resource clumps on a map where there are also a lot of NPCs/monsters trying to path, expect slowdown.

## Console commands
These were primarily used for debugging but exist if you want to spawn things directly in your inventory.

* `av.ggc.add_shovel` adds the shovel to your inventory
* `av.ggc.add_giant <identifier> [count] [GiantCropTweaksId]` as an inventory giant crop to your inventory, where the identifier is either the int ID or the name of the product, count is the stack count (optional, defaults to 1), and GiantCropTweaksId will use the GiantCropTweaks version if that is specified and it's valid. For example:
    - `av.ggc.add_giant Melon` to add a giant melon to your inventory. Note that identifiers are case sensitive.
* `av.ggc.add_resource <identifier> [count]` to add a resource clump to your inventory.
* `av.ggc.add_grass <identifier> [count]` to add grass starters to your inventory.
* `av.ggc.add_tree <identifier> [count] [stage]` to add inventory tree items to your inventory, where stage is the internal stage (ie 3 for `Tree.bushStage`)
* `av.ggc.add_fruittree <identifier> [count] [stage]` to add inventory fruit trees to your inventory, where identifier is the sapling and stage is the internal stage.
    - `av.ggc.add_sapling "Apple Sapling" 1 3` (note the quote marks because Apple Sapling has a space in it) to spawn a stage-3 apple sapling in your inventory.

## Compatibility

* Works with Stardew Valley 1.5.6 on Linux/macOS/Windows.
* Works in single player, multiplayer, and split-screen mode. **Absolutely** has to be installed by everyone in multiplayer.
* Should be compatible with most other mods. Tested with [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720)'s giant crops and [More Giant Crops](https://www.nexusmods.com/stardewvalley/mods/5263), as well as giant crops from [Giant Crop Tweaks](https://www.nexusmods.com/stardewvalley/mods/14370).
* Works with [More Grass Starters](https://www.nexusmods.com/stardewvalley/mods/1702). Note that the starters from More Grass Starters and the starters from this mod are different and will act differently. More Grass Starters starters are found at Pierre's, mine are at the little resource shop on Robin's desk.
* Specific compatibility notes: This mod uses SpaceCore's serializer.
    - I did not test this mod with Save Anywhere (either version), use at your own risk.
    - It should work fine with SolidFoundations, just remember to remove **all** instances of the inventory version of the giant crops, resource clumps, trees, and etc from all SolidFoundations buildings before removing this mod. If you do not, your buildings might vanish.

## Much thanks to
* [VoidWitchCult](https://www.nexusmods.com/stardewvalley/users/163267158) for providing me with graphics for Robin's shop! Check out [their goth bachelors/bachelorettes](https://www.nexusmods.com/stardewvalley/mods/15335)!
* [Violetlizabet](https://www.nexusmods.com/stardewvalley/users/120958053) for the winter palm trees! Check out her [Fireworks Festival](https://www.nexusmods.com/stardewvalley/mods/15261)!
* [Casey](https://www.nexusmods.com/stardewvalley/users/34250790) for...a lot at this point. This mod uses her serializer magic and would not be possible without it.

If anyone wants to re-texture the shovel, I'd greatly appreciate it, haha.

## Mods in screenshots
* The giant crops
    - Coffee mug and starfruit are from [6480's pack](https://www.nexusmods.com/stardewvalley/mods/5221). Giant coffee bean is the test pack for [Giant Crop Tweaks](https://www.nexusmods.com/stardewvalley/mods/14370).
    - Sunflower and blue jazz are from [BBR's pack](https://www.nexusmods.com/stardewvalley/mods/10751).
    - I couldn't track down the Giant Qi Crop anymore, sorry! Looks like that has been [hidden](https://www.nexusmods.com/stardewvalley/mods/14226).
    - The rest of the flowers are probably from [Lumi's flower pack](https://www.nexusmods.com/stardewvalley/mods/7110).

## See also

[Change log](docs/changelog.md)
