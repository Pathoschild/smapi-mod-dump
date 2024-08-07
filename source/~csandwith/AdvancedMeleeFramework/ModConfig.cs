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

namespace AdvancedMeleeFramework
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public SButton ReloadButton { get; set; } = SButton.NumPad0;
        public bool RequireModKey { get; set; } = false;
        public SButton ModKey { get; set; } = SButton.LeftShift;
    }
}
