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
using static StardewValley.Minigames.CraneGame;
using xTile.Layers;
using static StardewDruid.Cast.Weald.Wildbounty;

namespace StardewDruid.Cast.Weald
{
    public class Wildgrowth
    {
        public enum spawns
        {   
            flower,
            forage,
            tree,
            grass,
        }

        public Dictionary<Vector2, List<spawns>> spawnProspects = new();

        public Dictionary<Vector2, bool> activeProspects = new();

        GameLocation location;

        public Vector2 target;

        int spawnTree;

        int spawnForage;


        public Wildgrowth() { }

        public void CastActivate(GameLocation Location, Vector2 Target)
        {

            location = Location;

            target = Target;

            bool treeDirection = true;

            Dictionary<Vector2, int> targets = new();

            for(int i =0; i < 9; i++)
            {
                for(int j = 0; j < 9; j++)
                {

                    targets.Add(target + new Vector2(i, j), 0);

                }

            }

            Vector2 forageVector = new((int)(target.X - (target.X % 17)), (int)(target.Y - (target.Y % 17)));

            Layer back = location.map.GetLayer("Back");

            bool grass = false;

            int ignore = Mod.instance.randomIndex.Next(4);

            if (ModUtility.DirectionToTarget(Game1.player.Position, target * 64)[0] == Game1.player.FacingDirection)
            {

                treeDirection = false;

            }

            for (int f = targets.Count - 1; f >= 0; f--)
            {

                if (f % 4 != ignore) { continue; }

                KeyValuePair<Vector2, int> check = targets.ElementAt(f);

                spawnProspects[check.Key] = new();

                string tileCheck = ModUtility.GroundCheck(location, check.Key);

                if (tileCheck != "ground")
                {

                    continue;

                }

                Dictionary<string, List<Vector2>> occupied = ModUtility.NeighbourCheck(location, check.Key, 0, 0);

                if (occupied.Count != 0)
                {

                    continue;

                }

                Dictionary<string, List<Vector2>> neighbours = ModUtility.NeighbourCheck(location, check.Key, 1, 1);

                if (neighbours.Count > 0)
                {

                    if (Mod.instance.Config.disableGrass || !Mod.instance.rite.spawnIndex["grass"])
                    {
                        
                        continue;

                    }

                    bool goodNeighbours = true;

                    foreach(KeyValuePair<string,List<Vector2>> neighbour in neighbours)
                    {

                        if(neighbour.Key != "Grass" && neighbour.Key != "Tree")
                        {
                            Mod.instance.Monitor.Log(neighbour.Key, StardewModdingAPI.LogLevel.Debug);
                            goodNeighbours = false;

                            break;

                        }

                    }

                    if (!goodNeighbours)
                    {

                        continue;

                    }

                    grass = true;

                }

                int tileX = (int)check.Key.X;

                int tileY = (int)check.Key.Y;

                Tile backTile = back.PickTile(new xTile.Dimensions.Location(tileX * 64, tileY * 64), Game1.viewport.Size);

                if (backTile.TileIndexProperties.TryGetValue("Type", out var typeValue))
                {

                    if (typeValue == "Dirt" || backTile.TileIndexProperties.TryGetValue("Diggable", out _))
                    {

                        if (grass)
                        {

                            spawnProspects[check.Key].Add(spawns.grass); 
                            
                            activeProspects[check.Key] = true; 
                            
                            continue;

                        }

                        /*if (Mod.instance.rite.spawnIndex["trees"] && !Mod.instance.EffectDisabled("Trees") && treeDirection)
                        {

                            spawnProspects[check.Key].Add(spawns.tree);

                            activeProspects[check.Key] = true;
                        }*/

                        continue;

                    }

                    if (typeValue == "Grass" && backTile.TileIndexProperties.TryGetValue("NoSpawn", out _) == false)
                    {

                        if (grass)
                        {

                            spawnProspects[check.Key].Add(spawns.grass);

                            activeProspects[check.Key] = true;

                            continue;

                        }

                        if (Mod.instance.rite.spawnIndex["trees"] && !Mod.instance.Config.disableTrees && treeDirection)
                        {

                            spawnProspects[check.Key].Add(spawns.tree);

                            activeProspects[check.Key] = true;
                        }

                        if (Mod.instance.rite.spawnIndex["forage"] && !Mod.instance.rite.terrainCasts[location.Name].ContainsKey(forageVector))
                        {

                            spawnProspects[check.Key].Add(spawns.forage);

                            if (Mod.instance.questHandle.IsComplete(QuestHandle.wealdThree))
                            {

                                spawnProspects[check.Key].Add(spawns.flower);

                            }

                            activeProspects[check.Key] = true;

                        }

                        continue;

                    }

                }

            }

            if (activeProspects.Count > 0)
            {

                KeyValuePair<Vector2, bool> prospect = activeProspects.ElementAt(Mod.instance.randomIndex.Next(activeProspects.Count));

                spawns spawn = spawnProspects[prospect.Key].ElementAt(Mod.instance.randomIndex.Next(spawnProspects[prospect.Key].Count));

                switch (spawn)
                {

                    case spawns.tree:

                        SpawnTrees(prospect.Key);

                        break;

                    case spawns.flower:

                        SpawnFlower(prospect.Key);

                        break;

                    case spawns.forage:

                        SpawnForage(prospect.Key);

                        break;

                    case spawns.grass:

                        SpawnGrass(prospect.Key);

                        break;

                }

                if(spawnForage > 0)
                {
                    
                    if (!Mod.instance.questHandle.IsComplete(QuestHandle.wealdThree))
                    {

                        Mod.instance.questHandle.UpdateTask(QuestHandle.wealdThree, 1);

                    }

                    Mod.instance.rite.castCost = spawnForage * 8;

                    Mod.instance.rite.terrainCasts[location.Name][forageVector] = 1;

                }

            }

        }

