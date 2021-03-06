/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowBirthdays
{
	class ModConfig
	{
		public int cycleDuration = 120;
		public string cycleType = "Always";
		public bool showIcon = true;
		internal static string[] cycleTypes = new string[] { "Always", "Hover", "Click" };
	}
}
