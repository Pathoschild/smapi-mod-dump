using SpriteMaster.Types;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	public static class Range {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Clamp<T> (this T value, T min, T max) where T : IComparable, IComparable<T> {
			return (value.CompareTo(min) < 0) ?
				min :
				(value.CompareTo(max) > 0) ?
					max :
					value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Clamp (this int value, int min, int max) {
			return (value < min) ? min : (value > max) ? max : value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Clamp (this uint value, uint min, uint max) {
			return (value < min) ? min : (value > max) ? max : value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Clamp (this long value, long min, long max) {
			return (value < min) ? min : (value > max) ? max : value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Clamp (this ulong value, ulong min, ulong max) {
			return (value < min) ? min : (value > max) ? max : value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Clamp (this float value, float min, float max) {
			return (value < min) ? min : (value > max) ? max : value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Clamp (this double value, double min, double max) {
			return (value < min) ? min : (value > max) ? max : value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool WithinInclusive<T> (this T value, T min, T max) where T : IComparable, IComparable<T> {
			return (value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool WithinExclusive<T> (this T value, T min, T max) where T : IComparable, IComparable<T> {
			return (value.CompareTo(min) > 0 && value.CompareTo(max) < 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Within<T> (this T value, T min, T max) where T : IComparable, IComparable<T> {
			return WithinInclusive(value, min, max);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> ToInclusive (this int from, int to) {
			if (from >= to) {
				while (from >= to) {
					yield return from--;
				}
			}
			else {
				while (from <= to) {
					yield return from++;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<long> ToInclusive (this int from, long to) {
			if (from >= to) {
				while (from >= to) {
					yield return from--;
				}
			}
			else {
				while (from <= to) {
					yield return from++;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> ToExclusive (this int from, int to) {
			while (from < to) {
				yield return from++;
			}
			while (from > to) {
				yield return from--;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<long> ToExclusive (this int from, long to) {
			while (from < to) {
				yield return from++;
			}
			while (from > to) {
				yield return from--;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> To (this int from, int to) {
			return ToInclusive(from, to);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<long> To (this int from, long to) {
			return ToInclusive(from, to);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> Until (this int from, int to) {
			return ToExclusive(from, to);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<long> Until (this int from, long to) {
			return ToExclusive(from, to);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> For (this int from, int count) {
			return ToExclusive(from, from + count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<long> For (this int from, long count) {
			return ToExclusive(from, from + count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<long> ToInclusive (this long from, long to) {
			if (from < to) {
				while (from <= to) {
					yield return from++;
				}
			}
			else {
				while (from >= to) {
					yield return from--;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<long> ToExclusive (this long from, long to) {
			while (from < to) {
				yield return from++;
			}
			while (from > to) {
				yield return from--;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<long> To (this long from, long to) {
			return ToInclusive(from, to);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<long> Until (this long from, long to) {
			return ToExclusive(from, to);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Bounds ClampTo (this in Bounds source, in Bounds clamp) {
			var result = new Bounds(source.X, source.Y, source.Width, source.Height);

			int leftDiff = clamp.Left - result.Left;
			if (leftDiff > 0) {
				result.X += leftDiff;
				result.Width -= leftDiff;
			}

			int topDiff = clamp.Top - result.Top;
			if (topDiff > 0) {
				result.Y += topDiff;
				result.Height -= topDiff;
			}

			int rightDiff = result.Right - clamp.Right;
			if (rightDiff > 0) {
				result.Width -= rightDiff;
			}

			int bottomDiff = result.Bottom - clamp.Bottom;
			if (bottomDiff > 0) {
				result.Height -= bottomDiff;
			}

			return result;
		}
	}
}
