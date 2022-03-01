/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Models.GameObjects;

using Microsoft.Xna.Framework;
using StardewMods.FuryCore.Interfaces;
using StardewMods.FuryCore.Interfaces.GameObjects;
using StardewValley;

/// <summary>
///     Represents a <see cref="IGameObject" /> that is placed in a <see cref="GameLocation" />
/// </summary>
/// <param name="Location">The location of the object.</param>
/// <param name="Position">The position where the object is placed.</param>
public readonly record struct LocationObject(GameLocation Location, Vector2 Position) : IGameObjectType;