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
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Common.Crafting;

public interface IRecyclable {

	Texture2D GetRecycleTexture(Farmer who, Item? recycledItem, bool fuzzyItems);

	Rectangle GetRecycleSourceRect(Farmer who, Item? recycledItem, bool fuzzyItems);

	string GetRecycleDisplayName(Farmer who, Item? recycledItem, bool fuzzyItems);

	int GetRecycleQuantity(Farmer who, Item? recycledItem, bool fuzzyItems);

	/// <summary>
	/// Check to see if this can be recycled when recycling an <see cref="Item"/>.
	/// </summary>
	/// <param name="who">The player performing the recycle.</param>
	/// <param name="recycledItem">The item being recycled, assuming there is one.</param>
	/// <param name="fuzzyItems">Whether or not fuzzy items, such as categories,
	/// should be recycled.</param>
	bool CanRecycle(Farmer who, Item? recycledItem, bool fuzzyItems);


	/// <summary>
	/// Recycle an <see cref="Item"/> and return an optional enumeration of
	/// items that should be given to the player. Any other modifications, such
	/// as giving the player currency or modifying modData, etc. should be
	/// handled in this method.
	/// </summary>
	/// <param name="who">The player performing the recycle.</param>
	/// <param name="recycledItem">The item being recycled, assuming there is one.</param>
	/// <param name="fuzzyItems">Whether or not fuzzy items, such as categories,
	/// should be recycled.</param>
	IEnumerable<Item>? Recycle(Farmer who, Item? recycledItem, bool fuzzyItems);

	/// <summary>
	/// Return an array of <see cref="Item"/> instances, where the total stack
	/// size is equal to the requested count.
	/// </summary>
	/// <param name="item"></param>
	/// <param name="count"></param>
	public static Item[] GetManyOf(Item item, int count) {
		if (count < 1)
			return Array.Empty<Item>();

		int max = Math.Max(1, item.maximumStackSize());
		int stacks = (int) Math.Ceiling(count / (double) max);

		Item[] result = new Item[stacks];
		for(int i = 0; i < stacks; i++) {
			int quantity = Math.Min(max, count);
			count -= quantity;
			result[i] = item.getOne();
			result[i].Stack = quantity;
		}

		return result;
	}

}
