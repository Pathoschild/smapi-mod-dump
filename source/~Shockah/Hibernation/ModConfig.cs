/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.Hibernation
{
	internal class ModConfig
	{
		public IList<string> LengthOptions { get; set; } = new[] { "3d", "1w", "1s", "1y", "5y", "forever" }.ToList();

		[JsonIgnore]
		public IList<HibernateLength> ParsedLengthOptions
		{
			get
			{
				IList<HibernateLength> parsed = new List<HibernateLength>();
				foreach (var length in LengthOptions)
				{
					var parsedLength = HibernateLength.ParseOrNull(length);
					if (parsedLength is not null)
						parsed.Add(parsedLength.Value);
				}
				return parsed;
			}
		}
	}
}
