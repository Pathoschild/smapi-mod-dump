/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace ExpertSitting
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool AllowMapSit { get; set; } = true;
        public string[] SeatTypes { get; set; } = {
            "Stone",
            "Log Section",
            "Ornamental Hay Bale",
        };
        public SButton MapSitModKey { get; set; } = SButton.LeftShift;
    }
}
