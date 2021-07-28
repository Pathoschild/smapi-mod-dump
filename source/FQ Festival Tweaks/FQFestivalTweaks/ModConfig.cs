/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FletcherGoss/FQTweaks
**
*************************************************/

namespace FQFestivalTweaks
{
    public class ModConfig
    {
        public int MinMinutesAtFestival { get; set; }
        public int MaxEndTime { get; set; }

        public ModConfig()
        {
            this.MinMinutesAtFestival = 0;
            this.MaxEndTime = 2350;
        }
    }
}