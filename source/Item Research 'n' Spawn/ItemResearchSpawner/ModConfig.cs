/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using ItemResearchSpawner.Models.Enums;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ItemResearchSpawner
{
    internal sealed class ModConfig
    {
        public KeybindList ShowMenuButton { get; set; } = KeybindList.ForSingle(SButton.R);
        public ModMode DefaultMode { get; set; } = ModMode.Research;
        public bool UseDefaultBalanceConfig { get; set; } = true;

        public float ResearchAmountMultiplier { get; set; } = 1.5f;

        public float SellPriceMultiplier { get; set; } = 0.9f;

        public float BuyPriceMultiplier { get; set; } = 1.1f;
    }
}