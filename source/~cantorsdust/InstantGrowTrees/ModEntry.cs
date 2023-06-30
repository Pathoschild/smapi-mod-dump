/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using cantorsdust.Common;
using InstantGrowTrees.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace InstantGrowTrees
{
    /// <summary>The entry class called by SMAPI.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>The number of days after a tree is fully grown until it reaches iridium quality.</summary>
        /// <remarks>Fruit trees increase in quality once per year, so iridium is 30 days * 4 seasons * 3 quality increases.</remarks>
        private const int FruitTreeIridiumDays = 360;


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            CommonHelper.RemoveObsoleteFiles(this, "InstantGrowTrees.pdb");

            this.Config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            GenericModConfigMenuIntegration.Register(this.ModManifest, this.Helper.ModRegistry, this.Monitor,
                getConfig: () => this.Config,
                reset: () => this.Config = new(),
                save: () => this.Helper.WriteConfig(this.Config)
            );
        }

        /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // When the player loads a saved game, or after the overnight save,
            // check for any trees that should be grown.
            this.GrowTrees();
        }

        /****
        ** Methods
        ****/
        /// <summary>Grow all trees eligible for growth.</summary>
        private void GrowTrees()
        {
            // collect config
            bool ageFruitTrees = this.Config.FruitTrees.InstantlyAge;
            bool growFruitTrees = this.Config.FruitTrees.InstantlyGrow;
            bool growOtherTrees = this.Config.NonFruitTrees.InstantlyGrow;
            if (!ageFruitTrees && !growFruitTrees && !growOtherTrees)
                return;

            // apply
            foreach (GameLocation location in this.GetLocations())
            {
                foreach (KeyValuePair<Vector2, TerrainFeature> entry in location.terrainFeatures.Pairs)
                {
                    Vector2 tile = entry.Key;
                    TerrainFeature feature = entry.Value;
                    switch (feature)
                    {
                        case FruitTree fruitTree:
                            if (growFruitTrees)
                                this.GrowFruitTree(fruitTree, location, tile);
                            if (ageFruitTrees)
                                this.AgeFruitTree(fruitTree);
                            break;

                        case Tree tree:
                            if (growOtherTrees)
                                this.GrowTree(tree, location, tile);
                            break;
                    }
                }
            }
        }

        /// <summary>Get all game locations.</summary>
        private IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }

        /// <summary>Grow a tree if it's eligible for growth.</summary>
        /// <param name="tree">The tree to grow.</param>
        /// <param name="location">The tree's location.</param>
        /// <param name="tile">The tree's tile position.</param>
        private void GrowTree(Tree tree, GameLocation location, Vector2 tile)
        {
            RegularTreeConfig config = this.Config.NonFruitTrees;

            // skip if can't grow
            if (tree.growthStage.Value >= Tree.treeStage || !this.CanGrowNow(location, config.InstantlyGrowInWinter))
                return;

            // check growth rules
            int maxStage = Tree.treeStage;
            if (!config.InstantlyGrowWhenInvalid)
            {
                // ignore trees on no-spawn tiles
                string isNoSpawn = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "NoSpawn", "Back");
                if (isNoSpawn != null && (isNoSpawn == "All" || isNoSpawn == "Tree"))
                    return;

                // ignore blocked seeds
                if (tree.growthStage.Value == Tree.seedStage && location.objects.ContainsKey(tile))
                    return;

                // stunt tree if blocked
                foreach (Vector2 surroundingTile in Utility.getSurroundingTileLocationsArray(tile))
                {
                    // get tree on this tile
                    if (!location.terrainFeatures.TryGetValue(surroundingTile, out TerrainFeature feature) || !(feature is Tree otherTree))
                        continue;

                    // check if blocking growth
                    if (otherTree.growthStage.Value >= Tree.treeStage)
                    {
                        maxStage = Tree.treeStage - 1;
                        break;
                    }
                }
            }

            // grow tree to max allowed
            tree.growthStage.Value = maxStage;
        }

        /// <summary>Grow a fruit tree if it's eligible for growth.</summary>
        /// <param name="tree">The tree to grow.</param>
        /// <param name="location">The tree's location.</param>
        /// <param name="tile">The tree's tile position.</param>
        private void GrowFruitTree(FruitTree tree, GameLocation location, Vector2 tile)
        {
            FruitTreeConfig config = this.Config.FruitTrees;

            // skip if can't grow
            bool isGrown = tree.growthStage.Value >= FruitTree.treeStage;
            bool isMature = tree.daysUntilMature.Value <= 0;
            if ((isGrown && isMature) || !this.CanGrowNow(location, config.InstantlyGrowInWinter))
                return;

            // ignore if tree blocked
            if (!config.InstantlyGrowWhenInvalid)
            {
                foreach (Vector2 adjacentTile in Utility.getSurroundingTileLocationsArray(tile))
                {
                    bool occupied =
                        location.isTileOccupied(adjacentTile)
                        && (
                            !location.terrainFeatures.ContainsKey(tile)
                            || !(location.terrainFeatures[tile] is HoeDirt)
                            || ((HoeDirt)location.terrainFeatures[tile]).crop == null
                        );
                    if (occupied)
                        return;
                }
            }

            // grow tree
            if (!isGrown)
                tree.growthStage.Value = FruitTree.treeStage;
            if (!isMature)
                tree.daysUntilMature.Value = 0;
        }

        /// <summary>Age a fruit tree if it's eligible for aging.</summary>
        /// <param name="tree">The tree to grow.</param>
        private void AgeFruitTree(FruitTree tree)
        {
            FruitTreeConfig config = this.Config.FruitTrees;

            // skip if can't be aged
            if (!config.InstantlyAge || tree.growthStage.Value < FruitTree.treeStage || tree.daysUntilMature.Value <= -ModEntry.FruitTreeIridiumDays)
                return;

            // age tree
            tree.daysUntilMature.Value = -ModEntry.FruitTreeIridiumDays;
        }

        /// <summary>Get whether a tree can grow now for the location.</summary>
        /// <param name="location">The tree's location.</param>
        /// <param name="growInWinter">Whether the tree should grow in winter.</param>
        private bool CanGrowNow(GameLocation location, bool growInWinter)
        {
            return
                growInWinter
                || Game1.currentSeason != "winter"
                || location.IsGreenhouse
                || !location.IsOutdoors
                || location is Desert;
        }
    }
}
