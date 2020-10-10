/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.TractorMod.Framework.Config
{
    /// <summary>Configuration for the hoe attachment.</summary>
    internal class HoeConfig
    {
        /// <summary>Whether to till empty dirt.</summary>
        public bool TillDirt { get; set; } = true;

        /// <summary>Whether to clear weeds.</summary>
        public bool ClearWeeds { get; set; } = true;

        /// <summary>Whether to dig artifact spots.</summary>
        public bool DigArtifactSpots { get; set; } = true;
    }
}
