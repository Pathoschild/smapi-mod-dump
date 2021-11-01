/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;

namespace GenericModConfigMenu.Framework.ModOption
{
    /// <summary>A mod option which renders a paragraph of text.</summary>
    internal class ParagraphModOption : ReadOnlyModOption
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="text">The paragraph text to show in the form.</param>
        /// <param name="mod">The mod config UI that contains this option.</param>
        public ParagraphModOption(Func<string> text, ModConfig mod)
            : base(text, null, mod) { }
    }
}
