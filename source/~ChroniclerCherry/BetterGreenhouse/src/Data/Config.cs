/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace BetterGreenhouse.data
{
    public class Config
    {
        public bool EnableDebugging { get; set; } = false;
        public int SizeUpgradeCost { get; set; } = 50000;
        public int AutoWaterUpgradeCost { get; set; } = 50000;

        public Vector2 JojaMartUpgradeCoordinates { get; set; } = new Vector2(22,25);
        public Vector2 CommunityCenterUpgradeCoordinates { get; set; } = new Vector2(14, 4);
    }
}
