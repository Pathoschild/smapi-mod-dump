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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace StardewDruid.Cast
{
    internal class Stump : CastHandle
    {

        private ResourceClump resourceClump;

        private string resourceType;

        public Stump(Mod mod, Vector2 target, Rite rite, ResourceClump ResourceClump, string ResourceType)
            : base(mod, target, rite)
        {

            resourceClump = ResourceClump;

            resourceType = ResourceType;

        }

        public override void CastEarth()
        {

            int debrisType = 388;

            int debrisAmount = randomIndex.Next(1, 5);

            Dictionary<int,Throw> throwList = new();

            for (int i = 0; i < debrisAmount; i++)
            {

                throwList[i] = new(debrisType, 0);

                throwList[i].ThrowObject(targetPlayer, targetVector);

                //Game1.createObjectDebris(debrisType, (int)targetVector.X, (int)targetVector.Y + 1);

            }

            if (debrisAmount == 1)
            {

                throwList[1] = new(382, 0);

                throwList[1].ThrowObject(targetPlayer, targetVector);

                //Game1.createObjectDebris(382, (int)targetVector.X + 1, (int)targetVector.Y);

            }

            castFire = true;

            targetPlayer.gainExperience(2, 2); // gain foraging experience

            ModUtility.AnimateGrowth(targetLocation,targetVector);

        }

        public override void CastWater()
        {

            int axeLevel = mod.virtualAxe.UpgradeLevel;

            castCost = Math.Max(2, 36 - (targetPlayer.ForagingLevel * axeLevel));

            if (resourceClump == null)
            {

                return;

            }

            resourceClump.health.Set(1f);

            if(axeLevel < 3)
            {

                StardewValley.Tools.Axe betterAxe = new();

                betterAxe.UpgradeLevel = 3;

                betterAxe.DoFunction(targetLocation, 0, 0, 1, targetPlayer);

                resourceClump.performToolAction(betterAxe, 1, targetVector, targetLocation);

            }
            else
            {
                
                targetPlayer.Stamina += Math.Min(2, targetPlayer.MaxStamina - targetPlayer.Stamina);

                mod.virtualAxe.DoFunction(targetLocation, 0, 0, 1, targetPlayer);

                resourceClump.performToolAction(mod.virtualAxe, 1, targetVector, targetLocation);

            }

            resourceClump.NeedsUpdate = false;

            if (axeLevel >= 3)
            {
                Game1.createObjectDebris(709, (int)this.targetVector.X, (int)this.targetVector.Y);

                Game1.createObjectDebris(709, (int)this.targetVector.X + 1, (int)this.targetVector.Y);

            }

            switch (resourceType)
            {

                case "Woods":

                    Woods woodsLocation = riteData.castLocation as Woods;

                    if (woodsLocation.stumps.Contains(resourceClump))
                    {

                        woodsLocation.stumps.Remove(resourceClump);

                    }

                    break;

                case "Log":

                    Forest forestLocation = riteData.castLocation as Forest;

                    forestLocation.log = null;

                    break;

                default: // Farm

                    if (targetLocation._activeTerrainFeatures.Contains(resourceClump))
                    {

                        targetLocation._activeTerrainFeatures.Remove(resourceClump);

                    }

                    if (targetLocation.resourceClumps.Contains(resourceClump))
                    {

                        targetLocation.resourceClumps.Remove(resourceClump);

                    }

                    break;

            }

            resourceClump = null;

            castFire = true;

            ModUtility.AnimateBolt(targetLocation, targetVector);

            return;

        }

    }
}
