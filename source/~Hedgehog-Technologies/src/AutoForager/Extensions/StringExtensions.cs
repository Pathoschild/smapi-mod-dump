/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

namespace AutoForager.Extensions
{
	public static class StringExtensions
	{
		public static bool IEquals(this string a, string b)
		{
			return string.Equals(a, b, System.StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}
	}
}
