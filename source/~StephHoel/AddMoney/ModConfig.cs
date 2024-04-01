/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StephHoel/ModsStardewValley
**
*************************************************/

using StardewModdingAPI;

namespace AddMoney;

public class ModConfig
{
    /// <summary>
    /// Button to Add Money
    /// </summary>
    public SButton ButtonToAddMoney { get; set; } = SButton.G;

    /// <summary>
    /// Gold to Add on Wallet
    /// </summary>
    public int GoldToAdd { get; set; } = 100000;
}