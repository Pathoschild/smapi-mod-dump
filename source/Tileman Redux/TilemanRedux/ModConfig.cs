/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Freaksaus/Tileman-Redux
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace TilemanRedux;

/// <summary>
/// Configuration settings
/// </summary>
internal sealed class ModConfig
{
	/// <summary>
	/// Keybinding used to toggle the tile overlay
	/// </summary>
	public KeybindList ToggleOverlayKey { get; set; } = KeybindList.Parse("G");

	/// <summary>
	/// Keybinding used to toggle the tile overlay mode
	/// </summary>
	public KeybindList ToggleOverlayModeKey { get; set; } = KeybindList.Parse("H");

	/// <summary>
	/// Keybinding used to change the difficulty
	/// </summary>
	public KeybindList ChangeDifficultyKey { get; set; }

	/// <summary>
	/// Keybinding used to buy all tiles of the current location
	/// </summary>
	public KeybindList BuyLocationTilesKey { get; set; }

	/// <summary>
	/// Default tile price, this gets used as the base price upon which the prices get raised based on the difficulty
	/// </summary>
	public int TilePrice { get; set; } = 1;

	/// <summary>
	/// Amount to raise the tile price with when on the easiest difficulty
	/// </summary>
	public float TilePriceRaise { get; set; } = 0.0008f;

	/// <summary>
	/// The default difficulty mode to use for new saves
	/// </summary>
	public int DifficultyMode { get; set; } = 1;

	/// <summary>
	/// Allow the buying of a tile when the player keeps colliding without money to avoid getting stuck
	/// </summary>
	public bool BuyTileWhenCollidingWithoutMoney { get; set; }
}
