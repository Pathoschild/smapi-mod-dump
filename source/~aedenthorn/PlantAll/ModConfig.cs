/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace PlantAll
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool AllowDiagonal { get; set; } = true;

        public SButton ModButton { get; set; } = SButton.LeftShift;

    }
}
