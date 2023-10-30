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
using StardewValley.Monsters;

namespace StardewDruid.Cast
{
    internal class Tree : CastHandle
    {

        public Tree(Mod mod, Vector2 target, Rite rite)
            : base(mod, target, rite)
        {

            if(rite.caster.ForagingLevel >= 8)
            {

                castCost = 1;

            }

        }

        public override void CastEarth()
        {

            if (!targetLocation.terrainFeatures.ContainsKey(targetVector))
            {

                return;

            }

            if (targetLocation.terrainFeatures[targetVector] is not StardewValley.TerrainFeatures.Tree treeFeature)
            {

                return;

            }

            if (randomIndex.Next(30) == 0 && riteData.spawnIndex["critter"] && !riteData.castToggle.ContainsKey("forgetCritters"))
            {

                int spawnMonster = mod.SpawnMonster(targetLocation, targetVector, new() { 99, }, "tree");

                if (!riteData.castTask.ContainsKey("masterCreature") && spawnMonster >= 1)
                {

                    mod.UpdateTask("lessonCreature", 1);

                }

            }

            //StardewValley.TerrainFeatures.Tree treeFeature = targetLocation.terrainFeatures[targetVector] as StardewValley.TerrainFeatures.Tree;

            int debrisType = 388;

            int debrisAxe = mod.virtualAxe.UpgradeLevel + 1;

            int debrisMax = 3;

            if(targetPlayer.professions.Contains(12))
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

                throwList[i] = new(debrisType, 0);

                throwList[i].ThrowObject(targetPlayer, targetVector);

                //Game1.createObjectDebris(, (int)targetVector.X, (int)targetVector.Y + 1);

            }

            if (!treeFeature.stump.Value)
            {

                treeFeature.performUseAction(targetVector,targetLocation);

            }

            castFire = true;

            targetPlayer.gainExperience(2,2); // gain foraging experience


        }

        public override void CastWater()
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

            mod.virtualAxe.DoFunction(targetLocation, 0, 0, 1, targetPlayer);

            treeFeature.performToolAction(mod.virtualAxe,0,targetVector, targetLocation);

            if (randomIndex.Next(4) == 0 && resinIndex.ContainsKey(treeFeature.treeType.Value))
            {
                StardewDruid.Cast.Throw throwObject = new(resinIndex[treeFeature.treeType.Value],0);

                throwObject.ThrowObject(targetPlayer, targetVector);

            }

            targetLocation.terrainFeatures.Remove(targetVector);

        }

    }

}
