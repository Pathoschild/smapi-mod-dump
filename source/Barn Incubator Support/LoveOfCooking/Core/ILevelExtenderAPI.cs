/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace LoveOfCooking
{
	public interface ILevelExtenderAPI
	{
		int initializeSkill(string name, int xp, double? xp_mod = null, List<int> xp_table = null, int[] cats = null);
		dynamic TalkToSkill(string[] args);
	}
}
