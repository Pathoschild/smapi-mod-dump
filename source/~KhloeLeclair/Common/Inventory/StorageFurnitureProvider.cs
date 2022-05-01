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
using StardewValley.Network;
using StardewValley.Objects;

namespace Leclair.Stardew.Common.Inventory;

public class StorageFurnitureProvider : BaseInventoryProvider<StorageFurniture> {
	public override bool CanExtractItems(StorageFurniture obj, GameLocation? location, Farmer? who) {
		return true;
	}

	public override bool CanInsertItems(StorageFurniture obj, GameLocation? location, Farmer? who) {
		return true;
	}

	public override void CleanInventory(StorageFurniture obj, GameLocation? location, Farmer? who) {
		obj.ClearNulls();
	}

	public override int GetActualCapacity(StorageFurniture obj, GameLocation? location, Farmer? who) {
		return 36;
	}

	public override IList<Item?>? GetItems(StorageFurniture obj, GameLocation? location, Farmer? who) {
		// TODO: Implement a managed item list that does the stuff storage
		// furniture does when accessing items.
		return obj.heldItems;
	}

	public override bool IsItemValid(StorageFurniture obj, GameLocation? location, Farmer? who, Item item) {
		// TODO: Check if items are valid.
		return true;
	}

	public override Rectangle? GetMultiTileRegion(StorageFurniture obj, GameLocation? location, Farmer? who) {
		// TODO: Implement this
		return null;
	}

	public override NetMutex? GetMutex(StorageFurniture obj, GameLocation? location, Farmer? who) {
		return obj.mutex;
	}

	public override bool IsMutexRequired(StorageFurniture obj, GameLocation? location, Farmer? who) {
		return true;
	}

	public override Vector2? GetTilePosition(StorageFurniture obj, GameLocation? location, Farmer? who) {
		return obj.TileLocation;
	}

	public override bool IsValid(StorageFurniture obj, GameLocation? location, Farmer? who) {
		return true;
	}
}