        public void SpawnGrass(Vector2 tile)
        {

            StardewValley.TerrainFeatures.Grass grassFeature = new(1, 4);

            location.terrainFeatures.Add(tile, grassFeature);

            Microsoft.Xna.Framework.Rectangle tileRectangle = new((int)tile.X * 64 + 1, (int)tile.Y * 64 + 1, 62, 62);

            grassFeature.doCollisionAction(tileRectangle, 2, tile, Game1.player);

            Mod.instance.iconData.CursorIndicator(location, tile * 64, IconData.cursors.weald, new());

        }


        public void SpawnTrees(Vector2 tile)
        {

            ModUtility.RandomTree(location, tile);

            spawnTree++;

            Mod.instance.iconData.CursorIndicator(location, tile * 64, IconData.cursors.weald, new());

        }

        public void SpawnFlower(Vector2 tile)
        {

            int randomCrop = SpawnData.RandomFlower();

            StardewValley.Object newFlower = new(
                        randomCrop.ToString(), 1
                );

            newFlower.IsSpawnedObject = true;

            newFlower.Location = location;

            newFlower.TileLocation = tile;

            if(location.objects.TryAdd(tile, newFlower))
            {

                spawnForage++;

            }

            Mod.instance.iconData.CursorIndicator(location, tile * 64, IconData.cursors.weald, new());

        }

        public void SpawnForage(Vector2 tile)
        {

            int randomCrop = SpawnData.RandomForage(location);

            StardewValley.Object newForage = new StardewValley.Object(
                randomCrop.ToString(), 1
            );

            newForage.IsSpawnedObject = true;

            newForage.Location = location;

            newForage.TileLocation = tile;

            if (location.objects.TryAdd(tile, newForage))
            {
                spawnForage++;
            }

            Mod.instance.iconData.CursorIndicator(location, tile * 64, IconData.cursors.weald, new());

        }


    }

}
