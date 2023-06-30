/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

namespace PrismaticStatue.Utility;

internal static class Formatter
{
    internal static string FormatNStatues(int n_statues)
    {
        switch (n_statues)
        {
            case 0:
                return "No Statues";
            case 1:
                return "1 Statue";
            default:
                return $"{n_statues} Statues";
        }
    }

    internal static string FormatMinutes(int minutes)
    {
        int days = (minutes >= 1600) ? minutes / 1600 : 0;
        minutes %= 1600;
        int hours = (minutes >= 60) ? minutes / 60 : 0;
        minutes %= 60;

        string time = "";

        if (days > 0)
        {
            time += $"{days}d" + ((minutes > 0 || hours > 0) ? " " : "");
        }

        if (hours > 0)
        {
            time += $"{hours}h" + ((minutes > 0) ? " " : "");
        }

        if (minutes > 0 || (days == 0 && hours == 0))
        {
            time += $"{minutes}m";
        }
        return time;
    }
}
