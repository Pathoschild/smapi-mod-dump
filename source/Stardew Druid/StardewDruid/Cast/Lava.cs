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
    internal class Lava : CastHandle
    {

        public Lava(Mod mod, Vector2 target, Rite rite)
            : base(mod, target, rite)
        {

        }

        public override void CastWater()
        {

            VolcanoDungeon volcanoLocation = targetLocation as VolcanoDungeon;

            int waterRadius = Math.Max(2, (int)(mod.virtualCan.UpgradeLevel / 2));

            for(int i = 0; i < waterRadius + 1; i++)
            {

                List<Vector2> radialVectors = ModUtility.GetTilesWithinRadius(targetLocation, targetVector, i);

                foreach(Vector2 radialVector in radialVectors)
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

            ModUtility.AnimateBolt(targetLocation, targetVector);

            return;

        }

    }
}
