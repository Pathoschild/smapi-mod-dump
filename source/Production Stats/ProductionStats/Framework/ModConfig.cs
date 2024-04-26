/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

// derived from code by Jesse Plamondon-Willard under MIT license: https://github.com/Pathoschild/StardewMods

using System.Runtime.Serialization;

namespace ProductionStats.Framework;

internal class ModConfig
{
    /// <summary>
    /// The key bindings.
    /// </summary>
    public ModConfigKeys Controls { get; set; } = new();

    /// <param name="context">The deserialization context.</param>
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
        Controls ??= new ModConfigKeys();
    }
}