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
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace StardewDruid.Cast.Mists
{
    internal class Lava : CastHandle
    {

        public Lava(Vector2 target)
            : base(target)
        {

        }

        public override void CastEffect()
        {

            VolcanoDungeon volcanoLocation = targetLocation as VolcanoDungeon;

            int waterRadius = Math.Max(2, Mod.instance.virtualCan.UpgradeLevel / 2);

            for (int i = 0; i < waterRadius + 1; i++)
            {

                List<Vector2> radialVectors = ModUtility.GetTilesWithinRadius(targetLocation, targetVector, i);

                foreach (Vector2 radialVector in radialVectors)
                {
                    int tileX = (int)radialVector.X;
                    int tileY = (int)radialVector.Y;

                    if (volcanoLocation.waterTiles[tileX, tileY] && !volcanoLocation.cooledLavaTiles.ContainsKey(radialVector))
                    {

                        volcanoLocation.CoolLava(tileX, tileY);

                        volcanoLocation.UpdateLavaNeighbor(tileX, tileY);

                    }

                }

            }

            List<Vector2> fourthVectors = ModUtility.GetTilesWithinRadius(targetLocation, targetVector, waterRadius + 1);

            foreach (Vector2 fourthVector in fourthVectors)
            {
                int tileX = (int)fourthVector.X;
                int tileY = (int)fourthVector.Y;

                volcanoLocation.UpdateLavaNeighbor(tileX, tileY);

            }

            castFire = true;

            castCost = 0;

            ModUtility.AnimateBolt(targetLocation, targetVector * 64 + new Vector2(32));

            return;

        }

    }
}
