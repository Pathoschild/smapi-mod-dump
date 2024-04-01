/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.CustomBush.Framework.Models;

using StardewValley.GameData;

/// <summary>Model used for drops from custom tea plants.</summary>
internal sealed class DropsModel : GenericSpawnItemDataWithCondition
{
    /// <summary>Gets or sets the specific season when the item can be produced.</summary>
    public Season? Season { get; set; }

    /// <summary>Gets or sets the probability that the item will be produced.</summary>
    public float Chance { get; set; } = 1f;
}