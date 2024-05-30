/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Models.Cache;

using StardewMods.FauxCore.Common.Interfaces.Cache;

#else
namespace StardewMods.Common.Models.Cache;

using StardewMods.Common.Interfaces.Cache;
#endif

/// <inheritdoc />
public readonly struct CachedValue<T> : ICachedValue
{
    /// <summary>Initializes a new instance of the <see cref="CachedValue{T}" /> struct.</summary>
    /// <param name="originalValue">The original value.</param>
    /// <param name="cachedValue">The cached value.</param>
    public CachedValue(string originalValue, T cachedValue)
    {
        this.OriginalValue = originalValue;
        this.Value = cachedValue;
    }

    /// <inheritdoc />
    public string OriginalValue { get; }

    /// <summary>Gets the cached value.</summary>
    public T Value { get; }
}