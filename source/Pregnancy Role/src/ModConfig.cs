/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/pregnancyrole
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace PregnancyRole
{
	public class ModConfig
	{
		internal protected static IModHelper Helper => ModEntry.Instance.Helper;
		internal protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		internal static ModConfig Instance { get; private set; }

#pragma warning disable IDE1006

		public bool ShowPlayerDropdown { get; set; } = true;
		public Point PlayerDropdownOrigin { get; set; } = Point.Zero;

		public bool ShowSpouseDropdown { get; set; } = true;
		public Point SpouseDropdownOrigin { get; set; } = Point.Zero;

		public bool VerboseLogging { get; set; } = false;

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
	}
}
