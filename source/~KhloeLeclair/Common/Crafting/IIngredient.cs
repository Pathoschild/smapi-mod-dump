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

using Leclair.Stardew.Common.Inventory;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Common.Crafting {
	public interface IIngredient {

		// Support Level
		bool SupportsQuality { get; }

		// Display
		string DisplayName { get; }
		Texture2D Texture { get; }
		Rectangle SourceRectangle { get; }

		// Quantity
		int Quantity { get; }

		int GetAvailableQuantity(Farmer who, IList<Item> items, IList<IInventory> inventories, int max_quality);

		void Consume(Farmer who, IList<IInventory> inventories, int max_quality, bool low_quality_first);

	}
}
