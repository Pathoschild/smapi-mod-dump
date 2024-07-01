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

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Inventories;
using StardewValley.Network;

namespace Leclair.Stardew.Common.Inventory;

// Remember to update IBetterCrafting whenever this changes!

/// <summary>
/// An <c>IInventoryProvider</c> is used by Better Crafting to discover and
/// interact with various item storages in the game.
/// </summary>
public interface IInventoryProvider {

	/// <summary>
	/// Check to see if this object is valid for inventory operations.
	///
	/// If location is null, it should not be considered when determining
	/// the validity of the object.
	/// 
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	/// <returns>whether or not the object is valid</returns>
	bool IsValid(object obj, GameLocation? location, Farmer? who);

	/// <summary>
	/// Check to see if items can be inserted into this object.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	/// <returns></returns>
	bool CanInsertItems(object obj, GameLocation? location, Farmer? who);

	/// <summary>
	/// Check to see if items can be extracted from this object.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	/// <returns></returns>
	bool CanExtractItems(object obj, GameLocation? location, Farmer? who);

	/// <summary>
	/// For objects larger than a single tile on the map, return the rectangle representing
	/// the object. For single tile objects, return null.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	/// <returns></returns>
	Rectangle? GetMultiTileRegion(object obj, GameLocation? location, Farmer? who);

	/// <summary>
	/// Return the real position of the object. If the object has no position, returns null.
	/// For multi-tile objects, this should return the "main" object if there is one. 
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	Vector2? GetTilePosition(object obj, GameLocation? location, Farmer? who);

	/// <summary>
	/// Get the NetMutex that locks the object for multiplayer synchronization. This method must
	/// return a mutex. If null is returned, the object will be skipped.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	NetMutex? GetMutex(object obj, GameLocation? location, Farmer? who);

	/// <summary>
	/// Whether or not a mutex is required for interacting with this object's inventory.
	/// You should always use a mutex to ensure items are handled safely with multiplayer,
	/// but in case you're doing something exceptional and Better Crafting should not
	/// worry about locking, you can explicitly disable mutex handling.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	bool IsMutexRequired(object obj, GameLocation? location, Farmer? who) => true;

	/// <summary>
	/// Get a list of items in the object's inventory, for modification or viewing. Assume that
	/// anything using this list will use GetMutex() to lock the inventory before modifying.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	IList<Item?>? GetItems(object obj, GameLocation? location, Farmer? who);

	/// <summary>
	/// Get a vanilla IInventory for an object.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the object, or null if no player is involved</param>
	/// <returns></returns>
	IInventory? GetInventory(object obj, GameLocation? location, Farmer? who);

	/// <summary>
	/// Check to see if a specific item is allowed to be stored in the object's inventory.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	/// <param name="item">the item we're checking</param>
	bool IsItemValid(object obj, GameLocation? location, Farmer? who, Item item) => true;

	/// <summary>
	/// Clean the inventory of the object. This is for removing null entries, organizing, etc.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	void CleanInventory(object obj, GameLocation? location, Farmer? who);

	/// <summary>
	/// Get the actual inventory capacity of the object's inventory. New items may be added to the
	/// GetItems() list up until this count.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	int GetActualCapacity(object obj, GameLocation? location, Farmer? who);

}


/// <summary>
/// This is an extension of <see cref="IInventoryProvider"/> that allows you
/// to implement custom logic that runs before and after an inventory is
/// used for write operations. You can use this to perform locks or to
/// synchronize an object.
/// </summary>
public interface IEventedInventoryProvider : IInventoryProvider {

	/// <summary>
	/// The callback method you call when we have either obtained exclusive
	/// access to the inventory, or doing so has failed for some reason.
	/// </summary>
	/// <param name="success">Whether or not we have obtained exclusive access</param>
	delegate void StartExclusiveCallback(bool success);

	/// <summary>
	/// This is called when we need exclusive access to an inventory to perform
	/// write operations. You are expected to either return a boolean, or
	/// return <c>null</c> and then call <paramref name="callback"/>
	/// when any logic necessary to obtain write access to the inventory is
	/// completed, and may call it immediately.
	///
	/// If this method returns <c>null</c>, then you MUST call onComplete even
	/// if the object cannot be obtained for write access.
	///
	/// If you are using native NetMutex to control exclusive access, you do
	/// not need to use these methods and can just use the standard
	/// <see cref="IInventoryProvider.GetMutex(object, GameLocation?, Farmer?)"/>
	/// and <see cref="IInventoryProvider.IsMutexRequired(object, GameLocation?, Farmer?)"/>
	/// methods for exclusive access.
	/// 
	/// If you do not call onComplete within 5 seconds, the operation will
	/// time out and an error will be logged / shown to the user.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	/// <param name="callback">A callback to call when the object has been
	/// obtained for write access. Call this with <c>true</c> if the object
	/// was obtained successfully, or <c>false</c> if it could not be
	/// obtained successfully.</param>
	/// <returns>Whether there was an immediate success (<c>true</c>), an
	/// immediate failure (<c>false</c>), or we should expect the callback
	/// to be called (<c>null</c>).</returns>
	bool? StartExclusive(object obj, GameLocation? location, Farmer? who, StartExclusiveCallback callback);

	/// <summary>
	/// This is called whenever we are done using an inventory exclusively,
	/// and can be used to perform any necessary cleanup logic and to release
	/// any locks.
	/// </summary>
	/// <param name="obj">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	void EndExclusive(object obj, GameLocation? location, Farmer? who);

}



#endif
