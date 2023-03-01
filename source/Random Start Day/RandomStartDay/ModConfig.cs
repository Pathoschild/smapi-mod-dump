/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/RandomStartDay
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Threading;

class ModConfig
{
    public bool disableAll { get; set; }

    public bool isRandomSeedUsed { get; set; }

    // reset other randomize options only when isRandomSeedUsed is TRUE
    public string[] allowedSeasons { get; set; }
    public bool avoidFestivalDay { get; set; }
    public bool alwaysStartAt1st { get; set; }

    public bool useSeasonalSeeds { get; set; }
    public bool useWinter28toYear1 { get; set; }

    public ModConfig()
    {
        this.disableAll = false;
        this.isRandomSeedUsed = true;
        this.allowedSeasons = new string[] { "spring", "summer", "fall", "winter" };
        this.avoidFestivalDay = false;
        this.alwaysStartAt1st = false;

        this.useSeasonalSeeds = true;
        this.useWinter28toYear1 = true;
    }
}