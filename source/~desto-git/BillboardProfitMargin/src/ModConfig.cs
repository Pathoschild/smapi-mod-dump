/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/desto-git/sdv-mods
**
*************************************************/

namespace BillboardProfitMargin
{
	/// <summary>Mod Configuration settings.</summary>
	internal class ModConfig
	{
		/// <summary>Adjust quest rewards according to the profit margin.</summary>
		public bool UseProfitMargin { get; set; } = true;

		/// <summary>If UseProfitMargin is false, use this one instead.</summary>
		public float CustomProfitMargin { get; set; } = 0.75f;

		/// <summary>Adjust special order rewards according to the profit margin.</summary>
		public bool UseProfitMarginForSpecialOrders { get; set; } = true;

		/// <summary>If UseProfitMarginForSpecialOrders is false, use this one instead.</summary>
		public float CustomProfitMarginForSpecialOrders { get; set; } = 0.75f;
	}
}
