/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

namespace TwilightShards.LunarDisturbances
{
    public class MoonConfig
    {
        //required options
        public double BadMoonRising = .001;
        public bool EclipseOn = true;
        public double EclipseChance = .015;
        public double CropGrowthChance = .015;
        public double CropHaltChance = .015;
        public double BeachRemovalChance = .09;
        public double BeachSpawnChance = .35;
        public double GhostSpawnChance = .02;
        public double SuperMoonChances = .003;
        public double HarvestMoonDoubleGrowChance = .045;
        public bool UseMoreMonthlyCycle = true;
        public bool ShowMoonPhase = true;
        public bool SpawnMonsters = true;
        public bool HazardousMoonEvents = false;
        public bool Verbose = false;
    }
}
