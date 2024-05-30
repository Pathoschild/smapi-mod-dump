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

/// <summary>Represents a table of cached values.</summary>
public abstract class BaseCacheTable
{
    /// <summary>Removes all cached values that have not been accessed since before the specified tick count.</summary>
    /// <param name="ticks">The number of ticks.</param>
    public abstract void RemoveBefore(int ticks);
}