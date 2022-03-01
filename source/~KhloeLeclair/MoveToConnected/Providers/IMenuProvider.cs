/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

using Leclair.Stardew.Common.UI.Overlay;

namespace Leclair.Stardew.MoveToConnected.Providers {
	public interface IMenuProvider<out M> where M : IClickableMenu {

		/// <summary>
		/// Determine whether or not we can show our menu additions on
		/// a specific menu. This method is not responsible for determining
		/// whether or not there are connected inventories, only whether
		/// we can display our UI elements.
		/// </summary>
		/// <param name="menu">The current menu</param>
		/// <param name="who">The current player</param>
		/// <returns></returns>
		bool IsValid(IClickableMenu menu, Farmer who);

		IOverlay CreateOverlay(IClickableMenu menu, Farmer who);

	}
}
