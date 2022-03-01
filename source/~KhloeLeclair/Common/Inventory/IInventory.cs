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

namespace Leclair.Stardew.Common.Inventory
{
	public interface IInventory {

		object Object { get; }
		GameLocation Location { get; }
		Farmer Player { get; }
		NetMutex Mutex { get; }


		bool IsLocked();

		bool IsValid();

		bool CanInsertItems();

		bool CanExtractItems();

		Rectangle? GetMultiTileRegion();

		Vector2? GetTilePosition();

		IList<Item> GetItems();

		void CleanInventory();

		int GetActualCapacity();
	}
}
