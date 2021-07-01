/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/prismaticpride
**
*************************************************/

using System.Linq;
using StardewModdingAPI;

namespace PrismaticPride
{
	public class ModConfig
	{
		internal protected static IModHelper Helper => ModEntry.Instance.Helper;
		internal protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		internal protected static ColorData ColorData => ModEntry.Instance.colorData;

		internal static ModConfig Instance { get; private set; }

#pragma warning disable IDE1006

		public string DefaultColorSet { get; set; } = "progress";

		public SButton ColorSetMenuKey { get; set; } = SButton.U;

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

			api.SetDefaultIngameOptinValue (manifest, false);
			var sets = ColorData.sets.Values;
			api.RegisterChoiceOption (manifest,
				Helper.Translation.Get ("DefaultColorSet.name"),
				Helper.Translation.Get ("DefaultColorSet.description"),
				() => ColorData.sets[Instance.DefaultColorSet].displayName,
				(string value) => Instance.DefaultColorSet =
					sets.First ((set) => set.displayName == value).key,
				sets.Select ((set) => set.displayName).ToArray ());

			api.SetDefaultIngameOptinValue (manifest, true);
			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("ColorSetMenuKey.name"),
				Helper.Translation.Get ("ColorSetMenuKey.description"),
				() => Instance.ColorSetMenuKey,
				(SButton value) => Instance.ColorSetMenuKey = value);
		}
	}
}
