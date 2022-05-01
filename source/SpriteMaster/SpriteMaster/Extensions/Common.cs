/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Configuration;
using SpriteMaster.Types;

using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

static class Common {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void ConditionalSet<T>(this ref T obj, bool conditional, in T value) where T : struct {
		if (conditional) {
			obj = value;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void ConditionalSet<T>(this ref T obj, in T? value) where T : struct {
		if (value.HasValue) {
			obj = value.Value;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static WeakReference<T> MakeWeak<T>(this T obj) where T : class => new(obj);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int ClampDimension(this int value) => Math.Min(value, Config.ClampDimension);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I ClampDimension(this Vector2I value) => value.Min(Config.ClampDimension);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Swap<T>(ref T l, ref T r) => (r, l) = (l, r);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static string GetTypeName(this object obj) => obj.GetType().Name;
}
