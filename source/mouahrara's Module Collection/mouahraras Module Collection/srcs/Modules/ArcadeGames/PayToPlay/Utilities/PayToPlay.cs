/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace mouahrarasModuleCollection.ArcadeGames.PayToPlay.Utilities
{
	internal class PayToPlayUtility
	{
		private static readonly PerScreen<bool> onInsertCoinMenu = new(() => true);
		private static readonly PerScreen<bool> triedToInsertCoin = new(() => true);

		internal static void Reset()
		{
			OnInsertCoinMenu = true;
			TriedToInsertCoin = true;
		}

		internal static bool OnInsertCoinMenu
		{
			get => onInsertCoinMenu.Value;
			set => onInsertCoinMenu.Value = value;
		}

		internal static bool TriedToInsertCoin
		{
			get => triedToInsertCoin.Value;
			set => triedToInsertCoin.Value = value;
		}
	}
}
