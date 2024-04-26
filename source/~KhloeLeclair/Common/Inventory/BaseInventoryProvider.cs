/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Inventories;
using StardewValley.Network;

namespace Leclair.Stardew.Common.Inventory;

public abstract class BaseInventoryProvider<T> : IInventoryProvider where T : class {

	/// <inheritdoc />
	public bool CanExtractItems(object obj, GameLocation? location, Farmer? who) {
		return obj is T tobj && CanExtractItems(tobj, location, who);
	}

	/// <inheritdoc cref="CanExtractItems(object, GameLocation?, Farmer?)" />
	public abstract bool CanExtractItems(T obj, GameLocation? location, Farmer? who);

	/// <inheritdoc />
	public bool CanInsertItems(object obj, GameLocation? location, Farmer? who) {
		return obj is T tobj && CanInsertItems(tobj, location, who);
	}

	/// <inheritdoc cref="CanInsertItems(object, GameLocation?, Farmer?)" />
	public abstract bool CanInsertItems(T obj, GameLocation? location, Farmer? who);

	/// <inheritdoc />
	public void CleanInventory(object obj, GameLocation? location, Farmer? who) {
		if (obj is T tobj)
			CleanInventory(tobj, location, who);
	}

	/// <inheritdoc cref="CleanInventory(object, GameLocation?, Farmer?)" />
	public abstract void CleanInventory(T obj, GameLocation? location, Farmer? who);

	/// <inheritdoc />
	public int GetActualCapacity(object obj, GameLocation? location, Farmer? who) {
		return obj is T tobj ? GetActualCapacity(tobj, location, who) : 0;
	}

	/// <inheritdoc cref="GetActualCapacity(object, GameLocation?, Farmer?)" />
	public abstract int GetActualCapacity(T obj, GameLocation? location, Farmer? who);

	/// <inheritdoc />
	public IList<Item?>? GetItems(object obj, GameLocation? location, Farmer? who) {
		return obj is T tobj ? GetItems(tobj, location, who) : null;
	}

	/// <inheritdoc cref="GetItems(object, GameLocation?, Farmer?)" />
	public abstract IList<Item?>? GetItems(T obj, GameLocation? location, Farmer? who);

	/// <inheritdoc />
	public bool IsItemValid(object obj, GameLocation? location, Farmer? who, Item item) {
		return obj is T tobj && IsItemValid(tobj, location, who, item);
	}

	/// <inheritdoc cref="IsItemValid(object, GameLocation?, Farmer?, Item)" />
	public abstract bool IsItemValid(T obj, GameLocation? location, Farmer? who, Item item);

	/// <inheritdoc />
	public Rectangle? GetMultiTileRegion(object obj, GameLocation? location, Farmer? who) {
		return obj is T tobj ? GetMultiTileRegion(tobj, location, who) : null;
	}

	/// <inheritdoc cref="GetMultiTileRegion(object, GameLocation?, Farmer?)" />
	public abstract Rectangle? GetMultiTileRegion(T obj, GameLocation? location, Farmer? who);

	/// <inheritdoc />
	public NetMutex? GetMutex(object obj, GameLocation? location, Farmer? who) {
		return obj is T tobj ? GetMutex(tobj, location, who) : null;
	}

	/// <inheritdoc cref="GetMutex(object, GameLocation?, Farmer?)" />
	public abstract NetMutex? GetMutex(T obj, GameLocation? location, Farmer? who);

	/// <inheritdoc />
	public bool IsMutexRequired(object obj, GameLocation? location, Farmer? who) {
		return obj is not T tobj || IsMutexRequired(tobj, location, who);
	}

	/// <inheritdoc cref="IsMutexRequired(object, GameLocation?, Farmer?)" />
	public abstract bool IsMutexRequired(T obj, GameLocation? location, Farmer? who);

	/// <inheritdoc />
	public Vector2? GetTilePosition(object obj, GameLocation? location, Farmer? who) {
		return obj is T tobj ? GetTilePosition(tobj, location, who) : null;
	}

	/// <inheritdoc cref="GetTilePosition(object, GameLocation?, Farmer?)" />
	public abstract Vector2? GetTilePosition(T obj, GameLocation? location, Farmer? who);

	/// <inheritdoc />
	public bool IsValid(object obj, GameLocation? location, Farmer? who) {
		return obj is T tobj && IsValid(tobj, location, who);
	}

	/// <inheritdoc cref="IsValid(object, GameLocation?, Farmer?)" />
	public abstract bool IsValid(T obj, GameLocation? location, Farmer? who);

	/// <inheritdoc />
	public IInventory? GetInventory(object obj, GameLocation? location, Farmer? who) {
		return obj is T tobj ? GetInventory(tobj, location, who) : null;
	}

	/// <inheritdoc cref="GetInventory(object, GameLocation?, Farmer?)" />
	public abstract IInventory? GetInventory(T obj, GameLocation? location, Farmer? who);
}
