/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Linq;

namespace TheLion.Common
{
	public static class GeneralExtensions
	{
		/// <summary>Determine if the calling object is equivalent to any of the objects in a sequence.</summary>
		/// <param name="collection">A sequence of objects.</param>
		public static bool AnyOf<T>(this T source, params T[] collection)
		{
			return collection.Contains(source);
		}
	}
}