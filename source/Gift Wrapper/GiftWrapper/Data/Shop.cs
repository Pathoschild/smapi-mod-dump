/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GiftWrapper
**
*************************************************/

namespace GiftWrapper.Data
{
	public record Shop
	{
		/// <summary>
		/// Name of shop context or portrait NPC, prioritised respectively,
		/// to match either <see cref="StardewValley.Menus.ShopMenu.storeContext"/>
		/// or <see cref="StardewValley.Menus.ShopMenu.portraitPerson"/>.
		/// </summary>
		public string Context = null;
		/// <summary>
		/// List of conditions to validate this entry.
		/// Valid if null, or if any condition passes.
		/// Any condition passes if all fields pass.
		/// Parsed by <see cref="StardewValley.GameLocation.checkEventPrecondition(string)"/>.
		/// </summary>
		public string[] Conditions = null;
		/// <summary>
		/// List of names of items used to index items added by this shop entry.
		/// Prioritised in descending order.
		/// </summary>
		public string[] AddAtItem = null;
		/// <summary>
		/// Multiplier applied to sale price after all usual multipliers.
		/// Based on <see cref="StardewValley.Item.salePrice"/>.
		/// </summary>
		public float PriceMultiplier = 1;
		/// <summary>
		/// Entry is only valid if this field equals value of <see cref="Config.AlwaysAvailable"/>,
		/// for either boolean value.
		/// </summary>
		public bool IfAlwaysAvailable = false;
	}
}
