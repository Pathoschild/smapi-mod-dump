/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System;
using System.Linq;
using GreenhouseUpgrades.Upgrades;

namespace GreenhouseUpgrades
{
    class Utils
    {
        public static Upgrade GetUpgradeByName(string UpgradeName, bool CaseSensitive = false)
        {
            return Main.Upgrades.FirstOrDefault(u => u.Name.Equals(UpgradeName, CaseSensitive? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));
        }
    }
}
