/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using PredictiveCore;
using StardewModdingAPI;

namespace ScryingOrb
{
	public class ModConfig : IConfig
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		internal static ModConfig Instance { get; private set; }

#pragma warning disable IDE1006

		public bool InaccuratePredictions { get; set; } = false;

		public bool InstantRecipe { get; set; } = false;

		public bool UnlimitedUse { get; set; } = false;

		public SButton ActivateKey { get; set; } = SButton.None;

#pragma warning restore IDE1006

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
			api.SetDefaultIngameOptinValue (manifest, true);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("InaccuratePredictions.name"),
				Helper.Translation.Get ("InaccuratePredictions.description"),
				() => Instance.InaccuratePredictions,
				(bool value) => Instance.InaccuratePredictions = value);

			api.RegisterLabel (manifest,
				Helper.Translation.Get ("Cheats.name"),
				null);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("InstantRecipe.name"),
				Helper.Translation.Get ("InstantRecipe.description"),
				() => Instance.InstantRecipe,
				(bool value) =>
				{
					Instance.InstantRecipe = value;
					if (Context.IsWorldReady)
						ModEntry.Instance.checkRecipe ();
				});

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("UnlimitedUse.name"),
				Helper.Translation.Get ("UnlimitedUse.description"),
				() => Instance.UnlimitedUse,
				(bool value) => Instance.UnlimitedUse = value);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("ActivateKey.name"),
				Helper.Translation.Get ("ActivateKey.description"),
				() => Instance.ActivateKey,
				(SButton value) => Instance.ActivateKey = value);
		}
	}
}
