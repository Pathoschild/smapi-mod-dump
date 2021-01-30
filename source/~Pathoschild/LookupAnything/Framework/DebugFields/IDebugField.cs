/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.LookupAnything.Framework.DebugFields
{
    /// <summary>A debug field containing a raw datamining value.</summary>
    internal interface IDebugField
    {
        /*********
        ** Accessors
        *********/
        /// <summary>A short field label.</summary>
        string Label { get; }

        /// <summary>The field value.</summary>
        string Value { get; }

        /// <summary>Whether the field should be displayed.</summary>
        bool HasValue { get; }

        /// <summary>Whether the field should be highlighted for special attention.</summary>
        bool IsPinned { get; }

        /// <summary>The debug category text.</summary>
        public string OverrideCategory { get; set; }
    }
}
