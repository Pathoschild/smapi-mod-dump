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
        internal const string springBerries = "(O)296";
        internal const string fallBerries = "(O)410";

        public static bool IsHarvestableBush(Bush bush)
        {
            return bush != null && !bush.townBush.Value && bush.inBloom() && bush.size.Value != Bush.greenTeaBush && bush.size.Value != Bush.walnutBush;
        }

        public static void RewardBerryXP(ForageFantasyConfig config, Farmer who)
        {
            double chance = config.BerryBushChanceToGetXP / 100.0;

            if (config.BerryBushXPAmount > 0 && Game1.random.NextDouble() < chance)
            {
                who.gainExperience(Farmer.foragingSkill, config.BerryBushXPAmount);
            }
        }

        public static void ChangeBerryQualityAndGiveExp(Bush bush, ForageFantasyConfig config)
        {
            string shakeOff;

            var season = bush.Location.GetSeason();

            switch (season)
            {
                case Season.Spring:
                    shakeOff = springBerries;
                    break;

                case Season.Fall:
                    shakeOff = fallBerries;
                    break;

                default:
                    return;
            }

            bool gaveExp = false;

            // change quality of every nearby matching berry debris, and give XP if we find at least one
            foreach (var item in bush.Location.debris)
            {
                if (item?.item?.QualifiedItemId == shakeOff)
                {
                    if (!gaveExp)
                    {
                        gaveExp = true;
                        RewardBerryXP(config, Game1.player);
                    }

                    if (config.BerryBushQuality)
                    {
                        ((StardewObject)item.item).Quality = ForageFantasy.DetermineForageQuality(Game1.player);
                    }
                }
            }
        }
    }
}