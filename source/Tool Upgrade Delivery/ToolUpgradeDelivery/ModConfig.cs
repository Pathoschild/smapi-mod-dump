/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sorrylate/Stardew-Valley-Mod
**
*************************************************/

using System;
using StardewModdingAPI;

internal class ModConfig
{
	public SButton ReceiveUpgradeButton { get; set; } = SButton.F3;
	public bool InstantUpgrade { get; set; } = false;
	public int CostInstantUpgrade { get; set; } = 5000;
}
