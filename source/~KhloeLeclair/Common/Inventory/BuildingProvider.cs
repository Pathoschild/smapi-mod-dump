/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_BCINVENTORY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;
using StardewValley.Network;
using StardewValley.Objects;

namespace Leclair.Stardew.Common.Inventory;

public class BuildingProvider : BaseInventoryProvider<Building> {

	public Chest? GetChest(Building bld) {
		var chests = bld.GetData()?.Chests;
		if (chests is null)
			return null;

		foreach(var chest in chests) {
			if (chest.Type == StardewValley.GameData.Buildings.BuildingChestType.Chest)
				return bld.GetBuildingChest(chest.Id);
		}

		return null;
	}

	public override bool IsValid(Building obj, GameLocation? location, Farmer? who) {
		return GetChest(obj) != null;
	}


	public override bool CanExtractItems(Building obj, GameLocation? location, Farmer? who) {
		return IsValid(obj, location, who);
	}

	public override bool CanInsertItems(Building obj, GameLocation? location, Farmer? who) {
		return IsValid(obj, location, who);
	}

	public override void CleanInventory(Building obj, GameLocation? location, Farmer? who) {
		GetChest(obj)?.clearNulls();
	}

	public override int GetActualCapacity(Building obj, GameLocation? location, Farmer? who) {
		return GetChest(obj)?.GetActualCapacity() ?? 0;
	}

	public override IInventory? GetInventory(Building obj, GameLocation? location, Farmer? who) {
		var chest = GetChest(obj);
		if (chest == null)
			return null;

		if (who == null)
			return chest.Items;

		return chest.GetItemsForPlayer(who.UniqueMultiplayerID);
	}

	public override IList<Item?>? GetItems(Building obj, GameLocation? location, Farmer? who) {
		return GetInventory(obj, location, who);
	}

	public override Rectangle? GetMultiTileRegion(Building obj, GameLocation? location, Farmer? who) {
		return new Rectangle(
			obj.tileX.Value, obj.tileY.Value,
			obj.tilesWide.Value, obj.tilesHigh.Value
		);
	}

	public override NetMutex? GetMutex(Building obj, GameLocation? location, Farmer? who) {
		return GetChest(obj)?.GetMutex();
	}

	public override Vector2? GetTilePosition(Building obj, GameLocation? location, Farmer? who) {
		return new Vector2(obj.tileX.Value, obj.tileY.Value);
	}

	public override bool IsItemValid(Building obj, GameLocation? location, Farmer? who, Item item) {
		// TODO: This?
		return true;
	}

	public override bool IsMutexRequired(Building obj, GameLocation? location, Farmer? who) {
		return true;
	}

}

#endif
