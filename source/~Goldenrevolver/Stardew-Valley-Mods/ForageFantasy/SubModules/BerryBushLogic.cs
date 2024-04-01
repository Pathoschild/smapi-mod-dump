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
    using System;
    using StardewObject = StardewValley.Object;

    internal class BerryBushLogic
    {
        internal const string springBerries = "(O)296";
        internal const string fallBerries = "(O)410";

        public static bool IsHarvestableBush(Bush bush)
        {
            return bush != null && !bush.townBush.Value && bush.inBloom() && bush.size.Value != Bush.greenTeaBush && bush.size.Value != Bush.walnutBush;
        }

        public static void ChangeBerryQualityAndGiveExp(Bush bush, ForageFantasyConfig config)
        {
            if (!config.BerryBushQuality)
            {
                return;
            }

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

            // change quality of every nearby matching berry debris
            Random r = Utility.CreateDaySaveRandom(bush.Tile.X, bush.Tile.Y * 777f);
            foreach (var item in bush.Location.debris)
            {
                if (item?.item?.QualifiedItemId == shakeOff && item.timeSinceDoneBouncing == 0f)
                {
                    ((StardewObject)item.item).Quality = ForageFantasy.DetermineForageQuality(Game1.player, r);
                }
            }
        }
    }
}