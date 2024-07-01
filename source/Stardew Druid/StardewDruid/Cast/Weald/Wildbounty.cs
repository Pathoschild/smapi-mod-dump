/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewValley;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.TerrainFeatures;
using StardewDruid.Data;
using StardewValley.Locations;
using xTile.Tiles;
using StardewDruid.Journal;
using StardewValley.BellsAndWhistles;
using xTile.Layers;
using StardewValley.Internal;
using StardewValley.GameData.GiantCrops;
using StardewValley.Quests;
using StardewValley.GameData.WildTrees;
using StardewValley.Objects;


namespace StardewDruid.Cast.Weald
{
    public class Wildbounty
    {

        public enum bounties
        {
            bush,
            tea,
            clump,
            water,
            pool,
        }
        public enum debris
        {
            tree,
            treeseed,
            grass,
            giant,
        }

        public GameLocation location;

        public Vector2 target;

        public Dictionary<Vector2, bounties> bountyProspects = new();

        public Dictionary<Vector2, debris> debrisProspects = new();

        public Dictionary<Vector2, StardewValley.TerrainFeatures.ResourceClump> clumps = new(0);

        public int extractDebris;

        public int extractForage;

        public int extractWater;

        public Wildbounty() { }

        public void CastActivate(GameLocation Location, Vector2 Target)
        {

            location = Location;

            target = Target;

            int forageCost = Game1.player.ForagingLevel >= 6 ? 4 : 6;

            int waterCost = Game1.player.FishingLevel >= 6 ? 5 : 8;

            Dictionary<Vector2, int> targets = new();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {

                    targets.Add(target + new Vector2(i, j), 0);

                }

            }

            // ---------------------------------------------
            // Large Feature iteration
            // ---------------------------------------------

            if (location.largeTerrainFeatures.Count > 0)
            {

                foreach (LargeTerrainFeature largeTerrainFeature in location.largeTerrainFeatures)
                {

                    if (largeTerrainFeature is not StardewValley.TerrainFeatures.Bush bushFeature)
                    {

                        continue;

                    }

                    Vector2 bushTile = bushFeature.Tile;

                    if (targets.ContainsKey(bushTile))
                    {

                        RustleBush(bushFeature);

                    }

                }

            }

            if (location.resourceClumps.Count > 0)
            {

                foreach (ResourceClump resourceClump in location.resourceClumps)
                {

                    Vector2 clumpTile = resourceClump.Tile;

                    if (targets.ContainsKey(clumpTile))
                    {

                        clumps.Add(clumpTile, resourceClump);

                        if (resourceClump is GiantCrop)
                        {

                            debrisProspects.Add(clumpTile, debris.giant);

                        }
                        else
                        {
                            
                            bountyProspects.Add(clumpTile, bounties.clump);

                        }

                    }

                }

            }

            if (location is Beach && Mod.instance.rite.spawnIndex.fishes)
            {

                Layer buildingLayer = location.map.GetLayer("Buildings");

                for (int f = targets.Count - 1; f >= 0; f--)
                {

                    if (f % 4 != 0) { continue; }

                    KeyValuePair<Vector2, int> check = targets.ElementAt(f);

                    int tileX = (int)check.Key.X;

                    int tileY = (int)check.Key.Y;

                    Tile buildingTile = buildingLayer.PickTile(new xTile.Dimensions.Location(tileX * 64, tileY * 64), Game1.viewport.Size);

                    if (buildingTile != null)
                    {

                        List<int> tidalList = new() { 60, 61, 62, 63, 77, 78, 79, 80, 94, 95, 96, 97, 104, 287, 288, 304, 305, 321, 362, 363 };

                        if (tidalList.Contains(buildingTile.TileIndex))
                        {

                            bountyProspects.Add(check.Key, bounties.pool);

                        }

                        targets.Remove(check.Key);

                    }

                }

            }

            if (Mod.instance.rite.spawnIndex.fishes)
            {

                for (int f = targets.Count - 1; f >= 0; f--)
                {

                    if (f % 5 != 0) { continue; }

                    KeyValuePair<Vector2, int> check = targets.ElementAt(f);

                    string tileCheck = ModUtility.GroundCheck(location, check.Key);

                    if (tileCheck == "water")
                    {

                        if (ModUtility.WaterCheck(location, check.Key, 1))
                        {

                            if (location is Farm)
                            {

                                bountyProspects.Add(check.Key, bounties.pool);

                            }
                            else
                            {

                                bountyProspects.Add(check.Key, bounties.water);

                            }

                        }

                    }

                }

            }


