/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

static class ConditionalWeakTableExt {
	internal static IEnumerable<KeyValuePair<TKey, TValue>> AsEnumerable<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table) where TKey : class where TValue : class =>
		(IEnumerable<KeyValuePair<TKey, TValue>>)table;

	internal static IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table) where TKey : class where TValue : class =>
		table.AsEnumerable().GetEnumerator();
}
