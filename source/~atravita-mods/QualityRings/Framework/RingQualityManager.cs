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
using AtraShared.Utils.Extensions;
using StardewValley.Objects;

namespace QualityRings.Framework;

/// <summary>
/// Caches the quality of rings so I'm not bloody parsing them from modData all the time.
/// </summary>
internal static class RingQualityManager
{
    private const string RingQuality = "atravita.RingQuality";

    private record Holder(int quality);

    private static readonly ConditionalWeakTable<Ring, Holder> QualityMap = new();

    /// <summary>
    /// Gets the quality for a ring.
    /// </summary>
    /// <param name="ring">Ring in question.</param>
    /// <returns>quality of the ring.</returns>
    internal static int GetQuality(Ring ring)
        => QualityMap.TryGetValue(ring, out Holder? val) ? val.quality : ring.modData?.GetInt(RingQuality) ?? 0;

    /// <summary>
    /// Sets the quality for a ring.
    /// </summary>
    /// <param name="ring">Ring in question.</param>
    /// <param name="quality">quality of the ring.</param>
    internal static void SetQuality(Ring ring, int quality)
    {
        ring.modData?.SetInt(RingQuality, quality);
        QualityMap.AddOrUpdate(ring, new Holder(quality));
    }
}
