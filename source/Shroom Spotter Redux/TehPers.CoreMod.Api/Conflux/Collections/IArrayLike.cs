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
    public interface IArrayLike<T> : IReadonlyArrayLike<T> {
        /// <summary>Selects an element.</summary>
        /// <param name="index">The index of the element to select.</param>
        /// <returns>The selected element.</returns>
        new T this[Index index] { get; set; }
    }
}