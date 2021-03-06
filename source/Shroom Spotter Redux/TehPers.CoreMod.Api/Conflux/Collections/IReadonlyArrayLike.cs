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
using System.Collections.Generic;

namespace TehPers.CoreMod.Api.Conflux.Collections {
    public interface IReadonlyArrayLike<out T> : IEnumerable<T> {
        /// <summary>The number of elements.</summary>
        int Length { get; }

        /// <summary>Selects an element.</summary>
        /// <param name="index">The index of the element to select.</param>
        /// <returns>The selected element.</returns>
        T this[Index index] { get; }
    }
}