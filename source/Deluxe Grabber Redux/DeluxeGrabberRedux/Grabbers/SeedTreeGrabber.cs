/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace DeluxeGrabberRedux.Grabbers
{
    class SeedTreeGrabber : TerrainFeaturesMapGrabber
    {
        public SeedTreeGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
        }

        public override bool GrabFeature(Vector2 tile, TerrainFeature feature)
        {
            if (Config.seedTrees && feature is Tree tree && IsHarvestableSeedTree(tree))
            {
                List<Item> items = new List<Item>();

                int seedIndex = -1;
                switch (tree.treeType.Value)
                {
                    case 3:
                        seedIndex = ItemIds.PineCone;
                        break;
                    case 1:
                        seedIndex = ItemIds.Acorn;
                        break;
                    case 8:
                        seedIndex = ItemIds.MahoganySeed;
                        break;
                    case 2:
                        seedIndex = ItemIds.MapleSeed;
                        break;
                    case 6:
                    case 9:
                        seedIndex = ItemIds.Coconut;
                        break;
                }
                if (Game1.GetSeasonForLocation(Location).Equals("fall") && tree.treeType.Value == 2 && Game1.dayOfMonth >= 14)
                {
                    seedIndex = ItemIds.Hazelnut;
                }
                if (seedIndex != -1)
                {
                    items.Add(new SObject(seedIndex, 1));
                }
                if (seedIndex == 88 && new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)tile.X * 13 + (int)tile.Y * 54).NextDouble() < 0.1)
                {
                    items.Add(new SObject(ItemIds.GoldenCoconut, 1));
                }
                if (Game1.random.NextDouble() <= 0.5 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                {
                    items.Add(new SObject(ItemIds.QiBeans, 1));
                }

                if (TryAddItems(items))
                {
                    tree.hasSeed.Value = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool IsHarvestableSeedTree(Tree tree)
        {
            return tree.growthStage.Value >= 5 && !tree.stump.Value && tree.hasSeed.Value && (Game1.IsMultiplayer || Player.ForagingLevel >= 1);
        }
    }
}
