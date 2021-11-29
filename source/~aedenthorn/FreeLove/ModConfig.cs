/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/


namespace FreeLove
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool BuyPendantsAnytime { get; set; } = false;
        public int MinPointsToMarry { get; set; } = 2500;
        public int MinPointsToDate { get; set; } = 2000;
        public int MaxGiftsPerSpousePerDay { get; set; } = 1;
        public int MaxGiftsPerSpousePerWeek { get; set; } = 2;
        public bool PreventHostileDivorces { get; set; } = true;
        public bool ComplexDivorce { get; set; } = true;
        public bool RoommateRomance { get; set; } = true;
        public bool RomanceAllVillagers { get; set; } = false;
        public bool ShowParentNames { get; set; } = false;
        public string SpouseSleepOrder { get; set; } = "";
        public int PercentChanceForSpouseInBed { get; set; } = 25;
        public int PercentChanceForSpouseInKitchen { get; set; } = 25;
        public int PercentChanceForSpouseAtPatio { get; set; } = 25;

        //public bool RemoveSpouseOrdinaryDialogue { get; set; } = false;
    }
}
