/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace CustomPaintings
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public Dictionary<int, string> PaintingPaths { get; set; } = new Dictionary<int, string>();
        public SButton ModKey { get; set; } = SButton.LeftAlt;
    }
}
