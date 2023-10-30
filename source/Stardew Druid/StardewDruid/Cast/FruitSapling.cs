/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using System;

namespace StardewDruid.Cast
{
    internal class FruitSapling : CastHandle
    {

        public FruitSapling(Mod mod, Vector2 target, Rite rite)
            : base(mod, target, rite)
        {

            castCost = Math.Max(2,16 - rite.caster.FarmingLevel);

        }

        public override void CastEarth()
        {

            if (!targetLocation.terrainFeatures.ContainsKey(targetVector))
            {

                return;

            }

            if (targetLocation.terrainFeatures[targetVector] is not StardewValley.TerrainFeatures.FruitTree)
            {

                return;

            }

            StardewValley.TerrainFeatures.FruitTree treeFeature = targetLocation.terrainFeatures[targetVector] as StardewValley.TerrainFeatures.FruitTree;

            treeFeature.dayUpdate(targetLocation,targetVector);

            Utility.addSprinklesToLocation(targetPlayer.currentLocation, (int)targetVector.X, (int)targetVector.Y, 1, 2, 400, 40, Color.White);

            Game1.playSound("yoba");

            castFire = true;

        }

    }

}
