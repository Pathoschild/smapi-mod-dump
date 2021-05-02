/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods/-/tree/master/WalkOfLife
**
*************************************************/

using System;
using System.Linq;

namespace TheLion.Common
{
	public static class StringExtensions
	{
		/// <summary>Capitalize the first character in the calling string.</summary>
		public static string FirstCharToUpper(this string s)
		{
			return s switch
			{
				null => throw new ArgumentNullException(nameof(s)),
				"" => throw new ArgumentException($"{nameof(s)} cannot be empty."),
				_ => s.First().ToString().ToUpper() + s.Substring(1)
			};
		}
	}
}