/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework;
using SpriteMaster.Types;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	public static class Range {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Clamp<T> (this T value, T min, T max) where T : IComparable, IComparable<T> =>
			(value.CompareTo(min) < 0) ? min : (value.CompareTo(max) > 0) ? max : value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Clamp (this int value, int min, int max) => (value < min) ? min : (value > max) ? max : value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Clamp (this uint value, uint min, uint max) => (value < min) ? min : (value > max) ? max : value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Clamp (this long value, long min, long max) => (value < min) ? min : (value > max) ? max : value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Clamp (this ulong value, ulong min, ulong max) => (value < min) ? min : (value > max) ? max : value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Clamp (this float value, float min, float max) => (value < min) ? min : (value > max) ? max : value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Clamp (this double value, double min, double max) => (value < min) ? min : (value > max) ? max : value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Clamp (this Vector2 value, float min, float max) => new Vector2(value.X.Clamp(min, max), value.Y.Clamp(min, max));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Clamp (this Vector2 value, double min, double max) => value.Clamp((float)min, (float)max);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Clamp (this Vector2 value, Vector2 min, Vector2 max) => new Vector2(value.X.Clamp(min.X, max.X), value.Y.Clamp(min.Y, max.Y));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Min (this Vector2 value, float min) => new Vector2(Math.Min(value.X, min), Math.Min(value.Y, min));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Min (this Vector2 value, double min) => new Vector2(Math.Min(value.X, (float)min), Math.Min(value.Y, (float)min));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Min (this Vector2 value, Vector2 min) => new Vector2(Math.Min(value.X, min.X), Math.Min(value.Y, min.Y));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Max (this Vector2 value, float max) => new Vector2(Math.Max(value.X, max), Math.Max(value.Y, max));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Max (this Vector2 value, double max) => new Vector2(Math.Max(value.X, (float)max), Math.Max(value.Y, (float)max));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Max (this Vector2 value, Vector2 max) => new Vector2(Math.Max(value.X, max.X), Math.Max(value.Y, max.Y));

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
