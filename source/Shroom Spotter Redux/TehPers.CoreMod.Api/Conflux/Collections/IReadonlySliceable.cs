/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;

namespace TehPers.CoreMod.Api.Conflux.Collections {
    public interface IReadonlySliceable<T> : IReadonlyArrayLike<T> {
        /// <summary>Selects a range of elements.</summary>
        /// <param name="range">The range to select. Either use a <see cref="Range"/> object, or use the syntax <c>(start, end)</c>. <see cref="ValueTuple"/> is required for the second syntax.</param>
        /// <returns>A reference to a slice of elements.</returns>
        ReadonlySlice<T> this[Range range] { get; }
    }
}