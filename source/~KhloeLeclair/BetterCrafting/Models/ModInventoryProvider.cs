/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Leclair.Stardew.Common.Inventory;

using StardewValley;
using StardewValley.Network;

namespace Leclair.Stardew.BetterCrafting.Models {
	public class ModInventoryProvider : IInventoryProvider  {

		private readonly Func<object, GameLocation, Farmer, bool> canExtractItems;
		private readonly Func<object, GameLocation, Farmer, bool> canInsertItems;
		private readonly Action<object, GameLocation, Farmer> cleanInventory;
		private readonly Func<object, GameLocation, Farmer, int> getActualCapacity;
		private readonly Func<object, GameLocation, Farmer, IList<Item>> getItems;
		private readonly Func<object, GameLocation, Farmer, Rectangle?> getMultiTileRegion;
		private readonly Func<object, GameLocation, Farmer, Vector2?> getTilePosition;
		private readonly Func<object, GameLocation, Farmer, NetMutex> getMutex;
		private readonly Func<object, GameLocation, Farmer, bool> isValid;

		public ModInventoryProvider(Func<object, GameLocation, Farmer, bool> canExtractItems, Func<object, GameLocation, Farmer, bool> canInsertItems, Action<object, GameLocation, Farmer> cleanInventory, Func<object, GameLocation, Farmer, int> getActualCapacity, Func<object, GameLocation, Farmer, IList<Item>> getItems, Func<object, GameLocation, Farmer, Rectangle?> getMultiTileRegion, Func<object, GameLocation, Farmer, Vector2?> getTilePosition, Func<object, GameLocation, Farmer, NetMutex> getMutex, Func<object, GameLocation, Farmer, bool> isValid) {
			this.canExtractItems = canExtractItems;
			this.canInsertItems = canInsertItems;
			this.cleanInventory = cleanInventory;
			this.getActualCapacity = getActualCapacity;
			this.getItems = getItems;
			this.getMultiTileRegion = getMultiTileRegion;
			this.getTilePosition = getTilePosition;
			this.getMutex = getMutex;
			this.isValid = isValid;
		}

		public bool CanExtractItems(object obj, GameLocation location, Farmer who) {
			return canExtractItems?.Invoke(obj, location, who) ?? false;
		}

		public bool CanInsertItems(object obj, GameLocation location, Farmer who) {
			return canInsertItems?.Invoke(obj, location, who) ?? false;
		}

		public void CleanInventory(object obj, GameLocation location, Farmer who) {
			cleanInventory?.Invoke(obj, location, who);
		}

		public int GetActualCapacity(object obj, GameLocation location, Farmer who) {
			return getActualCapacity?.Invoke(obj, location, who) ?? 0;
		}

		public IList<Item> GetItems(object obj, GameLocation location, Farmer who) {
			return getItems?.Invoke(obj, location, who);
		}

		public Rectangle? GetMultiTileRegion(object obj, GameLocation location, Farmer who) {
			return getMultiTileRegion?.Invoke(obj, location, who);
		}

		public NetMutex GetMutex(object obj, GameLocation location, Farmer who) {
			return getMutex?.Invoke(obj, location, who);
		}

		public Vector2? GetTilePosition(object obj, GameLocation location, Farmer who) {
			return getTilePosition?.Invoke(obj, location, who);
		}

		public bool IsValid(object obj, GameLocation location, Farmer who) {
			return isValid?.Invoke(obj, location, who) ?? false;
		}
	}
}
