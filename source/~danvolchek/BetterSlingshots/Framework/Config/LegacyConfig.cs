/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace BetterSlingshots.Framework.Config
{
    internal class LegacyConfig
    {
        public bool DisableReverseAiming { get; set; } = true;
        public bool AutoReload { get; set; } = true;
        public string AutomaticSlingshots { get; set; } = "Galaxy, Master";
        public bool ShowActualMousePositionWhenAiming { get; set; } = true;

        public bool CanMoveWhileFiring { get; set; } = false;
        public bool InfiniteAmmo { get; set; } = false;
        public bool RapidFire { get; set; } = false;
        public int GalaxySlingshotPrice { get; set; } = 50000;
    }
}
