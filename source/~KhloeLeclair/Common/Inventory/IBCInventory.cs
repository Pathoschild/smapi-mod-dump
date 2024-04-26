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

using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Inventories;
using StardewValley.Network;

namespace Leclair.Stardew.Common.Inventory;

// Remember to update IBetterCrafting whenever this changes!

/// <summary>
/// An <c>IBCInventory</c> represents an item storage that
/// Better Crafting is interacting with, whether by extracting
/// items or inserting them.
/// </summary>
public interface IBCInventory {

	/// <summary>
	/// Optional. If this inventory is associated with an object, that object.
	/// </summary>
	object Object { get; }

	/// <summary>
	/// If this inventory is associated with an object, where that object is located.
	/// </summary>
	GameLocation? Location { get; }

	/// <summary>
	/// If this inventory is associated with a player, the player.
	/// </summary>
	Farmer? Player { get; }

	/// <summary>
	/// If this inventory is managed by a NetMutex, or an object with one,
	/// which should be locked before manipulating the inventory, then
	/// provide it here.
	/// </summary>
	NetMutex? Mutex { get; }

	/// <summary>
	/// Get this inventory as a vanilla IInventory, if possible. May
	/// be null if the inventory is not a vanilla inventory.
	/// </summary>
	IInventory? Inventory { get; }

	/// <summary>
	/// Whether or not the inventory is locked and ready for read/write usage.
	/// </summary>
	bool IsLocked();

	/// <summary>
	/// Whether or not the inventory is a valid inventory.
	/// </summary>
	bool IsValid();

	/// <summary>
	/// Whether or not we can insert items into this inventory.
	/// </summary>
	bool CanInsertItems();

	/// <summary>
	/// Whether or not we can extract items from this inventory.
	/// </summary>
	bool CanExtractItems();

	/// <summary>
	/// For inventories associated with multiple tile regions in a location,
	/// such as a farm house kitchen, this is the region the inventory fills.
	/// Only rectangular shapes are supported. This is used for discovering
	/// connections to nearby inventories.
	/// </summary>
	Rectangle? GetMultiTileRegion();

	/// <summary>
	/// For inventories associated with a tile position in a location, such
	/// as a chest placed in the world.
	/// 
	/// For multi-tile inventories, this should be the primary tile if
	/// one exists.
	/// </summary>
	Vector2? GetTilePosition();

	/// <summary>
	/// Get this inventory as a list of items. May be null if
	/// there is an issue accessing the object's inventory.
	/// </summary>
	IList<Item?>? GetItems();

	/// <summary>
	/// Check to see if a specific item is allowed to be stored in
	/// this inventory.
	/// </summary>
	/// <param name="item">The item we're checking</param>
	bool IsItemValid(Item item);

	/// <summary>
	/// Attempt to clean the object's inventory. This should remove null
	/// entries, and run any other necessary logic.
	/// </summary>
	void CleanInventory();

	/// <summary>
	/// Get the number of item slots in the object's inventory. When adding
	/// items to the inventory, we will never extend the list beyond this
	/// number of entries.
	/// </summary>
	int GetActualCapacity();
}
