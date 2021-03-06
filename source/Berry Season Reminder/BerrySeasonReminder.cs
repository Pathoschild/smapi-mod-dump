/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/BerrySeasonReminder
**
*************************************************/

using System;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using Harmony;

namespace BerrySeasonReminder
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID);

			// Patch the billboard's draw method to add berry icons as necessary.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Menus.Billboard), nameof(StardewValley.Menus.Billboard.draw), new Type[] { typeof(SpriteBatch) }),
						  postfix: new HarmonyMethod(typeof(BillboardPatches), nameof(BillboardPatches.draw_Postfix)));
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Menus.Billboard), nameof(StardewValley.Menus.Billboard.performHoverAction)),
						  postfix: new HarmonyMethod(typeof(BillboardPatches), nameof(BillboardPatches.performHoverAction_Postfix)));
		}
	}
}