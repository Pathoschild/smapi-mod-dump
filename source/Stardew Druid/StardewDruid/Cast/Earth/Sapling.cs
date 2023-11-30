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

namespace StardewDruid.Cast.Earth
{
    internal class Sapling : CastHandle
    {

        public Sapling(Vector2 target, Rite rite)
            : base(target, rite)
        {

            if (rite.caster.ForagingLevel >= 8)
            {

                castCost = 1;

            }

        }

        public override void CastEffect()
        {

            if (!targetLocation.terrainFeatures.ContainsKey(targetVector))
            {

                return;

            }

            if (targetLocation.terrainFeatures[targetVector] is not StardewValley.TerrainFeatures.Tree treeFeature)
            {

                return;

            }

            treeFeature.dayUpdate(targetLocation, targetVector);

            Utility.addSprinklesToLocation(targetLocation, (int)targetVector.X, (int)targetVector.Y, 1, 1, 1000, 500, new Color(0.8f, 1, 0.8f, 1));

            castFire = true;

        }

    }

}
