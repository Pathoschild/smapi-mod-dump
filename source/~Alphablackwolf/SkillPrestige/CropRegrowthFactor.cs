/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using StardewValley;

namespace SkillPrestige
{
    /// <summary>
    /// Provides an access point to determine crop 'regrowth' adjustments (seeds given at harvest time).
    /// </summary>
    public static class CropRegrowthFactor
    {
        public static decimal RegrowthChance { get; set; }
        public static decimal DeadRegrowthChance { private get; set; }

        public static bool GetCropRegrowthSuccess()
        {
            var randomizedValue = Game1.random.Next(1, 10);
            return RegrowthChance * 10 >= randomizedValue;
        }

        public static bool GetDeadCropRegrowthSuccess()
        {
            var randomizedValue = Game1.random.Next(1, 100);
            return DeadRegrowthChance * 100 >= randomizedValue;
        }

    }
}