using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	public static class Floating {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NearestInt (this float v) => (int)Math.Round(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NearestInt (this double v) => (int)Math.Round(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NextInt (this float v) => (int)Math.Ceiling(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NextInt (this double v) => (int)Math.Ceiling(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TruncateInt (this float v) => (int)v;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TruncateInt (this double v) => (int)v;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long NearestLong (this float v) => (long)Math.Round(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long NearestLong (this double v) => (long)Math.Round(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long NextLong (this float v) => (long)Math.Ceiling(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long NextLong (this double v) => (long)Math.Ceiling(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long TruncateLong (this float v) => (long)v;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long TruncateLong (this double v) => (long)v;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Saturate (this float v) => v.Clamp(0.0f, 1.0f);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Saturate (this double v) => v.Clamp(0.0, 1.0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Lerp (this float x, float y, float s) => x * (1.0f - s) + y * s;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Lerp (this double x, double y, double s) => x * (1.0 - s) + y * s;
	}
}