            foreach (KeyValuePair<Vector2, int> check in targets)
            {

                if (location.terrainFeatures.ContainsKey(check.Key))
                {

                    TerrainFeature terrainFeature = location.terrainFeatures[check.Key];

                    if (terrainFeature is StardewValley.TerrainFeatures.FruitTree fruitFeature)
                    {

                        if (fruitFeature.growthStage.Value >= 4)
                        {

                            fruitFeature.performUseAction(check.Key);

                        }

                    }
                    else if (terrainFeature is StardewValley.TerrainFeatures.Tree treeFeature)
                    {

                        if (treeFeature.growthStage.Value >= 5)
                        {

                            if (!treeFeature.stump.Value)
                            {

                                treeFeature.performUseAction(check.Key);

                            }

                            debrisProspects.Add(check.Key, debris.tree);

                        }

                    }
                    else if (terrainFeature is StardewValley.TerrainFeatures.Grass grassFeature)
                    {

                        Microsoft.Xna.Framework.Rectangle tileRectangle = new((int)(check.Key.X * 64) + 1, (int)(check.Key.Y * 64) + 1, 62, 62);

                        grassFeature.doCollisionAction(tileRectangle, 2, check.Key, Game1.player);

                        debrisProspects.Add(check.Key, debris.grass);

                    }

                    continue;

                }

            }

            for (int i = 0; i < 1; i++)
            {

                if (bountyProspects.Count == 0)
                {
                    continue;
                }

                int index = Mod.instance.randomIndex.Next(bountyProspects.Count);

                KeyValuePair<Vector2, bounties> prospect = bountyProspects.ElementAt(index);

                switch (prospect.Value)
                {

                    case bounties.bush:

                        BountyBush(prospect.Key);

                        Mod.instance.iconData.ImpactIndicator(location, prospect.Key * 64, IconData.impacts.glare, 2f, new() { color = Color.LightGreen });

                        break;

                    case bounties.tea:

                        BountyTea(prospect.Key);

                        Mod.instance.iconData.ImpactIndicator(location, prospect.Key * 64, IconData.impacts.glare, 2f, new() { color = Color.LightGreen });

                        break;

                    case bounties.clump:

                        BountyClump(prospect.Key);

                        Mod.instance.iconData.ImpactIndicator(location, prospect.Key * 64, IconData.impacts.glare, 2f, new() { color = Color.LightGreen });

                        break;

                    case bounties.water:

                        BountyWater(prospect.Key);

                        Game1.player.currentLocation.playSound("pullItemFromWater");

                        Mod.instance.iconData.ImpactIndicator(location, prospect.Key * 64, IconData.impacts.fish, 2f, new());

                        break;

                    case bounties.pool:

                        BountyWater(prospect.Key, true);

                        Game1.player.currentLocation.playSound("pullItemFromWater");

                        Mod.instance.iconData.ImpactIndicator(location, prospect.Key * 64, IconData.impacts.fish, 2f, new());

                        break;

                }

                bountyProspects.Remove(prospect.Key);

            }

            for (int i = 0; i < 2; i++)
            {

                if (debrisProspects.Count == 0)
                {
                    continue;
                }

                int index = Mod.instance.randomIndex.Next(debrisProspects.Count);

                KeyValuePair<Vector2, debris> prospect = debrisProspects.ElementAt(index);

                switch (prospect.Value)
                {

                    case debris.tree:

                        BountyTree(prospect.Key);

                        Mod.instance.iconData.ImpactIndicator(location, prospect.Key * 64, IconData.impacts.glare, 1f, new() { color = Color.LightGreen });

                        break;

                    case debris.grass:

                        int yield = 1;
                        if(debrisProspects.Count > 10) { yield++; }
                        if(debrisProspects.Count > 20) { yield++; }

                        BountyGrass(prospect.Key, yield);

                        Mod.instance.iconData.ImpactIndicator(location, prospect.Key * 64, IconData.impacts.glare, 1f, new() { color = Color.LightGreen });

                        break;

                    case debris.giant:

                        BountyGiant(prospect.Key);

                        Mod.instance.iconData.ImpactIndicator(location, prospect.Key * 64, IconData.impacts.glare, 1f, new() { color = Color.LightGreen });

                        break;

                }

                //Game1.player.gainExperience(2, 2); // gain foraging experience

                debrisProspects.Remove(prospect.Key);

            }

