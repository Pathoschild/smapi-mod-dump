/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/PredictiveMods
**
*************************************************/

using PredictiveCore;
using StardewModdingAPI;

namespace PublicAccessTV
{
	public class ModConfig : IConfig
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		internal static ModConfig Instance { get; private set; }

#pragma warning disable IDE1006

		public bool InaccuratePredictions { get; set; } = false;

		public bool BypassFriendships { get; set; } = false;

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
				(bool value) =>
				{
					Instance.InaccuratePredictions = value;
					if (Context.IsWorldReady)
						ModEntry.Instance.updateChannels ();
				});

			api.RegisterLabel (manifest,
				Helper.Translation.Get ("Cheats.name"),
				null);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("BypassFriendships.name"),
				Helper.Translation.Get ("BypassFriendships.description"),
				() => Instance.BypassFriendships,
				(bool value) =>
				{
					Instance.BypassFriendships = value;
					if (Context.IsWorldReady)
						ModEntry.Instance.updateChannels ();
				});
		}
	}
}
