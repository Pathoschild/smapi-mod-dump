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
	public struct WorkingInventory : IInventory {

		public object Object { get; }
		public IInventoryProvider Provider { get; }
		public NetMutex Mutex { get; }
		public GameLocation Location { get; }
		public Farmer Player { get; }

		public WorkingInventory(object @object, IInventoryProvider provider, NetMutex mutex, GameLocation location, Farmer player) {
			Object = @object;
			Provider = provider;
			Mutex = mutex;
			Location = location;
			Player = player;
		}

		public bool IsLocked() => Mutex.IsLocked() && Mutex.IsLockHeld();
		public bool IsValid() => Provider.IsValid(Object, Location, Player);
		public bool CanInsertItems() => Provider.CanInsertItems(Object, Location, Player);
		public bool CanExtractItems() => Provider.CanExtractItems(Object, Location, Player);
		public Rectangle? GetMultiTileRegion() => Provider.GetMultiTileRegion(Object, Location, Player);
		public Vector2? GetTilePosition() => Provider.GetTilePosition(Object, Location, Player);
		public IList<Item> GetItems() => Provider.GetItems(Object, Location, Player);
		public void CleanInventory() => Provider.CleanInventory(Object, Location, Player);
		public int GetActualCapacity() => Provider.GetActualCapacity(Object, Location, Player);
	}
}