            if (extractDebris > 0)
            {

                //Game1.player.gainExperience(2, extractDebris); // gain foraging experience

                Mod.instance.rite.castCost += extractDebris;

            }

            if (extractForage > 0)
            {

                Game1.player.gainExperience(2, extractForage * 4); // gain foraging experience

                Mod.instance.rite.castCost += extractForage * forageCost;

            }

            if (extractWater > 0)
            {

                Game1.player.gainExperience(1, extractWater * 8); // gain fishing experience

                Mod.instance.rite.castCost += extractWater * waterCost;

            }

        }


        public void RustleBush(StardewValley.TerrainFeatures.Bush bushFeature)
        {


            Vector2 bushTile = bushFeature.Tile;

            if (bountyProspects.ContainsKey(bushTile)) { return; }

            bushFeature.performToolAction(null, 1, bushTile);

            if (bushFeature.size.Value == 3)
            {

                bountyProspects.Add(bushTile, bounties.tea);

                return;

            }
            else if (bushFeature.size.Value == 2)
            {

                bountyProspects.Add(bushTile, bounties.bush);

                if (Game1.currentSeason == "summer")
                {

                    Game1.currentLocation.critters.Add(new Firefly(bushTile + new Vector2(Mod.instance.randomIndex.Next(-2, 3), Mod.instance.randomIndex.Next(-2, 3))));

                }
                else
                {

                    Game1.currentLocation.critters.Add(new Butterfly(location, bushTile + new Vector2(Mod.instance.randomIndex.Next(-2, 3), Mod.instance.randomIndex.Next(-2, 3)), false));

                }

            }

        }

        public void BountyTea(Vector2 teaTile)
        {

            ThrowHandle throwObject = new(Game1.player, teaTile * 64, 815, 0);

            throwObject.register();

            extractForage++;

        }

        public void BountyBush(Vector2 bushTile)
        {

            int objectIndex = SpawnData.RandomBushForage();

            int randomQuality = Mod.instance.randomIndex.Next(11 - Game1.player.foragingLevel.Value);

            int objectQuality = 0;

            if (randomQuality == 0)
            {

                objectQuality = 2;

            }

            if (Game1.player.professions.Contains(16))
            {

                objectQuality = 4;

            }

            int throwAmount = 1;

            if (Game1.player.professions.Contains(13))
            {

                throwAmount = Mod.instance.randomIndex.Next(1, 3);

            }

            for (int i = 0; i < throwAmount; i++)
            {

                ThrowHandle throwObject = new(Game1.player, bushTile * 64, objectIndex, objectQuality);

                throwObject.register();

                extractForage++;

            };

            if (!Mod.instance.questHandle.IsComplete(QuestHandle.wealdTwo))
            {

                Mod.instance.questHandle.UpdateTask(QuestHandle.wealdTwo, 1);

            }

        }


        public void BountyClump(Vector2 clumpTile)
        {

            ResourceClump clump = clumps[clumpTile];

            if (clump.parentSheetIndex.Value == ResourceClump.stumpIndex || clump.parentSheetIndex.Value == ResourceClump.hollowLogIndex)
            {

                int debrisType = 388;

                int debrisAmount = Mod.instance.randomIndex.Next(1, 5);

                for (int i = 0; i < debrisAmount; i++)
                {

                    new ThrowHandle(Game1.player, clumpTile * 64, debrisType, 0).register();

                    extractForage++;

                }

                if (debrisAmount == 1)
                {

                    new ThrowHandle(Game1.player, clumpTile * 64, 382, 0).register();

                    extractForage++;

                }

            }
            else
            {

                int debrisType = 390;

                int debrisAmount = 2 + Mod.instance.randomIndex.Next(Mod.instance.PowerLevel);

                if (clump.parentSheetIndex.Value == ResourceClump.meteoriteIndex)
                {

                    debrisType = 386;

                    debrisAmount *= 2;

                }

                Dictionary<int, ThrowHandle> throwList = new();

                for (int i = 0; i < debrisAmount; i++)
                {

                    new ThrowHandle(Game1.player, clumpTile * 64, debrisType, 0).register();

                    extractForage++;

                }

                if (debrisAmount == 1)
                {

                    new ThrowHandle(Game1.player, clumpTile * 64, 382, 0).register();

                    extractForage++;

                }

            }

        }


