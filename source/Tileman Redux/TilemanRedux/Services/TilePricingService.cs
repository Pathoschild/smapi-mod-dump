/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Freaksaus/Tileman-Redux
**
*************************************************/

namespace TilemanRedux.Services;

public class TilePricingService : ITilePricingService
{
	public float CalculateTilePrice(
		int difficultyMode,
		float currentTilePrice,
		float startingTilePrice,
		float tilePriceRaise,
		int tilesBought)
	{
		switch (difficultyMode)
		{
			case 0:
				if (tilesBought > 0)
				{
					currentTilePrice += tilePriceRaise;
				}
				return currentTilePrice;

			case 1:
				if (tilesBought > 1_000_000) return startingTilePrice * 128;
				if (tilesBought > 100_000) return startingTilePrice * 64;
				if (tilesBought > 10_000) return startingTilePrice * 32;
				if (tilesBought > 1_000) return startingTilePrice * 16;
				if (tilesBought > 100) return startingTilePrice * 8;
				if (tilesBought > 10) return startingTilePrice * 4;
				if (tilesBought > 1) return startingTilePrice * 2;

				return currentTilePrice;
			case 2:
				return tilesBought;
			default:
				return currentTilePrice;
		}
	}
}
