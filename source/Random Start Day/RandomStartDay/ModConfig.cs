/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/RandomStartDay
**
*************************************************/

class ModConfig
{
    public bool DisableAll { get; set; }


    // reset other randomize options only when isRandomSeedUsed is TRUE
    public bool[] AllowSpringSummerFallWinter { get; set; }

    public bool AlwaysStartAt1st { get; set; }
    public bool AvoidFestivalDay { get; set; }
    public bool AvoidPassiveFestivalDay { get; set; }

    public bool UseWheatSeeds { get; set; }
    public bool UseWinter28toYear1 { get; set; }
    public bool TVRecipeWithSeasonContext { get; set; }

    public ModConfig()
    {
        this.DisableAll = false;
        this.AllowSpringSummerFallWinter = new bool[] { true, true, true, true };
        this.AlwaysStartAt1st = false;
        this.AvoidFestivalDay = false;
        this.AvoidPassiveFestivalDay = false;
        this.UseWheatSeeds = true;
        this.UseWinter28toYear1 = true;
        this.TVRecipeWithSeasonContext = true;
    }
}