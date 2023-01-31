/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Leclair.Stardew.Common.Types;

internal ref struct BitHelper {

	private readonly Span<int> Span;

	internal BitHelper(Span<int> span, bool clear) {
		if (clear)
			span.Clear();
		Span = span;
	}

	internal void Mark(int position) {
		int idx = position / 32;
		if (idx >= 0 && idx < Span.Length)
			Span[idx] |= 1 << position % 32;
	}

	internal bool IsMarked(int position) {
		int idx = position / 32;
		if (idx >= 0 && idx < Span.Length)
			return (Span[idx] & (1 << position % 32)) != 0;
		return false;
	}

	internal static int ToIntArrayLength(int count) {
		if (count <= 0) return 0;
		return (count - 1) / 32 + 1;
	}

}
