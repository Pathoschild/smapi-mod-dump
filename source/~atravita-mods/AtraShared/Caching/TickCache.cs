/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;

using AtraBase.Toolkit;

namespace AtraShared.Caching;

/// <summary>
/// Wrapper class: caches a value for approximately four ticks.
/// </summary>
/// <typeparam name="T">Type of the value.</typeparam>
/// <remarks>Constraining to just value types since reference types should run through a WeakReference.</remarks>
public struct TickCache<T>
    where T : struct
{
    private readonly Func<T> get;

    private int lastTick = -1;
    private T result = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="TickCache{T}"/> struct.
    /// </summary>
    /// <param name="get">Function that will get the value.</param>
    public TickCache(Func<T> get)
    {
        this.get = get;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <returns>Value.</returns>
    [MethodImpl(TKConstants.Hot)]
    public T GetValue()
    {
        if ((Game1.ticks & ~0b11) != this.lastTick)
        {
            this.lastTick = Game1.ticks & ~0b11;
            this.result = this.get();
        }
        return this.result;
    }

    /// <summary>
    /// Clears the cache.
    /// </summary>
    public void Reset() => this.lastTick = -1;
}