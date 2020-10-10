/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StardewConfigFramework/StardewConfigMenu
**
*************************************************/

//#define DEBUG

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewConfigMenu {
	public class ModEntry: Mod {

		private MenuController MenuController;

		/*********
    ** Public methods
    *********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			MenuController = new MenuController(helper, Monitor);
			StardewConfigFrameworkLoaded();
		}

		public override object GetApi() {
			return MenuController;
		}

		/*********
    ** Private methods
    *********/
		/// <summary>The method invoked when the game is opened.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void StardewConfigFrameworkLoaded() {
			Monitor.Log($"StardewConfigFramework Loaded");
		}
	}
}
