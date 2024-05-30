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
#else
namespace StardewMods.Common.Models.Cache;
#endif

/// <summary>Represents a cached object.</summary>
/// <typeparam name="T">The cached object type.</typeparam>
internal sealed class CachedObject<T> : BaseCachedObject
{
    private T value;

    /// <summary>Initializes a new instance of the <see cref="CachedObject{T}" /> class.</summary>
    /// <param name="value">The initial value.</param>
    public CachedObject(T value) => this.value = value;

    /// <summary>Gets or sets the value of the cached object.</summary>
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