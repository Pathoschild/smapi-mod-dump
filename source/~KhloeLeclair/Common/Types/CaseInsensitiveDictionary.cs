/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;

namespace Leclair.Stardew.Common.Types;

public class CaseInsensitiveDictionary<TValue> : Dictionary<string, TValue> {

	public CaseInsensitiveDictionary() : base(StringComparer.OrdinalIgnoreCase) { }

	public CaseInsensitiveDictionary(int capacity) : base(capacity, StringComparer.OrdinalIgnoreCase) { }

	public CaseInsensitiveDictionary(IEnumerable<KeyValuePair<string, TValue>> collection) : base(collection, StringComparer.OrdinalIgnoreCase) { }

	public CaseInsensitiveDictionary(IDictionary<string, TValue> collection) : base(collection, StringComparer.OrdinalIgnoreCase) { }

}
