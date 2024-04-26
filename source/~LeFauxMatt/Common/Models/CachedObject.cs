/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Models;

using StardewMods.Common.Interfaces;

/// <inheritdoc cref="StardewMods.Common.Interfaces.ICachedObject" />
internal sealed class CachedObject<T> : CachedObject, ICachedObject<T>
{
    private T value;

    /// <summary>Initializes a new instance of the <see cref="CachedObject{T}" /> class.</summary>
    /// <param name="value">The initial value.</param>
    public CachedObject(T value) => this.value = value;

    /// <inheritdoc />
    public T Value
    {
        get
        {
            this.Ticks = Game1.ticks;
            return this.value;
        }

        set
        {
            this.Ticks = Game1.ticks;
            this.value = value;
        }
    }
}

internal class CachedObject : ICachedObject
{
    /// <summary>Initializes a new instance of the <see cref="CachedObject" /> class.</summary>
    public CachedObject() => this.Ticks = Game1.ticks;

    public int Ticks { get; protected set; }
}