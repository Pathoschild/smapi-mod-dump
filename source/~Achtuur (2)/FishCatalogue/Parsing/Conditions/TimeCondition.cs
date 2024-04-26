/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Framework.Borders;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue.Parsing.Conditions;
internal class TimeCondition : BaseCondition
{
    private string WallClockItemID = "(F)WallClock";

    List<(int, int)> times;

    public TimeCondition(IEnumerable<(int, int)> times)
    {
        this.times = times.ToList();
    }

    public override string Description()
    {
        IEnumerable<string> time_strings = times.Select(time => $"{TimeToText(time.Item1)}-{TimeToText(time.Item2)}");
        return string.Join("\n", time_strings);
    }

    protected override string ItemID()
    {
        return this.WallClockItemID;
    }

    public override bool IsTrue()
    {
        return times.Any(time => Game1.timeOfDay >= time.Item1 && Game1.timeOfDay <= time.Item2);

    }

    private string TimeToText(int time)
    {
        int hour = time / 100;
        int minute = time % 100;
        bool isPM = hour >= 12;
        string AmPm = isPM ? "pm" : "am";

        bool isNight = hour >= 24;
        if (isNight)
        {
            hour -= 24;
            AmPm = "am";
        }

        if (minute == 0)
        {
            return $"{hour}:00{AmPm}";
        }

        return $"{hour}:{minute}{AmPm}";
    }
}
