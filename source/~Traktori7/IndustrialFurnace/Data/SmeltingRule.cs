/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

namespace IndustrialFurnace.Data
{
	/// <summary>
	/// The data class for a smelting rule.
	/// </summary>
	public class SmeltingRule
	{
		public int InputItemID { get; set; }
		public int InputItemAmount { get; set; }
		public int OutputItemID { get; set; }
		public int OutputItemAmount { get; set; }
		public string[]? RequiredModID { get; set; }
	}
}
