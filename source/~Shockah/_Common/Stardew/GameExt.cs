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
using System.Collections.Generic;
using System.Linq;

namespace Shockah.CommonModCode.Stardew
{
    public enum MultiplayerMode { SinglePlayer, Client, Server }
	
	public static class GameExt
	{
		public static MultiplayerMode GetMultiplayerMode()
			=> (MultiplayerMode)Game1.multiplayerMode;

		public static Farmer GetHostPlayer()
			=> Game1.getAllFarmers().First(p => p.slotCanHost);

		public static IReadOnlyList<GameLocation> GetAllLocations()
		{
			List<GameLocation> locations = new();
			Utility.ForAllLocations(l => locations.Add(l));
			return locations;
		}
	}
}
