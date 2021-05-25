/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JudeRV/Stardew-PassiveFriendship
**
*************************************************/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace PassiveFriendship
{
    public class ModEntry : Mod
    {
        private ModConfig Config;

        GameLocation gameLocation;

        readonly Dictionary<string, string> disposition = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");

        //Config bools
        int radius;
        int friendshipGainedPerInterval;
        bool notifyInConsole;

        readonly int fourteenHeartPoints = 3749;
        readonly int tenHeartPoints = 2749;
        readonly int eightHeartPoints = 2249;
        public override void Entry(IModHelper helper)
        {
            //Config options being set
            Config = Helper.ReadConfig<ModConfig>();
            radius = Config.FriendshipRadius;
            friendshipGainedPerInterval = Config.AmountOfFriendshipGainedPerTenMinutes;
            notifyInConsole = Config.NotifyAboutFriendshipInConsole;

            //Event handler
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.Player.Warped += OnPlayerWarped;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        //Apparently getting currentLocation can return null for non-main players when warping, so I decided on this instead
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            gameLocation = Game1.player.currentLocation;
        }
        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            gameLocation = e.NewLocation;
        }

        //Triggers every 10 in-game minutes
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            //Stops if world isn't ready, then resets lists for next time interval
            if (!Context.IsWorldReady) return;
            SetListsForNewTime();

            //
            Vector2 playerLocation = Game1.player.getTileLocation();
            foreach (NPC character in gameLocation.characters)
            {
                Vector2 npcLocation = character.getTileLocation();
                if (!disposition.ContainsKey(character.name)) continue;
                if (VillagerIsNearPlayer(playerLocation, npcLocation))
                {
                    NPCList.nearbyVillagersNow.Add(character.name.ToString().ToLower(), character.datable);
                }
            }
            //If character is present for 2 or more time changes & friendship isn't maxed, add a point of friendship
            foreach (KeyValuePair<string, Friendship> character in Game1.player.friendshipData.Pairs)
            {
                if (NPCList.nearbyVillagersBefore.ContainsKey(character.Key.ToLower()))
                {
                    if (NPCList.nearbyVillagersNow.ContainsKey(character.Key.ToLower()))
                    {
                        try
                        {
                            bool isDatable = NPCList.nearbyVillagersNow[character.Key.ToLower()];

                            if (!isDatable && character.Value.Points < tenHeartPoints)
                            {
                                character.Value.Points += friendshipGainedPerInterval;
                            }
                            else if (isDatable)
                            {
                                /* If you're married and below 15 hearts, or
                                 * if you're dating and below 11 hearts, or
                                 * if you're not dating and below 9 hearts,
                                 * add a friendship point. */
                                if ((character.Value.Status == FriendshipStatus.Married && character.Value.Points < fourteenHeartPoints) ||
                                    (character.Value.Status == FriendshipStatus.Dating && character.Value.Points < tenHeartPoints) ||
                                    (character.Value.Points < eightHeartPoints))
                                {
                                    character.Value.Points += friendshipGainedPerInterval;
                                }
                            }
                            if (notifyInConsole)
                            {
                                Monitor.Log($"Gained a point of friendship with {character.Key} at time {e.NewTime}. " +
                                            $"Now at {character.Value.Points} points.", LogLevel.Debug);
                            }
                        }
                        catch
                        {
                            this.Monitor.Log($"ERROR! Something happened with {character.Key}. Please report this " +
                                $"to the evidently ineffective mod author.", LogLevel.Error);
                        }
                    }
                }
            }
        }
        //Checks if character is within radius
        private bool VillagerIsNearPlayer(Vector2 playerLocation, Vector2 npcLocation)
        {
            float xDistance = npcLocation.X - playerLocation.X;
            float yDistance = npcLocation.Y - playerLocation.Y;
            return (Math.Abs(xDistance) <= radius && Math.Abs(yDistance) <= radius);
        }

        //Just resets lists for new time
        private void SetListsForNewTime()
        {
            NPCList.nearbyVillagersBefore.Clear();
            foreach (KeyValuePair<string, bool> kvp in NPCList.nearbyVillagersNow)
            {
                NPCList.nearbyVillagersBefore.Add(kvp.Key, kvp.Value);
            }
            NPCList.nearbyVillagersNow.Clear();
        }
    }

    class NPCList
    {
        public static Dictionary<string, bool> nearbyVillagersNow = new Dictionary<string, bool>();
        public static Dictionary<string, bool> nearbyVillagersBefore = new Dictionary<string, bool>();
    }

    //Class for config.json
    class ModConfig
    {
        public int FriendshipRadius { get; set; } = 2;
        public bool NotifyAboutFriendshipInConsole { get; set; } = false;
        public int AmountOfFriendshipGainedPerTenMinutes { get; set; } = 1;
    }
}