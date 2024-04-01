/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using System.Collections.Generic;
using CJB.Common;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace CJBCheatsMenu.Framework.Cheats
{
    /// <summary>A cheat which grows crops and trees under the cursor.</summary>
    internal class GrowCheat : BaseCheat
    {
        /*********
        ** Fields
        *********/
        /// <summary>Whether to grow crops under the cursor.</summary>
        private bool ShouldGrowCrops;

        /// <summary>Whether to grow trees under the cursor.</summary>
        private bool ShouldGrowTrees;

        /// <summary>The last tile position around which crops and trees were grown.</summary>
        private Vector2 LastGrowOrigin;


        /*********
        ** Public methods
        *********/
        /// <summary>Get the config UI fields to show in the cheats menu.</summary>
        /// <param name="context">The cheat context.</param>
        public override IEnumerable<OptionsElement> GetFields(CheatContext context)
        {
            yield break; // handled by cheats menu directly
        }

        /// <summary>Handle the cheat options being loaded or changed.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="needsUpdate">Whether the cheat should be notified of game updates.</param>
        /// <param name="needsInput">Whether the cheat should be notified of button presses.</param>
        /// <param name="needsRendering">Whether the cheat should be notified of render ticks.</param>
        public override void OnConfig(CheatContext context, out bool needsInput, out bool needsUpdate, out bool needsRendering)
        {
            needsInput = context.Config.GrowCropsKey.IsBound || context.Config.GrowTreeKey.IsBound;
            needsUpdate = needsInput;
            needsRendering = false;
        }

        /// <summary>Handle the player pressing or releasing any buttons if <see cref="ICheat.OnSaveLoaded"/> indicated input was needed.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="e">The input event arguments.</param>
        public override void OnButtonsChanged(CheatContext context, ButtonsChangedEventArgs e)
        {
            this.ShouldGrowCrops = context.Config.GrowCropsKey.IsDown();
            this.ShouldGrowTrees = context.Config.GrowTreeKey.IsDown();
        }

        /// <summary>Handle a game update if <see cref="ICheat.OnSaveLoaded"/> indicated updates were needed.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="e">The update event arguments.</param>
        public override void OnUpdated(CheatContext context, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree || (!this.ShouldGrowCrops && !this.ShouldGrowTrees))
                return;

            Vector2 playerTile = Game1.player.Tile;
            if (playerTile != this.LastGrowOrigin || e.IsMultipleOf(30))
            {
                this.Grow(playerTile, radius: context.Config.GrowRadius);
                this.LastGrowOrigin = playerTile;
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Grow crops and trees around the given position.</summary>
        /// <param name="origin">The origin around which to grow crops and trees.</param>
        /// <param name="radius">The number of tiles in each direction to include, not counting the origin.</param>
        public void Grow(Vector2 origin, int radius)
        {
            // get location
            GameLocation location = Game1.currentLocation;
            if (location == null)
                return;

            // check tile area
            foreach (Vector2 tile in CommonHelper.GetTileArea(origin, radius))
            {
                // get target
                TerrainFeature? target = null;
                {
                    // terrain feature
                    if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature terrainFeature))
                    {
                        if (terrainFeature is HoeDirt { crop: not null } dirt)
                            target = dirt;
                        else if (terrainFeature is Bush or FruitTree or Tree)
                            target = terrainFeature;
                    }

                    // indoor pot
                    if (target == null && location.objects.TryGetValue(tile, out SObject obj) && obj is IndoorPot pot)
                    {
                        if (pot.hoeDirt.Value is { crop: not null } dirt)
                            target = dirt;

                        if (pot.bush.Value is { } bush)
                            target = bush;
                    }
                }

                // grow target
                switch (target)
                {
                    case HoeDirt dirt:
                        if (this.ShouldGrowCrops)
                        {
                            Crop crop = dirt.crop;
                            // grow crop using newDay to apply full logic like giant crops, wild seed randomization, etc
                            for (int i = 0; i < 100 && !crop.fullyGrown.Value; i++)
                                crop.newDay(HoeDirt.watered);

                            // trigger regrowth logic for multi-harvest crops
                            crop.growCompletely();
                        }
                        break;

                    case Bush bush:
                        if (this.ShouldGrowCrops && bush.size.Value == Bush.greenTeaBush)
                        {
                            if (bush.getAge() < Bush.daysToMatureGreenTeaBush)
                            {
                                bush.datePlanted.Value = (int)(Game1.stats.DaysPlayed - Bush.daysToMatureGreenTeaBush);
                                bush.dayUpdate(); // update sprite, etc
                            }

                            if (bush.inBloom() && bush.tileSheetOffset.Value == 0)
                                bush.dayUpdate(); // grow tea leaves
                        }
                        break;

                    case FruitTree fruitTree:
                        if (this.ShouldGrowTrees && !fruitTree.stump.Value)
                        {
                            if (fruitTree.growthStage.Value < FruitTree.treeStage)
                            {
                                fruitTree.growthStage.Value = Tree.treeStage;
                                fruitTree.daysUntilMature.Value = 0;
                            }

                            if (fruitTree.IsInSeasonHere())
                                fruitTree.TryAddFruit();
                        }
                        break;

                    case Tree tree:
                        if (this.ShouldGrowTrees && !tree.stump.Value)
                        {
                            if (tree.growthStage.Value < Tree.treeStage)
                                tree.growthStage.Value = Tree.treeStage;

                            tree.wasShakenToday.Value = false;
                        }
                        break;
                }
            }
        }
    }
}
