/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using System.Linq;

namespace Shockah.Kokoro.Stardew
{
	public static class FarmHouseExt
	{
		public static Cellar? GetCellar(this FarmHouse farmhouse)
			=> Game1.getLocationFromName(farmhouse.GetCellarName()) as Cellar;
	}

	public static class CellarExt
	{
		public static FarmHouse? GetFarmHouse(this Cellar cellar)
			=> GameExt.GetAllLocations().OfType<FarmHouse>().FirstOrDefault(fh => fh.GetCellarName() == cellar.Name);
	}
}