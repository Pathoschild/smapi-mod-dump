/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

namespace ToolUpgradeCosts
{
	public class Upgrade
	{
		public int Cost { get; set; }

		public string MaterialName { get; set; }

		internal int MaterialIndex { get; set; }

		public int MaterialStack { get; set; }
	}
}
