/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/MessyCrops
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MessyCrops
{
	public class ModEntry : Mod
	{
		internal static ITranslationHelper i18n => helper.Translation;
		internal static IMonitor monitor;
		internal static IModHelper helper;
		internal static Harmony harmony;
		internal static string ModID;
		internal static Config config;

		public override void Entry(IModHelper helper)
		{
			Monitor.Log("Starting up...", LogLevel.Debug);

			monitor = Monitor;
			ModEntry.helper = Helper;
			harmony = new(ModManifest.UniqueID);
			ModID = ModManifest.UniqueID;
			config = helper.ReadConfig<Config>();

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

		public void OnGameLaunched(object s, GameLaunchedEventArgs ev)
		{
			config.RegisterConfig(ModManifest);
			CropPatch.Setup();
		}
	}
}
