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

namespace StardewDruid.Cast.Water
{
    internal class Stump : CastHandle
    {

        private ResourceClump resourceClump;

        private string resourceType;

        public Stump(Vector2 target, Rite rite, ResourceClump ResourceClump, string ResourceType)
            : base(target, rite)
        {

            resourceClump = ResourceClump;

            resourceType = ResourceType;

        }

        public override void CastEffect()
        {

            int axeLevel = Mod.instance.virtualAxe.UpgradeLevel;

            castCost = Math.Max(2, 36 - targetPlayer.ForagingLevel * axeLevel);

            if (resourceClump == null)
            {

                return;

            }

            resourceClump.health.Set(1f);

            if (axeLevel < 3)
            {

                StardewValley.Tools.Axe betterAxe = new();

                betterAxe.UpgradeLevel = 3;

                betterAxe.DoFunction(targetLocation, 0, 0, 1, targetPlayer);

                resourceClump.performToolAction(betterAxe, 1, targetVector, targetLocation);

            }
            else
            {

                targetPlayer.Stamina += Math.Min(2, targetPlayer.MaxStamina - targetPlayer.Stamina);

                Mod.instance.virtualAxe.DoFunction(targetLocation, 0, 0, 1, targetPlayer);

                resourceClump.performToolAction(Mod.instance.virtualAxe, 1, targetVector, targetLocation);

            }

            resourceClump.NeedsUpdate = false;

            if (axeLevel >= 3)
            {
                Game1.createObjectDebris(709, (int)targetVector.X, (int)targetVector.Y);

                Game1.createObjectDebris(709, (int)targetVector.X + 1, (int)targetVector.Y);

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
