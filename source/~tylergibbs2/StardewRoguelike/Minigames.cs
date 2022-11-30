/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley.Minigames;
using StardewValley;
using System;

namespace StardewRoguelike
{
    public static class Minigames
    {
        public static int PrairieKingKills { get; set; } = 0;

        public static int JunimoKartScore { get; set; } = 0;

        public static void MinigameClosed(IMinigame minigame)
        {
            if (minigame is MineCart cartGame)
            {
                int score = (int)cartGame.GetType().GetField("score", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(cartGame)!;
                JunimoKartScore += score;
            }

            if (Perks.HasPerk(Perks.PerkType.Gamer))
            {
                int goldAmount = (JunimoKartScore / 3333) + (PrairieKingKills / 5);
                if (goldAmount > 0)
                {
                    SObject gold = new(384, goldAmount);
                    Game1.player.addItemByMenuIfNecessaryElseHoldUp(gold);
                }
            }

            PrairieKingKills = 0;
            JunimoKartScore = 0;
            Game1.player.jotpkProgress.Value = null;
        }
    }
}
