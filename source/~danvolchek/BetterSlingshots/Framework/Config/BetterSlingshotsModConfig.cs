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
    internal class BetterSlingshotsModConfig
    {
        public bool DisableReverseAiming { get; set; } = true;
        public bool AutoReload { get; set; } = true;
        public string[] AutomaticSlingshots { get; set; } = { "Galaxy", "Master" };
        public bool ShowActualMousePositionWhenAiming { get; set; } = true;

        public bool CanMoveWhileFiring { get; set; } = false;
        public bool InfiniteAmmo { get; set; } = false;
        public bool RapidFire { get; set; } = false;
        public int GalaxySlingshotPrice { get; set; } = 50000;

        public BetterSlingshotsModConfig()
        {
        }

        public BetterSlingshotsModConfig(bool disableReverseAiming, bool autoReload, string[] automaticSlingshots, bool showActualMousePositionWhenAiming, bool canMoveWhileFiring, bool infiniteAmmo, bool rapidFire, int galaxySlingshotPrice)
        {
            this.DisableReverseAiming = disableReverseAiming;
            this.AutoReload = autoReload;
            this.AutomaticSlingshots = automaticSlingshots;
            this.ShowActualMousePositionWhenAiming = showActualMousePositionWhenAiming;
            this.CanMoveWhileFiring = canMoveWhileFiring;
            this.InfiniteAmmo = infiniteAmmo;
            this.RapidFire = rapidFire;
            this.GalaxySlingshotPrice = galaxySlingshotPrice;
        }
    }
}
