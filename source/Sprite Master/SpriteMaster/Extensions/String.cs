using SpriteMaster.Types;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	public static class String {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBlank (this string str) {
			return str == null || str == "";
		}

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
	}
}
