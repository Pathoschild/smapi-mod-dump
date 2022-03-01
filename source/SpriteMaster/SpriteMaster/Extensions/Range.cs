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

namespace SpriteMaster.Extensions;

static class Range {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T Clamp<T>(this T value, T min, T max) where T : IComparable, IComparable<T> =>
		(value.CompareTo(min) < 0) ? min : (value.CompareTo(max) > 0) ? max : value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Clamp(this int value, int min, int max) => (value < min) ? min : (value > max) ? max : value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static uint Clamp(this uint value, uint min, uint max) => (value < min) ? min : (value > max) ? max : value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long Clamp(this long value, long min, long max) => (value < min) ? min : (value > max) ? max : value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Clamp(this ulong value, ulong min, ulong max) => (value < min) ? min : (value > max) ? max : value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static float Clamp(this float value, float min, float max) => (value < min) ? min : (value > max) ? max : value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double Clamp(this double value, double min, double max) => (value < min) ? min : (value > max) ? max : value;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Clamp(this Vector2 value, float min, float max) => new(value.X.Clamp(min, max), value.Y.Clamp(min, max));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Clamp(this Vector2 value, double min, double max) => value.Clamp((float)min, (float)max);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max) => new(value.X.Clamp(min.X, max.X), value.Y.Clamp(min.Y, max.Y));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Min(this Vector2 value, float min) => new(MathF.Min(value.X, min), MathF.Min(value.Y, min));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Min(this Vector2 value, double min) => new(MathF.Min(value.X, (float)min), MathF.Min(value.Y, (float)min));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Min(this Vector2 value, Vector2 min) => new(MathF.Min(value.X, min.X), MathF.Min(value.Y, min.Y));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Max(this Vector2 value, float max) => new(MathF.Max(value.X, max), MathF.Max(value.Y, max));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Max(this Vector2 value, double max) => new(MathF.Max(value.X, (float)max), MathF.Max(value.Y, (float)max));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Max(this Vector2 value, Vector2 max) => new(MathF.Max(value.X, max.X), MathF.Max(value.Y, max.Y));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool WithinInclusive<T>(this T value, T min, T max) where T : IComparable, IComparable<T> => (value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool WithinExclusive<T>(this T value, T min, T max) where T : IComparable, IComparable<T> => (value.CompareTo(min) > 0 && value.CompareTo(max) < 0);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool Within<T>(this T value, T min, T max) where T : IComparable, IComparable<T> => WithinInclusive(value, min, max);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool WithinInclusive(this int value, int min, int max) => value >= min && value <= max;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool WithinExclusive(this int value, int min, int max) => value > min && value < max;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool Within(this int value, int min, int max) => WithinInclusive(value, min, max);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool WithinInclusive(this float value, float min, float max) => value >= min && value <= max;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool WithinExclusive(this float value, float min, float max) => value > min && value < max;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool Within(this float value, float min, float max) => WithinInclusive(value, min, max);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool WithinInclusive(this double value, double min, double max) => value >= min && value <= max;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool WithinExclusive(this double value, double min, double max) => value > min && value < max;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool Within(this double value, double min, double max) => WithinInclusive(value, min, max);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<int> ToInclusive(this int from, int to) {
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<long> ToInclusive(this int from, long to) {
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<int> ToExclusive(this int from, int to) {
		while (from < to) {
			yield return from++;
		}
		while (from > to) {
			yield return from--;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<long> ToExclusive(this int from, long to) {
		while (from < to) {
			yield return from++;
		}
		while (from > to) {
			yield return from--;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<int> To(this int from, int to) {
		return ToInclusive(from, to);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<long> To(this int from, long to) {
		return ToInclusive(from, to);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<int> Until(this int from, int to) {
		return ToExclusive(from, to);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<long> Until(this int from, long to) {
		return ToExclusive(from, to);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<int> For(this int from, int count) {
		return ToExclusive(from, from + count);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<long> For(this int from, long count) {
		return ToExclusive(from, from + count);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<long> ToInclusive(this long from, long to) {
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<long> ToExclusive(this long from, long to) {
		while (from < to) {
			yield return from++;
		}
		while (from > to) {
			yield return from--;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<long> To(this long from, long to) {
		return ToInclusive(from, to);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static IEnumerable<long> Until(this long from, long to) {
		return ToExclusive(from, to);
	}
}
