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
using Newtonsoft.Json;
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Revitalize.Framework.Objects.InformationFiles
{
    public class OreResourceInformation:ResourceInformation
    {


        /// <summary>
        /// The floors of the mine that this resource should spawn in in the regular mine.
        /// </summary>
        public List<IntRange> floorsToSpawnOn;

        /// <summary>
        /// A function that compares whether or not the resource can be spawned on this floor. Used in conjecture with floorsToSpawnOn
        /// </summary>
        [JsonIgnore]
        public Func<int, bool> canSpawnOnThisFloor;
        [JsonIgnore]
        public Func<int, bool> excludeSpawnOnThisFloor;

        /// <summary>
        /// The list of floors to exclude spawning on in the regular mine.
        /// </summary>
        public List<IntRange> floorsToExclude;

        /// <summary>
        /// The floors this resource should spawn on in skull cave.
        /// </summary>
        public List<IntRange> floorsToSpawnOnSkullCave;
        /// <summary>
        /// The floors this resource should not spawn on in skull cave.
        /// </summary>
        public List<IntRange> floorsToExcludeSkullCave;

        /// <summary>
        /// A function that compares whether or not the resource can be spawned on this floor in skull cave. Used in conjecture with floorsToSpawnOn
        /// </summary>
        ///
        [JsonIgnore]
        public Func<int, bool> canSpawnOnThisFloorSkullCave;
        [JsonIgnore]
        public Func<int, bool> excludeSpawnOnThisFloorSkullCave;

        /// <summary>
        /// Should this resource spawn in the mine in the mountains?
        /// </summary>
        public bool spawnInRegularMine;
        /// <summary>
        /// Should this resource spawn in Skull Cavern?
        /// </summary>
        public bool spawnInSkullCavern;
        /// <summary>
        /// Should this resource spawn on farms. Notably the hiltop farm?
        /// </summary>
        public bool spawnsOnFarm;
        /// <summary>
        /// Should this resource spawn in the quarry?
        /// </summary>
        public bool spawnsInQuarry;

        /// <summary>
        /// The range of the number of nodes to spawn on the farm.
        /// </summary>
        public IntRange farmSpawnAmount;
        /// <summary>
        /// The range of the number of nodes to spawn in the quarry.
        /// </summary>
        public IntRange quarrySpawnAmount;
        /// <summary>
        /// The range of the number of nodes to spawn in skull cave.
        /// </summary>
        public IntRange skullCaveSpawnAmount;
        /// <summary>
        /// The chance that this resource spawns on the farm.
        /// </summary>
        public double farmSpawnChance;
        /// <summary>
        /// The chance that this resource spawns in the quarry.
        /// </summary>
        public double quarrySpawnChance;
        /// <summary>
        /// The chance that this resource spawns in skull cave.
        /// </summary>
        public double skullCaveSpawnChance;

        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public OreResourceInformation() : base()
        {

        }

        /// <summary>
        /// Constructor for a resource that spawns only in the regular mine.
        /// </summary>
        /// <param name="I"></param>
        /// <param name="FloorsToSpawnOn"></param>
        /// <param name="FloorsToExclude"></param>
        /// <param name="MinDropAmount"></param>
        /// <param name="MaxDropAmount"></param>
        /// <param name="MinNumberOfNodes"></param>
        /// <param name="MaxNumberOfNodes"></param>
        /// <param name="ChanceToSpawn"></param>
        /// <param name="ChanceToDrop"></param>
        /// <param name="SpawnChanceLuckFactor"></param>
        /// <param name="SpawnAmountLuckFactor"></param>
        /// <param name="DropChanceLuckFactor"></param>
        /// <param name="DropAmountLuckFactor"></param>
        public OreResourceInformation(StardewValley.Object I, List<IntRange> FloorsToSpawnOn, List<IntRange> FloorsToExclude,Func<int,bool> CanSpawnOnGivenFloor,Func<int,bool> FloorsToExcludeFun,int MinDropAmount, int MaxDropAmount, int MinNumberOfNodes, int MaxNumberOfNodes, double ChanceToSpawn = 1f, double ChanceToDrop = 1f, double SpawnChanceLuckFactor = 0f, double SpawnAmountLuckFactor = 0f, double DropChanceLuckFactor = 0f, double DropAmountLuckFactor = 0f) : base(I, MinDropAmount, MaxDropAmount, MinNumberOfNodes, MaxNumberOfNodes, ChanceToSpawn, ChanceToDrop, SpawnChanceLuckFactor, SpawnAmountLuckFactor, DropChanceLuckFactor, DropAmountLuckFactor)
        {
            this.spawnsOnFarm = false;
            this.spawnsInQuarry = false;
            this.floorsToSpawnOn = FloorsToSpawnOn;
            this.floorsToExclude = FloorsToExclude != null ? FloorsToExclude : new List<IntRange>();
            this.spawnInRegularMine = true;
            this.spawnInSkullCavern = false;

            this.farmSpawnAmount = new IntRange();
            this.quarrySpawnAmount = new IntRange();
            this.farmSpawnChance = 0f;
            this.quarrySpawnChance = 0f;

            if (CanSpawnOnGivenFloor != null)
            {
                this.canSpawnOnThisFloor = CanSpawnOnGivenFloor;
            }
            else
            {
                this.canSpawnOnThisFloor = null;
            }

            this.excludeSpawnOnThisFloor = FloorsToExcludeFun;

            this.canSpawnOnThisFloorSkullCave = null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="I"></param>
        /// <param name="SpawnsOnFarm"></param>
        /// <param name="SpawnsInQuarry"></param>
        /// <param name="SpawnInRegularMine"></param>
        /// <param name="SpawnInSkullCave"></param>
        /// <param name="FloorsToSpawnOn"></param>
        /// <param name="FloorsToExclude"></param>
        /// <param name="MinDropAmount"></param>
        /// <param name="MaxDropAmount"></param>
        /// <param name="MinNumberOfNodes"></param>
        /// <param name="MaxNumberOfNodes"></param>
        /// <param name="FarmSpawnAmount"></param>
        /// <param name="QuarrySpawnAmount"></param>
        /// <param name="SkullCaveSpawnAmount"></param>
        /// <param name="FloorsToSpawnOnSkullCave"></param>
        /// <param name="FloorsToExludeSkullCave"></param>
        /// <param name="ChanceToSpawn"></param>
        /// <param name="FarmSpawnChance"></param>
        /// <param name="QuarrySpawnChance"></param>
        /// <param name="SkullCaveSpawnChance"></param>
        /// <param name="ChanceToDrop"></param>
        /// <param name="SpawnChanceLuckFactor"></param>
        /// <param name="SpawnAmountLuckFactor"></param>
        /// <param name="DropChanceLuckFactor"></param>
        /// <param name="DropAmountLuckFactor"></param>
        public OreResourceInformation(StardewValley.Object I,bool SpawnsOnFarm, bool SpawnsInQuarry, bool SpawnInRegularMine, bool SpawnInSkullCave,List<IntRange> FloorsToSpawnOn,List<IntRange>FloorsToExclude,Func<int,bool> CanSpawnOnGivenFloor,Func<int,bool> FloorsToExludeFun,int MinDropAmount, int MaxDropAmount, int MinNumberOfNodes, int MaxNumberOfNodes, IntRange FarmSpawnAmount,IntRange QuarrySpawnAmount,IntRange SkullCaveSpawnAmount,List<IntRange> FloorsToSpawnOnSkullCave,List<IntRange>FloorsToExludeSkullCave,Func<int,bool> CanSpawnOnGivenFloorSkullCave,Func<int,bool>FloorsToExludeFunSkullCave,double ChanceToSpawn = 1f,double FarmSpawnChance=1f,double QuarrySpawnChance=1f,double SkullCaveSpawnChance=1f,double ChanceToDrop = 1f, double SpawnChanceLuckFactor = 0f, double SpawnAmountLuckFactor = 0f, double DropChanceLuckFactor = 0f, double DropAmountLuckFactor = 0f) : base(I, MinDropAmount, MaxDropAmount, MinNumberOfNodes, MaxNumberOfNodes,ChanceToSpawn,ChanceToDrop,SpawnChanceLuckFactor,SpawnAmountLuckFactor,DropChanceLuckFactor,DropAmountLuckFactor)
        {
            // Deals with setting where this ore can spawn.
            this.spawnsOnFarm = SpawnsOnFarm;
            this.spawnsInQuarry = SpawnsInQuarry;
            this.spawnInRegularMine = SpawnInRegularMine;
            this.spawnInSkullCavern = SpawnInSkullCave;

            //Deals with inclusion/Exclusion for floors in regular mine.
            this.floorsToSpawnOn = this.spawnInRegularMine?FloorsToSpawnOn:new List<IntRange>();
            this.floorsToExclude = FloorsToExclude != null ? FloorsToExclude : new List<IntRange>();

            ///Checks if a given resource shouds spawn and if not sets defaulted 0 values.
            this.farmSpawnAmount = this.spawnsOnFarm? FarmSpawnAmount:new IntRange(0,0);
            this.quarrySpawnAmount =this.spawnsInQuarry? QuarrySpawnAmount:new IntRange(0,0);
            this.skullCaveSpawnAmount = this.spawnInSkullCavern ? SkullCaveSpawnAmount : new IntRange(0, 0);
            this.farmSpawnChance = this.spawnsOnFarm?FarmSpawnChance:0f;
            this.quarrySpawnChance = this.spawnsInQuarry?QuarrySpawnChance:0f;
            this.skullCaveSpawnChance = this.spawnInSkullCavern ? SkullCaveSpawnChance : 0f;

            //Deals with inclusion/Exclusion for floors in skull cave.
            this.floorsToExcludeSkullCave = FloorsToExludeSkullCave!=null? FloorsToExludeSkullCave: new List<IntRange>();
            this.floorsToSpawnOnSkullCave = FloorsToSpawnOnSkullCave!=null? FloorsToSpawnOnSkullCave: new List<IntRange>();

            this.canSpawnOnThisFloorSkullCave = this.spawnInSkullCavern ? CanSpawnOnGivenFloorSkullCave : null;
            this.excludeSpawnOnThisFloorSkullCave = FloorsToExludeFunSkullCave;
            this.excludeSpawnOnThisFloor = FloorsToExludeFun;


        }

        /// <summary>
        /// Gets the number of drops that should spawn for this ore.
        /// </summary>
        /// <param name="limitToMax"></param>
        /// <returns></returns>
        public override int getNumberOfDropsToSpawn(bool limitToMax = true)
        {
            return base.getNumberOfDropsToSpawn(limitToMax);
        }

        /// <summary>
        /// Gets the number of nodes that should spawn for this ore.
        /// </summary>
        /// <param name="limitToMax"></param>
        /// <returns></returns>
        public override int getNumberOfNodesToSpawn(bool limitToMax = true)
        {
            return base.getNumberOfNodesToSpawn(limitToMax);
        }

        /// <summary>
        /// Gets the number of nodes to spawn in the farm.
        /// </summary>
        /// <param name="limitToMax"></param>
        /// <returns></returns>
        public virtual int getNumberOfNodesToSpawnFarm(bool limitToMax = true)
        {
            if (this.spawnsOnFarm == false || this.farmSpawnAmount == null) return 0;
            int amount = this.farmSpawnAmount.getRandomInclusive();
            if (limitToMax)
            {
                amount = (int)Math.Min(amount + (this.spawnAmountLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)), this.maxNumberOfNodesSpawned);
            }
            else
            {
                amount = (int)(amount + (this.spawnAmountLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)));
            }
            return amount;
        }
        /// <summary>
        /// Gets the number of nodes to spawn in the quarry.
        /// </summary>
        /// <param name="limitToMax"></param>
        /// <returns></returns>
        public virtual int getNumberOfNodesToSpawnQuarry(bool limitToMax = true)
        {
            if (this.spawnsInQuarry == false || this.quarrySpawnAmount == null) return 0;
            int amount = this.quarrySpawnAmount.getRandomInclusive();
            if (limitToMax)
            {
                amount = (int)Math.Min(amount + (this.spawnAmountLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)), this.maxNumberOfNodesSpawned);
            }
            else
            {
                amount = (int)(amount + (this.spawnAmountLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)));
            }
            return amount;
        }
        /// <summary>
        /// Gets the number of nodes to spawn in skull cave.
        /// </summary>
        /// <param name="limitToMax"></param>
        /// <returns></returns>
        public virtual int getNumberOfNodesToSpawnSkullCavern(bool limitToMax = true)
        {
            if (this.spawnInSkullCavern == false || this.skullCaveSpawnAmount == null) return 0;
            int amount = this.quarrySpawnAmount.getRandomInclusive();
            if (limitToMax)
            {
                amount = (int)Math.Min(amount + (this.spawnAmountLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)), this.maxNumberOfNodesSpawned);
            }
            else
            {
                amount = (int)(amount + (this.spawnAmountLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)));
            }
            return amount;
        }


        /// <summary>
        /// Checks to see if this resource can spawn in the quarry based off of RNG.
        /// </summary>
        /// <returns></returns>
        public virtual bool shouldSpawnInQuarry()
        {
            double chance = Game1.random.NextDouble();
            chance = (chance - (this.spawnChanceLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)));
            if (this.quarrySpawnChance >= chance) return true;
            else return false;
        }

        /// <summary>
        /// Checks to see if this resource should spawn on the farm based of of rng.
        /// </summary>
        /// <returns></returns>
        public virtual bool shouldSpawnOnFarm()
        {
            double chance = Game1.random.NextDouble();
            chance = (chance - (this.spawnChanceLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)));
            if (this.farmSpawnChance >= chance) return true;
            else return false;
        }

        /// <summary>
        /// Checks to see if this resource should spawn in the skull cavern based of of rng.
        /// </summary>
        /// <returns></returns>
        public virtual bool shouldSpawnInSkullCave()
        {
            double chance = Game1.random.NextDouble();
            chance = (chance - (this.spawnChanceLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)));
            if (this.skullCaveSpawnChance >= chance) return true;
            else return false;
        }


        /// <summary>
        /// Can this ore spawn at the given location?
        /// </summary>
        /// <returns></returns>
        public override bool canSpawnAtLocation()
        {
            if (this.spawnsOnFarm && Game1.player.currentLocation is StardewValley.Farm)
            {
                return true;
            }
            if (this.spawnsInQuarry && Game1.player.currentLocation is StardewValley.Locations.Mountain)
            {
                return true;
            }
            if (this.spawnInRegularMine && LocationUtilities.IsPlayerInMine())
            {
                return true;
            }
            if (this.spawnInSkullCavern && LocationUtilities.IsPlayerInSkullCave())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the resource can spawn at the given game location.
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        public override bool canSpawnAtLocation(GameLocation Location)
        {
            if(this.spawnsOnFarm && Location is StardewValley.Farm)
            {
                return true;
            }
            if(this.spawnsInQuarry && Location is StardewValley.Locations.Mountain)
            {
                return true;
            }
            if(this.spawnInRegularMine && LocationUtilities.IsPlayerInMine())
            {
                return true;
            }
            if(this.spawnInSkullCavern && LocationUtilities.IsPlayerInSkullCave())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if this ore can be spawned on the current mine level.
        /// </summary>
        /// <returns></returns>
        public bool canSpawnOnCurrentMineLevel()
        {
            int level=LocationUtilities.CurrentMineLevel();
            foreach(IntRange range in this.floorsToSpawnOn)
            {
                bool compareFun = false;
                if (this.canSpawnOnThisFloor == null)
                {
                    compareFun = false;
                }
                else
                {
                    compareFun = this.canSpawnOnThisFloor(level);
                }

                if (range.ContainsInclusive(level) || compareFun==true)
                {
                    foreach(IntRange exclude in this.floorsToExclude)
                    {
                        bool excludeFun = false;
                        if (this.excludeSpawnOnThisFloor == null)
                        {
                            excludeFun = false;
                        }
                        else
                        {
                            excludeFun = this.excludeSpawnOnThisFloor(level);
                        }
                        //Make this include exlude fun for regular mine. See above in this function.
                        if (exclude.ContainsInclusive(level) || excludeFun) return false;
                    }
                    return true;
                }
            }
            return false;
        }
        
    }
}
