/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomDeathPenaltyPlus
**
*************************************************/

using StardewValley;
using System;

namespace CustomDeathPenaltyPlus
{
    internal class PlayerStateSaver
    {
        internal class PlayerMoneyTracker
        {
            public int money;

            public double moneylost;

            public PlayerMoneyTracker(int m, double l)
            {
                this.money = m;
                this.moneylost = l;
            }
        }

        public static PlayerMoneyTracker state;

        private static ModConfig config;

        public static void SetConfig(ModConfig config)
        {
            PlayerStateSaver.config = config;
        }

        // Saves player's current money and amount to be lost
        public static void Save()
        {
            state = new PlayerMoneyTracker(Game1.player.Money, Math.Min(config.MoneyLossCap, Game1.player.Money * (1-config.MoneytoRestorePercentage)));
        }

        //Load Player state
        public static void Load()
        {
            //Restore money
            Game1.player.Money = state.money - (int)Math.Round(state.moneylost);
           
            //Restore stamina
            Game1.player.stamina = (int)(Game1.player.maxStamina * config.EnergytoRestorePercentage);

            //Restore health
            Game1.player.health = (int)(Game1.player.maxHealth * config.HealthtoRestorePercentage);

            //Restore items
            if (config.RestoreItems == true)
            {
                foreach (Item item in Game1.player.itemsLostLastDeath)
                {
                    Game1.player.addItemToInventory(item);
                }
                //Clears items lost, prevents being purchasable at Guild
                Game1.player.itemsLostLastDeath.Clear();
            }
        }
    }
}
