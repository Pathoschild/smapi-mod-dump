/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/phrasefable/StardewMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;

namespace AggressiveAcorns
{
    public interface IModConfig
    {
        bool PreventScythe { get; }

        bool SeedsReplaceGrass { get; }

        int MaxShadedGrowthStage { get; }

        int MaxPassibleGrowthStage { get; }

        double DailyGrowthChance { get; }

        bool DoGrowInWinter { get; }

        double DailySpreadChance { get; }

        bool DoTappedSpread { get; }

        bool DoSpreadInWinter { get; }

        bool DoGrowInstantly { get; }

        bool DoSeedsPersist { get; }

        double DailySeedChance { get; }

        bool DoMushroomTreesHibernate { get; }

        bool DoMushroomTreesRegrow { get; }
    }


    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class ModConfig : IModConfig
    {
        public bool PreventScythe { get; set; } = false;

        public bool SeedsReplaceGrass { get; set; } = false;

        public int MaxShadedGrowthStage { get; set; } = 4;

        public int MaxPassibleGrowthStage { get; set; } = 0;

        public double DailyGrowthChance { get; set; } = 0.20;

        public bool DoGrowInWinter { get; set; } = false;

        public double DailySpreadChance { get; set; } = 0.15;

        public bool DoTappedSpread { get; set; } = true;

        public bool DoSpreadInWinter { get; set; } = true;

        public bool DoGrowInstantly { get; set; } = false;

        public bool DoSeedsPersist { get; set; } = false;

        public double DailySeedChance { get; set; } = 0.05;

        public bool DoMushroomTreesHibernate { get; set; } = true;

        public bool DoMushroomTreesRegrow { get; set; } = false;
    }
}