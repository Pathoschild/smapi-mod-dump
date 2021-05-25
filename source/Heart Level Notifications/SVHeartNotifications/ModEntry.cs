/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JudeRV/Stardew-HeartLevelNotifier
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SVHeartNotifications
{
    public class ModEntry : Mod
    {
        private ModConfig Config;

        bool isFirstDayAfterLoading;
        bool includeHeartAlerts;
        readonly int pointsPerHeart = 250;

        public override void Entry(IModHelper helper)
        {
            isFirstDayAfterLoading = true;
            this.Config = this.Helper.ReadConfig<ModConfig>();
            includeHeartAlerts = this.Config.IncludeLowPointAlerts;

            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            //Only starts running on the second day playing to compare values against yesterday's
            if (isFirstDayAfterLoading)
            {
                //Creates dictionary of friendship points to compare against tomorrow's
                foreach (KeyValuePair<string, Friendship> friend in Game1.player.friendshipData.Pairs)
                {
                    Friends.yesterdayFriendships.Add(friend.Key, friend.Value.Points / pointsPerHeart);
                }
                isFirstDayAfterLoading = false;
                return;
            }

            //Creates dictionary of today's friendship points to compare against yesterday's
            //Friendship points are divided by 250 to represent heart levels with decimals
            Dictionary<string, float> todayFriendship = new Dictionary<string, float>();
            foreach (KeyValuePair<string, Friendship> friend in Game1.player.friendshipData.Pairs)
            {
                todayFriendship.Add(friend.Key, friend.Value.Points / (float)pointsPerHeart);
            }

            //Compares today's and yesterday's values, displays messages accordingly
            foreach (KeyValuePair<string, float> todayFriend in todayFriendship)
            {
                //If player hasn't met villager yet, add villager to list and set value to 0 for yesterday
                if (!Friends.yesterdayFriendships.ContainsKey(todayFriend.Key))
                {
                    Friends.yesterdayFriendships.Add(todayFriend.Key, 0);
                }

                string villagerName = todayFriend.Key;
                float heartAlertThreshold = 0.016f;
                int todayFriendHeartLevel = (int)Math.Floor((decimal)todayFriend.Value);
                int yesterdayFriendHeartLevel = (int)Math.Floor((decimal)Friends.yesterdayFriendships[todayFriend.Key]);

                if (todayFriendHeartLevel > yesterdayFriendHeartLevel)
                {
                    Game1.chatBox.addMessage($"You've gained a heart with {villagerName}! " +
                        $"You're now at {todayFriendHeartLevel} heart(s).", Color.LawnGreen);
                }
                else if (todayFriendHeartLevel < yesterdayFriendHeartLevel)
                {
                    Game1.chatBox.addMessage($"You've lost a heart with {villagerName}. " +
                        $"You're now at {todayFriendHeartLevel} heart(s).", Color.Red);
                }
                else if (todayFriend.Value < Friends.yesterdayFriendships[todayFriend.Key])
                {
                    if (includeHeartAlerts && todayFriend.Value - todayFriendHeartLevel <= heartAlertThreshold)
                    {
                        Game1.chatBox.addMessage($"Heads up! If you don't talk with {villagerName} soon, " +
                            $"you'll lose a heart with them.", Color.LightSalmon);
                    }
                }
            }

            //Clears yesterday's friendships and replaces it with today's so it can be compared tomorrow
            Friends.yesterdayFriendships.Clear();
            foreach (KeyValuePair<string, float> todayFriend in todayFriendship)
            {
                Friends.yesterdayFriendships.Add(todayFriend.Key, todayFriend.Value);
            }
        }
    }

    //Just used to store friendship values one day to compare against tomorrow's
    class Friends
    {
        public static Dictionary<string, float> yesterdayFriendships = new Dictionary<string, float>();
    }

    //Configuration options
    class ModConfig
    {
        //Should the mod warn the player if they're about to lose a heart with someone in the next few days?
        public bool IncludeLowPointAlerts { get; set; } = false;
    }
}