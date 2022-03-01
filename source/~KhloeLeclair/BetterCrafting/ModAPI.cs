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
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Inventory;
using Leclair.Stardew.Common.Crafting;

using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;

namespace Leclair.Stardew.BetterCrafting {

	public interface IBetterCrafting {

		bool OpenCraftingMenu(
			bool cooking,
			IList<Chest> containers = null,
			GameLocation location = null,
			Vector2? position = null,
			bool silent_open = false,
			IList<string> listed_recipes = null
		);

		bool OpenCraftingMenu(
			bool cooking,
			IList<object> containers = null,
			GameLocation location = null,
			Vector2? position = null,
			Rectangle? area = null,
			bool silent_open = false,
			bool discover_containers = true,
			IList<string> listed_recipes = null
		);

		/// <summary>
		/// Try to open the Better Crafting menu. This may fail if there is another
		/// menu open that cannot be replaced.
		///
		/// If opening the menu from an object in the world, such as a workbench,
		/// its location and tile position can be provided for automatic detection
		/// of nearby chests.
		///
		/// Better Crafting has its own handling of mutexes, so please do not worry
		/// about locking Chests before handing them off to the menu.
		///
		/// When discovering additional containers, Better Crafting scans all tiles
		/// around each of its existing known containers. If a location and position
		/// for the menu source is provided, the tiles around that position will
		/// be scanned as well.
		///
		/// Discovery depends on the user's settings, though at a minimum a 3x3 area
		/// will be scanned to mimic the scanning radius of the vanilla workbench.
		/// </summary>
		/// <param name="cooking">If true, open the cooking menu. If false, open the crafting menu.</param>
		/// <param name="silent_open">If true, do not make a sound upon opening the menu.</param>
		/// <param name="location">The map the associated object is in, or null if there is no object</param>
		/// <param name="position">The tile position the associated object is at, or null if there is no object</param>
		/// <param name="area">The tile area the associated object covers, or null if there is no object or if the object only covers a single tile</param>
		/// <param name="discover_containers">If true, attempt to discover additional material containers.</param>
		/// <param name="containers">An optional list of containers to draw extra crafting materials from.</param>
		/// <param name="listed_recipes">An optional list of recipes by name. If provided, only these recipes will be listed in the crafting menu.</param>
		/// <returns>Whether or not the menu was opened successfully</returns>
		bool OpenCraftingMenu(
			bool cooking,
			bool silent_open = false,
			GameLocation location = null,
			Vector2? position = null,
			Rectangle? area = null,
			bool discover_containers = true,
			IList<Tuple<object, GameLocation>> containers = null,
			IList<string> listed_recipes = null
		);

		/// <summary>
		/// Return the Better Crafting menu's type. In case you want to do
		/// spooky stuff to it, I guess.
		/// </summary>
		/// <returns>The BetterCraftingMenu type.</returns>
		Type GetMenuType();

		/// <summary>
		/// Register a recipe provider with Better Crafting. Calling this
		/// will also invalidate the recipe cache.
		///
		/// If the recipe provider was already registered, this does nothing.
		/// </summary>
		/// <param name="provider">The recipe provider to add</param>
		void AddRecipeProvider(IRecipeProvider provider);

		/// <summary>
		/// Unregister a recipe provider. Calling this will also invalidate
		/// the recipe cache.
		///
		/// If the recipe provider was not registered, this does nothing.
		/// </summary>
		/// <param name="provider">The recipe provider to remove</param>
		void RemoveRecipeProvider(IRecipeProvider provider);

		/// <summary>
		/// Invalidate the recipe cache. You should call this if your recipe
		/// provider ever adds new recipes after registering it.
		/// </summary>
		void InvalidateRecipeCache();

		/// <summary>
		/// Get all known recipes from all providers.
		/// </summary>
		/// <param name="cooking">If true, return cooking recipes. If false, return crafting recipes.</param>
		/// <returns>A collection of the recipes.</returns>
		IReadOnlyCollection<IRecipe> GetRecipes(bool cooking);

		void RegisterInventoryProvider(Type type, IInventoryProvider provider);

		void UnregisterInventoryProvider(Type type);

		void RegisterInventoryProvider(
			Type type,
			Func<object, GameLocation, Farmer, bool> isValid,
			Func<object, GameLocation, Farmer, bool> canExtractItems,
			Func<object, GameLocation, Farmer, bool> canInsertItems,
			Func<object, GameLocation, Farmer, NetMutex> getMutex,
			Func<object, GameLocation, Farmer, int> getActualCapacity,
			Func<object, GameLocation, Farmer, IList<Item>> getItems,
			Action<object, GameLocation, Farmer> cleanInventory,
			Func<object, GameLocation, Farmer, Rectangle?> getMultiTileRegion,
			Func<object, GameLocation, Farmer, Vector2?> getTilePosition
		);
	}


	public class ModAPI : IBetterCrafting {

