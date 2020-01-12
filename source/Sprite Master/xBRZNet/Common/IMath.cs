using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SpriteMaster.xBRZ.Common {
	public static class IMath {
		[DebuggerStepThrough]
		private ref struct Argument<T> where T : unmanaged {
			public readonly T Value;
			public readonly string Name;

			[DebuggerStepThrough, DebuggerHidden(), MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Argument (T value, string name) {
				Value = value;
				Name = name;
			}

			[DebuggerStepThrough, DebuggerHidden(), MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static implicit operator T (in Argument<T> arg) {
				return arg.Value;
			}
		}

		[DebuggerStepThrough, DebuggerHidden(), MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Argument<T> AsArg<T> (this T value, string name) where T : unmanaged {
			return new Argument<T>(value, name);
		}

		[DebuggerStepThrough, DebuggerHidden(), MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string GetCheckNotNegativeMessage<T> (in Argument<T> argument, string memberName) where T : unmanaged {
			return memberName != null ?
				$"Argument {argument.Name} with value {argument.Value} from method {memberName} was less than zero" :
				$"Argument {argument.Name} with value {argument.Value} was less than zero";
		}

		[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void CheckNotNegative (
			this in Argument<int> argument,
			[CallerMemberName] string memberName = null
		) {
			if (argument < 0) {
				throw new ArgumentOutOfRangeException(GetCheckNotNegativeMessage(argument, memberName));
			}
		}

		[Conditional("DEBUG"), DebuggerStepThrough, DebuggerHidden(), MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void CheckNotNegative (
			this in Argument<long> argument,
			[CallerMemberName] string memberName = null
		) {
			if (argument < 0) {
				throw new ArgumentOutOfRangeException(GetCheckNotNegativeMessage(argument, memberName));
			}
		}

		// Precalculate square roots.
		private const int SQRT_MIN = 0;
		private const int SQRT_MAX = 100;
		private static readonly int[] SQRT_TABLE = new int[SQRT_MAX - SQRT_MIN];
		static IMath () {
			for (int i = 0; i < SQRT_TABLE.Length; ++i) {
				SQRT_TABLE[i] = (int)Math.Sqrt(i + SQRT_MIN);
			}
		}

		// Truncating Square Root
		internal static int Sqrt (this int value) {
			value.AsArg(nameof(value)).CheckNotNegative();

			if (value >= SQRT_MIN && value < SQRT_MAX) {
				return SQRT_TABLE[value];
			}

			return unchecked((int)Math.Sqrt(value));
		}

		// Truncating Square Root
		internal static uint Sqrt (this uint value) {
			if (value >= SQRT_MIN && value < SQRT_MAX) {
				return unchecked((uint)SQRT_TABLE[value]);
			}

			return unchecked((uint)Math.Sqrt(value));
		}

		// Truncating Square Root
		internal static long Sqrt (this long value) {
			value.AsArg(nameof(value)).CheckNotNegative();

			if (value >= SQRT_MIN && value < SQRT_MAX) {
				return (long)SQRT_TABLE[value];
			}

			return unchecked((long)Math.Sqrt(value));
		}

		// Truncating Square Root
		internal static ulong Sqrt (this ulong value) {
			if (value >= SQRT_MIN && value < SQRT_MAX) {
				return unchecked((ulong)SQRT_TABLE[value]);
			}

			return unchecked((ulong)Math.Sqrt(value));
		}

		internal static int Widen (this int value) {
			return value * 0x101;
		}

		internal static uint Widen (this uint value) {
			return value * 0x101U;
		}

		internal static long Widen (this long value) {
			return value * 0x101L;
		}

		internal static ulong Widen (this ulong value) {
			return value * 0x101UL;
		}

		internal static int Narrow (this int value) {
			return value / 0x101;
		}

		internal static uint Narrow (this uint value) {
			return value / 0x101U;
		}

		internal static long Narrow (this long value) {
			return value / 0x101L;
		}

		internal static ulong Narrow (this ulong value) {
			return value / 0x101UL;
		}

		internal static uint asUnsigned(this int value) {
			return unchecked((uint)value);
		}

		internal static ulong asUnsigned (this long value) {
			return unchecked((ulong)value);
		}

		internal static int asSigned (this uint value) {
			return unchecked((int)value);
		}

		internal static long asSigned (this ulong value) {
			return unchecked((long)value);
		}

		// actually should be in a floating point lib

		// Square Root
		internal static float Sqrt (this float v) {
			return (float)Math.Sqrt(v);
		}

		// Square Root
		internal static double Sqrt (this double v) {
			return (float)Math.Sqrt(v);
		}

		// Square
		internal static float Square (this float v) {
			return v * v;
		}

		// Square
		internal static double Square (this double v) {
			return v * v;
		}

		// Linear Interpolation
		internal static float Lerp (this float x, float y, float s) {
			return x * (1 - s) + y * s;
		}

		// Linear Interpolation
		internal static float Lerp (this double x, float y, float s) {
			return (float)(x * (1 - s) + y * s);
		}

		// Linear Interpolation
		internal static float Lerp (this float x, double y, float s) {
			return (float)(x * (1 - s) + y * s);
		}

		// Linear Interpolation
		internal static float Lerp (this float x, float y, double s) {
			return (float)(x * (1 - s) + y * s);
		}

		// Linear Interpolation
		internal static double Lerp (this double x, double y, double s) {
			return x * (1 - s) + y * s;
		}
	}
}
