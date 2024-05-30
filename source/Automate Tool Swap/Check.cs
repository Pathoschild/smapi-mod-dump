/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Caua-Oliveira/StardewValley-AutomateToolSwap
**
*************************************************/

using AutomateToolSwap;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;
using StardewValley.Buildings;
using StardewValley.Monsters;
using Netcode;
using System.Threading;


//Get the object at the specified tile
//Obter o objeto no azulejo especificado
public class Check
{
    private ModEntry ModEntry;
    private ModConfig config;

    public Check(ModEntry modEntry)
    {
        ModEntry = modEntry;
        config = ModEntry.Config;
    }

    public bool Objects(GameLocation location, Vector2 tile, Farmer player)
    {
        // Get the object at the specified tile
        // Obter o objeto no azulejo especificado
        var obj = location.getObjectAtTile((int)tile.X, (int)tile.Y);
        bool itemCantBreak = !(player.CurrentItem is Pickaxe or Axe);
        var i18n = ModEntry.Helper.Translation;

        // If obj is null, return false immediately
        // Se obj for nulo, retorne falso imediatamente
        if (obj == null)
            return false;

        // Checks for characteristics of the object, and swaps items accordlingly
        //Verifica as características do objeto e troca os itens conforme necessário
        switch (obj)
        {
            case var _ when obj.IsWeeds():
                if (config.AnyToolForWeeds && location is not MineShaft)
                    ModEntry.SetTool(player, typeof(Pickaxe), anyTool: true);
                else if (config.ScytheForWeeds)
                    ModEntry.SetTool(player, typeof(MeleeWeapon), "Scythe");
                return true;

            case var _ when obj.IsBreakableStone():
                if (config.PickaxeForStoneAndOres && (player.CurrentItem == null || !player.CurrentItem.Name.Contains("Bomb") && !player.CurrentItem.Name.Contains("Staircase")))
                    ModEntry.SetTool(player, typeof(Pickaxe));
                return true;

            case var _ when obj.IsTwig():
                if (config.AxeForTwigs)
                    ModEntry.SetTool(player, typeof(Axe));
                return true;

            case var _ when obj.isForage():
                if (config.ScytheForForage)
                    ModEntry.SetTool(player, typeof(MeleeWeapon), "ScytheOnly");
                return true;

            case CrabPot crabPot when crabPot.bait.Value == null:
                if (config.BaitForCrabPot)
                    ModEntry.SetItem(player, "Bait", "Bait", aux: -21);
                return true;
        }

        // Checks for the name of the objects, and swaps items accordlingly
        //Verifica o nome dos objetos e troca os itens conforme necessário
        switch (obj.Name)
        {
            case "Furnace":
                if (config.OresForFurnaces && itemCantBreak && (player.CurrentItem == null || !player.CurrentItem.Name.Contains("Ore")))
                    ModEntry.SetItem(player, "Resource", "Ore");
                return true;

            case "Cheese Press":
                if (config.MilkForCheesePress && itemCantBreak)
                    ModEntry.SetItem(player, "Animal Product", "Milk", aux: -6);
                return true;

            case "Mayonnaise Machine":
                if (config.EggsForMayoMachine && itemCantBreak)
                    ModEntry.SetItem(player, "Animal Product", "Egg", aux: -5);
                return true;

            case "Artifact Spot":
                if (config.HoeForArtifactSpots)
                    ModEntry.SetTool(player, typeof(Hoe));
                return true;

            case "Garden Pot":
                if (config.WateringCanForGardenPot && (player.CurrentItem == null || (itemCantBreak && player.CurrentItem.category != -74)))
                    ModEntry.SetTool(player, typeof(WateringCan));
                return true;

            case "Seed Spot":
                if (config.HoeForArtifactSpots)
                    ModEntry.SetTool(player, typeof(Hoe));
                return true;

            case "Barrel":
                if (config.WeaponForMineBarrels)
                    ModEntry.SetTool(player, typeof(MeleeWeapon), "Weapon");
                return true;

            case "Supply Crate":
                if (config.AnyToolForSupplyCrates)
                    ModEntry.SetTool(player, typeof(Hoe), anyTool: true);
                return true;

            case "Recycling Machine":
                if (config.TrashForRecycling && itemCantBreak)
                    ModEntry.SetItem(player, "Trash", "Joja", aux: -20);
                return true;

            case "Bone Mill":
                if (config.BoneForBoneMill && itemCantBreak)
                    ModEntry.SetItem(player, "Resource", "Bone Fragment");
                return true;

            case "Loom":
                if (config.WoolForLoom && itemCantBreak)
                    ModEntry.SetItem(player, "Animal Product", "Wool", aux: -18);
                return true;

            case "Fish Smoker":
                if (config.FishForSmoker && player.CurrentItem.category != -4 && itemCantBreak)
                    ModEntry.SetItem(player, "Fish", aux: -4);
                return true;

            case "Bait Maker":
                if (config.FishForBaitMaker && itemCantBreak)
                    ModEntry.SetItem(player, "Fish", aux: -4);
                return true;

            case "Crystalarium":
                if (config.MineralsForCrystalarium && (player.CurrentItem == null || (itemCantBreak && player.CurrentItem.category != -2)))
                    ModEntry.SetItem(player, "Mineral", aux: -2);
                return true;

            case "Seed Maker":
                if (config.SwapForSeedMaker && itemCantBreak)
                    ModEntry.SetItem(player, "Crops");
                return true;

            case "Keg":
                if (itemCantBreak && config.SwapForKegs != "None")
                    ModEntry.SetItem(player, "Crops", crops: config.SwapForKegs);
                return true;

            case "Preserves Jar":
                if (itemCantBreak && config.SwapForPreservesJar != "None")
                    ModEntry.SetItem(player, "Crops", crops: config.SwapForPreservesJar);
                return true;
        }
        return true;
    }


