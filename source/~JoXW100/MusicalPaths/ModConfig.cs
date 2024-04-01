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

namespace MusicalPaths
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public SButton ModKey { get; set; } = SButton.LeftAlt;
        public bool ConsumeBlock { get; set; } = true;
        public bool ShowBlockOutLine { get; set; } = true;
        public float BlockOutLineOpacity { get; set; } = 0.5f;
    }
}
