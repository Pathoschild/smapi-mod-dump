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
			SetOnInsertCoinMenu(true);
			SetTriedToInsertCoin(true);
		}

		internal static void SetOnInsertCoinMenu(bool value)
		{
			onInsertCoinMenu.Value = value;
		}

		internal static bool GetOnInsertCoinMenu()
		{
			return onInsertCoinMenu.Value;
		}

		internal static void SetTriedToInsertCoin(bool value)
		{
			triedToInsertCoin.Value = value;
		}

		internal static bool GetTriedToInsertCoin()
		{
			return triedToInsertCoin.Value;
		}
	}
}
