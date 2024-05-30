/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/ToolAssembly
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace Tool_Assembly
{
    public class ToolSwitchHandler
    {
        // -1: not applicable
        // 0: can use
        // 1: very suitable
        // 2: only choice
        // 3: very very good
        public static Dictionary<string, Func<GameLocation, Vector2, ResourceClump?, Item, int>> switchLogic = new()
        {
            { "StardewValley.Tools.Axe", Axe },
            { "StardewValley.Tools.Pickaxe", PickAxe },
            { "StardewValley.Tools.MeleeWeapon", MeleeWeapon },
            { "StardewValley.Tools.Hoe", Hoe },
            { "StardewValley.Tools.WateringCan", WateringCan },
            { "StardewValley.Tools.Pan", Pan },
            { "StardewValley.Tools.FishingRod", Rod },
            { "StardewValley.Tools.MilkPail",  MilkPail}
        };

        public static int Shears(GameLocation location, Vector2 position, ResourceClump? clump, Item item)
        {
            foreach (FarmAnimal animal in location.getAllFarmAnimals())
            {
                float distanceToAnimal = Vector2.Distance(position, animal.Tile);

                if (animal.displayType.Contains("Sheep")
                && distanceToAnimal <= 1 && animal.currentLocation == Game1.player.currentLocation)
                {
                    return 2;
                }
            }

            return -1;
        }

        public static int MilkPail(GameLocation location, Vector2 position, ResourceClump? clump, Item item)
        {
            string[] animalsThatCanBeMilked = { "Goat", "Cow" };

            foreach (FarmAnimal animal in location.getAllFarmAnimals())
            {
                float distanceToAnimal = Vector2.Distance(position, animal.Tile);

                if (animalsThatCanBeMilked.Any(animal.displayType.Contains)
                    && distanceToAnimal <= 1 && animal.currentLocation == location)
                {
                    return 2;
                }
            }

            return -1;
        }

        public static int Rod(GameLocation location, Vector2 position, ResourceClump? clump, Item item)
        {
            if (location.doesTileHaveProperty((int)position.X, (int)position.Y, "Water", "Back") != null)
            {
                if (location.IsOutdoors && !location.IsFarm)
                {
                    return 1;
                }
                if (location.IsFarm && (Game1.GetFarmTypeKey() == "Beach" || Game1.GetFarmTypeKey() == "Riverland"))
                {
                    return 1;
                }
            }

            return -1;
        }

        public static int Pan(GameLocation location, Vector2 position, ResourceClump? clump, Item item)
        {
            var orePanRect = new Rectangle(Game1.player.currentLocation.orePanPoint.X * 64 - 64, Game1.player.currentLocation.orePanPoint.Y * 64 - 64, 256, 256);
            return (orePanRect.Contains((int)position.X * 64, (int)position.Y * 64) && Utility.distance(Game1.player.StandingPixel.X, orePanRect.Center.X, Game1.player.StandingPixel.Y, orePanRect.Center.Y) <= 192f) ? 1 : -1;
        }

        public static int WateringCan(GameLocation location, Vector2 position, ResourceClump? clump, Item item)
        {
            if (location.Objects.TryGetValue(position, out var objects))
            {
                if (objects.Name == "Garden Pot") return 2;
            }

            if (location.terrainFeatures.TryGetValue(position, out var terrainFeatures))
            {
                if (terrainFeatures is HoeDirt dirt)
                {
                    if (dirt.crop != null && !dirt.isWatered() && !dirt.readyForHarvest())
                    {
                        return 2;
                    }
                }
            }

            var building = location.getBuildingAt(position);
            if (building != null && (building.GetType() == typeof(PetBowl) || building.GetType() == typeof(Stable)))
            {
                return 2;
            }

            if (location.doesTileHaveProperty((int)position.X, (int)position.Y, "WaterSource", "Back") != null)
            {
                return 2;
            }

            if (location.doesTileHaveProperty((int)position.X, (int)position.Y, "Water", "Back") != null)
            {
                if(!location.IsOutdoors || location.IsFarm && Game1.GetFarmTypeKey() != "Beach" && Game1.GetFarmTypeKey() == "Riverland")
                {
                    return 1;
                }
            }

            return -1;
        }

        public static int Hoe(GameLocation location, Vector2 position, ResourceClump? clump, Item item)
        {
            if (location.Objects.TryGetValue(position, out var objects))
            {
                if (objects.Name == "Artifact Spot") return 2;
                if (objects.Name == "Seed Spot") return 2;
                if (objects.Name == "Supply Crate") return 0;
                else return -1;
            }

            if (clump != null) return -1;
            if (location.terrainFeatures.ContainsKey(position)) return -1;

            if (location.doesTileHaveProperty((int)position.X, (int)position.Y, "Diggable", "Back") != null) return 0;

            return -1;
        }

        public static int MeleeWeapon(GameLocation location, Vector2 position, ResourceClump? clump, Item item)
        {
            if (location.Objects.TryGetValue(position, out var objects))
            {
                if (objects.IsWeeds() && item.Name.Contains("Scythe")) return 3;
                if (objects.IsWeeds()) return 1;
                if (objects.Name == "Barrel") return 1;
            }

            foreach (var character in location.characters)
            {
                if (character.IsMonster && Vector2.Distance(position, character.Tile) < 3)
                {
                    if (character is RockCrab crab)
                    {
                        var isShellLess = ModEntry._Helper?.Reflection.GetField<NetBool>(crab, "shellGone").GetValue().Value;
                        if (isShellLess == true)
                        {
                            return 2;
                        }
                    }

                    return 2;
                }
            }

            if (location.terrainFeatures.TryGetValue(position, out var terrainFeatures))
            {
                if (terrainFeatures is Tree tree)
                {
                    if (tree.hasMoss.Value && tree.growthStage.Value >= Tree.stageForMossGrowth) return 2;
                }

                if (terrainFeatures is Grass)
                {
                    if (item.Name.Contains("Scythe")) return 3;
                    else return 1;
                }

                if(terrainFeatures is HoeDirt dirt)
                {
                    if(dirt.crop != null && (dirt.readyForHarvest() || dirt.crop.dead.Value) && item.Name.Contains("Scythe"))
                    {
                        return 2;
                    }
                }
            }

            return -1;
        }

        public static int Axe(GameLocation location, Vector2 position, ResourceClump? clump, Item item)
        {
            if (location.terrainFeatures.TryGetValue(position, out var terrainFeatures))
            {
                if (terrainFeatures is Tree tree)
                {
                    if (tree.growthStage.Value > Tree.treeStage) 
                        return 2;
                }
                if (terrainFeatures is HoeDirt dirt && dirt.crop != null && dirt.crop.dead.Value) return 0;
            }

            if (location.Objects.TryGetValue(position, out var objects))
            {
                if (objects.bigCraftable.Value)
                {
                    if (objects.Fragility == 1) 
                        return 1;
                    if (objects.Fragility == 0) 
                        return 0;
                }
                if (objects.IsTwig()) return 2;
            }

            if (clump is GiantCrop)
            {
                return 2;
            }

            if (clump != null && (clump.parentSheetIndex.Value == 602 || clump.parentSheetIndex.Value == 600))
            { 
                return 2;
            }
             
            return -1;
        }

        public static int PickAxe(GameLocation location, Vector2 position, ResourceClump? clump, Item item)
        {
            if (location.Objects.TryGetValue(position, out var objects))
            {
                if (objects.bigCraftable.Value)
                {
                    if (objects.Fragility == 1) return 1;
                    if (objects.Fragility == 0) return 0;
                }
                if (objects.IsWeeds()) return 0;
                if (objects.IsBreakableStone()) return 2;
            }

            if (location.terrainFeatures.TryGetValue(position, out var terrainFeatures))
            {
                if (terrainFeatures is HoeDirt dirt && dirt.crop == null && !dirt.HasFertilizer()) return 0;
            }

            if (clump != null && new List<int> { 758, 756, 754, 752, 672, 622, 148 }.Contains(clump.parentSheetIndex.Value))
            {
                return 2;
            }

            foreach (var character in location.characters)
            {
                if (character.IsMonster && Vector2.Distance(position, character.Tile) < 3)
                {
                    if (character is RockCrab crab)
                    {
                        var isShellLess = ModEntry._Helper?.Reflection.GetField<NetBool>(crab, "shellGone").GetValue().Value;
                        if (isShellLess == false)
                        {
                            return 2;
                        }
                    }
                }
            }

            return -1;
        }

        public static Item GetBestIndex(int index, Inventory inv)
        {
            GameLocation location = Game1.player.currentLocation;
            Vector2 position = Game1.player.GetGrabTile();
            float maxPiority = -1;
            Item currentItem = inv[index];
            ResourceClump? clump = null;
            foreach (var resourceClump in location.resourceClumps)
            {
                if (resourceClump.occupiesTile((int)position.X, (int)position.Y))
                {
                    clump = resourceClump;
                }
            }

            foreach (Item item in inv)
            {
                if(switchLogic.TryGetValue(item.QualifiedItemId, out var func1))
                {
                    float temppiority = func1.Invoke(location, position, clump, item);
                    if (item is Tool tool) temppiority += tool.UpgradeLevel * 0.1f;
                    if (item is MeleeWeapon weapon) temppiority += weapon.maxDamage.Value * 0.1f;
                    else temppiority += item.Quality * 0.1f;
                    if(temppiority > maxPiority)
                    {
                        maxPiority = temppiority;
                        currentItem = item;
                    }
                }

                if (switchLogic.TryGetValue(item.GetType().ToString(), out var func2))
                {
                    float temppiority = func2.Invoke(location, position, clump, item);
                    if (item is Tool tool) temppiority += tool.UpgradeLevel * 0.1f;
                    if (item is MeleeWeapon weapon) temppiority += weapon.maxDamage.Value * 0.1f;
                    else temppiority += item.Quality * 0.1f;
                    if (temppiority > maxPiority)
                    {
                        maxPiority = temppiority;
                        currentItem = item;
                    }
                }
            }
            return currentItem;
        }
    }
}
