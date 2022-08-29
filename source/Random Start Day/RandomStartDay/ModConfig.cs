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
    public bool isRandomSeedUsed { get; set; }
    public string[] allowedSeasons { get; set;}
    public int MaxOfDayOfMonth { get; set;}

    public ModConfig()
    {
        this.isRandomSeedUsed = true;
        this.allowedSeasons = new string[] {"spring", "summer", "fall", "winter"};
        this.MaxOfDayOfMonth = 28;
    }
}