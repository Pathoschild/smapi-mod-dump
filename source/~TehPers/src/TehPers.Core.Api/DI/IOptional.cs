/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.DI
{
    /// <summary>An object with an optional value.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    public interface IOptional<T>
    {
        /// <summary>Gets the value of this <see cref="IOptional{T}"/>.</summary>
        /// <exception cref="InvalidOperationException">This <see cref="IOptional{T}"/> has no value.</exception>
        T Value { get; }

        /// <summary>Gets a value indicating whether this <see cref="IOptional{T}"/> has a value.</summary>
        bool HasValue { get; }

        /// <summary>Tries to get the value from this <see cref="IOptional{T}"/>.</summary>
        /// <param name="value">The value if it exists.</param>
        /// <returns><see langword="true"/> if the value exists, <see langword="false"/> otherwise.</returns>
        bool TryGetValue([NotNullWhen(true)] out T? value);
    }
}