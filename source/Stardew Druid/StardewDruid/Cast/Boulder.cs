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
    internal class Boulder : CastHandle
    {

        private readonly ResourceClump resourceClump;

        public Boulder(Mod mod, Vector2 target, Rite rite, ResourceClump ResourceClump)
            : base(mod, target, rite)
        {

            resourceClump = ResourceClump;

        }

        public override void CastEarth()
        {

            int debrisType = 390;

            int debrisAmount = randomIndex.Next(1, 5);

            Dictionary<int, Throw> throwList = new();

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

            ModUtility.AnimateGrowth(targetLocation, targetVector);

        }

        public override void CastWater()
        {

            int pickLevel = mod.virtualPick.UpgradeLevel;

            castCost = Math.Max(2, 36 - (targetPlayer.MiningLevel * pickLevel));

            resourceClump.health.Set(1f);

            if (pickLevel < 3)
            {

                StardewValley.Tools.Pickaxe betterPick = new();

                betterPick.UpgradeLevel = 3;

                targetPlayer.Stamina += Math.Min(2, targetPlayer.MaxStamina - targetPlayer.Stamina);

                betterPick.DoFunction(targetLocation, 0, 0, 1, targetPlayer);

                resourceClump.performToolAction(betterPick, 1, targetVector, targetLocation);

            }
            else
            {

                targetPlayer.Stamina += Math.Min(2, targetPlayer.MaxStamina - targetPlayer.Stamina);

                mod.virtualPick.DoFunction(targetLocation, 0, 0, 1, targetPlayer);

                resourceClump.performToolAction(mod.virtualPick, 1, targetVector, targetLocation);

            }

            resourceClump.NeedsUpdate = false;

            targetLocation._activeTerrainFeatures.Remove(resourceClump);

            targetLocation.resourceClumps.Remove(resourceClump);

            resourceClump.currentLocation = null;

            if (pickLevel >= 3)
            {
                Game1.createObjectDebris(709, (int)this.targetVector.X, (int)this.targetVector.Y);

                Game1.createObjectDebris(709, (int)this.targetVector.X + 1, (int)this.targetVector.Y);

            }

            int debrisMax = 1;

            if (targetPlayer.professions.Contains(22))
            {
                debrisMax = 2;

            }

            for (int i = 0; i < randomIndex.Next(1,debrisMax); i++)
            {

                switch ((int)resourceClump.parentSheetIndex.Value)
                {

                    case 756:
                    case 758:

                        Game1.createObjectDebris(536, (int)targetVector.X, (int)targetVector.Y);

                        break;

                    default:

                        if (targetLocation is MineShaft)
                        {
                            MineShaft mineLocation = (MineShaft)targetLocation;

                            if (mineLocation.mineLevel >= 80)
                            {
                                Game1.createObjectDebris(537, (int)targetVector.X, (int)targetVector.Y);

                                break;

                            }
                            else if (mineLocation.mineLevel >= 121)
                            {
                                Game1.createObjectDebris(749, (int)targetVector.X, (int)targetVector.Y);

                                break;

                            }

                        }

                        Game1.createObjectDebris(535, (int)targetVector.X, (int)targetVector.Y);

                        break;

                }

            }

            castFire = true;

            castCost = 24;

            ModUtility.AnimateBolt(targetLocation, targetVector);

            return;

        }

    }
}
