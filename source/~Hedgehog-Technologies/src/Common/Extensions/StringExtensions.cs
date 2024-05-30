/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

namespace HedgeTech.Common.Extensions
{
	public static class StringExtensions
	{
		public static string FormatWith(this string str, params object[] args)
		{
			return string.Format(str, args);
		}

		public static bool IEquals(this string a, string b)
		{
			if (a is null || b is null) return false;

			return string.Equals(a, b, System.StringComparison.InvariantCultureIgnoreCase);
		}

		public static bool IsNullOrEmpty(this string? str)
		{
			return string.IsNullOrEmpty(str);
		}
	}
}
