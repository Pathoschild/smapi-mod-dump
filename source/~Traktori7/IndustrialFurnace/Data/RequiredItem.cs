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
	/// The data class for blueprint's item requirements.
	/// </summary>
	public class RequiredItem
	{
		public string? ItemName { get; set; }
		public int ItemAmount { get; set; }
		public int ItemID { get; set; }


		public override string ToString()
		{
			return $"{ItemID} {ItemAmount}";
		}
	}
}