    // TerrainFeatures are trees, bushes, grass and tilled dirt
    // TerrainFeatures são árvores, arbustos, gramas e terra arada
    public bool TerrainFeatures(GameLocation location, Vector2 tile, Farmer player)
    {


        foreach (var terrainFeature in location.largeTerrainFeatures)
        {
            if (terrainFeature is not Bush || !config.ScytheForBushes)
                break;

            // Gets the bounding box of the bush because it does not occupie only one tile
            // Obtem a caixa delimitadora do arbusto porque ele não ocupa apenas um bloco
            var bush = terrainFeature as Bush;
            var bushBox = bush.getBoundingBox();
            var tilePixel = new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize);

            if (bushBox.Contains((int)tilePixel.X, (int)tilePixel.Y) && bush.inBloom())
            {
                ModEntry.SetTool(player, typeof(MeleeWeapon), "Scythe");
                return true;
            }
        }

        if (!location.terrainFeatures.ContainsKey(tile))
            return false;

        var feature = location.terrainFeatures[tile];

        if (feature is Tree tree)
        {
            // Remove moss if needed swapping to Scythe
            // Remove o musgo se necessário trocando para Foice
            if (tree.hasMoss && tree.growthStage >= Tree.stageForMossGrowth && config.ScytheForMossOnTrees)
            {
                ModEntry.SetTool(player, typeof(MeleeWeapon), "Scythe");
                return true;
            }

            // Return if the player has a tapper in hand (item that can be put in tree)
            // Retorna se o jogador estiver segurando um tapper na mão (item que pode ser colocado em árvores)
            if (!config.AxeForTrees || (player.CurrentItem != null && (player.CurrentItem.Name == "Tapper" || player.CurrentItem.Name == "Tree Fertilizer")))
                return true;

            // If the tree is not fully grown and the config to ignore it is enabled, skips, otherwise swaps to Axe 
            // Se a árvore não estiver completamente crescida e a configuração para ignorá-la estiver ativada, pula, caso contrário, troca para o Machado
            if (!(tree.growthStage < Tree.treeStage && config.IgnoreGrowingTrees))
            {
                ModEntry.SetTool(player, typeof(Axe));
                return true;
            }

            return true;
        }

        // It does not swap to Scythe if the player is holding a animal tool, because it could break grass by mistake
        // Não troca para a Foice se o jogador estiver segurando uma ferramenta de animal, porque poderia quebrar grama por engano
        if (feature is Grass && !(player.CurrentTool is MilkPail || player.CurrentTool is Shears) && config.ScytheForGrass)
        {
            ModEntry.SetTool(player, typeof(MeleeWeapon), "ScytheOnly");
            return true;
        }


