/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace DiagonalAim
{
    public class ModConfig
    {
        public Keybind ModToggleKey { get; set; } = new Keybind(SButton.None);
        public bool AllowAnyToolOnStandingTile { get; set; } = true;
        public int ExtraReachRadius { get; set; } = 0;
    }
}