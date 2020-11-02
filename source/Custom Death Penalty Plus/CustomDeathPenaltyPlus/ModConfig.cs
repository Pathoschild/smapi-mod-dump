/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomDeathPenaltyPlus
**
*************************************************/

using System;

namespace CustomDeathPenaltyPlus
{
    internal class ModConfig
    {
        public bool RestoreItems { get; set; } = true;
        public int MoneyLossCap { get; set; } = 500;
        public double MoneytoRestorePercentage { get; set; } = 0.95;
        public double EnergytoRestorePercentage { get; set; } = 0.10;
        public double HealthtoRestorePercentage { get; set; } = 0.50;
    }
}
