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
    /// <summary>As part of <see cref="ModConfig"/>, the minimum field values needed before they're auto-collapsed.</summary>
    public class ModCollapseLargeFieldsConfig
    {
        /// <summary>Whether to collapse large fields.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>In an item lookup, the minimum recipes needed before the field is collapsed by default.</summary>
        public int ItemRecipes { get; set; } = 11;

        /// <summary>In a character lookup, the minimum gift tastes needed before the field is collapsed by default.</summary>
        public int NpcGiftTastes { get; set; } = 31;
    }
}
