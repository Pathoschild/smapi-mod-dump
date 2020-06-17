using StardewModdingAPI;

namespace FlowerBombs
{
	public class ModConfig
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		internal static ModConfig Instance { get; private set; }

		public bool ClintMudstone { get; set; } = true;

		public bool KentGifts { get; set; } = true;

		public bool LeahRecipe { get; set; } = true;

		internal static void Load ()
		{
			Instance = Helper.ReadConfig<ModConfig> ();
		}

		internal static void Save ()
		{
			Helper.WriteConfig (Instance);
		}

		internal static void Reset ()
		{
			Instance = new ModConfig ();
		}

		internal static void SetUpMenu ()
		{
			var api = Helper.ModRegistry.GetApi<GenericModConfigMenu.IApi>
				("spacechase0.GenericModConfigMenu");
			if (api == null)
				return;

			var manifest = ModEntry.Instance.ModManifest;
			api.RegisterModConfig (manifest, Reset, Save);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("ClintMudstone.name"),
				Helper.Translation.Get ("ClintMudstone.description"),
				() => Instance.ClintMudstone,
				(bool value) => Instance.ClintMudstone = value);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("KentGifts.name"),
				Helper.Translation.Get ("KentGifts.description"),
				() => Instance.KentGifts,
				(bool value) => Instance.KentGifts = value);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("LeahRecipe.name"),
				Helper.Translation.Get ("LeahRecipe.description"),
				() => Instance.LeahRecipe,
				(bool value) => Instance.LeahRecipe = value);
		}
	}
}
