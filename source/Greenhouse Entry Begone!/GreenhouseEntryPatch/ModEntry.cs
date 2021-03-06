/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GreenhouseEntryPatch
**
*************************************************/

using Harmony; // el diavolo
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using System;
using System.Reflection;

namespace GreenhouseEntryPatch
{
	public interface IGenericModConfigMenuAPI
	{
		void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
		void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
	}

	public class Config
	{
		public bool HideTiles { get; set; } = true;
		public bool HideShadow { get; set; } = true;
	}

	public class ModEntry : Mod
	{
		internal static ModEntry Instance;
		internal Config Config;
		internal ITranslationHelper i18n => Helper.Translation;

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();
			Helper.Events.GameLoop.GameLaunched += this.GameLoopOnGameLaunched;
			this.ApplyPatches();
		}

		private void GameLoopOnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			this.RegisterGenericModConfigMenuPage();
		}

		private void ApplyPatches()
		{
			HarmonyInstance harmony = HarmonyInstance.Create(Helper.ModRegistry.ModID);
			harmony.Patch(
				original: AccessTools.Method(typeof(GreenhouseBuilding), "DrawEntranceTiles"),
				prefix: new HarmonyMethod(this.GetType(), nameof(Greenhouse_DrawTiles_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(GreenhouseBuilding), "drawShadow"),
				prefix: new HarmonyMethod(this.GetType(), nameof(Greenhouse_DrawShadow_Prefix)));
		}

		private void RegisterGenericModConfigMenuPage()
		{
			IGenericModConfigMenuAPI api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
			if (api == null)
				return;

			api.RegisterModConfig(ModManifest,
				revertToDefault: () => Config = new Config(),
				saveToFile: () => Helper.WriteConfig(Config));
			foreach (PropertyInfo property in Config.GetType().GetProperties())
			{
				api.RegisterSimpleOption(ModManifest,
					optionName: i18n.Get("config." + property.Name.ToLower() + ".name"),
					optionDesc: null,
					optionGet: () => (bool) property.GetValue(Config),
					optionSet: (bool value) => property.SetValue(Config, value));
			}
		}

		public static bool Greenhouse_DrawTiles_Prefix()
		{
			return !Instance.Config.HideTiles;
		}

		public static bool Greenhouse_DrawShadow_Prefix()
		{
			return !Instance.Config.HideShadow;
		}
	}
}
