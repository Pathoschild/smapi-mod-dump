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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	public static class Floating {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NearestInt (this float v) => (int)Math.Round(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NearestInt (this double v) => (int)Math.Round(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I NearestInt (this Vector2 v) => Vector2I.From(v.X.NearestInt(), v.Y.NearestInt());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NextInt (this float v) => (int)Math.Ceiling(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int NextInt (this double v) => (int)Math.Ceiling(v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I NextInt (this Vector2 v) => Vector2I.From(v.X.NextInt(), v.Y.NextInt());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TruncateInt (this float v) => (int)v;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TruncateInt (this double v) => (int)v;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I TruncateInt (this Vector2 v) => Vector2I.From(v.X.TruncateInt(), v.Y.TruncateInt());

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
		public static Vector2 Saturate (this Vector2 v) => new Vector2(v.X.Saturate(), v.Y.Saturate());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Lerp (this float s, float x, float y) => x * (1.0f - s) + y * s;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Lerp (this float s, float x, double y) => unchecked((float)(x * (1.0f - s) + y * s));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Lerp (this double s, double x, float y) => x * (1.0 - s) + y * s;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Lerp (this double s, double x, double y) => x * (1.0 - s) + y * s;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Lerp (this float s, Vector2 x, Vector2 y) => new Vector2(s.Lerp(x.X, y.X), s.Lerp(x.Y, y.Y));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Lerp (this double s, Vector2 x, Vector2 y) => new Vector2((float)s.Lerp((double)x.X, (double)y.X), (float)s.Lerp((double)x.Y, (double)y.Y));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Lerp (this Vector2 s, Vector2 x, Vector2 y) => new Vector2(s.X.Lerp(x.X, y.X), s.Y.Lerp(x.Y, y.Y));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Swap (this Vector2 v) => new Vector2(v.Y, v.X);
	}
}
