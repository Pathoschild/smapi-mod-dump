/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/prismaticpride
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System.IO;

namespace PrismaticPride
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		// This instance is shared across screens and uses PerScreen internally.
		public ColorData colorData { get; private set; }

		public int bootsSheetIndex { get; private set; } = -1;
		public int bootsColorIndex { get; private set; } = -1;

		internal HarmonyInstance harmony { get; private set; }
		private JsonAssets.IApi jsonAssets;

		protected static ModConfig Config => ModConfig.Instance;

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			ModConfig.Load ();
			colorData = Helper.Data.ReadJsonFile<ColorData> (Path.Combine ("assets",
				"colors.json"));
			colorData.prepare ();

			// Apply Harmony patches.
			harmony = HarmonyInstance.Create (ModManifest.UniqueID);
			UtilityPatches.Apply ();
			FarmerRendererPatches.Apply ();
			TailoringMenuPatches.Apply ();

			// Handle game events.
			Helper.Events.GameLoop.GameLaunched += onGameLaunched;
			Helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
			Helper.Events.GameLoop.UpdateTicked += onUpdateTicked;
			Helper.Events.Input.ButtonPressed += onButtonPressed;

			// Set up asset loaders/editors.
			Helper.Content.AssetEditors.Add (new ClothingEditor ());
			Helper.Content.AssetEditors.Add (new ShopEditor ());
			Helper.Content.AssetEditors.Add (new TailoringEditor ());
		}

		private void onGameLaunched (object _sender, GameLaunchedEventArgs _e)
		{
			// Set up Json Assets, if it is available.
			jsonAssets = Helper.ModRegistry.GetApi<JsonAssets.IApi> ("spacechase0.JsonAssets");

			// Set up Generic Mod Config Menu, if it is available.
			ModConfig.SetUpMenu ();
		}

		private void onSaveLoaded (object _sender, SaveLoadedEventArgs _e)
		{
			// Load the current color set for the player.
			colorData.loadForPlayer ();

			// Discover the IDs of the Prismatic Boots.
			if (jsonAssets != null)
			{
				bootsSheetIndex = jsonAssets.GetObjectId ("Prismatic Boots");
				if (bootsSheetIndex != -1)
					bootsColorIndex = new Boots (bootsSheetIndex).indexInColorSheet.Value;
			}
		}

		private void onUpdateTicked (object _sender, UpdateTickedEventArgs e)
		{
			// Run every 250ms while a save is loaded.
			if (!Context.IsWorldReady || !e.IsMultipleOf (15))
				return;

			// If a sleeved prismatic shirt is being worn, mark the shirt
			// as dirty on the renderer to force the sleeves to update.
			Clothing shirt = Game1.player.shirtItem.Value;
			if (shirt != null && shirt.isPrismatic.Value &&
				!shirt.GetOtherData ().Contains ("Sleeveless"))
			{
				Game1.player.FarmerRenderer.changeShirt (0);
				Game1.player.FarmerRenderer.changeShirt (Game1.player.shirt.Value);
			}
		}

		private void onButtonPressed (object _sender, ButtonPressedEventArgs e)
		{
			if (!Context.IsWorldReady || !Context.IsPlayerFree || !Config.ApplyColors)
				return;

			// Handle the configured key by opening the color set menu.
			if (e.Button == Config.ColorSetMenuKey)
			{
				Game1.activeClickableMenu = new ColorSetMenu ();
				Helper.Input.Suppress (e.Button);
			}

			// Demonstrate the mod by cycling through the color set with
			// random farmer appearance and random prismatic clothing items.
			// if (e.Button == SButton.Z)
			// {
			// 	Demo.Demonstrate ();
			// 	Helper.Input.Suppress (e.Button);
			// }
		}
	}
}
