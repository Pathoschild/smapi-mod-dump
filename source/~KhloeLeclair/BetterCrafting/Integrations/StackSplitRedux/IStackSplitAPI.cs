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

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;

namespace StackSplitRedux {
	public interface IStackSplitAPI {
		bool TryRegisterMenu(Type menuType);

		/// <summary>
		/// Register a new menu type with StackSplitRedux with support for
		/// an InventoryMenu, along with support for arbitrary clicks via
		/// callbacks.
		/// 
		/// If inventoryGetter and hoveredItemFieldGetter both exist and return
		/// non-null values, SSR will initialize its standard inventory handler
		/// support.
		/// 
		/// stackChecker is called with the menu instance and a Point representing
		/// the mouse coordinates when the user triggers SSR. If your func returns
		/// null, nothing will happen.
		/// 
		/// The return tuple contains the initial value to display, and an Action
		/// to be called when the user confirms the value they want. The action
		/// will be called with a success boolean and the user's input as an int.
		/// </summary>
		/// <example>
		/// <code>
		/// RegisterBasicMenu(
		///     typeof(MyMenu),
		///     menu => (menu as MyMenu)?.inventory,
		///     menu => menu is MyMenu ? Mod.Helper.Reflection.GetField<Item>(menu, "hoverItem") : null,
		///     null,
		///     (menu, point) => {
		///         if (menu is not MyMenu mm)
		///             return null;
		///         if (!mm.CanSplit(point.X, point.Y))
		///             return null;
		///         return new Tuple<int, Action<bool, int>>(5, (success, amount) => {
		///             mm.PerformSplit(point.X, point.Y, amount);
		///         });
		///     }
		/// );
		/// </code>
		/// </example>
		/// <param name="menuType">The class of the menu to be handled</param>
		/// <param name="inventoryGetter">Function to get a menu's child InventoryMenu</param>
		/// <param name="hoveredItemFieldGetter">Function to get a menu's hovered item field</param>
		/// <param name="heldItemFieldGetter">Function to get a menu's held item field. If this is null, Game1.player.CursorSlotItem will be used instead.</param>
		/// <param name="stackChecker"></param>
		/// <returns></returns>
		bool RegisterBasicMenu(
			Type menuType,
			Func<IClickableMenu, InventoryMenu> inventoryGetter,
			Func<IClickableMenu, IReflectedField<Item>> hoveredItemFieldGetter,
			Func<IClickableMenu, IReflectedField<Item>> heldItemFieldGetter,
			Func<IClickableMenu, Point, Tuple<int, Action<bool, int>>> stackChecker
		);

	}
}
