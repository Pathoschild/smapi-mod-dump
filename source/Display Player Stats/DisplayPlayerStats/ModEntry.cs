/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FleemUmbleem/DisplayPlayerStats
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ModSandbox
{
	/// <summary>The mod entry point.</summary>
	internal sealed class ModEntry : Mod
	{
		private Texture2D? tileSheet;
		private Farmer player;
		private UpdateHud updateHud;
		public int tickCount = 0;
		ModConstants modConstants = new ModConstants();

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// Load in tilesheet
			tileSheet = helper.ModContent.Load<Texture2D>(Path.Combine(Constants.GamePath + modConstants.cursorsTileSheetPath));

			// Subscribe to the HUD rendering ticks
			helper.Events.Display.RenderingHud += OnRenderingHud;
		}

		private void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			// Initialize player and HUD class
			player = Game1.player;
			updateHud = new UpdateHud();

			// Update the HUD
			if (tileSheet != null)
			{
				updateHud.UpdateHudStatusText(player, tileSheet, e);
			}

			// Log every 180 ticks (roughly 3 seconds)
			//TickLogger();
		}

		public void TickLogger()
		{
			// Only display debug info every few seconds
			tickCount++;
			if (tickCount == 180)
			{
				// Place debug logs here - leaving some commented as they are good reference
				//Monitor.Log($"OnRenderingHudCalled - zoomLevel: {Game1.options.zoomLevel} - WxH: {Game1.viewport.Width}x{Game1.viewport.Height}", LogLevel.Debug);
				//Monitor.Log($"health: {player.health} - maxHealth: {player.maxHealth}", LogLevel.Debug);
				//Monitor.Log($"SafeArea TxB: {testSafeArea.Top}x{testSafeArea.Bottom}", LogLevel.Debug);
				//Monitor.Log($"SafeArea LxR: {testSafeArea.Left}x{testSafeArea.Right}", LogLevel.Debug);
				tickCount = 0;
			}
		}
	}
}