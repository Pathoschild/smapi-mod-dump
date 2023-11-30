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

namespace StardewDruid.Cast.Water
{
    internal class Boulder : CastHandle
    {

        private readonly ResourceClump resourceClump;

        public Boulder(Vector2 target, Rite rite, ResourceClump ResourceClump)
            : base(target, rite)
        {

            resourceClump = ResourceClump;

        }

        public override void CastEffect()
        {

            int pickLevel = Mod.instance.virtualPick.UpgradeLevel;

            castCost = Math.Max(2, 36 - targetPlayer.MiningLevel * pickLevel);

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

                Mod.instance.virtualPick.DoFunction(targetLocation, 0, 0, 1, targetPlayer);

                resourceClump.performToolAction(Mod.instance.virtualPick, 1, targetVector, targetLocation);

            }

            resourceClump.NeedsUpdate = false;

            targetLocation._activeTerrainFeatures.Remove(resourceClump);

            targetLocation.resourceClumps.Remove(resourceClump);

            resourceClump.currentLocation = null;

            if (pickLevel >= 3)
            {
                Game1.createObjectDebris(709, (int)targetVector.X, (int)targetVector.Y);

                Game1.createObjectDebris(709, (int)targetVector.X + 1, (int)targetVector.Y);

            }

            int debrisMax = 1;

            if (targetPlayer.professions.Contains(22))
            {
                debrisMax = 2;

            }

            for (int i = 0; i < randomIndex.Next(1, debrisMax); i++)
            {

                switch (resourceClump.parentSheetIndex.Value)
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

        }

    }
}
