/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using static SpriteMaster.Runtime;

namespace SpriteMaster.Extensions;

static class Collections {
	[MethodImpl(MethodImpl.Hot)]
	internal static V GetOrAddDefault<K, V>(this Dictionary<K, V> dictionary, K key, Func<V> defaultGetter) {
		if (dictionary.TryGetValue(key, out V value)) {
			return value;
		}
		var newValue = defaultGetter.Invoke();
		dictionary.Add(key, newValue);
		return newValue;
	}
}