        // Tilled dirt
        // Terra arada
        if (feature is HoeDirt hoeDirt)
        {
            // Swap to seed if it can be used
            // Troca para semente se puder ser usada
            if (hoeDirt.crop == null && config.SeedForTilledDirt)
            {
                if (!(config.PickaxeOverWateringCan && player.CurrentTool is Pickaxe))
                    if (player.CurrentItem == null || player.CurrentItem.category != -74 || player.CurrentItem.HasContextTag("tree_seed_item"))
                        ModEntry.SetItem(player, "Seed");

                return true;
            }

            // Swap to scythe if it is a grown crop or a dead crop
            // Troca para Foice se for uma planta crescida ou planta colheita morta
            if (hoeDirt.crop != null && (hoeDirt.readyForHarvest() || hoeDirt.crop.dead) && config.ScytheForCrops)
            {
                ModEntry.SetTool(player, typeof(MeleeWeapon), "ScytheOnly");
                return true;
            }

            // Swap to fertilizer if it can be used
            // Troca para fertilizante se puder ser usado
            if (hoeDirt.crop != null && !hoeDirt.HasFertilizer() && hoeDirt.CanApplyFertilizer("(O)369") && config.FertilizerForCrops)
            {
                if (!(config.PickaxeOverWateringCan) && player.CurrentTool is not Pickaxe && player.CurrentItem.category != -19)
                    ModEntry.SetItem(player, "Fertilizer", "Tree", aux: -19);
                return true;
            }

            // Swap to Hoe if it is a Ginger Crop
            // Troca para Enxada se for uma planta de Gengibre
            if (hoeDirt.crop != null && hoeDirt.crop.whichForageCrop == "2" && config.HoeForGingerCrop)
            {
                ModEntry.SetTool(player, typeof(Hoe));
                return true;
            }

            // Swap to Watering Can if plant is not watered
            // Troca para Regador se a planta não estiver regada
            if (hoeDirt.crop != null && !hoeDirt.isWatered() && !hoeDirt.readyForHarvest() && config.WateringCanForUnwateredCrop && !(player.isRidingHorse() && player.mount.Name.ToLower().Contains("tractor") && player.CurrentTool is Hoe))
            {
                if (!(config.PickaxeOverWateringCan && player.CurrentTool is Pickaxe))
                    ModEntry.SetTool(player, typeof(WateringCan));

                return true;
            }

            return true;
        }

