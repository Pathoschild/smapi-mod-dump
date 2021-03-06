/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	public static class String {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBlank (this string str) => str == null || str == "";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Reverse (this string str) {
			Contract.AssertNotNull(str);

			unsafe {
				fixed (char* p = str) {
					foreach (int i in 0.To(str.Length / 2)) {
						int endIndex = (str.Length - i) - 1;
						Common.Swap(ref p[i], ref p[endIndex]);
					}
				}
			}

			return str;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Reversed (this string str) {
			Contract.AssertNotNull(str);
			var strArray = str.ToCharArray().Reverse();
			return new string(strArray);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Enquote (this string str, string quote = "\'") {
			if (str.StartsWith(quote) && str.EndsWith(quote)) {
				return str;
			}
			return $"{quote}{str}{quote}";
		}

		private static readonly char[] NewlineChars = new[] { '\n', '\r' };
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> Lines (this string str, bool removeEmpty = false) {
			var strings = str.Split(NewlineChars);
			var validLines = removeEmpty ? strings.Where(l => (l != null && l.Length > 0)) : strings.Where(l => l != null);
			return validLines;
		}
	}
}
