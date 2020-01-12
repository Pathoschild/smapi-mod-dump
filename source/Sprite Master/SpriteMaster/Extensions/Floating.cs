using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	public static class Floating {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NearestInt (this float v) {
			return (int)Math.Round(v);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NearestInt (this double v) {
			return (int)Math.Round(v);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NextInt (this float v) {
			return (int)Math.Ceiling(v);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NextInt (this double v) {
			return (int)Math.Ceiling(v);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TruncateInt (this float v) {
			return (int)v;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TruncateInt (this double v) {
			return (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long NearestLong (this float v) {
			return (long)Math.Round(v);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long NearestLong (this double v) {
			return (long)Math.Round(v);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long NextLong (this float v) {
			return (long)Math.Ceiling(v);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long NextLong (this double v) {
			return (long)Math.Ceiling(v);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long TruncateLong (this float v) {
			return (long)v;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long TruncateLong (this double v) {
			return (long)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Saturate (this float v) {
			return v.Clamp(0.0f, 1.0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Saturate (this double v) {
			return v.Clamp(0.0, 1.0);
		}

		public static float Lerp (this float x, float y, float s) {
			return x * (1 - s) + y * s;
		}

		public static double Lerp (this double x, double y, double s) {
			return x * (1 - s) + y * s;
		}
	}
}
