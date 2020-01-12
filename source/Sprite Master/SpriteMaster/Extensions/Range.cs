using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	using XRectangle = Microsoft.Xna.Framework.Rectangle;
	public static class Range {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Clamp<T> (this T v, T min, T max) where T : IComparable, IComparable<T> {
			if (v.CompareTo(min) < 0)
				return min;
			if (v.CompareTo(max) > 0)
				return max;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Clamp (this int v, int min, int max) {
			if (v < min)
				return min;
			if (v > max)
				return max;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Clamp (this uint v, uint min, uint max) {
			if (v < min)
				return min;
			if (v > max)
				return max;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Clamp (this long v, long min, long max) {
			if (v < min)
				return min;
			if (v > max)
				return max;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Clamp (this ulong v, ulong min, ulong max) {
			if (v < min)
				return min;
			if (v > max)
				return max;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Clamp (this float v, float min, float max) {
			if (v < min)
				return min;
			if (v > max)
				return max;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Clamp (this double v, double min, double max) {
			if (v < min)
				return min;
			if (v > max)
				return max;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool WithinInclusive<T> (this T v, T min, T max) where T : IComparable, IComparable<T> {
			return (v.CompareTo(min) >= 0 && v.CompareTo(max) <= 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool WithinExclusive<T> (this T v, T min, T max) where T : IComparable, IComparable<T> {
			return (v.CompareTo(min) > 0 && v.CompareTo(max) < 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Within<T> (this T v, T min, T max) where T : IComparable, IComparable<T> {
			return WithinInclusive(v, min, max);
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

		public static XRectangle ClampTo (this in XRectangle source, in XRectangle clamp) {
			var result = new XRectangle(source.X, source.Y, source.Width, source.Height);

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Matches (this Texture2D texture, in XRectangle rectangle) {
			return rectangle.X == 0 && rectangle.Y == 0 && rectangle.Width == texture.Width && rectangle.Height == texture.Height;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Matches (this Texture2D texture, in Rectangle rectangle) {
			return rectangle.X == 0 && rectangle.Y == 0 && rectangle.Width == texture.Width && rectangle.Height == texture.Height;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Matches (this in XRectangle rectangle, Texture2D texture) {
			return texture.Matches(rectangle);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Matches (this in Rectangle rectangle, Texture2D texture) {
			return texture.Matches(rectangle);
		}
	}
}
