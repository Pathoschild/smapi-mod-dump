/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Resources
{
    public class OreResourceSpawner
    {

        public Dictionary<string, OreVein> oreVeins;
        /// <summary>
        /// A list of all visited floors on the current visit to the mines.
        /// </summary>
        public List<int> visitedFloors;

        public OreResourceSpawner()
        {
            this.oreVeins = new Dictionary<string, OreVein>();
            this.visitedFloors = new List<int>();
        }

        public virtual void registerOreVeins()
        {
            //this.oreVeins.Add(ResourceObjectIds.BauxiteOreVein, RevitalizeModCore.ModContentManager.objectManager.getObject<OreVein>(ResourceObjectIds.BauxiteOreVein));
            this.oreVeins.Add(ResourceObjectIds.TinOreVein, RevitalizeModCore.ModContentManager.objectManager.getObject<OreVein>(ResourceObjectIds.TinOreVein));
            this.oreVeins.Add(ResourceObjectIds.LeadOreVein, RevitalizeModCore.ModContentManager.objectManager.getObject<OreVein>(ResourceObjectIds.LeadOreVein));
            this.oreVeins.Add(ResourceObjectIds.SilverOreVein, RevitalizeModCore.ModContentManager.objectManager.getObject<OreVein>(ResourceObjectIds.SilverOreVein));
            //this.oreVeins.Add(ResourceObjectIds.TitaniumOreVein, RevitalizeModCore.ModContentManager.objectManager.getObject<OreVein>(ResourceObjectIds.TitaniumOreVein));
            this.oreVeins.Add(ResourceObjectIds.PrismaticOreVein, RevitalizeModCore.ModContentManager.objectManager.getObject<OreVein>(ResourceObjectIds.PrismaticOreVein));
        }


        /// <summary>
        /// Spawns an ore vein at the given location if possible.
        /// </summary>
        /// <param name="name"></param>
        public bool spawnOreVein(string name, GameLocation Location, Vector2 TilePosition)
        {
            if (this.oreVeins.ContainsKey(name))
            {
                OreVein spawn;
                this.oreVeins.TryGetValue(name, out spawn);
                if (spawn != null)
                {
                    spawn = (OreVein)spawn.getOne();
                    bool spawnable = this.canResourceBeSpawnedHere(spawn, Location, TilePosition);
                    if (spawnable)
                        //ModCore.log("Location is: " + Location.Name);
                        spawn.placementAction(Location, (int)TilePosition.X * Game1.tileSize, (int)TilePosition.Y * Game1.tileSize, null);
                    else
                        RevitalizeModCore.log("Can't spawn ore: " + name + "at tile location: " + TilePosition);
                    return spawnable;
                }
                RevitalizeModCore.log("Key doesn't exist. Weird.");
                return false;
            }
            else
                throw new Exception("The ore dictionary doesn't contain they key for resource: " + name);
        }

        /// <summary>
        /// Spawns an orevein at the tile position at the same location as the player.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="TilePosition"></param>
        /// <returns></returns>
        public bool spawnOreVein(string name, Vector2 TilePosition)
        {
            return this.spawnOreVein(name, Game1.player.currentLocation, TilePosition);
        }

        /// <summary>
        /// Spawns ore in the mine depending on a lot of given variables such as floor level and spawn chance.
        /// </summary>
        public virtual void spawnOreInMine()
        {
            int floorLevel = GameLocationUtilities.GetPlayerCurrentMineLevel();
            if (this.hasVisitedFloor(floorLevel))
                //Already has spawned ores for this visit.
                return;
            else
                this.visitedFloors.Add(floorLevel);
            List<OreVein> spawnableOreVeins = new List<OreVein>();
            //Get a list of all of the ores that can spawn on this mine level.
            foreach (KeyValuePair<string, OreVein> pair in this.oreVeins)
                if (pair.Value.resourceInfo.Value.canSpawnAtLocation() && (pair.Value.resourceInfo.Value as OreResourceInformation).canSpawnOnCurrentMineLevel())
                    spawnableOreVeins.Add(pair.Value);

            foreach (OreVein ore in spawnableOreVeins)
                if (ore.resourceInfo.Value.shouldSpawn())
                {
                    int amount = ore.resourceInfo.Value.getNumberOfNodesToSpawn();
                    List<Vector2> openTiles = GameLocationUtilities.GetOpenObjectTiles(Game1.player.currentLocation, (OreVein)ore.getOne());
                    amount = Math.Min(amount, openTiles.Count); //Only spawn for as many open tiles or the amount of nodes to spawn.
                    for (int i = 0; i < amount; i++)
                    {
                        int position = Game1.random.Next(openTiles.Count);
                        bool didSpawn = this.spawnOreVein(ore.getItemInformation().id, openTiles[position]);
                        if (didSpawn == false)
                        {
                            i--; //If the tile didn't spawn due to some odd reason ensure that the amount is spawned.
                            openTiles.Remove(openTiles[position]);
                        }
                        else
                            openTiles.Remove(openTiles[position]); //Remove that tile from the list of open tiles.
                    }
                }
                else
                {
                    //Ore doesn't meet spawn chance.
                }

        }

        /// <summary>
        /// Checks to see if the player has visited the given floor.
        /// </summary>
        /// <param name="Floor"></param>
        /// <returns></returns>
        public bool hasVisitedFloor(int Floor)
        {
            return this.visitedFloors.Contains(Floor);
        }

        /// <summary>
        /// Source: SDV. 
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <returns></returns>
        private bool isTileOpenForQuarryStone(int tileX, int tileY)
        {
            GameLocation loc = GameLocationUtilities.GetGameLocation(Enums.StardewLocation.Mountain);
            if (loc.doesTileHaveProperty(tileX, tileY, "Diggable", "Back") != null)
                return loc.isTileLocationTotallyClearAndPlaceable(new Vector2(tileX, tileY));
            return false;
        }

        /// <summary>
        /// Update the quarry every day with new ores to spawn.
        /// </summary>
        public virtual void quarryDayUpdate()
        {
            List<OreVein> spawnableOreVeins = new List<OreVein>();
            //Get a list of all of the ores that can spawn on this mine level.
            foreach (KeyValuePair<string, OreVein> pair in this.oreVeins)
                if ((pair.Value.resourceInfo.Value as OreResourceInformation).spawnsInQuarry)
                    spawnableOreVeins.Add(pair.Value);
            foreach (OreVein ore in spawnableOreVeins)
                if ((ore.resourceInfo.Value as OreResourceInformation).shouldSpawnInQuarry())
                {
                    int amount = (ore.resourceInfo.Value as OreResourceInformation).getNumberOfNodesToSpawnQuarry();
                    List<Vector2> openTiles = this.getOpenQuarryTiles(ore);
                    amount = Math.Min(amount, openTiles.Count); //Only spawn for as many open tiles or the amount of nodes to spawn.
                    for (int i = 0; i < amount; i++)
                    {
                        int position = Game1.random.Next(openTiles.Count);
                        bool didSpawn = this.spawnOreVein(ore.getItemInformation().id, GameLocationUtilities.GetGameLocation(Enums.StardewLocation.Mountain), openTiles[position]);
                        if (didSpawn == false)
                        {
                            i--; //If the tile didn't spawn due to some odd reason ensure that the amount is spawned.
                            openTiles.Remove(openTiles[position]);
                            //amount = Math.Min(amount, openTiles.Count); //Only spawn for as many open tiles or the amount of nodes to spawn.
                        }
                        else
                            //ModCore.log("Spawned ore in the quarry!");
                            openTiles.Remove(openTiles[position]); //Remove that tile from the list of open tiles.
                    }
                }
                else
                {
                    //Ore doesn't meet spawn chance.
                }

        }

        /// <summary>
        /// Spawns ore in the mountain farm every day.
        /// </summary>
        public void mountainFarmDayUpdate()
        {
            if (GameLocationUtilities.Farm_IsFarmHiltopFarm() == false)
                //ModCore.log("Farm is not hiltop farm!");
                return;
            GameLocation farm = Game1.getFarm();

            List<OreVein> spawnableOreVeins = new List<OreVein>();
            //Get a list of all of the ores that can spawn on this mine level.
            foreach (KeyValuePair<string, OreVein> pair in this.oreVeins)
                if ((pair.Value.resourceInfo.Value as OreResourceInformation).spawnsOnFarm)
                    spawnableOreVeins.Add(pair.Value);
            foreach (OreVein ore in spawnableOreVeins)
                if ((ore.resourceInfo.Value as OreResourceInformation).shouldSpawnOnFarm())
                {
                    int amount = (ore.resourceInfo.Value as OreResourceInformation).getNumberOfNodesToSpawnFarm();
                    List<Vector2> openTiles = this.getFarmQuarryOpenTiles(ore);
                    if (openTiles.Count == 0)
                    {
                        //ModCore.log("No open farm tiles!");
                    }
                    amount = Math.Min(amount, openTiles.Count); //Only spawn for as many open tiles or the amount of nodes to spawn.
                    for (int i = 0; i < amount; i++)
                    {
                        int position = Game1.random.Next(openTiles.Count);
                        bool didSpawn = this.spawnOreVein(ore.getItemInformation().id, farm, openTiles[position]);
                        if (didSpawn == false)
                        {
                            i--; //If the tile didn't spawn due to some odd reason ensure that the amount is spawned.
                            openTiles.Remove(openTiles[position]);
                            //amount = Math.Min(amount, openTiles.Count); //Only spawn for as many open tiles or the amount of nodes to spawn.
                            //ModCore.log("Did not spawn ore in the farm quarry!");
                        }
                        else
                            //ModCore.log("Spawned ore in the farm quarry!");
                            openTiles.Remove(openTiles[position]); //Remove that tile from the list of open tiles.
                    }
                }
                else
                {
                    //Ore doesn't meet spawn chance.
                }

        }

        /// <summary>
        /// Gets a list of all of the open quarry tiles.
        /// </summary>
        /// <returns></returns>
        private List<Vector2> getOpenQuarryTiles(CustomObject obj)
        {
            List<Vector2> tiles = new List<Vector2>();
            Rectangle r = new Rectangle(106, 13, 21, 21);
            for (int i = r.X; i <= r.X + r.Width; i++)
                for (int j = r.Y; j <= r.Y + r.Height; j++)
                    if (this.isTileOpenForQuarryStone(i, j) && this.canResourceBeSpawnedHere(obj, GameLocationUtilities.GetGameLocation(Enums.StardewLocation.Mountain), new Vector2(i, j)))
                        tiles.Add(new Vector2(i, j));
            if (tiles.Count == 0)
            {
                //ModCore.log("Quarry is full! Can't spawn more resources!");
            }
            return tiles;
        }

        /// <summary>
        /// Gets all of the open tiles in the farm quarry.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private List<Vector2> getFarmQuarryOpenTiles(CustomObject obj)
        {
            List<Vector2> tiles = new List<Vector2>();
            Rectangle r = new Rectangle(5, 37, 22, 8);
            GameLocation farm = Game1.getFarm();
            for (int i = r.X; i <= r.X + r.Width; i++)
                for (int j = r.Y; j <= r.Y + r.Height; j++)
                {
                    Vector2 pos = new Vector2(i, j);
                    if (farm.doesTileHavePropertyNoNull((int)pos.X, (int)pos.Y, "Type", "Back").Equals("Dirt") && this.canResourceBeSpawnedHere(obj, farm, new Vector2(i, j)))
                        tiles.Add(pos);
                }
            if (tiles.Count == 0)
            {
                //ModCore.log("Quarry is full! Can't spawn more resources!");
            }
            return tiles;
        }

        /// <summary>
        /// Checks to see if a resource can be spawned here.
        /// </summary>
        /// <param name="OBJ"></param>
        /// <param name="Location"></param>
        /// <param name="TilePosition"></param>
        /// <returns></returns>
        public bool canResourceBeSpawnedHere(CustomObject OBJ, GameLocation Location, Vector2 TilePosition)
        {
            return OBJ.canBePlacedHere(Location, TilePosition) && Location.isTileLocationTotallyClearAndPlaceable(TilePosition);
        }

    }
}
