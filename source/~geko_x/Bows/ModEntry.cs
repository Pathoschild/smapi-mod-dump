/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/geko_x/stardew-mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using SObject = StardewValley.Object;

using PyTK.Types;
using PyTK.Extensions;
using PyTK.CustomElementHandler;

using ContentPatcher;
using System.Collections.Generic;
using System.Linq;

namespace GekosBows {
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod, IAssetEditor {

		public static Mod INSTANCE;
		public static IModHelper modhelper;
		public static ITranslationHelper i18n;

		internal ObjectArrow objectArrow;
		internal ToolBow trainingBow;
		internal ToolBow shortbow;
		internal ToolBow compoundBow;
		internal ToolBow galaxyBow;

		public static bool sveInstalled = false;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			INSTANCE = this;
			modhelper = helper;
			i18n = helper.Translation;

			Monitor.Log("Mod Entry", LogLevel.Trace);

			modhelper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

		private void OnGameLaunched(object sender,  GameLaunchedEventArgs e) {

			Monitor.Log("Init items", LogLevel.Trace);

			objectArrow = new ObjectArrow();
			objectArrow.Init();

			trainingBow = new ToolBow(0, "allbow");
			trainingBow.Init("Training Bow");

			shortbow = new ToolBow(1, "allbow");
			shortbow.Init("Short bow");

			compoundBow = new ToolBow(2, "allbow");
			compoundBow.Init("Compound Bow");

			galaxyBow = new ToolBow(3, "allbow");
			galaxyBow.Init("Galaxy Bow");

			// Mod intergrations
			sveInstalled = modhelper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP");

			// Invalidate cache once everything is loaded
			modhelper.Content.InvalidateCache("Data/NPCGiftTastes");
		}

		/*
		 * IAssetEditor
		 */

		public bool CanEdit<T>(IAssetInfo asset) {
			if (asset.AssetNameEquals("Data/NPCGiftTastes"))
				return true;

			return false;
		}

		public void Edit<T>(IAssetData asset) {

			if (!Context.IsGameLaunched)
				return;

			if (asset.AssetNameEquals("Data/NPCGiftTastes")) {
				IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

				string d = data["Universal_Dislike"];
				data["Universal_Dislike"] = addItemToUniversalTaste(d, objectArrow.itemID);

				d = data["Abigail"];
				data["Abigail"] = addItemIDToNPCTaste(d, objectArrow.itemID, Constants.TASTE_LIKE);

				if(ModEntry.sveInstalled) {
					d = data["Marlon"];
					data["Marlon"] = addItemIDToNPCTaste(d, objectArrow.itemID, Constants.TASTE_LOVE);
				}
			}
		}

		private string addItemToUniversalTaste(string originalTaste, int itemID) {
			if (!originalTaste.Contains(itemID.ToString())) {
				originalTaste += $" {itemID}";
			}

			return originalTaste;
		}

		private string addItemIDToNPCTaste(string oringalTaste, int itemID, int taste) {

			var tasteData = oringalTaste.Split('/');
			if (!tasteData[taste].Contains(itemID.ToString())) {
				tasteData[taste] += $" {itemID}";
			}

			return string.Join("/", tasteData);
		}
	}
}