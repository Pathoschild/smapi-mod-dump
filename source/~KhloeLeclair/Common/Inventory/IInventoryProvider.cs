/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Network;

namespace Leclair.Stardew.Common.Inventory {
	public interface IInventoryProvider {

		/// <summary>
		/// Check to see if this object is valid for inventory operations.
		///
		/// If location is null, it should not be considered when determining
		/// the validitiy of the object.
		/// 
		/// </summary>
		/// <param name="obj">the object</param>
		/// <param name="location">the map where the object is</param>
		/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
		/// <returns>whether or not the object is valid</returns>
		bool IsValid(object obj, GameLocation location, Farmer who);

		/// <summary>
		/// Check to see if items can be inserted into this object.
		/// </summary>
		/// <param name="obj">the object</param>
		/// <param name="location">the map where the object is</param>
		/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
		/// <returns></returns>
		bool CanInsertItems(object obj, GameLocation location, Farmer who);

		/// <summary>
		/// Check to see if items can be extracted from this object.
		/// </summary>
		/// <param name="obj">the object</param>
		/// <param name="location">the map where the object is</param>
		/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
		/// <returns></returns>
		bool CanExtractItems(object obj, GameLocation location, Farmer who);

		/// <summary>
		/// For objects larger than a single tile on the map, return the rectangle representing
		/// the object. For single tile objects, return null.
		/// </summary>
		/// <param name="obj">the object</param>
		/// <param name="location">the map where the object is</param>
		/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
		/// <returns></returns>
		Rectangle? GetMultiTileRegion(object obj, GameLocation location, Farmer who);

		/// <summary>
		/// Return the real position of the object. If the object has no position, returns null.
		/// For multi-tile objects, this should return the "main" object if there is one. 
		/// </summary>
		/// <param name="obj">the object</param>
		/// <param name="location">the map where the object is</param>
		/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
		/// <returns></returns>
		Vector2? GetTilePosition(object obj, GameLocation location, Farmer who);

		/// <summary>
		/// Get the NetMutex that locks the object for multiplayer synchronization. This method must
		/// return a mutex. If null is returned, the object will be skipped.
		/// </summary>
		/// <param name="obj">the object</param>
		/// <param name="location">the map where the object is</param>
		/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
		/// <returns></returns>
		NetMutex GetMutex(object obj, GameLocation location, Farmer who);

		/// <summary>
		/// Get a list of items in the object's inventory, for modification or viewing. Assume that
		/// anything using this list will use GetMutex() to lock the inventory before modifying.
		/// </summary>
		/// <param name="obj">the object</param>
		/// <param name="location">the map where the object is</param>
		/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
		/// <returns></returns>
		IList<Item> GetItems(object obj, GameLocation location, Farmer who);

		/// <summary>
		/// Clean the inventory of the object. This is for removing null entries, organizing, etc.
		/// </summary>
		/// <param name="obj">the object</param>
		/// <param name="location">the map where the object is</param>
		/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
		void CleanInventory(object obj, GameLocation location, Farmer who);

		/// <summary>
		/// Get the actual inventory capacity of the object's inventory. New items may be added to the
		/// GetItems() list up until this count.
		/// </summary>
		/// <param name="obj">the object</param>
		/// <param name="location">the map where the object is</param>
		/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
		/// <returns></returns>
		int GetActualCapacity(object obj, GameLocation location, Farmer who);

	}
}
