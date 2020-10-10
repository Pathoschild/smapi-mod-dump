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