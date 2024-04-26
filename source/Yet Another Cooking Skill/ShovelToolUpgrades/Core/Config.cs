/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using MoonShared.Config;

namespace ShovelToolUpgrades
{
    [ConfigClass]
    internal class Config
    {

        [ConfigOption(Min = 0, Max = 500, Interval = 1)]
        public int ShovelMaxEnergyUsage { get; set; } = 20;

        [ConfigOption(Min = 0, Max = 400, Interval = 1)]
        public int ShovelEnergyDecreasePerLevel { get; set; } = 3;


        [ConfigOption]
        public bool MargoCompact { get; set; } = false;
    }
}
