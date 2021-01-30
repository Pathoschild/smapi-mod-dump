/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ForageFantasy
{
    using StardewValley;
    using StardewValley.TerrainFeatures;
    using StardewObject = StardewValley.Object;

    internal class BerryBushLogic
    {
        public static bool IsHarvestableBush(Bush bush)
        {
            return bush != null && !bush.townBush && bush.inBloom(Game1.GetSeasonForLocation(bush.currentLocation), Game1.dayOfMonth) && bush.size == 1;
        }

        public static void RewardBerryXP(ForageFantasy mod)
        {
            double chance = mod.Config.BerryBushChanceToGetXP / 100.0;

            if (mod.Config.BerryBushXPAmount > 0 && Game1.random.NextDouble() < chance)
            {
                Game1.player.gainExperience(2, mod.Config.BerryBushXPAmount);
            }
        }

        public static void ChangeBerryQualityAndGiveExp(Bush bush, ForageFantasy mod)
        {
            int shakeOff;

            if (Game1.currentSeason == "spring")
            {
                shakeOff = 296;
            }
            else if (Game1.currentSeason == "fall")
            {
                shakeOff = 410;
            }
            else
            {
                return;
            }

            bool gaveExp = false;

            foreach (var item in bush.currentLocation.debris)
            {
                if (item != null && item.item != null && item.item.ParentSheetIndex == shakeOff)
                {
                    if (!gaveExp)
                    {
                        gaveExp = true;
                        RewardBerryXP(mod);
                    }

                    if (mod.Config.BerryBushQuality)
                    {
                        int quality = ForageFantasy.DetermineForageQuality(Game1.player);

                        ((StardewObject)item.item).Quality = quality;
                    }
                }
            }
        }
    }
}