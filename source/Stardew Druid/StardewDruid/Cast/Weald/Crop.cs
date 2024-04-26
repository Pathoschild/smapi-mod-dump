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
using StardewDruid.Journal;
using StardewValley;
using StardewValley.TerrainFeatures;
//using System.Numerics;

namespace StardewDruid.Cast.Weald
{
    internal class Crop : CastHandle
    {

        bool reseed;

        bool watered;

        public Crop(Vector2 target,  bool Reseed = false, bool Watered = false)
            : base(target)
        {

            castCost = 2;

            if (Game1.player.FarmingLevel >= 6)
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

                    hoeDirt.destroyCrop(true);

                    if (Game1.currentSeason == "winter" && !Game1.currentLocation.IsGreenhouse)
                    {

                        Mod.instance.targetCasts[targetLocation.Name][targetVector] = "Hoed";

                        return;

                    }

                    string wildSeed = "498";

                    switch (Game1.currentSeason)
                    {

                        case "spring":

                            wildSeed = "495";
                            break;

                        case "summer":

                            wildSeed = "496";
                            break;

                        case "fall":

                            wildSeed = "497";
                            break;

                    }

                    hoeDirt.plant(wildSeed, targetPlayer, false);

                }

            }

            if (!hoeDirt.HasFertilizer())
            {

                hoeDirt.plant("466", targetPlayer, true);

                castFire = true;

            }

            if(hoeDirt.crop == null)
            {

                Mod.instance.targetCasts[targetLocation.Name][targetVector] = "Hoed";

                return;

            }

            if (hoeDirt.crop.isWildSeedCrop() && hoeDirt.crop.currentPhase.Value <= 1 && (Game1.currentSeason != "winter" || targetLocation.isGreenhouse.Value))
            {

                bool enableQuality = Mod.instance.questHandle.IsComplete(QuestHandle.cropLesson) ? true : false;

                ModUtility.UpgradeCrop(hoeDirt, (int)targetVector.X, (int)targetVector.Y, targetPlayer, targetLocation, enableQuality);

                if (hoeDirt.crop == null)
                {

                    Mod.instance.targetCasts[targetLocation.Name][targetVector] = "Hoed";

                    return;

                }

                castFire = true;

                if (!Mod.instance.questHandle.IsComplete(QuestHandle.cropLesson))
                {

                    Mod.instance.questHandle.UpdateTask(QuestHandle.cropLesson, 1);

                }

            }

            if (hoeDirt.crop.currentPhase.Value <= 1)
            {

                hoeDirt.crop.currentPhase.Value = 2;

                hoeDirt.crop.dayOfCurrentPhase.Value = 0;

                hoeDirt.crop.updateDrawMath(targetVector);

                castFire = true;

            }

            if (watered)
            {

                hoeDirt.state.Value = 1;

            }

            Mod.instance.targetCasts[targetLocation.Name][targetVector] = "Crop";

        }

    }

}
