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
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewModdingAPI.Utilities;

namespace CustomDeathPenaltyPlus
{
    internal class PlayerStateRestorer
    {
        /// <summary>
        /// Tracks the players state
        /// </summary>
        internal class PlayerDataTracker
        {
            public int money;

            public double moneylost;

            public PlayerDataTracker(int m, double ml)
            {
                this.money = m;
                this.moneylost = ml;
            }
        }

        internal static readonly PerScreen<PlayerDataTracker> statedeathps = new PerScreen<PlayerDataTracker>(createNewState: () => null);

        internal static readonly PerScreen<PlayerDataTracker> statepassoutps = new PerScreen<PlayerDataTracker>(createNewState: () => null);

        private static ModConfig config;

        private static readonly int[] debuffs = { 12, 14, 17, 25, 26, 27 };

        // Change friendship of marriage candidate NPCs
        public static void ApplyFriendshipChange(string name)
        {            
            var configvalue = config.OtherPenalties.HarveyFriendshipChange;

            if (name == "Maru")
            {
                configvalue = config.OtherPenalties.MaruFriendshipChange;
            }


            if (Game1.player.friendshipData.ContainsKey(name) == true && (Game1.currentLocation.NameOrUniqueName == "Hospital" || config.OtherPenalties.WakeupNextDayinClinic == true))
            {
                //Yes, change friendship level
                if (configvalue < 0)
                {
                    Game1.player.changeFriendship(Math.Max(configvalue, -Game1.player.getFriendshipLevelForNPC(name)), Game1.getCharacterFromName(name, true));
                }
                else if (Game1.player.friendshipData[name].Status == FriendshipStatus.Married)
                {
                    Game1.player.changeFriendship(Math.Min(configvalue, (3749 - Game1.player.getFriendshipLevelForNPC(name))), Game1.getCharacterFromName(name, true));
                }
                else if (Game1.player.friendshipData[name].Status == FriendshipStatus.Dating)
                {
                    Game1.player.changeFriendship(Math.Min(configvalue, (2749 - Game1.player.getFriendshipLevelForNPC(name))), Game1.getCharacterFromName(name, true));
                }
                else
                {
                    Game1.player.changeFriendship(Math.Min(configvalue, (2249 - Game1.player.getFriendshipLevelForNPC(name))), Game1.getCharacterFromName(name, true));
                }
            }
        }

        // Allows the class to access the ModConfig properties
        public static void SetConfig(ModConfig config)
        {
            PlayerStateRestorer.config = config;
        }

        // Saves player's current money, and amount to be lost, killed
        public static void SaveStateDeath()
        {
            statedeathps.Value = new PlayerDataTracker(Game1.player.Money, Math.Min(config.DeathPenalty.MoneyLossCap, Game1.player.Money * (1 - config.DeathPenalty.MoneytoRestorePercentage)));
        }

        // Saves player's current money, and amount to be lost, passed out
        public static void SaveStatePassout()
        {
            statepassoutps.Value = new PlayerDataTracker(Game1.player.Money, Math.Min(config.PassOutPenalty.MoneyLossCap, Game1.player.Money * (1 - config.PassOutPenalty.MoneytoRestorePercentage)));
        }

        // Load Player state, killed
        public static void LoadStateDeath()
        {
            // Change money to state saved in statedeath if money is lost
            if(Game1.player.Money != statedeathps.Value.money || Game1.currentLocation.NameOrUniqueName == "Hospital")
            {
                Game1.player.Money = statedeathps.Value.money - (int)Math.Round(statedeathps.Value.moneylost);
            }

            // Restore stamina to amount as specified by config values
            Game1.player.stamina = (int)(Game1.player.maxStamina.Value * config.DeathPenalty.EnergytoRestorePercentage);

            // Restore health to amount as specified by config values
            Game1.player.health = Math.Max((int)(Game1.player.maxHealth * config.DeathPenalty.HealthtoRestorePercentage), 1);

            // Apply debuff if needed
            if (config.OtherPenalties.DebuffonDeath == true)
            {
                // Remove negative debuffs
                foreach(int debuff in debuffs)
                {
                    if (Game1.player.hasBuff(debuff))
                    {
                        Game1.buffsDisplay.removeOtherBuff(debuff);
                    }
                }

                if (Game1.currentLocation as IslandLocation != null)
                {
                    var burntdebuff = new Buff(12)
                    {
                        totalMillisecondsDuration = 60000,
                        millisecondsDuration = 60000,
                        glow = Color.White
                    };
                    Game1.buffsDisplay.addOtherBuff(burntdebuff);
                }
                else
                {
                    Random random = new Random((int)(Game1.uniqueIDForThisGame / Game1.player.stats.DaysPlayed + Game1.player.stats.timesUnconscious));

                    var applieddebuff = new Buff((int)debuffs.GetValue(random.Next(1, debuffs.Length)))
                    {
                        totalMillisecondsDuration = 60000,
                        millisecondsDuration = 60000,
                        glow = Color.White
                    };
                    Game1.buffsDisplay.addOtherBuff(applieddebuff);
                }
            }

            // Is RestoreItems true?
            if (config.DeathPenalty.RestoreItems == true)
            {
                //Yes, restore items and clear itemsLostLastDeath collection

                // Go through each item lost and saved to itemsLostLastDeath
                foreach (Item item in Game1.player.itemsLostLastDeath)
                {
                    //Is the player's inventory full?
                    if (Game1.player.isInventoryFull() == true)
                    {
                        // Yes, drop item on the floor
                        Game1.player.dropItem(item);
                    }

                    else
                    {
                        // No, add item to player's inventory
                        Game1.player.addItemToInventory(item);
                    }  
                }
                // Clears items lost, prevents being purchasable at Guild
                Game1.player.itemsLostLastDeath.Clear();
            }

            ApplyFriendshipChange("Harvey");

            if (Game1.Date.DayOfWeek == DayOfWeek.Tuesday || Game1.Date.DayOfWeek == DayOfWeek.Thursday)
            {
                ApplyFriendshipChange("Maru");
            }
        }

        // Load Player state, passed out
        public static void LoadStatePassout()
        {
            // Change money to state saved in statepassout
            Game1.player.Money = statepassoutps.Value.money - (int)Math.Round(statepassoutps.Value.moneylost);
        }
    }
}
