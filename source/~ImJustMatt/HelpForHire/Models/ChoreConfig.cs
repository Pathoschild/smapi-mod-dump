/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace HelpForHire.Models;

internal record ChoreConfig
{
    public ChoreConfig(int dailyRate, int unitRate = 0)
    {
        this.DailyRate = dailyRate;
        this.UnitRate = unitRate;
    }

    public bool Enabled { get; set; } = true;

    public int DailyRate { get; set; }

    public int UnitRate { get; set; }
}