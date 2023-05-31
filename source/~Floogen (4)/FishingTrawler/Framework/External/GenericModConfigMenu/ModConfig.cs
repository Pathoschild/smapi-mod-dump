/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

namespace FishingTrawler.Framework.External.GenericModConfigMenu
{
    public class ModConfig
    {
        public int minimumFishingLevel = 3;
        public bool disableScreenFade = false;
        public bool useOldTrawlerSprite = false;
        public float fishPerNet = 1f;
        public int engineFishBonus = 1;
        public int hullEventFrequencyLower = 1;
        public int hullEventFrequencyUpper = 5;
        public int netEventFrequencyLower = 3;
        public int netEventFrequencyUpper = 8;
        public string dayOfWeekChoice = "Wednesday";
        public string dayOfWeekChoiceIsland = "Saturday";
        internal static string[] murphyDayToAppear = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    }
}