		private readonly ModEntry Mod;

		public ModAPI(ModEntry mod) {
			Mod = mod;
		}

		// Opening Menu

		public bool OpenCraftingMenu(
			bool cooking,
			IList<Chest> containers = null,
			GameLocation location = null,
			Vector2? position = null,
			bool silent_open = false,
			IList<string> listed_recipes = null
		) {

			return OpenCraftingMenu(
				cooking: cooking,
				containers: containers?.ToList<object>(),
				location: location,
				position: position,
				silent_open: silent_open,
				discover_containers: false,
				listed_recipes: listed_recipes
			);
		}

		public bool OpenCraftingMenu(
			bool cooking,
			IList<object> containers = null,
			GameLocation location = null,
			Vector2? position = null,
			Rectangle? area = null,
			bool silent_open = false,
			bool discover_containers = true,
			IList<string> listed_recipes = null
		) {
			if (listed_recipes == null)
				listed_recipes = cooking ?
					Mod.intCCStation.GetCookingRecipes() :
					Mod.intCCStation.GetCraftingRecipes();

			var menu = Game1.activeClickableMenu;
			if (menu != null) {
				if (!menu.readyToClose())
					return false;

				CommonHelper.YeetMenu(menu);
				Game1.exitActiveMenu();
			}

			Game1.activeClickableMenu = Menus.BetterCraftingPage.Open(
				Mod,
				location,
				position,
				area,
				cooking: cooking,
				standalone_menu: true,
				material_containers: containers,
				silent_open: silent_open,
				discover_containers: discover_containers,
				listed_recipes: listed_recipes
			);

			return true;
		}

		public bool OpenCraftingMenu(
			bool cooking,
			bool silent_open = false,
			GameLocation location = null,
			Vector2? position = null,
			Rectangle? area = null,
			bool discover_containers = true,
			IList<Tuple<object, GameLocation>> containers = null,
			IList<string> listed_recipes = null
		) {
			if (listed_recipes == null)
				listed_recipes = cooking ?
					Mod.intCCStation.GetCookingRecipes() :
					Mod.intCCStation.GetCraftingRecipes();

			var menu = Game1.activeClickableMenu;
			if (menu != null) {
				if (!menu.readyToClose())
					return false;

				CommonHelper.YeetMenu(menu);
				Game1.exitActiveMenu();
			}

			Game1.activeClickableMenu = Menus.BetterCraftingPage.Open(
				Mod,
				location,
				position,
				area,
				cooking: cooking,
				standalone_menu: true,
				material_containers: containers?.Select(val => new LocatedInventory(val.Item1, val.Item2)).ToList(),
				silent_open: silent_open,
				discover_containers: discover_containers,
				listed_recipes: listed_recipes
			);

			return true;
		}

		public Type GetMenuType() {
			return typeof(Menus.BetterCraftingPage);
		}

		// IRecipeProvider Management

		public void AddRecipeProvider(IRecipeProvider provider) {
			Mod.Recipes.AddProvider(provider);
		}

		public void RemoveRecipeProvider(IRecipeProvider provider) {
			Mod.Recipes.RemoveProvider(provider);
		}

		// Recipe Management

		public void InvalidateRecipeCache() {
			Mod.Recipes.Invalidate();
		}

		public IReadOnlyCollection<IRecipe> GetRecipes(bool cooking) {
			return Mod.Recipes.GetRecipes(cooking).AsReadOnly();
		}

		// IInventoryProvider Management

		public void RegisterInventoryProvider(Type type, IInventoryProvider provider) {
			Mod.RegisterInventoryProvider(type, provider);
		}

		public void UnregisterInventoryProvider(Type type) {
			Mod.UnregisterInventoryProvider(type);
		}

		public void RegisterInventoryProvider(
			Type type,
			Func<object, GameLocation, Farmer, bool> isValid,
			Func<object, GameLocation, Farmer, bool> canExtractItems,
			Func<object, GameLocation, Farmer, bool> canInsertItems,
			Func<object, GameLocation, Farmer, NetMutex> getMutex,
			Func<object, GameLocation, Farmer, int> getActualCapacity,
			Func<object, GameLocation, Farmer, IList<Item>> getItems,
			Action<object, GameLocation, Farmer> cleanInventory,
			Func<object, GameLocation, Farmer, Rectangle?> getMultiTileRegion,
			Func<object, GameLocation, Farmer, Vector2?> getTilePosition
		) {
			

			var provider = new Models.ModInventoryProvider(
				canExtractItems: canExtractItems,
				canInsertItems: canInsertItems,
				cleanInventory: cleanInventory,
				getActualCapacity: getActualCapacity,
				getItems: getItems,
				getMultiTileRegion: getMultiTileRegion,
				getTilePosition: getTilePosition,
				getMutex: getMutex,
				isValid: isValid
			);

			Mod.RegisterInventoryProvider(type, provider);
		}
	}
}
