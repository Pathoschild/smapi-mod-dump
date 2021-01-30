**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Floogen/GreenhouseGatherers**

----

# Greenhouse Gatherers

**Greenhouse Gatherers** is a mod for [Stardew Valley](http://stardewvalley.net/) that adds a way for Junimos to harvest crops inside buildings.

The mod adds a new craftable item called the **Harvest Statue**, which is given by mail after unlocking the Junimo Huts from the Wizard. This statue can only be placed indoors and only one can be placed per building.

Once placed inside, the Junimos will appear each morning and immediately harvest all available crops, forage products and fruit trees within the building. Right clicking the Harvest Statue will open up a chest-like inventory, allowing you to extract the harvested items.

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. [Install all the dependencies](#dependencies).
3. Install [this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/7619).
4. Run the game using SMAPI.

## Unlocking the Harvest Statue
To unlock the Harvest Statue, you must finish the [Goblin Problem Quest](https://stardewcommunitywiki.com/Quests#Goblin_Problem "Quests"). The day after completing the quest, you will be sent the recipe by the Wizard.

Alternatively, you can set the `ForceRecipeUnlock` config option to `true`, which will cause the recipe to appear in your mailbox.

## Crafting the Harvest Statue
The Harvest Statue requires the following ingredients:

 - Prismatic Shard x1
 - Stone x350
 - Starfruit x150

The recipe can be changed to your preference by editing the `big-craftable.json` file under the `...\GreenhouseGatherers\GreenhouseGatherers\[JA] GreenhouseGatherers\BigCraftables\Harvest Statue` folder.

## Using the Harvest Statue
Once per morning, the Junimos will search each building that a Harvest Statue is located in. The Junimos will gather crops, forage products and fruit trees that are ready for harvesting nearby.

If the Junimos successfully harvested anything, you will get a message in the morning letting you know (the message will tell you what building they harvested from). The Harvest Statue will also have an updated sprite, showing that it contains items.

Right clicking the Harvest Statue will open up a chest-like inventory, allowing you to extract the harvested items.

Note that if the Harvest Statue gets full, the Junimos will eat any excess products they gather! This can disabled within the config if you so wish.

## Replanting
While harvesting the Junimos will replant any non-renewing crops, so long as the Harvest Statue contains the matching seeds.

e.g. If the Junimos harvest a parsnip, they will look inside the Harvest Statue for a parsnip seed. If there is at least one, they will use it to replant the harvested parsnip.


## Configurations
This mod creates a `config.json` under the `...\GreenhouseGatherers\GreenhouseGatherers` folder. It contains options that you can modify to change how **Greenhouse Gatherers** interacts with the game.

**Settings**
|Name| Description | Default Value |
|--|--|--|
| `DoJunimosEatExcessCrops` | If `true`, the Junimos will eat the excess harvest if the local Harvest Statue is full. If set to `false` the Junimos will not harvest until the Harvest Statue has room. | `true` |
| `DoJunimosHarvestFromPots` | If `true`, the Junimos will harvest from Garden Pots. | `true` |
| `DoJunimosHarvestFromFruitTrees` | If `true`, the Junimos will harvest from Fruit Trees. | `true` |
| `MinimumFruitOnTreeBeforeHarvest` | The minimum amount of fruit on a tree to appear before the Junimos harvest from it. | `3` |
| `DoJunimosAppearAfterHarvest` | If `true`, the Junimos will appear in the building they harvest from. They will disappear if approached. | `true` |
| `MaxAmountOfJunimosToAppearAfterHarvest` | The maximum amount of Junimos to appear after a harvest. If set to `-1`, the amount will scale on the amount of crops harvested. | `-1` |
| `DoJunimosSowSeedsAfterHarvest` | If `true`, the Junimos will replant the seeds of the harvested crop. The seeds must be stored in the Harvest Statue for Junimos to replant them. | `true` |
| `EnableHarvestMessage` | If `true`, you will get a message in the morning for every Harvest Statue that successfully harvested that day. | `true` |
| `ForceRecipeUnlock`| If `true`, the recipe for the Harvest Statue will arrive by mail the next day. | `false` |


## Dependencies
 - [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720)

## Compatibility
**Greenhouse Gatherers** is compatible with Stardew Valley v1.5+ for single-player (multiplayer is not tested, though it may work).
