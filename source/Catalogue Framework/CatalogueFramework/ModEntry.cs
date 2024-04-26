/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/Catalogue-Framework
**
*************************************************/

using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace CatalogueFramework
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
		private static IMonitor? monitor;

		public static readonly Dictionary<string, string> shops = new();
		public static readonly Dictionary<string, string> shop_origins = new();

		public static void log(string message, LogLevel log_level = LogLevel.Info)
		{
			if (monitor != null)
				monitor.Log(message, log_level);
			
		}

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
			monitor = Monitor;

			helper.Events.GameLoop.GameLaunched += on_game_launched;
			
			var harmony = new Harmony(ModManifest.UniqueID);
			patch_furniture_checkForAction(harmony);
        }

		private void patch_furniture_checkForAction(Harmony harmony)
		{
			// fetching Furniture.checkForAction MethodInfo
			MethodInfo? original_method = typeof(Furniture)
				.GetMethods()
				.Where(x => x.Name == "checkForAction")
				.Where(x => x.DeclaringType != null
					&& x.DeclaringType.Name == "Furniture")
				.FirstOrDefault();
			
			harmony.Patch(
				original: original_method,
				prefix: new HarmonyMethod(
					typeof(FurniturePatches),
					nameof(FurniturePatches.checkForAction_prefix)
				)
			);
		}

		/// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void on_game_launched(object? sender, GameLaunchedEventArgs e)
		{
			log("Loading Custom Catalogue Content Packs...", LogLevel.Debug);
			foreach (IContentPack pack in Helper.ContentPacks.GetOwned())
			{
				string mod_id = pack.Manifest.UniqueID;
				string mod_name = pack.Manifest.Name;
				log($"Loading Custom Catalogue of {mod_name}...", LogLevel.Debug);

				Dictionary<string, string> catalogue_data;
				try
				{
					catalogue_data = pack.ModContent.Load
						<Dictionary<string, string>>
						("catalogue.json");
				}
				catch (ContentLoadException ex)
				{
					log($"Could not load catalogue_data : {ex}", LogLevel.Error);
					continue;
				}
				catch (Exception ex)
				{
					log($"An error occured : {ex}", LogLevel.Error);
					continue;
				}

				foreach ((string key, string value) in catalogue_data)
				{
					if (shops.ContainsKey(key))
					{
						log(
							$"Furniture with id {key} is already used by {shop_origins[key]}, skipping catalogue.",
							LogLevel.Warn
						);
						continue;
					}
					shops[key] = value;
					shop_origins[key] = mod_id;
				}
			}

			log("Finished loading Custom Catalogue Content Packs.", LogLevel.Debug);
		}
    }
}