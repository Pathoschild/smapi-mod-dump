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

namespace StardewDruid.Cast.Weald
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

            if (randomIndex.Next(30) == 0 && riteData.spawnIndex["wildspawn"] && !Mod.instance.EffectDisabled("Wildspawn"))
            {

                StardewValley.Monsters.Monster spawnMonster = Mod.instance.SpawnMonster(targetLocation, targetVector, new() { 99, }, "tree");

            }

            if (riteData.spawnIndex["grass"] && !Mod.instance.EffectDisabled("Trees")) // 2/10 grass
            {

                Dictionary<string, List<Vector2>> neighbourList = ModUtility.NeighbourCheck(targetLocation, targetVector);

                if (neighbourList.Count == 0)
                {

                    List<Vector2> grassVectors = new();

                    if (neighbourList.ContainsKey("Dirt"))
                    {

                        foreach (Vector2 neighbour in neighbourList["Dirt"])
                        {
                            grassVectors.Add(neighbour);

                        }

                    }

                    if (neighbourList.ContainsKey("Grass"))
                    {

                        foreach (Vector2 neighbour in neighbourList["Grass"])
                        {
                            grassVectors.Add(neighbour);

                        }

                    }

                    foreach (Vector2 grassVector in grassVectors)
                    {

                        StardewValley.TerrainFeatures.Grass grassFeature = new(1, 4);

                        targetLocation.terrainFeatures.Add(grassVector, grassFeature);

                        Microsoft.Xna.Framework.Rectangle tileRectangle = new((int)grassVector.X * 64 + 1, (int)grassVector.Y * 64 + 1, 62, 62);

                        grassFeature.doCollisionAction(tileRectangle, 2, grassVector, null, targetLocation);

                    }

                }

            }

            int debrisType = 388;

            int debrisAxe = Mod.instance.virtualAxe.UpgradeLevel + 1;

            int debrisMax = 3;

            if (targetPlayer.professions.Contains(12))
            {

                debrisMax++;

            }

            if (treeFeature.treeType.Value == 8) //mahogany
            {

                debrisType = 709; debrisMax = 1;

                if (targetPlayer.professions.Contains(14))
                {

                    debrisMax++;

                }

            }

            if (treeFeature.treeType.Value == 7) // mushroom
            {

                debrisType = 420; debrisMax = 1;

            }

            Dictionary<int, Throw> throwList = new();

            for (int i = 0; i < randomIndex.Next(1, Math.Min(debrisMax, debrisAxe)); i++)
            {

                throwList[i] = new(targetPlayer, targetVector * 64, debrisType, 0);

                throwList[i].ThrowObject();

            }

            if (!treeFeature.stump.Value)
            {

                treeFeature.performUseAction(targetVector, targetLocation);

            }

            castFire = true;

            targetPlayer.gainExperience(2, 2); // gain foraging experience


        }

    }

}
