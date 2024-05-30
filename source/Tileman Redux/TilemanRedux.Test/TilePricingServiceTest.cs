/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Freaksaus/Tileman-Redux
**
*************************************************/

using TilemanRedux.Services;

namespace TilemanRedux.Test;

public sealed class TilePricingServiceTest
{
	private ITilePricingService _tilePricingService;

	[SetUp]
	public void Setup()
	{
		_tilePricingService = new TilePricingService();
	}

	[Test]
	public void CalculateTilePrice_WithEasyDifficultyAndNoTilesBought_ReturnsZero()
	{
		var price = _tilePricingService.CalculateTilePrice(0, 0, 0, 0, 0);
		Assert.That(price, Is.Zero);
	}

	[Test]
	public void CalculateTilePrice_WithEasyDifficultyAndOneTileBought_ReturnsTilesBoughtPlusRaise()
	{
		float currentPrice = 1;
		float priceRaise = 1;
		int tilesBought = 1;
		var price = _tilePricingService.CalculateTilePrice(0, currentPrice, 0, priceRaise, tilesBought);

		Assert.That(price, Is.EqualTo(tilesBought + priceRaise));
	}

	[Test]
	public void CalculateTilePrice_WithEasyDifficultyAndHundredTilesBought_ReturnsTilesBoughtPlusRaise()
	{
		float currentPrice = 100;
		float priceRaise = 1;
		int tilesBought = 100;
		var price = _tilePricingService.CalculateTilePrice(0, currentPrice, 0, priceRaise, tilesBought);

		Assert.That(price, Is.EqualTo(tilesBought + priceRaise));
	}

	[Test]
	public void CalculateTilePrice_WithNormalDifficultyAndNoTilesBought_ReturnsZero()
	{
		float currentPrice = 0;
		int tilesBought = 0;

		var price = _tilePricingService.CalculateTilePrice(1, currentPrice, 0, 0, tilesBought);
		Assert.That(price, Is.Zero);
	}

	[Test]
	public void CalculateTilePrice_WithNormalDifficultyAndOneTileBought_ReturnsCurrentPrice()
	{
		float currentPrice = 1;
		int tilesBought = 1;

		var price = _tilePricingService.CalculateTilePrice(1, currentPrice, 0, 0, tilesBought);

		Assert.That(price, Is.EqualTo(currentPrice));
	}

	[Test]
	public void CalculateTilePrice_WithNormalDifficultyAndTwoTilesBought_ReturnsDoubleStartingTilePrice()
	{
		float startingTilePrice = 1;
		int tilesBought = 2;

		var price = _tilePricingService.CalculateTilePrice(1, 0, startingTilePrice, 0, tilesBought);

		Assert.That(price, Is.EqualTo(startingTilePrice * 2));
	}

	[Test]
	public void CalculateTilePrice_WithNormalDifficultyAndElevenTilesBought_ReturnsQuadrupleStartingTilePrice()
	{
		float startingTilePrice = 3;
		int tilesBought = 11;

		var price = _tilePricingService.CalculateTilePrice(1, 0, startingTilePrice, 0, tilesBought);

		Assert.That(price, Is.EqualTo(startingTilePrice * 4));
	}

	[Test]
	public void CalculateTilePrice_WithHardDifficultyAndNoTilesBought_ReturnsZero()
	{
		int tilesBought = 0;

		var price = _tilePricingService.CalculateTilePrice(2, 0, 0, 0, tilesBought);
		Assert.That(price, Is.Zero);
	}

	[Test]
	public void CalculateTilePrice_WithHardDifficultyAndOneTileBought_ReturnsTilesBought()
	{
		float currentPrice = 1;
		int tilesBought = 1;

		var price = _tilePricingService.CalculateTilePrice(2, currentPrice, 0, 0, tilesBought);

		Assert.That(price, Is.EqualTo(currentPrice));
	}

	[Test]
	public void CalculateTilePrice_WithHardDifficultyAndNineHundredTilesBought_ReturnsTilesBought()
	{
		int tilesBought = 900;

		var price = _tilePricingService.CalculateTilePrice(2, 1, 30, 4, tilesBought);

		Assert.That(price, Is.EqualTo(tilesBought));
	}
}
