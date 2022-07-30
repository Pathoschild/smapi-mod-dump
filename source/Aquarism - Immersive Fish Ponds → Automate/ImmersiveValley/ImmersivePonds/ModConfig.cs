/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Ponds;

/// <summary>The mod user-defined settings.</summary>
internal class ModConfig
{
    /// <summary>Multiplies a fish's base chance to produce roe each day.</summary>
    public float RoeProductionChanceMultiplier = 1f;

    /// <summary>Number of days until an empty pond will begin spawning algae.</summary>
    public int DaysUntilAlgaeSpawn = 2;
}