using PredictiveCore;
using StardewModdingAPI;

namespace ScryingOrb
{
	public class ModConfig : IConfig
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		internal static ModConfig Instance { get; private set; }

		public bool InstantRecipe { get; set; } = false;

		public bool UnlimitedUse { get; set; } = false;

		public bool InaccuratePredictions { get; set; } = false;

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
	}
}
