/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/portabletv
**
*************************************************/

using StardewModdingAPI;

namespace PortableTV
{
	public class ModConfig
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		internal static ModConfig Instance { get; private set; }

#pragma warning disable IDE1006

		public bool Animate { get; set; } = true;

		public bool Static { get; set; } = true;

		public bool Music { get; set; } = true;

		public SButton ActivateKey { get; set; } = SButton.R;

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
				Helper.Translation.Get ("Animate.name"),
				Helper.Translation.Get ("Animate.description"),
				() => Instance.Animate,
				(bool value) => Instance.Animate = value);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("Static.name"),
				Helper.Translation.Get ("Static.description"),
				() => Instance.Static,
				(bool value) => Instance.Static = value);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("Music.name"),
				Helper.Translation.Get ("Music.description"),
				() => Instance.Music,
				(bool value) => Instance.Music = value);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("ActivateKey.name"),
				Helper.Translation.Get ("ActivateKey.description"),
				() => Instance.ActivateKey,
				(SButton value) => Instance.ActivateKey = value);
		}
	}
}
