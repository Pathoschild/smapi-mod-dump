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

namespace SpriteMaster.Extensions;

static class Floating {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int NearestInt(this float v) => (int)Math.Round(v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int NearestInt(this double v) => (int)Math.Round(v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I NearestInt(this Vector2 v) => Vector2I.From(v.X.NearestInt(), v.Y.NearestInt());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int NextInt(this float v) => (int)Math.Ceiling(v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int NextInt(this double v) => (int)Math.Ceiling(v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I NextInt(this Vector2 v) => Vector2I.From(v.X.NextInt(), v.Y.NextInt());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int TruncateInt(this float v) => (int)v;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int TruncateInt(this double v) => (int)v;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I TruncateInt(this Vector2 v) => Vector2I.From(v.X.TruncateInt(), v.Y.TruncateInt());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long NearestLong(this float v) => (long)Math.Round(v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long NearestLong(this double v) => (long)Math.Round(v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long NextLong(this float v) => (long)Math.Ceiling(v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long NextLong(this double v) => (long)Math.Ceiling(v);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long TruncateLong(this float v) => (long)v;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static long TruncateLong(this double v) => (long)v;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static float Saturate(this float v) => v.Clamp(0.0f, 1.0f);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double Saturate(this double v) => v.Clamp(0.0, 1.0);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Saturate(this Vector2 v) => new(v.X.Saturate(), v.Y.Saturate());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static float Lerp(this float s, float x, float y) => x * (1.0f - s) + y * s;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static float Lerp(this float s, float x, double y) => (float)(x * (1.0f - s) + y * s);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double Lerp(this double s, double x, float y) => x * (1.0 - s) + y * s;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static double Lerp(this double s, double x, double y) => x * (1.0 - s) + y * s;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Lerp(this float s, Vector2 x, Vector2 y) => new(s.Lerp(x.X, y.X), s.Lerp(x.Y, y.Y));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Lerp(this double s, Vector2 x, Vector2 y) => new((float)s.Lerp((double)x.X, (double)y.X), (float)s.Lerp((double)x.Y, (double)y.Y));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Lerp(this Vector2 s, Vector2 x, Vector2 y) => new(s.X.Lerp(x.X, y.X), s.Y.Lerp(x.Y, y.Y));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2 Swap(this Vector2 v) => new(v.Y, v.X);
}
