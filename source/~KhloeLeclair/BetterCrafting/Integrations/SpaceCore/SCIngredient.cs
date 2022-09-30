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

using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

using StardewValley.Objects;
using StardewValley;
using Leclair.Stardew.Common.Inventory;

namespace Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;

public class SCIngredient : IIngredient {

	public readonly object Ingredient;
	private readonly ModEntry Mod;

	private readonly IReflectedProperty<string> DisplayNameProp;
	private readonly IReflectedProperty<Texture2D> TextureProp;
	private readonly IReflectedProperty<Rectangle?> SourceProp;
	private readonly IReflectedProperty<int> QuantityProp;
	private readonly IReflectedMethod GetAmountInListMethod;
	private readonly IReflectedMethod ConsumeMethod;

	public SCIngredient(object ingredient, ModEntry mod) {
		Ingredient = ingredient;
		Mod = mod;

		DisplayNameProp = Mod.Helper.Reflection.GetProperty<string>(Ingredient, "DispayName");
		TextureProp = Mod.Helper.Reflection.GetProperty<Texture2D>(Ingredient, "IconTexture");
		SourceProp = Mod.Helper.Reflection.GetProperty<Rectangle?>(Ingredient, "IconSubrect");
		QuantityProp = Mod.Helper.Reflection.GetProperty<int>(Ingredient, "Quantity");
		GetAmountInListMethod = Mod.Helper.Reflection.GetMethod(Ingredient, "GetAmountInList");
		ConsumeMethod = Mod.Helper.Reflection.GetMethod(Ingredient, "Consume");

		// Ensure we can do stuff.
		DisplayNameProp.GetValue();
		TextureProp.GetValue();
		SourceProp.GetValue();
		QuantityProp.GetValue();
	}

	#region IIngredient

	public bool SupportsQuality => false;

	public string DisplayName => DisplayNameProp.GetValue();

	public Texture2D Texture => TextureProp.GetValue();

	public Rectangle SourceRectangle => SourceProp.GetValue() ?? Texture.Bounds;

	public int Quantity => QuantityProp.GetValue();

	public int GetAvailableQuantity(Farmer who, IList<Item?>? _, IList<IInventory>? inventories, int max_quality) {
		if (who != Game1.player)
			return 0;

		List<Item> items = new();
		items.AddRange(who.Items);

		// Rather than using the provided Item list, we need
		// a list that is only items from chests because
		// SpaceCore ingredient matchers only understand
		// how to consume items from chests.
		if (inventories != null)
			foreach (var inv in inventories) {
				if (inv.Object is Chest chest)
					items.AddRange(chest.items);
			}

		return GetAmountInListMethod.Invoke<int>(items);
	}

	public void Consume(Farmer who, IList<IInventory>? inventories, int max_quality, bool lower_quality_first) {
		// Unfortunately, we're always going to need chests for this
		// due to how SpaceCore is implemented.
		if (who == Game1.player)
			ConsumeMethod.Invoke(GetChests(inventories));
	}

	private static List<Chest> GetChests(IList<IInventory>? inventories) {
		if (inventories is null)
			return new List<Chest>();

		return inventories
			.Where(val => val.Object is Chest)
			.Select(val => (Chest) val.Object)
			.ToList();
	}

	#endregion

}
