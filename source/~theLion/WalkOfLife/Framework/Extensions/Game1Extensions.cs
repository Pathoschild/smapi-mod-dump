/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using StardewValley;
using System.Linq;

namespace TheLion.Stardew.Professions.Framework.Extensions
{
	public static class Game1Extensions
	{
		/// <summary>Whether any farmer in the current game session has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="numberOfPlayersWithThisProfession">How many players have this profession.</param>
		public static bool DoesAnyPlayerHaveProfession(this Game1 game1, string professionName, out int numberOfPlayersWithThisProfession)
		{
			if (!Game1.IsMultiplayer)
			{
				if (Game1.player.HasProfession(professionName))
				{
					numberOfPlayersWithThisProfession = 1;
					return true;
				}
			}

			numberOfPlayersWithThisProfession = Game1.getAllFarmers().Count(player => player.isActive() && player.HasProfession(professionName));
			return numberOfPlayersWithThisProfession > 0;
		}
	}
}