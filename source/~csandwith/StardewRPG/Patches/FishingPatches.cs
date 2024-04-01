/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace StardewRPG
{
	public partial class ModEntry
	{
		private static void BobberBar_Postfix(ref int ___bobberBarHeight, ref float ___bobberBarPos)
		{
			if (!Config.EnableMod)
				return;
			var add = GetStatMod(GetStatValue(Game1.player, "dex", Config.BaseStatValue)) * Config.DexDaggerSpeedBonus;
			SMonitor.Log($"Modifying bobber height {___bobberBarHeight} + {add}");
			___bobberBarHeight += add;
			___bobberBarPos = 568 - ___bobberBarHeight;
		}
		private static void BobberBar_update_Postfix(bool ___bobberInBar, bool ___fadeIn, bool ___fadeOut, bool ___treasure, bool ___treasureCaught, float ___treasurePosition, int ___bobberBarHeight, float ___bobberBarPos, ref float ___treasureCatchLevel, ref float ___distanceFromCatching)
		{
			if (!Config.EnableMod || ___fadeIn || ___fadeOut || (!___bobberInBar && (!___treasure || ___treasureCaught)))
				return;
            if (___bobberInBar)
            {
				___distanceFromCatching += GetStatMod(GetStatValue(Game1.player, "str", Config.BaseStatValue)) * Config.StrFishingReelSpeedBonus;
			}
            else if (___treasurePosition + 12f <= ___bobberBarPos - 32f + ___bobberBarHeight && ___treasurePosition - 16f >= ___bobberBarPos - 32f)
            {
				___treasureCatchLevel += GetStatMod(GetStatValue(Game1.player, "str", Config.BaseStatValue)) * Config.StrFishingTreasureSpeedBonus;
			}
		}
	}
}