        return false;
    }


    // ResourceClumps are stumps, logs, boulders and giant crops (they occupie more than one tile)
    // ResourceClumps são tocos, troncos, pedregulhos plantas gigantes (ocupam mais de um bloco)
    public bool ResourceClumps(GameLocation location, Vector2 tilePosition, Farmer player)
    {
        bool IsStumpOrLog(ResourceClump resourceClump)
        {
            return new List<int> { 602, 600 }.Contains(resourceClump.parentSheetIndex);
        }

        bool IsBoulder(ResourceClump resourceClump)
        {
            return new List<int> { 758, 756, 754, 752, 672, 622, 148 }.Contains(resourceClump.parentSheetIndex);
        }

        foreach (var resourceClump in location.resourceClumps)
        {
            if (resourceClump.occupiesTile((int)tilePosition.X, (int)tilePosition.Y))
            {
                if (config.AxeForGiantCrops && resourceClump is GiantCrop)
                {
                    if (player.CurrentItem.Name != "Tapper")
                        ModEntry.SetTool(player, typeof(Axe));
                    return true;
                }

                if (config.AxeForStumpsAndLogs && IsStumpOrLog(resourceClump))
                {
                    ModEntry.SetTool(player, typeof(Axe));
                    return true;
                }

                if (config.PickaxeForBoulders && IsBoulder(resourceClump))
                {
                    ModEntry.SetTool(player, typeof(Pickaxe));
                    return true;
                }
            }
        }

        return false;
    }


    public bool Monsters(GameLocation location, Vector2 tile, Farmer player)
    {
        foreach (var character in location.characters)
        {
            // If monster is close to player, swaps to weapon
            // Se o monstro estiver perto do jogador, troca para arma
            if (character.IsMonster && Vector2.Distance(tile, character.Tile) < config.MonsterRangeDetection)
            {
                // Crabs are exception
                // Carangueijos são exceções
                if (character is RockCrab crab)
                {
                    if (config.IgnoreCrabs)
                        return true;

                    var isShellLess = ModEntry.Helper.Reflection.GetField<NetBool>(crab, "shellGone").GetValue();
                    if (!isShellLess && !crab.isMoving())
                    {
                        ModEntry.SetTool(player, typeof(Pickaxe));
                        return true;
                    }
                }

                // If player is holding a bomb or staircase, don't swaps to weapon
                // Se o jogador estiver carregando uma bomba ou escada, não troca para arma
                if (player.CurrentItem == null || !player.CurrentItem.Name.Contains("Bomb") && !player.CurrentItem.Name.Contains("Staircase"))
                {
                    ModEntry.SetTool(player, typeof(MeleeWeapon), "Weapon");
                    return true;
                }

                return true;
            }
        }

        return false;
    }

    // Check for water or things that neeed water
    public bool Water(GameLocation location, Vector2 tile, Farmer player)
    {
        bool shouldUseCan = location is Farm || location is VolcanoDungeon || location.InIslandContext() || location.isGreenhouse;

        if (IsPetBowlOrStable(location, tile) && config.WateringCanForPetBowl)
        {
            ModEntry.SetTool(player, typeof(WateringCan));
            return true;
        }

        if (IsPanSpot(location, tile, player) && config.PanForPanningSpots)
        {
            ModEntry.SetTool(player, typeof(Pan));
            return true;
        }

        if ((IsWaterSource(location, tile) || IsWater(location, tile, player)) && shouldUseCan && config.WateringCanForWater)
        {
            ModEntry.SetTool(player, typeof(WateringCan));
            return true;
        }

        if (IsWater(location, tile, player) && config.FishingRodOnWater)
        {
            ModEntry.SetTool(player, typeof(FishingRod));
            return true;
        }

        bool IsPetBowlOrStable(GameLocation location, Vector2 tile)
        {
            var building = location.getBuildingAt(tile);
            return building != null && (building.GetType() == typeof(PetBowl) || building.GetType() == typeof(Stable));
        }
        bool IsPanSpot(GameLocation location, Vector2 tile, Farmer player)
        {
            var toolLocation = player.GetToolLocation(false) / 64;
            var orePanRect = new Rectangle(player.currentLocation.orePanPoint.X * 64 - 64, player.currentLocation.orePanPoint.Y * 64 - 64, 256, 256);
            return orePanRect.Contains((int)tile.X * 64, (int)tile.Y * 64) && Utility.distance((float)player.StandingPixel.X, (float)orePanRect.Center.X, (float)player.StandingPixel.Y, (float)orePanRect.Center.Y) <= 192f;
        }
        bool IsWater(GameLocation location, Vector2 tile, Farmer player)
        {
            return location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") != null && !(player.CurrentTool is WateringCan || player.CurrentTool is Pan);
        }
        bool IsWaterSource(GameLocation location, Vector2 tile)
        {
            return location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "WaterSource", "Back") != null;
        }

        return false;
    }


    public bool Animals(GameLocation location, Vector2 tile, Farmer player)
    {
        if (!(location is Farm or AnimalHouse))
            return false;

        string[] animalsThatCanBeMilked = { "Goat", "Cow" };
        string[] animalsThatCanBeSheared = { "Sheep" };

        foreach (FarmAnimal animal in location.getAllFarmAnimals())
        {
            float distanceToAnimal = Vector2.Distance(tile, animal.Tile);

            // If animal is in range, swaps to milk pail or shears
            // Se o animal estiver perto, troca para balde de leite ou tesouras
            if (config.MilkPailForCowsAndGoats && animalsThatCanBeMilked.Any(animal.type.Contains)
                && distanceToAnimal <= 1 && animal.currentLocation == player.currentLocation)
            {
                ModEntry.SetTool(player, typeof(MilkPail));
                return true;
            }
            if (config.ShearsForSheeps && animalsThatCanBeSheared.Any(animal.type.Contains)
                && distanceToAnimal <= 1 && animal.currentLocation == player.currentLocation)
            {
                ModEntry.SetTool(player, typeof(Shears));
                return true;
            }
        }

        // Check for feeding bench availability and swaps to hay
        // Verifica a mesa de alimentação está disponível e troca para palha
        bool isFeedingBench = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Trough", "Back") != null;
        if (location is AnimalHouse && isFeedingBench)
        {
            ModEntry.SetItem(player, "", "Hay", aux: 0);
            return true;
        }

        return false;
    }

    public bool DiggableSoil(GameLocation location, Vector2 tile, Farmer player)
    {
        if (!ModEntry.isTractorModInstalled || (player.isRidingHorse() && player.mount.Name.ToLower().Contains("tractor")))
            return false;

        bool isNotScythe = player.CurrentItem?.category == -98;
        bool isDiggable = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") != null;
        bool isFightingLocations = location is Mine or MineShaft or VolcanoDungeon;

        if (!config.HoeForDiggableSoil || !isDiggable || isFightingLocations || location.isPath(tile))
            return false;
        if (player.CurrentItem is MeleeWeapon && isNotScythe && Game1.spawnMonstersAtNight)
            return false;
        if (player.CurrentItem is FishingRod or GenericTool or Wand)
            return false;

        if (player.CurrentItem == null || !player.CurrentItem.canBePlacedHere(location, tile, CollisionMask.All, true))
        {
            ModEntry.SetTool(player, typeof(Hoe));
            return true;
        }

        return false;
    }
}
