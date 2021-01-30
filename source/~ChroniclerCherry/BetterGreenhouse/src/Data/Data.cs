/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;
using GreenhouseUpgrades.Upgrades;

namespace GreenhouseUpgrades.Data
{
    class Data
    {
        public Dictionary<UpgradeTypes, UpgradeData> UpgradesStatus { get; set; } = new Dictionary<UpgradeTypes, UpgradeData>();
        public int JunimoPoints { get; set; }
    }

    public class UpgradeData
    {
        public bool Unlocked { get; set; }
        public bool Active { get; set; }

    }
}
