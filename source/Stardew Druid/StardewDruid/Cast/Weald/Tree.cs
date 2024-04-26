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
using StardewDruid.Data;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Cast.Weald
{
    internal class Tree : CastHandle
    {

        public Tree(Vector2 target)
            : base(target)
        {

            if (Game1.player.ForagingLevel >= 8)
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

            if (!treeFeature.stump.Value && Mod.instance.rite.spawnIndex["grass"] && !Mod.instance.EffectDisabled("Grass")) // 2/10 grass
            {

                List<Vector2> surrounding = ModUtility.GetTilesWithinRadius(targetLocation, targetVector, 1);

                for(int i = 0; i < surrounding.Count; i++)
                {

                    if(i % 2 == 1) { continue; }

                    if(randomIndex.Next(2) != 0)
                    {
                        continue;
                    }

                    if(ModUtility.NeighbourCheck(targetLocation, surrounding[i],0,0).Count > 0)
                    {
                        continue;
                    }

                    StardewValley.TerrainFeatures.Grass grassFeature = new(1, 4);

                    //targetLocation.terrainFeatures.Add(surrounding[i], grassFeature);

                    Microsoft.Xna.Framework.Rectangle tileRectangle = new((int)surrounding[i].X * 64 + 1, (int)surrounding[i].Y * 64 + 1, 62, 62);

                    grassFeature.doCollisionAction(tileRectangle, 2, surrounding[i], Game1.player);

                }

            }

            int debrisType = 388;

            int debrisMax = 3;

            if (targetPlayer.professions.Contains(12))
            {

                debrisMax++;

            }

            if (treeFeature.treeType.Value == "8") //mahogany
            {

                debrisType = 709; debrisMax = 1;

                if (targetPlayer.professions.Contains(14))
                {

                    debrisMax++;

                }

            }

            if (treeFeature.treeType.Value == "7") // mushroom
            {

                debrisType = 420; debrisMax = 1;

            }

            Dictionary<int, Throw> throwList = new();

            for (int i = 0; i < randomIndex.Next(1, Math.Min(debrisMax, (Mod.instance.PowerLevel))); i++)
            {

                throwList[i] = new(targetPlayer, targetVector * 64, debrisType, 0);

                throwList[i].ThrowObject();

            }

            if (!treeFeature.stump.Value)
            {

                treeFeature.performUseAction(targetVector);

            }

            castFire = true;

            targetPlayer.gainExperience(2, 2); // gain foraging experience


            Vector2 cursorVector = targetVector * 64 + new Vector2(0, 8);
            Mod.instance.iconData.CursorIndicator(targetLocation, cursorVector, IconData.cursors.weald);
        }

    }

}
