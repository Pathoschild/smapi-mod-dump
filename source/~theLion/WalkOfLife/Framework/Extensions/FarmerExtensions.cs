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
	public static class FarmerExtensions
	{
		/// <summary>Whether the farmer has a particular profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		public static bool HasProfession(this Farmer farmer, string professionName)
		{
			return Util.Professions.IndexByName.Forward.TryGetValue(professionName, out var professionIndex) &&
				   farmer.professions.Contains(professionIndex);
		}

		/// <summary>Whether the farmer has a particular profession.</summary>
		/// <param name="professionIndex">The index of the profession.</param>
		public static bool HasProfession(this Farmer farmer, int professionIndex)
		{
			return Util.Professions.IndexByName.Contains(professionIndex) &&
				   farmer.professions.Contains(professionIndex);
		}

		/// <summary>Whether the farmer has any of the specified professions.</summary>
		/// <param name="professionNames">Sequence of profession names.</param>
		public static bool HasAnyOfProfessions(this Farmer farmer, params string[] professionNames)
		{
			return professionNames.Any(p =>
				Util.Professions.IndexByName.Forward.TryGetValue(p, out var professionIndex) &&
				farmer.professions.Contains(professionIndex));
		}
	}
}