using System;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using AnimalHouse = StardewValley.AnimalHouse;
using FarmAnimal = StardewValley.FarmAnimal;
using SObject = StardewValley.Object;

namespace MayonnaisePlusPlus
{
	public class ModEntry : Mod
	{
		public static IMonitor MOD_MONITOR;
		internal static Loader _data;
		internal static IJsonAssetApi _jsonAssets;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			MOD_MONITOR = Monitor;
			helper.Events.GameLoop.GameLaunched += onGameLaunched;
		}

		/// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void onGameLaunched(object sender, GameLaunchedEventArgs e) {
			if (_data != null) return;
			_data = new Loader(Helper);
			
			var harmony = HarmonyInstance.Create("Xirsoi.MayoMod");

			harmony.Patch(
					original: AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
					prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.PerformObjectDropInAction))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
				prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.FarmAnimalDayUpdate))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalHouse), nameof(AnimalHouse.addNewHatchedAnimal)),
				prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.AddHatchedAnimal))
			);

			_jsonAssets = Helper.ModRegistry.GetApi<IJsonAssetApi>("spacechase0.JsonAssets");
			_jsonAssets.LoadAssets(Helper.DirectoryPath);
			_jsonAssets.IdsAssigned += (object s, EventArgs ea) => {
				foreach (var item in _jsonAssets.GetAllObjectIds()) {
					Loader.DATA.Add(item.Key, item.Value);
				}
			};
		}
	}
}