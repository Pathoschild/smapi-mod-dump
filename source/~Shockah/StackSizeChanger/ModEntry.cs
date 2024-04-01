/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SObject = StardewValley.Object;

namespace Shockah.StackSizeChanger
{
	public class ModEntry : Mod
	{
		private static ModEntry Instance = null!;
		internal ModConfig Config { get; private set; } = null!;

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;

			var harmony = new Harmony(ModManifest.UniqueID);
			harmony.TryPatchVirtual(
				monitor: Monitor,
				original: () => AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.maximumStackSize)),
				postfix: new HarmonyMethod(typeof(ModEntry), nameof(SObject_maximumStackSize_Postfix))
			);
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			SetupConfig();
		}

		private void SetupConfig()
		{
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;
			GMCMI18nHelper helper = new(api, ModManifest, Helper.Translation);

			api.Register(
				ModManifest,
				reset: () => Config = new(),
				save: () => Helper.WriteConfig(Config)
			);

			helper.AddNumberOption("config.size", () => Config.Size, min: 1);
		}

		private static void SObject_maximumStackSize_Postfix(ref int __result)
		{
			if (__result > 1)
				__result = Instance.Config.Size;
		}
	}
}