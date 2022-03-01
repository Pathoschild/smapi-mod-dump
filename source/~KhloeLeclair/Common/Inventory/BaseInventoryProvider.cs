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
	public abstract class BaseInventoryProvider<T> : IInventoryProvider where T : class {
		public bool CanExtractItems(object obj, GameLocation location, Farmer who) {
			return obj is T tobj && CanExtractItems(tobj, location, who);
		}

		public abstract bool CanExtractItems(T obj, GameLocation location, Farmer who);

		public bool CanInsertItems(object obj, GameLocation location, Farmer who) {
			return obj is T tobj && CanInsertItems(tobj, location, who);
		}

		public abstract bool CanInsertItems(T obj, GameLocation location, Farmer who);

		public void CleanInventory(object obj, GameLocation location, Farmer who) {
			if (obj is T tobj)
				CleanInventory(tobj, location, who);
		}

		public abstract void CleanInventory(T obj, GameLocation location, Farmer who);

		public int GetActualCapacity(object obj, GameLocation location, Farmer who) {
			return obj is T tobj ? GetActualCapacity(tobj, location, who) : 0;
		}

		public abstract int GetActualCapacity(T obj, GameLocation location, Farmer who);

		public IList<Item> GetItems(object obj, GameLocation location, Farmer who) {
			return obj is T tobj ? GetItems(tobj, location, who) : null;
		}

		public abstract IList<Item> GetItems(T obj, GameLocation location, Farmer who);

		public Rectangle? GetMultiTileRegion(object obj, GameLocation location, Farmer who) {
			return obj is T tobj ? GetMultiTileRegion(tobj, location, who) : null;
		}

		public abstract Rectangle? GetMultiTileRegion(T obj, GameLocation location, Farmer who);

		public NetMutex GetMutex(object obj, GameLocation location, Farmer who) {
			return obj is T tobj ? GetMutex(tobj, location, who) : null;
		}

		public abstract NetMutex GetMutex(T obj, GameLocation location, Farmer who);

		public Vector2? GetTilePosition(object obj, GameLocation location, Farmer who) {
			return obj is T tobj ? GetTilePosition(tobj, location, who) : null;
		}

		public abstract Vector2? GetTilePosition(T obj, GameLocation location, Farmer who);

		public bool IsValid(object obj, GameLocation location, Farmer who) {
			return obj is T tobj && IsValid(tobj, location, who);
		}

		public abstract bool IsValid(T obj, GameLocation location, Farmer who);
	}
}
