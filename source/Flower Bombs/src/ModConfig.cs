/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using StardewModdingAPI;

namespace FlowerBombs
{
	public class ModConfig
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		internal static ModConfig Instance { get; private set; }

#pragma warning disable IDE1006

		public bool ClintMudstone { get; set; } = true;

		public bool KentGifts { get; set; } = true;

		public bool LeahRecipe { get; set; } = true;

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
				Helper.Translation.Get ("ClintMudstone.name"),
				Helper.Translation.Get ("ClintMudstone.description"),
				() => Instance.ClintMudstone,
				(bool value) => Instance.ClintMudstone = value);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("KentGifts.name"),
				Helper.Translation.Get ("KentGifts.description"),
				() => Instance.KentGifts,
				(bool value) =>
				{
					Instance.KentGifts = value;
					MailEditor.Invalidate ();
				});

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("LeahRecipe.name"),
				Helper.Translation.Get ("LeahRecipe.description"),
				() => Instance.LeahRecipe,
				(bool value) =>
				{
					Instance.LeahRecipe = value;
					MailEditor.Invalidate ();
				});
		}
	}
}
