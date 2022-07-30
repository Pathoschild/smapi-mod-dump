/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.ConstantsAndEnums;
using Microsoft.Xna.Framework;

namespace AtraShared.Niceties;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter. These are records.
/// <summary>
/// A specific tile.
/// </summary>
/// <param name="Map">Map the tile should appear on.</param>
/// <param name="Pos">The position of the tile on the map.</param>
public readonly record struct Tile(string Map, Vector2 Pos);

/// <summary>
/// An item.
/// </summary>
/// <param name="Type">The type of the item.</param>
/// <param name="Id">The integer ID of the item.</param>
public readonly record struct ItemRecord(ItemTypeEnum Type, int Id);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter