/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.GarbageDay.Framework.Models;

using StardewMods.GarbageDay.Framework.Interfaces;

/// <inheritdoc />
internal sealed class DefaultConfig : IModConfig
{
    /// <inheritdoc />
    public DayOfWeek GarbageDay { get; set; } = DayOfWeek.Monday;

    /// <inheritdoc />
    public bool OnByDefault { get; set; } = true;
}