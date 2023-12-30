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
using System;
using System.Collections.Generic;

namespace StardewDruid.Cast.Mists
{
    internal class Tree : CastHandle
    {

        public Tree(Vector2 target, Rite rite)
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

            Dictionary<int, int> resinIndex = new()
            {
                [1] = 725, // Oak
                [2] = 724, // Maple
                [3] = 726, // Pine
                [6] = 247, // Palm
                [7] = 422, // Purple Mushroom // Mushroom
                [8] = 419, // Vinegar // Mahogany
                [9] = 247, // Palm
            };

            treeFeature.health.Value = 1;

            targetPlayer.Stamina += Math.Min(2, targetPlayer.MaxStamina - targetPlayer.Stamina);

            Mod.instance.virtualAxe.DoFunction(targetLocation, 0, 0, 1, targetPlayer);

            treeFeature.performToolAction(Mod.instance.virtualAxe, 0, targetVector, targetLocation);

            if (randomIndex.Next(4) == 0 && resinIndex.ContainsKey(treeFeature.treeType.Value))
            {
                Throw throwObject = new(targetPlayer, targetVector * 64, resinIndex[treeFeature.treeType.Value], 0);

                throwObject.ThrowObject();

            }

            targetLocation.terrainFeatures.Remove(targetVector);

        }

    }

}
