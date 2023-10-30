/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Force.DeepCloner;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Minigames;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace StardewDruid.Cast
{
    internal class Artifact : CastHandle
    {

        public Artifact(Mod mod, Vector2 target, Rite rite)
            : base(mod, target, rite)
        {

        }

        public override void CastWater()
        {
            if (!targetLocation.objects.ContainsKey(targetVector))
            {
                return;

            }

            StardewValley.Object artifactSpot = targetLocation.objects[targetVector];

            if(artifactSpot == null)
            {
                return;

            }

            //StardewValley.Tools.Hoe newHoe = new();

            //newHoe.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);

            //if (newHoe == null)
            //{
            //    return;

            //}

            //artifactSpot.performToolAction(newHoe, targetLocation);

            mod.virtualHoe.DoFunction(targetLocation, 0, 0, 1, targetPlayer);

            targetPlayer.Stamina += Math.Min(2, targetPlayer.MaxStamina - targetPlayer.Stamina);

            artifactSpot.performToolAction(mod.virtualHoe, targetLocation);

            castFire = true;

            castCost = 8;

            ModUtility.AnimateBolt(targetLocation, targetVector);

            return;

        }

    }
}
