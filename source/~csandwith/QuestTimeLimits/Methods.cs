/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using System;

namespace QuestTimeLimits
{
    public partial class ModEntry
    {
        private static int MultiplyQuestDays(int days)
        {
            if(!Config.ModEnabled || Config.DailyQuestMult <= 0)
                return days;
            return (int)Math.Round(days * Config.DailyQuestMult);
        }

    }
}