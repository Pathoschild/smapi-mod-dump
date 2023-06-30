/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.LookupAnything.Framework
{
    /// <summary>As part of <see cref="ModConfig"/>, which gift taste levels to show in NPC and item lookups.</summary>
    internal class ModGiftTasteConfig
    {
        /// <summary>Whether to show loved gifts.</summary>
        public bool Loved { get; set; } = true;

        /// <summary>Whether to show liked gifts.</summary>
        public bool Liked { get; set; } = true;

        /// <summary>Whether to show neutral gifts.</summary>
        public bool Neutral { get; set; }

        /// <summary>Whether to show disliked gifts.</summary>
        public bool Disliked { get; set; }

        /// <summary>Whether to show hated gifts.</summary>
        public bool Hated { get; set; }
    }
}
