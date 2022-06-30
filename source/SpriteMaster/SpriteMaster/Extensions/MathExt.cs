/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types.Fixed;
using System;

namespace SpriteMaster.Extensions;

internal static class MathExt {
	#region Min
	internal static Fixed8 Min(this Fixed8 a, Fixed8 b) => Math.Min(a.Value, b.Value);
	internal static Fixed16 Min(this Fixed16 a, Fixed16 b) => Math.Min(a.Value, b.Value);
	internal static TimeSpan Min(this TimeSpan a, TimeSpan b) => TimeSpan.FromTicks(Math.Min(a.Ticks, b.Ticks));

	internal static byte Min(byte a, byte b, byte c) => Math.Min(a, Math.Min(b, c));
	internal static sbyte Min(sbyte a, sbyte b, sbyte c) => Math.Min(a, Math.Min(b, c));
	internal static ushort Min(ushort a, ushort b, ushort c) => Math.Min(a, Math.Min(b, c));
	internal static short Min(short a, short b, short c) => Math.Min(a, Math.Min(b, c));
	internal static uint Min(uint a, uint b, uint c) => Math.Min(a, Math.Min(b, c));
	internal static int Min(int a, int b, int c) => Math.Min(a, Math.Min(b, c));
	internal static ulong Min(ulong a, ulong b, ulong c) => Math.Min(a, Math.Min(b, c));
	internal static long Min(long a, long b, long c) => Math.Min(a, Math.Min(b, c));
	internal static float Min(float a, float b, float c) => MathF.Min(a, MathF.Min(b, c));
	internal static double Min(double a, double b, double c) => Math.Min(a, Math.Min(b, c));
	internal static Fixed8 Min(Fixed8 a, Fixed8 b, Fixed8 c) => Min(a, Min(b, c));
	internal static Fixed16 Min(Fixed16 a, Fixed16 b, Fixed16 c) => Min(a, Min(b, c));
	#endregion

	#region Max
	internal static Fixed8 Max(this Fixed8 a, Fixed8 b) => Math.Max(a.Value, b.Value);
	internal static Fixed16 Max(this Fixed16 a, Fixed16 b) => Math.Max(a.Value, b.Value);
	internal static TimeSpan Max(this TimeSpan a, TimeSpan b) => TimeSpan.FromTicks(Math.Max(a.Ticks, b.Ticks));

	internal static byte Max(byte a, byte b, byte c) => Math.Max(a, Math.Max(b, c));
	internal static sbyte Max(sbyte a, sbyte b, sbyte c) => Math.Max(a, Math.Max(b, c));
	internal static ushort Max(ushort a, ushort b, ushort c) => Math.Max(a, Math.Max(b, c));
	internal static short Max(short a, short b, short c) => Math.Max(a, Math.Max(b, c));
	internal static uint Max(uint a, uint b, uint c) => Math.Max(a, Math.Max(b, c));
	internal static int Max(int a, int b, int c) => Math.Max(a, Math.Max(b, c));
	internal static ulong Max(ulong a, ulong b, ulong c) => Math.Max(a, Math.Max(b, c));
	internal static long Max(long a, long b, long c) => Math.Max(a, Math.Max(b, c));
	internal static float Max(float a, float b, float c) => MathF.Max(a, MathF.Max(b, c));
	internal static double Max(double a, double b, double c) => Math.Max(a, Math.Max(b, c));
	internal static Fixed8 Max(Fixed8 a, Fixed8 b, Fixed8 c) => Max(a, Max(b, c));
	internal static Fixed16 Max(Fixed16 a, Fixed16 b, Fixed16 c) => Max(a, Max(b, c));
	#endregion

	#region Clamp
	internal static Fixed8 Clamp(this Fixed8 v, Fixed8 min, Fixed8 max) => Math.Clamp(v.Value, min.Value, max.Value);
	internal static Fixed16 Clamp(this Fixed16 v, Fixed16 min, Fixed16 max) => Math.Clamp(v.Value, min.Value, max.Value);

	internal static TimeSpan Clamp(this TimeSpan v, TimeSpan min, TimeSpan max) => TimeSpan.FromTicks(Math.Clamp(v.Ticks, min.Ticks, max.Ticks));
	#endregion

	#region RoundToInt/Long
	internal static int RoundToInt(this float v) => (int)MathF.Round(v);
	internal static int RoundToInt(this double v) => (int)Math.Round(v);

	internal static long RoundToLong(this float v) => (long)MathF.Round(v);
	internal static long RoundToLong(this double v) => (long)Math.Round(v);
	#endregion

	#region Trig
	// This is in .NET 6, but not 5
	internal static (float Sine, float Cosine) SinCos(this float v) {
		var sine = MathF.Sin(v);
		var cosine = MathF.Cos(v);
		return (sine, cosine);
	}

	// This is in .NET 6, but not 5
	internal static (double Sine, double Cosine) SinCos(this double v) {
		var sine = Math.Sin(v);
		var cosine = Math.Cos(v);
		return (sine, cosine);
	}
	#endregion
}
