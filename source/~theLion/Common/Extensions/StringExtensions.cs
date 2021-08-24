/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TheLion.Stardew.Common.Extensions
{
	public static class StringExtensions
	{
		/// <summary>Capitalize the first character in the calling string.</summary>
		public static string FirstCharToUpper(this string s)
		{
			return string.IsNullOrEmpty(s)
				? throw new ArgumentException($"Argument is null or empty.")
				: s.First().ToString().ToUpper() + s.Substring(1);
		}

		/// <summary>Try to parse the calling string to a generic type.</summary>
		/// <param name="val">Parsed <typeparamref name="T"/>-type object if successful, else default.</param>
		/// <returns>True if parse was successful, otherwise false.</returns>
		public static bool TryParse<T>(this string s, out T val)
		{
			var converter = TypeDescriptor.GetConverter(typeof(T));
			if (converter.CanConvertTo(typeof(T)) && converter.CanConvertFrom(typeof(string)))
			{
				val = (T)converter.ConvertFromString(s);
				return true;
			}
			else
			{
				val = default;
				return false;
			}
		}

		/// <summary>Converts a string ID into an 8-digit hash code.</summary>
		public static int Hash(this string s)
		{
			return (int)(Math.Abs(s.GetHashCode()) / Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(s.GetHashCode()))) - 8 + 1));
		}

		/// <summary>Removes invalid file name or path characters from the calling string.</summary>
		public static string RemoveInvalidChars(this string s)
		{
			var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			return new Regex($"[{Regex.Escape(invalidChars)}]").Replace(s, "");
		}
	}
}