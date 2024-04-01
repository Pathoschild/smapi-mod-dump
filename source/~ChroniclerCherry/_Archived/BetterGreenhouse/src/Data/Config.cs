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
using Microsoft.Xna.Framework;

namespace GreenhouseUpgrades.Data
{
    public class Config
    {
        public bool EnableDebugging { get; set; } = false;

        public Dictionary<UpgradeTypes, int> UpgradeCosts = new Dictionary<UpgradeTypes, int>()
        {
            {UpgradeTypes.SizeUpgrade,50000},
            {UpgradeTypes.AutoWaterUpgrade,50000},
            {UpgradeTypes.AutoHarvestUpgrade,50000}
        };

        public Vector2 JojaMartUpgradeCoordinates { get; set; } = new Vector2(22,25);
        public Vector2 CommunityCenterUpgradeCoordinates { get; set; } = new Vector2(14, 4);
    }
}
