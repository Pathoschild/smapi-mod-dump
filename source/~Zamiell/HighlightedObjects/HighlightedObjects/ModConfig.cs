/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

namespace HighlightedObjects
{
    public sealed class ModConfig
    {
        public string HighlightType { get; set; } = "Highlight";

        public bool HighlightJars { get; set; } = true;
        public bool HighlightKegs { get; set; } = true;
        public bool HighlightCasks { get; set; } = true;
        public bool HighlightCrystalariums { get; set; } = true;
    }
}
