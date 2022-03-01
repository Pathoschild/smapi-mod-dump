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

namespace Leclair.Stardew.Common.Types
{
	public class CaseInsensitiveHashSet : HashSet<string>
	{

		public CaseInsensitiveHashSet() : base(StringComparer.OrdinalIgnoreCase) { }

		public CaseInsensitiveHashSet(IEnumerable<string> values) : base(values, StringComparer.OrdinalIgnoreCase) { }

		public CaseInsensitiveHashSet(string value) : base(new[] { value }, StringComparer.OrdinalIgnoreCase) { }

	}
}
