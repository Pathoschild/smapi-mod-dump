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
using StardewValley;
using StardewValley.TerrainFeatures;
//using System.Numerics;

namespace StardewDruid.Cast.Weald
{
    internal class Crop : CastHandle
    {

        bool reseed;

        bool watered;

        public Crop(Vector2 target, Rite rite, bool Reseed = false, bool Watered = false)
            : base(target, rite)
        {

            castCost = 2;

            if (rite.caster.FarmingLevel >= 6)
            {

                castCost = 1;

            }

            reseed = Reseed;

            watered = Watered;

        }

        public override void CastEffect()
        {

            if (!targetLocation.terrainFeatures.ContainsKey(targetVector))
            {

                return;

            }

            if (targetLocation.terrainFeatures[targetVector] is not HoeDirt hoeDirt)
            {

                return;

            }

            if (hoeDirt.crop != null)
            {

                if (hoeDirt.crop.dead.Value)
                {

                    hoeDirt.destroyCrop(targetVector, true, targetLocation);

                }

            }

            if (hoeDirt.crop == null)
            {

                if (!reseed)
                {
                    return;

                }

                int wildSeed = 498;

                switch (Game1.currentSeason)
                {

                    case "spring":

                        wildSeed = 495;
                        break;

                    case "summer":

                        wildSeed = 496;
                        break;

                    case "fall":

                        wildSeed = 497;
                        break;

                }

                hoeDirt.plant(wildSeed, (int)targetVector.X, (int)targetVector.Y, targetPlayer, false, targetLocation);

            }

            if (hoeDirt.fertilizer.Value == 0)
            {

                hoeDirt.plant(466, (int)targetVector.X, (int)targetVector.Y, targetPlayer, true, targetLocation);

                castFire = true;

            }

            StardewValley.Crop targetCrop = hoeDirt.crop;

            if (targetCrop.isWildSeedCrop() && targetCrop.currentPhase.Value <= 1 && (Game1.currentSeason != "winter" || targetLocation.isGreenhouse.Value))
            {

                bool enableQuality = riteData.castTask.ContainsKey("masterCrop") ? true : false;

                ModUtility.UpgradeCrop(hoeDirt, (int)targetVector.X, (int)targetVector.Y, targetPlayer, targetLocation, enableQuality);

                if (hoeDirt.crop == null)
                {

                    return;

                }

                targetCrop = hoeDirt.crop;

                castFire = true;

                if (!riteData.castTask.ContainsKey("masterCrop"))
                {

                    Mod.instance.UpdateTask("lessonCrop", 1);

                }

            }

            if (targetCrop.currentPhase.Value <= 1)
            {

                targetCrop.currentPhase.Value = 2;

                hoeDirt.crop.dayOfCurrentPhase.Value = 0;

                hoeDirt.crop.updateDrawMath(targetVector);

                castFire = true;

            }

            if (watered)
            {

                hoeDirt.state.Value = 1;

            }

        }

    }

}
