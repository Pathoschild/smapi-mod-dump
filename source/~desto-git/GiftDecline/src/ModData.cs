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
	using System.Collections.Generic;

	/// <summary>Mod save game data.</summary>
	internal class ModData
	{
		/// <summary>
		/// List of gift taste differences.
		/// Format: NpcName => itemId = taste.
		/// </summary>
		public Dictionary<string, Dictionary<string, int>> GiftTasteOverwrites { get; set; } =
			new Dictionary<string, Dictionary<string, int>>();

		/// <summary>
		/// List of how many items a gift has been received.
		/// Format: NpcName => itemId = amount.
		/// </summary>
		public Dictionary<string, Dictionary<string, int>> GiftTasteDeclineBuffer { get; set; } =
			new Dictionary<string, Dictionary<string, int>>();
	}
}