        public void BountyGrass(Vector2 grassTile, int yield = 1)
        {

            if (Mod.instance.randomIndex.Next(200) == 0)
            {

                new ThrowHandle(Game1.player, grassTile * 64, 114, 0).register();

            }

            List<string> items = new()
            {

                "771",
                "771",
                "771",
                "771",
                "771",

            };

            if (!Mod.instance.Config.disableSeeds && Mod.instance.questHandle.IsComplete(QuestHandle.wealdTwo))
            {

                switch (Game1.currentSeason)
                {

                    case "spring":

                        items[2] = "495";

                        items[3] = "495";

                        break;

                    case "summer":

                        items[2] = "496";

                        items[3] = "496";

                        break;

                    case "fall":

                        items[2] = "497";

                        items[3] = "497";

                        break;

                    default:

                        items[2] = "498";

                        items[3] = "498";

                        break;

                }
                

            }

            StardewValley.Object candidate = new(items[Mod.instance.randomIndex.Next(items.Count)], Mod.instance.randomIndex.Next(1,yield + (int)(Mod.instance.PowerLevel/2)));

            new ThrowHandle(Game1.player, grassTile * 64, candidate).register();

            extractDebris++;

        }


        public void BountyTree(Vector2 treeTile)
        {

            StardewValley.TerrainFeatures.Tree treeFeature = location.terrainFeatures[treeTile] as StardewValley.TerrainFeatures.Tree;

            int debrisType = 388;

            int debrisMax = 2;

            if (Game1.player.professions.Contains(12))
            {

                debrisMax++;

            }

            if (treeFeature.treeType.Value == "8") //mahogany
            {

                debrisType = 709; debrisMax = 1;

                if (Game1.player.professions.Contains(14))
                {

                    debrisMax++;

                }

            }

            if (treeFeature.treeType.Value == "7") // mushroom
            {

                debrisType = 420; debrisMax = 1;

            }

            if (treeFeature.hasMoss.Value)
            {

                new ThrowHandle(Game1.player, treeTile * 64, ItemQueryResolver.TryResolveRandomItem("Moss", new ItemQueryContext(location, Game1.player, null))).register();

                extractDebris++;

            }

            for (int i = 0; i < debrisMax; i++)
            {

                new ThrowHandle(Game1.player, treeTile * 64, debrisType, 0).register();

                extractDebris++;

            }

        }

        public void BountyTreeseed(Vector2 treeTile)
        {

            StardewValley.TerrainFeatures.Tree treeFeature = location.terrainFeatures[treeTile] as StardewValley.TerrainFeatures.Tree;

            location.playSound("woodyHit");

            location.playSound("axchop");

            WildTreeData data = treeFeature.GetData();
            
            if (data != null && data.SeedItemId != null)
            {

                new ThrowHandle(Game1.player, treeTile * 64, ItemQueryResolver.TryResolveRandomItem(data.SeedItemId, new ItemQueryContext(location, Game1.player, null))).register();

                extractDebris++;

            }

            location.terrainFeatures.Remove(treeTile);

        }

        public void BountyWater(Vector2 waterTile, bool pool = false)
        {
            string randomFish;

            if (pool)
            {

                randomFish = SpawnData.RandomPoolFish(location);

            }
            else
            {

                randomFish = SpawnData.RandomLowFish(location);

            }

            int objectQuality = 0;

            if (11 - Game1.player.fishingLevel.Value <= 0)
            {

                objectQuality = 2;

            }
            else if (Mod.instance.randomIndex.Next(11 - Game1.player.fishingLevel.Value) == 0)
            {

                objectQuality = 2;

            }

            StardewValley.Object candidate = new(randomFish,1,quality:objectQuality);

            StardewDruid.Cast.ThrowHandle throwObject = new(Game1.player, waterTile * 64, candidate);

            throwObject.register();

            Game1.player.checkForQuestComplete(null, -1, 1, null, randomFish, 7);

            extractWater++;

        }

        public void BountyGiant(Vector2 giantTile)
        {

            StardewValley.TerrainFeatures.GiantCrop giantFeature = clumps[giantTile] as GiantCrop;

            GiantCropData data = giantFeature.GetData();

            if (data?.HarvestItems != null)
            {

                Item item = ItemQueryResolver.TryResolveRandomItem(data.HarvestItems.First(), new ItemQueryContext(location, Game1.player, null));
                
                if (item != null)
                {

                    for (int i = 0; i < Mod.instance.randomIndex.Next(1,4); i++)
                    {

                        StardewDruid.Cast.ThrowHandle throwObject = new(Game1.player, giantTile * 64, item.ParentSheetIndex, item.Quality);

                        throwObject.register();

                        extractForage++;

                    }

                }

            }

        }

    }

}
