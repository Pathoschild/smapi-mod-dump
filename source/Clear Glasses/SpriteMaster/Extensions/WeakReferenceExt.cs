/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;

namespace SpriteMaster.Extensions;

internal static class WeakReferenceExt {
	internal static bool TryGet<T>(this WeakReference<T>? weakRef, [NotNullWhen(true)] out T? value) where T : class {
		if (weakRef?.TryGetTarget(out value) ?? false) {
			return true;
		}

		value = null;
		return false;
	}
}
