/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Freaksaus/Tileman-Redux
**
*************************************************/

namespace TilemanRedux;

internal sealed class ModData
{
	public bool ToPlaceTiles { get; set; } = true;
	public bool DoCollision { get; set; } = true;
	public bool AllowPlayerPlacement { get; set; } = false;
	public bool ToggleOverlay { get; set; } = true;
	public float TilePrice { get; set; } = 1f;
	public float TilePriceRaise { get; set; } = 0.0008f;
	public int DifficultyMode { get; set; } = 1;
	public int PurchaseCount { get; set; } = 0;
	public int OverlayMode { get; set; } = 0;
	public float TotalMoneySpent { get; set; } = 0;
}
