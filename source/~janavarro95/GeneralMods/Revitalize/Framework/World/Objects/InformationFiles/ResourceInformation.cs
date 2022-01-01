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
using StardewValley;

namespace Revitalize.Framework.Objects.InformationFiles
{
    /// <summary>
    /// Deals with information reguarding resources.
    /// </summary>
    public class ResourceInformation
    {
        /// <summary>
        /// The item to drop.
        /// </summary>
        public StardewValley.Object droppedItem;

        /// <summary>
        /// The min amount of resources to drop given the getNumberOfDrops function.
        /// </summary>
        public int minResourcePerDrop;
        /// <summary>
        /// The max amount of resources to drop given the getNumberOfDrops function.
        /// </summary>
        public int maxResourcePerDrop;
        /// <summary>
        /// The min amount of nodes that would be spawned given the getNumberOfNodesToSpawn function.
        /// </summary>
        public int minNumberOfNodesSpawned;
        /// <summary>
        /// The max amount of nodes that would be spawned given the getNumberOfNodesToSpawn function.
        /// </summary>
        public int maxNumberOfNodesSpawned;
        /// <summary>
        /// The influence multiplier that luck has on how many nodes of this resource spawn.
        /// </summary>
        public double spawnAmountLuckFactor;
        /// <summary>
        /// The influence multiplier that luck has on ensuring the resource spawns.
        /// </summary>
        public double spawnChanceLuckFactor;

        public double dropAmountLuckFactor;
        /// <summary>
        /// The influence multiplier that luck has on ensuring the resource drops.
        /// </summary>
        public double dropChanceLuckFactor;

        /// <summary>
        /// The chance for the resource to spawn from 0.0 ~ 1.0
        /// </summary>
        public double chanceToSpawn;
        /// <summary>
        /// The chance for the resource to drop from 0.0 ~ 1.0
        /// </summary>
        public double chanceToDrop;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public ResourceInformation()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="I"></param>
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
        public ResourceInformation(StardewValley.Object I, int MinDropAmount, int MaxDropAmount, int MinNumberOfNodes, int MaxNumberOfNodes,double ChanceToSpawn=1f,double ChanceToDrop=1f, double SpawnChanceLuckFactor = 0f, double SpawnAmountLuckFactor = 0f,double DropChanceLuckFactor=0f, double DropAmountLuckFactor = 0f)
        {
            this.droppedItem = I;
            this.minResourcePerDrop = MinDropAmount;
            this.maxResourcePerDrop = MaxDropAmount;
            this.minNumberOfNodesSpawned = MinNumberOfNodes;
            this.maxNumberOfNodesSpawned = MaxNumberOfNodes;
            this.spawnAmountLuckFactor = SpawnAmountLuckFactor;
            this.dropAmountLuckFactor = DropAmountLuckFactor;
            this.chanceToSpawn = ChanceToSpawn;
            this.chanceToDrop = ChanceToDrop;
            this.spawnChanceLuckFactor = SpawnChanceLuckFactor;
            this.dropChanceLuckFactor = DropChanceLuckFactor;
        }


        /// <summary>
        /// Gets the number of drops to spawn for the given resource;
        /// </summary>
        /// <returns></returns>
        public virtual int getNumberOfDropsToSpawn(bool limitToMax = true)
        {
            int amount = Game1.random.Next(this.minResourcePerDrop, this.maxResourcePerDrop + 1);

            if (limitToMax)
            {
                amount = (int)Math.Min(amount + (this.dropAmountLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)), this.maxResourcePerDrop);
            }
            else
            {
                amount = (int)(amount + (this.dropAmountLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)));
            }
            return amount;
        }

        /// <summary>
        /// Gets the number of resource nodes to spawn when spawning multiple clusters.
        /// </summary>
        /// <returns></returns>
        public virtual int getNumberOfNodesToSpawn(bool limitToMax = true)
        {
            int amount = Game1.random.Next(this.minNumberOfNodesSpawned, this.maxNumberOfNodesSpawned + 1);
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
        /// Checks to see if the resource can spawn at the player's location.
        /// </summary>
        /// <returns></returns>
        public virtual bool canSpawnAtLocation()
        {
            return true;
        }
        /// <summary>
        /// Checks to see if the resource can spawn at the given location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual bool canSpawnAtLocation(GameLocation location)
        {
            return true;
        }

        /// <summary>
        /// Checks to see if this resource's spawn chance is greater than the spawn chance it is checked against.
        /// </summary>
        /// <returns></returns>
        public virtual bool shouldSpawn()
        {
            double chance = Game1.random.NextDouble();
            chance = (chance - (this.spawnChanceLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)));
            if (this.chanceToSpawn >= chance) return true;
            else return false;
        }

        /// <summary>
        /// Checks to see if this resource's drop chance is greater than the spawn chance it is checked against.
        /// </summary>
        /// <returns></returns>
        public virtual bool shouldDropResource()
        {
            double chance = Game1.random.NextDouble();
            chance= (chance - (this.dropChanceLuckFactor * (Game1.player.LuckLevel + Game1.player.addedLuckLevel.Value)));

            if (this.chanceToDrop >= chance) return true;
            else return false;
        }

        /// <summary>
        /// Gets an item that should be dropped from this resource with the appropriate drop amount;
        /// </summary>
        /// <returns></returns>
        public Item getItemDrops()
        {
            Item I = this.droppedItem.getOne();
            I.Stack = this.getNumberOfDropsToSpawn();
            return I;
        }
    }
}
