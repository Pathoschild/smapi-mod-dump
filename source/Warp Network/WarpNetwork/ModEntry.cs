/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using WarpNetwork.api;
using WarpNetwork.framework;
using WarpNetwork.models;

namespace WarpNetwork
{
	// TODO add icon failsafe
	class ModEntry : Mod
	{
		internal static string AssetPath;
		internal static string LegacyAssetPath = "Data/WarpNetwork";

		//main
		internal static Config config;
		internal static IModHelper helper;
		internal static IMonitor monitor;
		internal static ITranslationHelper i18n;
		public static API api = new();

		public override void Entry(IModHelper helper)
		{
			config = helper.ReadConfig<Config>();
			ModEntry.helper = helper;
			monitor = Monitor;
			i18n = helper.Translation;

			helper.Events.GameLoop.GameLaunched += GameLaunched;

			AssetPath = "Mods/" + ModManifest.UniqueID;
		}

		public void GameLaunched(object sender, GameLaunchedEventArgs ev)
		{
			config.RegisterGMCM(ModManifest);

			var harmony = new Harmony(ModManifest.UniqueID);
			framework.Patches.Patch(harmony);
			framework.ObeliskPatch.Patch(harmony);

			helper.ConsoleCommands.Add(
				"warpnet",
				"Master command for Warp Network mod. Use 'warpnet' or 'warpnet help' to see a list of subcommands.",
				CommandHandler.Main);
			DataPatcher.Init();
			Queries.Register();
			CPIntegration.AddTokens(ModManifest);
		}

		public override object GetApi() => api;
	}
}
