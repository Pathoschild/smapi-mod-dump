/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace StardewMods.FuryCore.Models.GameObjects;

using StardewMods.FuryCore.Interfaces;
using StardewMods.FuryCore.Interfaces.GameObjects;
using StardewValley;

/// <summary>
///     Represents a <see cref="IGameObject" /> that is in a player's inventory.
/// </summary>
/// <param name="Player">The player whose inventory has the object.</param>
/// <param name="Index">The item slot where the object is held.</param>
public readonly record struct InventoryItem(Farmer Player, int Index) : IGameObjectType;