/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

namespace LogSpamFilter
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public float SizeIncreasePerDay { get; set; } = 0.1f;
        public int MaxDaysSizeIncrease { get; set; } = 100;
        public float LootIncreasePerDay { get; set; } = 0.1f;

    }
}
