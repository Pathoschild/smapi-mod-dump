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
using System.Linq;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;

namespace Leclair.Stardew.Common.Inventory {
	public class WorkbenchProvider : BaseInventoryProvider<Workbench> {

		public override bool CanExtractItems(Workbench obj, GameLocation location, Farmer who) {
			return false;
		}

		public override bool CanInsertItems(Workbench obj, GameLocation location, Farmer who) {
			return false;
		}

		public override void CleanInventory(Workbench obj, GameLocation location, Farmer who) {
			
		}

		public override int GetActualCapacity(Workbench obj, GameLocation location, Farmer who) {
			return 0;
		}

		public override IList<Item> GetItems(Workbench obj, GameLocation location, Farmer who) {
			return null;
		}

		public override Rectangle? GetMultiTileRegion(Workbench obj, GameLocation location, Farmer who) {
			return null;
		}

		public override NetMutex GetMutex(Workbench obj, GameLocation location, Farmer who) {
			return obj.mutex;
		}

		public override Vector2? GetTilePosition(Workbench obj, GameLocation location, Farmer who) {
			return obj.TileLocation;
		}

		public override bool IsValid(Workbench obj, GameLocation location, Farmer who) {
			return true;
		}
	}
}
