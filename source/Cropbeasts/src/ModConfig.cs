/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using BerrybeastFaceType = Cropbeasts.Beasts.Berrybeast.FaceType;

namespace Cropbeasts
{
	public class ModConfig
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		internal static ModConfig Instance { get; private set; }

#pragma warning disable IDE1006

		public bool SpawnOnAnyFarm { get; set; } = false;

		public float DailyChance { get; set; } = 1f;

		public bool AllowSimultaneous { get; set; } = false;

		public int OutdoorSpawnLimit { get; set; } = 5;

		public int IndoorSpawnLimit { get; set; } = 2;

		public int WickedStatueRange { get; set; } = 9;

		public bool WitchFlyovers { get; set; } = true;

		public bool HighContrast { get; set; } = false;

		public bool TrackingArrows { get; set; } = false;

		public bool Testing { get; set; } = false;

		public List<string> ExcludedBeasts { get; } = new List<string> ();

		public BerrybeastFaceType BerrybeastFace { get; set; } =
			BerrybeastFaceType.Random;

		public bool CactusbeastSandblast { get; set; } = true;

		public bool RootbeastHiding { get; set; } = true;

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

			api.RegisterLabel (manifest,
				Helper.Translation.Get ("Spawning.name"),
				null);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("SpawnOnAnyFarm.name"),
				Helper.Translation.Get ("SpawnOnAnyFarm.description"),
				() => Instance.SpawnOnAnyFarm,
				(bool value) => Instance.SpawnOnAnyFarm = value);

			api.RegisterClampedOption (manifest,
				Helper.Translation.Get ("DailyChance.name"),
				Helper.Translation.Get ("DailyChance.description"),
				() => Instance.DailyChance,
				(float value) => Instance.DailyChance = value,
				0f, 1f);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("AllowSimultaneous.name"),
				Helper.Translation.Get ("AllowSimultaneous.description"),
				() => Instance.AllowSimultaneous,
				(bool value) => Instance.AllowSimultaneous = value);

			api.RegisterClampedOption (manifest,
				Helper.Translation.Get ("OutdoorSpawnLimit.name"),
				Helper.Translation.Get ("OutdoorSpawnLimit.description"),
				() => Instance.OutdoorSpawnLimit,
				(int value) => Instance.OutdoorSpawnLimit = value,
				-1, 30);

			api.RegisterClampedOption (manifest,
				Helper.Translation.Get ("IndoorSpawnLimit.name"),
				Helper.Translation.Get ("IndoorSpawnLimit.description"),
				() => Instance.IndoorSpawnLimit,
				(int value) => Instance.IndoorSpawnLimit = value,
				-1, 30);

			api.RegisterClampedOption (manifest,
				Helper.Translation.Get ("WickedStatueRange.name"),
				Helper.Translation.Get ("WickedStatueRange.description"),
				() => Instance.WickedStatueRange,
				(int value) => Instance.WickedStatueRange = value,
				-1, 30);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("WitchFlyovers.name"),
				Helper.Translation.Get ("WitchFlyovers.description"),
				() => Instance.WitchFlyovers,
				(bool value) => Instance.WitchFlyovers = value);

			api.RegisterLabel (manifest,
				Helper.Translation.Get ("Visibility.name"),
				null);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("HighContrast.name"),
				Helper.Translation.Get ("HighContrast.description"),
				() => Instance.HighContrast,
				(bool value) => Instance.HighContrast = value);

			api.RegisterSimpleOption (manifest,
				Helper.Translation.Get ("TrackingArrows.name"),
				Helper.Translation.Get ("TrackingArrows.description"),
				() => Instance.TrackingArrows,
				(bool value) => Instance.TrackingArrows = value);

			api.RegisterSimpleOption (manifest,
				"Testing Features",
				"Activate features intended for testing of the mod.",
				() => Instance.Testing,
				(bool value) => Instance.Testing = value);

			api.RegisterLabel (manifest,
				Helper.Translation.Get ("ExcludedBeasts.name"),
				Helper.Translation.Get ("ExcludedBeasts.description"));

			foreach (string beast in Assets.MonsterEditor.List ())
			{
				var crops = Mappings.GetForBeast (beast)
					.Select ((m) => m.harvestDisplayName);
				api.RegisterSimpleOption (manifest,
					Helper.Translation.Get (beast),
					string.Join (", ", crops),
					() => !Instance.ExcludedBeasts.Contains (beast),
					(bool value) =>
					{
						if (value)
							Instance.ExcludedBeasts.Remove (beast);
						else
							Instance.ExcludedBeasts.Add (beast);
					});

				switch (beast)
				{
				case "Berrybeast":
					Type type = typeof (BerrybeastFaceType);
					Dictionary<string, BerrybeastFaceType> options = new ();
					foreach (BerrybeastFaceType option in Enum.GetValues (type))
					{
						var message = Helper.Translation.Get
							($"BerrybeastFace.{option}");
						options[message.ToString ()] = option;
					}
					api.RegisterChoiceOption (manifest,
						Helper.Translation.Get ("BerrybeastFace.name"),
						Helper.Translation.Get ("BerrybeastFace.description"),
						() => Helper.Translation.Get
							($"BerrybeastFace.{Instance.BerrybeastFace}"),
						(string message) =>
							Instance.BerrybeastFace = options[message],
						options.Keys.ToArray ());
					break;
				case "Cactusbeast":
					api.RegisterSimpleOption (manifest,
						Helper.Translation.Get ("CactusbeastSandblast.name"),
						Helper.Translation.Get ("CactusbeastSandblast.description"),
						() => Instance.CactusbeastSandblast,
						(bool value) => Instance.CactusbeastSandblast = value);
					break;
				case "Rootbeast":
					api.RegisterSimpleOption (manifest,
						Helper.Translation.Get ("RootbeastHiding.name"),
						Helper.Translation.Get ("RootbeastHiding.description"),
						() => Instance.RootbeastHiding,
						(bool value) => Instance.RootbeastHiding = value);
					break;
				}
			}
		}
	}
}
