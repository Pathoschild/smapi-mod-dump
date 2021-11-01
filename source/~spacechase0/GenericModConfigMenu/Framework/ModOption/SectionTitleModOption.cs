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
    /// <summary>A mod option which renders a section title.</summary>
    internal class SectionTitleModOption : ReadOnlyModOption
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="text">The title text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="mod">The mod config UI that contains this option.</param>
        public SectionTitleModOption(Func<string> text, Func<string> tooltip, ModConfig mod)
            : base(text, tooltip, mod) { }
    }
}
