/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/UpgradeablePan
**
*************************************************/

using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyLib;

namespace UpgradablePan
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod, IAssetLoader, IAssetEditor
	{
		public static IMonitor M = null;
		public static Texture2D panIcons = null;
		public static Texture2D panSprites = null;
		public static DynamicMethod actionWhenPurchasedOriginal = null;

		/*********
		** Public methods
		*********/

		/// <summary>Get whether this instance can load the initial version of the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanLoad<T>(IAssetInfo asset)
		{
			// Ensure we can load the new pan graphics.
			return asset.AssetNameEquals(Helper.Content.GetActualAssetKey("assets/pans.png", ContentSource.ModFolder));
		}

		/// <summary>Load a matched asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public T Load<T>(IAssetInfo asset)
		{
			// Load the new pan graphics!
			if (asset.AssetNameEquals(Helper.Content.GetActualAssetKey("assets/pans.png", ContentSource.ModFolder)))
			{
				return (T) Convert.ChangeType(panIcons, typeof(T));
			}

			throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
		}

		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			if (asset.AssetNameEquals("Strings/StringsFromCSFiles") || 
				asset.AssetNameEquals("TileSheets/tools") ||
				asset.AssetNameEquals("Characters/Farmer/farmer_base") ||
				asset.AssetNameEquals("Characters/Farmer/farmer_base_bald") ||
				asset.AssetNameEquals("Characters/Farmer/farmer_girl_base") ||
				asset.AssetNameEquals("Characters/Farmer/farmer_girl_base_bald"))
			{
				return true;
			}

			return false;
		}

		/// <summary>Edit a matched asset.</summary>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
		public void Edit<T>(IAssetData asset)
		{
			// Update the name of the copper pan to just "Pan".
			if (asset.AssetNameEquals("Strings/StringsFromCSFiles"))
			{
				IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
				if (data.Keys.Contains("Pan.cs.14180"))
				{
					data["Pan.cs.14180"] = "Pan";
				}
			}

			// Update the tools tilesheet to include our new pan icons.
			if (asset.AssetNameEquals("TileSheets/tools"))
			{
				var editor = asset.AsImage();
				editor.PatchImage(panIcons, targetArea: new Rectangle(272, 0, 64, 16));
			}

			// Update the farmer tilesheets to include our new pan sprites.
			if (asset.AssetNameEquals("Characters/Farmer/farmer_base") ||
				asset.AssetNameEquals("Characters/Farmer/farmer_base_bald") ||
				asset.AssetNameEquals("Characters/Farmer/farmer_girl_base") ||
				asset.AssetNameEquals("Characters/Farmer/farmer_girl_base_bald"))
			{
				var editor = asset.AsImage();

				// Add the new pan sprites.
				editor.PatchImage(panSprites, targetArea: new Rectangle(96, 656, 192, 16), patchMode: PatchMode.Overlay);

				// Update feature offset arrays to support additional frames.
				int[] featureOffsetsY = new int[126 + 18];
				int[] featureOffsetsX = new int[126 + 18];

				Array.Copy(FarmerRenderer.featureYOffsetPerFrame, featureOffsetsY, FarmerRenderer.featureYOffsetPerFrame.Length);
				featureOffsetsY[123 + 6] = featureOffsetsY[123];
				featureOffsetsY[124 + 6] = featureOffsetsY[124];
				featureOffsetsY[125 + 6] = featureOffsetsY[125];
				featureOffsetsY[123 + 12] = featureOffsetsY[123];
				featureOffsetsY[124 + 12] = featureOffsetsY[124];
				featureOffsetsY[125 + 12] = featureOffsetsY[125];
				featureOffsetsY[123 + 18] = featureOffsetsY[123];
				featureOffsetsY[124 + 18] = featureOffsetsY[124];
				featureOffsetsY[125 + 18] = featureOffsetsY[125];
				FarmerRenderer.featureYOffsetPerFrame = featureOffsetsY;

				Array.Copy(FarmerRenderer.featureXOffsetPerFrame, featureOffsetsX, FarmerRenderer.featureXOffsetPerFrame.Length);
				FarmerRenderer.featureXOffsetPerFrame = featureOffsetsX;
			}
		}

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			M = Monitor;

			// Load the custom pan graphics.
			panIcons = Helper.Content.Load<Texture2D>("assets/pans.png");
			panSprites = Helper.Content.Load<Texture2D>("assets/pan_sprites.png");
			PanHat.panHatTexture = Helper.Content.Load<Texture2D>("assets/pan_hats.png");

			// When a save is loaded, try updating any out-of-date pans.
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

			// Listen for newly-added pans.
			helper.Events.Player.InventoryChanged += OnInventoryChanged;

			Harmony harmony = new Harmony(ModManifest.UniqueID);

			// Patch the blacksmith upgrade inventory to add the pan as an upgradable tool.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Utility), nameof(StardewValley.Utility.getBlacksmithUpgradeStock)),
						  postfix: new HarmonyMethod(typeof(UtilityPatches), nameof(UtilityPatches.getBlacksmithUpgradeStock_Postfix)));

			// Patch the fish shop inventory to remove pans (since they are "proper" tools now).
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Utility), nameof(StardewValley.Utility.getFishShopStock)),
						  postfix: new HarmonyMethod(typeof(UtilityPatches), nameof(UtilityPatches.getFishShopStock_Postfix)));

			// Patch the special pickup/put down functionality that allows pans to be worn as hats.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Utility), nameof(StardewValley.Utility.PerformSpecialItemGrabReplacement)),
						  prefix: new HarmonyMethod(typeof(UtilityPatches), nameof(UtilityPatches.PerformSpecialItemGrabReplacement_Prefix)));
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Utility), nameof(StardewValley.Utility.PerformSpecialItemPlaceReplacement)),
						  prefix: new HarmonyMethod(typeof(UtilityPatches), nameof(UtilityPatches.PerformSpecialItemPlaceReplacement_Prefix)));

			// Patch the inventory's left-click handler to properly support new pans as hats.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Menus.InventoryPage), nameof(StardewValley.Menus.InventoryPage.receiveLeftClick)),
						  prefix: new HarmonyMethod(typeof(InventoryPagePatches), nameof(InventoryPagePatches.receiveLeftClick_Prefix)));

			// Patch the item's can be trashed method to disable throwing pans away.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Item), nameof(StardewValley.Item.canBeTrashed)),
						  prefix: new HarmonyMethod(typeof(ItemPatches), nameof(ItemPatches.canBeTrashed_Prefix)));

			// Get a copy of the actionWhenPurchased method prior to patching so we can call it without causing a stack overflow.
			actionWhenPurchasedOriginal = (DynamicMethod) harmony.Patch(AccessTools.Method(typeof(StardewValley.Item), nameof(StardewValley.Item.actionWhenPurchased)));

			// Patch the tool's upgrade purchase method to allow proper pan upgrading functionality.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Tool), nameof(StardewValley.Tool.actionWhenPurchased)),
						  prefix: new HarmonyMethod(typeof(ToolPatches), nameof(ToolPatches.actionWhenPurchased_Prefix)));

			// Patch the getter and setter for IndexOfMenuItemView to let the new pan upgrade icons show properly.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Tool), "set_IndexOfMenuItemView"),
						  prefix: new HarmonyMethod(typeof(ToolPatches), nameof(ToolPatches.set_IndexOfMenuItemView_Prefix)));
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Tool), "get_IndexOfMenuItemView"),
						  prefix: new HarmonyMethod(typeof(ToolPatches), nameof(ToolPatches.get_IndexOfMenuItemView_Prefix)));

			// Patch the pan's usage method and rolled loot to support new upgrades, and the getOne method for mod compatibility.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Tools.Pan), nameof(StardewValley.Tools.Pan.beginUsing)),
						  prefix: new HarmonyMethod(typeof(PanPatches), nameof(PanPatches.beginUsing_Prefix)));
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Tools.Pan), nameof(StardewValley.Tools.Pan.getPanItems)),
						  prefix: new HarmonyMethod(typeof(PanPatches), nameof(PanPatches.getPanItems_Prefix)));
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Tools.Pan), nameof(StardewValley.Tools.Pan.getOne)),
						  postfix: new HarmonyMethod(typeof(PanPatches), nameof(PanPatches.getOne_Postfix)));

			// Patch the farmer renderer's draw method to draw our new pan animations.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.FarmerRenderer), nameof(StardewValley.FarmerRenderer.draw),
						  new Type[] {typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer)}),
						  prefix:  new HarmonyMethod(typeof(FarmerRendererPatches), nameof(FarmerRendererPatches.draw_Prefix)),
						  postfix: new HarmonyMethod(typeof(FarmerRendererPatches), nameof(FarmerRendererPatches.draw_Postfix)));
			
			helper.ConsoleCommands.Add("spawnpanpoint", "Spawns a \"shiny spot\" at the current location for panning.\n\nUsage: spawnpanpoint", SpawnPanPointCommand);

			// Save handlers to prevent custom objects from being saved to file.
			helper.Events.GameLoop.Saving += (s, e) => makePlaceholderHat();
			helper.Events.GameLoop.Saved += (s, e) => restorePlaceholderHat();
			helper.Events.GameLoop.SaveLoaded += (s, e) => restorePlaceholderHat();
		}

		/// <summary>Replaces all instances of PanHat with a placeholder.</summary>
		private void makePlaceholderHat()
		{
			if (Game1.player?.hat?.Value != null && Game1.player.hat.Value is PanHat panHat)
			{
				Hat placeholder = new Hat(71);
				placeholder.Name = $"PanHat({panHat.UpgradeLevel})";
				Game1.player.hat.Set(placeholder);
			}
		}

		/// <summary>Replaces a placeholder object with its PanHat counterpart.</summary>
		private void restorePlaceholderHat()
		{
			if (Game1.player?.hat?.Value != null)
			{
				string xmlString = Game1.player.hat.Value.Name;
				if (xmlString.StartsWith("PanHat("))
				{
					int upgradeLevel = int.Parse(xmlString.Substring("PanHat(".Length, 1));
					Pan pan = new Pan();
					pan.UpgradeLevel = upgradeLevel;

					PanHat panHat = new PanHat(pan);
					Game1.player.hat.Set(panHat);
				}
			}
		}


		/*********
		** Private methods
		*********/
		/// <summary>Spawns a shiny spot when the command is invoked.</summary>
		/// <param name="command">The name of the command invoked.</param>
		/// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
		private void SpawnPanPointCommand(string command, string[] args)
		{
			Random r = new Random();
			int tries = 50;

			// If there is already a shiny spot, get rid of it so it moves.
			if (!Game1.currentLocation.orePanPoint.Value.Equals(Point.Zero))
			{
				Game1.currentLocation.orePanPoint.Set(Point.Zero);
			}

			// Try to spawn a shiny spot up to ten times.
			while (tries-- >= 0)
			{
				Game1.currentLocation.performOrePanTenMinuteUpdate(r);
				if (!Game1.currentLocation.orePanPoint.Value.Equals(Point.Zero))
				{
					Monitor.Log($"Ore pan point spawned at {Game1.currentLocation.orePanPoint.Value}.", LogLevel.Info);
					break;
				}
			}

			if (Game1.currentLocation.orePanPoint.Value.Equals(Point.Zero))
			{
				Monitor.Log($"Failed to spawn ore pan point!", LogLevel.Warn);
			}
		}

		/*********
		** Private methods
		*********/

		/// <summary>The method called after a save is loaded.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="args">The event arguments.</param>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs args)
		{
			// Update pans to be upgradable.
			Tool pan = Game1.player.getToolFromName("Pan");
			if (pan != null)
			{
				UpdatePan(pan);
			}
		}

		/// <summary>The method called after the player's inventory changes.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="args">The event arguments.</param>
		private void OnInventoryChanged(object sender, InventoryChangedEventArgs args)
		{
			// Try updating new pans.
			foreach (Item item in args.Added)
			{
				UpdatePan(item);
			}
		}

		private void UpdatePan(Item item)
		{
			if (item != null && item is Tool tool && item.Name.Contains("Pan"))
			{
				tool.BaseName = "Pan";
				if (tool.UpgradeLevel == 0)
				{
					tool.UpgradeLevel = 1;
				}
			}
		}
	}
}