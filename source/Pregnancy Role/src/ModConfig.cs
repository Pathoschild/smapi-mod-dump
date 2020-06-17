using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace PregnancyRole
{
	public class ModConfig
	{
		internal protected static IModHelper Helper => ModEntry.Instance.Helper;
		internal protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		internal static ModConfig Instance { get; private set; }

		public bool ShowPlayerDropdown { get; set; } = true;
		public Point PlayerDropdownOrigin { get; set; } = Point.Zero;

		public bool ShowSpouseDropdown { get; set; } = true;
		public Point SpouseDropdownOrigin { get; set; } = Point.Zero;

		public bool VerboseLogging { get; set; } =
#if DEBUG
			true
#else
			false
#endif
;

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
