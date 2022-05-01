/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

using System.Globalization;

namespace Pathoschild.Stardew.LookupAnything.Framework.DebugFields
{
    /// <summary>A generic debug field containing a raw datamining value.</summary>
    internal class GenericDebugField : IDebugField
    {
        /*********
        ** Accessors
        *********/
        /// <inheritdoc />
        public string Label { get; protected set; }

        /// <inheritdoc />
        public string Value { get; protected set; }

        /// <inheritdoc />
        public bool HasValue { get; protected set; }

        /// <inheritdoc />
        public bool IsPinned { get; protected set; }

        /// <inheritdoc />
        public string OverrideCategory { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="label">A short field label.</param>
        /// <param name="value">The field value.</param>
        /// <param name="hasValue">Whether the field should be displayed (or <c>null</c> to check the <paramref name="value"/>).</param>
        /// <param name="pinned">Whether the field should be highlighted for special attention.</param>
        public GenericDebugField(string label, string value, bool? hasValue = null, bool pinned = false)
        {
            this.Label = label;
            this.Value = value;
            this.HasValue = hasValue ?? !string.IsNullOrWhiteSpace(this.Value);
            this.IsPinned = pinned;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="label">A short field label.</param>
        /// <param name="value">The field value.</param>
        /// <param name="hasValue">Whether the field should be displayed (or <c>null</c> to check the <paramref name="value"/>).</param>
        /// <param name="pinned">Whether the field should be highlighted for special attention.</param>
        public GenericDebugField(string label, int value, bool? hasValue = null, bool pinned = false)
            : this(label, value.ToString(CultureInfo.InvariantCulture), hasValue, pinned) { }

        /// <summary>Construct an instance.</summary>
        /// <param name="label">A short field label.</param>
        /// <param name="value">The field value.</param>
        /// <param name="hasValue">Whether the field should be displayed (or <c>null</c> to check the <paramref name="value"/>).</param>
        /// <param name="pinned">Whether the field should be highlighted for special attention.</param>
        public GenericDebugField(string label, float value, bool? hasValue = null, bool pinned = false)
            : this(label, value.ToString(CultureInfo.InvariantCulture), hasValue, pinned) { }
    }
}
