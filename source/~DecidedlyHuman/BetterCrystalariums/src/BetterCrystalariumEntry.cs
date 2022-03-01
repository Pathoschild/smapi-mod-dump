/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using BetterCrystalariums.Helpers;
using BetterCrystalariums.Utilities;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BetterCrystalariums
{
	public class BetterCrystalariumEntry : Mod
	{
		private IModHelper _helper;
		private IMonitor _monitor;
		private Logger _logger;
		private Patches _patches;
		private ModConfig _config;

		public override void Entry(IModHelper helper)
		{
			_helper = helper;
			_monitor = Monitor;
			_logger = new Logger(_monitor);
			_config = _helper.ReadConfig<ModConfig>();
			_patches = new Patches(_monitor, _helper, _logger, _config);

			Harmony harmony = new(ModManifest.UniqueID);

			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.Object),
				nameof(StardewValley.Object.performObjectDropInAction),
				new Type[] { typeof(Item), typeof(bool), typeof(Farmer) }),
			prefix: new HarmonyMethod(typeof(Patches),
				nameof(Patches.ObjectDropIn_Prefix))
				);

			_helper.Events.GameLoop.GameLaunched += GameLaunched;
		}

		private void GameLaunched(object sender, GameLaunchedEventArgs e)
		{
			try
			{
				RegisterWithGmcm();
			}
			catch (Exception ex)
			{
				_logger.Log(_helper.Translation.Get("bettercrystalariums.no-gmcm"));
			}
		}

		private void RegisterWithGmcm()
		{
			GenericModConfigMenuAPI configMenuApi =
				Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

			configMenuApi.RegisterModConfig(ModManifest,
				() => _config = new ModConfig(),
				() => Helper.WriteConfig(_config));

			configMenuApi.RegisterSimpleOption(ModManifest, _helper.Translation.Get("bettercrystalariums.debug-setting-title"),
				_helper.Translation.Get("bettercrystalariums.debug-setting-description"),
				() => _config.DebugMode,
				(bool value) => _config.DebugMode = value);
		}
	}
}
