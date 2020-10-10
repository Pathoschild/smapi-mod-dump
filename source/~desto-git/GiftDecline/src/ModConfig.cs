/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/desto-git/sdv-mods
**
*************************************************/

namespace GiftDecline
{
	/// <summary>Mod Configuration settings.</summary>
	internal class ModConfig
	{
		/// <summary>
		/// Reset gift tastes after this many days, starting from the very first day (Day 1, Year 1).
		/// If 0, gift taste won't ever automatically reset.
		/// </summary>
		public int ResetEveryXDays { get; set; } = 112; // 28 * 4 = 1 year

		/// <summary>Limit how much the taste for a gift can drop.</summary>
		public int MaxReduction { get; set; } = 4;

		/// <summary>Reduce the gift taste only after the item has been gifted to the NPC this many times.</summary>
		public int ReduceAfterXGifts { get; set; } = 1;
	}
}