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
	public static class Int32Extensions
	{
		/// <summary>Raise a number to an integer power.</summary>
		/// <param name="exp">Positive integer exponent.</param>
		public static int Pow(this int num, int exp)
		{
			return Enumerable
				.Repeat(num, exp)
				.Aggregate(1, (a, b) => a * b);
		}
	}
}