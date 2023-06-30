/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace Shockah.Kokoro;

public static class CollectionExt
{
	public static void Toggle<T>(this ISet<T> set, T element)
	{
		if (set.Contains(element))
			set.Remove(element);
		else
			set.Add(element);
	}
}