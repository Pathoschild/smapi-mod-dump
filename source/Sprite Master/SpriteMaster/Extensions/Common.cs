/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	public static class Common {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ConditionalSet<T> (this ref T obj, bool conditional, in T value) where T : struct {
			if (conditional) {
				obj = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ConditionalSet<T> (this ref T obj, in T? value) where T : struct {
			if (value.HasValue) {
				obj = value.Value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WeakReference<T> MakeWeak<T> (this T obj) where T : class => new WeakReference<T>(obj);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ClampDimension (this int value) => Math.Min(value, Config.ClampDimension);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2I ClampDimension (this Vector2I value) => value.Min(Config.ClampDimension);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Swap<T> (ref T l, ref T r) {
			var temp = l;
			l = r;
			r = temp;
		}
	}
}
