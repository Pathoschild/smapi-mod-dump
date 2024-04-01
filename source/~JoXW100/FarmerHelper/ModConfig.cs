/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace FarmerHelper
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public string IgnoreHarvestCrops { get; set; } = "";
        public bool IgnoreFlowers { get; set; } = true;
        public bool LabelLatePlanting { get; set; } = true;
        public bool PreventLatePlant { get; set; } = true;
        public bool WarnAboutPlantsUnwateredBeforeSleep { get; set; } = true;
        public bool WarnAboutPlantsUnharvestedBeforeSleep { get; set; } = true;
        public bool WarnAboutAnimalsOutsideBeforeSleep { get; set; } = true;
        public bool WarnAboutAnimalsHungryBeforeSleep { get; set; } = true;
        public bool WarnAboutAnimalsUnharvestedBeforeSleep { get; set; } = true;
        public bool WarnAboutAnimalsNotPetBeforeSleep { get; set; } = true;
        public int DaysPerMonth { get; set; } = 28;

    }
}
