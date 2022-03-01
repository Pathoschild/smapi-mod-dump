/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Models;

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

internal readonly record struct LocationChest(GameLocation Location, Vector2 Position, Chest Chest, string Name);