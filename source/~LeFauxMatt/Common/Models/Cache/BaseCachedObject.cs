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
internal class BaseCachedObject
{
    /// <summary>Initializes a new instance of the <see cref="BaseCachedObject" /> class.</summary>
    protected BaseCachedObject() => this.Ticks = Game1.ticks;

    /// <summary>Gets the number of ticks since the cached object was last accessed.</summary>
    public int Ticks { get; protected set; }
}