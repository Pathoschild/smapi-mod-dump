/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mucchan/sv-mod-prairie-king
**
*************************************************/

using System;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace PrairieKingMadeEasy
{
    public class PrairieKingMadeEasy : Mod
    {
        public static ModConfig config { get; private set; }

        public override void Entry(params object[] objects)
        {
            config = new ModConfig();
            config = config.InitializeConfig<ModConfig>(base.BaseConfigPath);
            GameEvents.UpdateTick += Event_UpdateTick;
        }

        private static void Event_UpdateTick (object sender, EventArgs e)
        {
            if (Game1.currentMinigame != null && "AbigailGame".Equals(Game1.currentMinigame.GetType().Name))
            {
                Type minigameType = Game1.currentMinigame.GetType();

                if (config.infiniteLives)
                {
                    minigameType.GetField("lives").SetValue(Game1.currentMinigame, 99);
                }

                if (config.infiniteCoins)
                {
                    minigameType.GetField("coins").SetValue(Game1.currentMinigame, 99);
                }

                if (config.rapidFire)
                {
                    minigameType.GetField("shootingDelay").SetValue(Game1.currentMinigame, 25);
                }

                if (config.alwaysInvincible)
                {
                    minigameType.GetField("playerInvincibleTimer").SetValue(Game1.currentMinigame, 5000);
                }
            }
        }
      
    }
}
