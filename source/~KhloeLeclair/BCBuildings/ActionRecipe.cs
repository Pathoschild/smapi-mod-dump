/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System.Collections.Generic;

using Leclair.Stardew.BetterCrafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Locations;

namespace Leclair.Stardew.BCBuildings;

public enum ActionType {
	Build,
	Upgrade,
	Move,
	Paint,
	Demolish
};

public class ActionRecipe : IRecipe {

	public static readonly Dictionary<ActionType, Rectangle> Sources = new() {
		[ActionType.Move] = new Rectangle(257, 284, 16, 16),
		[ActionType.Paint] = new Rectangle(80, 208, 16, 16),
		[ActionType.Demolish] = new Rectangle(348, 372, 17, 17)
	};

	public readonly ModEntry Mod;
	public readonly ActionType Action;

	public ActionRecipe(ActionType action, ModEntry mod) {
		Mod = mod;
		Action = action;
		Name = $"buildaction:{Action}";

		List<IIngredient> ingredients = new();

		Ingredients = ingredients.ToArray();
	}

	// Identity

	public int SortValue { get; }
	public string Name { get; }
	public string DisplayName {
		get {
			if (Action == ActionType.Move)
				return Game1.content.LoadString(@"Strings\UI:Carpenter_MoveBuildings");
			if (Action == ActionType.Demolish)
				return Game1.content.LoadString(@"Strings\UI:Carpenter_Demolish");
			if (Action == ActionType.Paint)
				return Game1.content.LoadString(@"Strings\UI:Carpenter_PaintBuildings");

			return Action.ToString();
		}
	}

	public string? Description => null;

	public virtual bool HasRecipe(Farmer who) {
		return true;
	}

	public virtual int GetTimesCrafted(Farmer who) {
		return 0;
	}

	public CraftingRecipe? CraftingRecipe => null;

	// Display

	public Texture2D Texture => Action == ActionType.Paint ? Game1.mouseCursors2 : Game1.mouseCursors;

	public Rectangle SourceRectangle => Sources.ContainsKey(Action) ? Sources[Action] : Rectangle.Empty;

	public int GridHeight => 1;
	public int GridWidth => 1;

	// Cost

	public int QuantityPerCraft => 1;
	public IIngredient[] Ingredients { get; }


	// Creation

	public bool Stackable => false;

	public Item? CreateItem() {
		return null;
	}

	public bool CanCraft(Farmer who) {
		return who.currentLocation is BuildableGameLocation;
	}

	public string? GetTooltipExtra(Farmer who) {
		if (who.currentLocation is BuildableGameLocation)
			return null;

		return I18n.Error_NotBuildable();
	}

	public void PerformCraft(IPerformCraftEvent evt) {

		var menu = new BuildMenu(null, Action, evt, Mod);
		var old_menu = Game1.activeClickableMenu;

		Game1.activeClickableMenu = menu;

		menu.exitFunction = () => {
			Game1.activeClickableMenu = old_menu;
			evt.Cancel();
		};
	}

}
