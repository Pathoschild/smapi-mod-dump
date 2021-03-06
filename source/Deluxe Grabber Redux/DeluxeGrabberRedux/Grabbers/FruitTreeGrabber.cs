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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace DeluxeGrabberRedux.Grabbers
{
    class FruitTreeGrabber : TerrainFeaturesMapGrabber
    {
        public FruitTreeGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
        }

        public override bool GrabFeature(Vector2 tile, TerrainFeature feature)
        {
            if (Config.fruitTrees && feature is FruitTree tree && IsHarvestableFruitTree(tree))
            {
                // impl @ StardewValley::FruitTree::shake
                var daysUntilMature = tree.daysUntilMature.Value;
                var isStruckByLightning = tree.struckByLightningCountdown.Value > 0;

                var fruitQuality = SObject.lowQuality;
                if (isStruckByLightning) fruitQuality = SObject.lowQuality;
                else if (daysUntilMature <= -336) fruitQuality = SObject.bestQuality;
                else if (daysUntilMature <= -224) fruitQuality = SObject.highQuality;
                else if (daysUntilMature <= -112) fruitQuality = SObject.medQuality;

                var crop = new SObject(isStruckByLightning ? ItemIds.Coal : tree.indexOfFruit.Value, tree.fruitsOnTree.Value, false, -1, fruitQuality);
                if (TryAddItem(crop))
                {
                    tree.fruitsOnTree.Value = 0;
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

        private bool IsHarvestableFruitTree(FruitTree tree)
        {
            return !tree.stump.Value && tree.growthStage.Value >= 4 && tree.fruitsOnTree.Value > 0;
        }
    }
}
