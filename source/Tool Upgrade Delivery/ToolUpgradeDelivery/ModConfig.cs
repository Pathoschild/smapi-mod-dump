using System;
using StardewModdingAPI;

internal class ModConfig
{
	public SButton ReceiveUpgradeButton { get; set; } = SButton.F3;
	public bool InstantUpgrade { get; set; } = false;
	public int CostInstantUpgrade { get; set; } = 5000;
}
