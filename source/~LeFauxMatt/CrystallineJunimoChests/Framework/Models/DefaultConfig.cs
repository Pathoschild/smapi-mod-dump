/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.CrystallineJunimoChests.Framework.Models;

using StardewMods.CrystallineJunimoChests.Framework.Interfaces;

/// <inheritdoc />
internal sealed class DefaultConfig : IModConfig
{
    /// <inheritdoc />
    public int GemCost { get; set; } = 1;

    /// <inheritdoc />
    public string Sound { get; set; } = "wand";
}