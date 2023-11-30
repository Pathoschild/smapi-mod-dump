/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using BirbCore.Attributes;

namespace RealtimeFramework;

[SAsset]
class Assets
{
    [SAsset.Asset("assets/holidays.json")]
    public Dictionary<string, HolidayModel> Holidays { get; set; }
}

class HolidayModel
{
    public int ComingDays { get; set; } = 7;
    public int PassingDays { get; set; } = 1;

    public int StartDelayHours { get; set; } = 0;

    public int EndDelayHours { get; set; } = 0;

    public int[] Date { get; set; } = { 1, 1 };
    public Dictionary<string, int[]> VaryingDates { get; set; } = null;
}
