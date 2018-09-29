using MoreMultiplayerInfo.Helpers;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoreMultiplayerInfo.EventHandlers
{
    public class PlayerStateWatcher
    {
        private readonly IModHelper _helper;

        public class PlayerLastActivity
        {
            private static Dictionary<string, string> ActivityDisplayNames => new Dictionary<string, string>
            {
                { "hoe", "Dug with a hoe" },
                { "pickaxe", "Swung a pickaxe" },
                { "axe", "Swung an axe" },
                { "watering can", "Sprinkled some water" },
                { "warped", "Switched areas" },
                { "scythe", "Slashed a scythe" },
                { "dirk", "Slashed a dagger" },
                { "sword", "Slashed a sword" },
                { "falchion", "Slashed a sword" },
                { "edge", "Slashed a sword" },
                { "blade", "Slashed a sword" },
                { "mallet", "Slammed a mallet" },
                { "swapped items", "Inventory management" },
                { "pole", "Went fishing" },
                { "rod", "Went fishing" },
                { "slingshot", "Fired a slingshot" },
                { "event", "Watching a cutscene" },
            };


            public string Activity { get; set; }

            public int When { get; set; }

            public bool Hidden { get; set; }

            private int WhenInMinutes => GameTimeHelper.GameTimeToMinutes(When);            

            public string LocationName { get; set; }

            private int OneHourSpan => 60;

            private int HalfHour => OneHourSpan / 2;

            private int TwoHours => OneHourSpan * 2;

            private int MinutesSinceWhen => GameTimeHelper.GameTimeToMinutes(Game1.timeOfDay) - WhenInMinutes;

            

            public string GetDisplayText()
            {
                return $"Last Activity: {GetActivityDisplay()}";
            }

            private string GetActivityDisplay()
            {
                if (string.IsNullOrEmpty(Activity))
                {
                    Activity = "Something suspicious";

                    return Activity;
                }

                if (MinutesSinceWhen >= TwoHours)
                {
                    Activity = "Nothing noteworthy";

                    return Activity;
                }

                if (ActivityDisplayNames.Keys.Any(k => Activity.Contains(k)))
                {
                    return ActivityDisplayNames.First(k => Activity.Contains(k.Key)).Value;
                }

                return Activity;
            }

            public string GetWhenDisplay()
            {
                if (MinutesSinceWhen <= HalfHour)
                {
                    return "just now";
                }

                if (MinutesSinceWhen < OneHourSpan)
                {
                    return $"{MinutesSinceWhen} minutes ago";
                }

                if (MinutesSinceWhen < TwoHours)
                {
                    return "one hour ago";
                }

                return "since " + Game1.getTimeOfDayString(When);
            }

        }

        public static Dictionary<long, PlayerLastActivity> LastActions { get; set; }

        public PlayerStateWatcher(IModHelper helper)
        {
            _helper = helper;
            LastActions = new Dictionary<long, PlayerLastActivity>();
            GameEvents.EighthUpdateTick += WatchPlayerActions;
        }

        private void WatchPlayerActions(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady) return;

            var players = PlayerHelpers.GetAllCreatedFarmers();

            foreach (var player in players)
            {
                var playerId = player.uniqueMultiplayerID;

                LastActions.GetOrCreateDefault(playerId);
                
                var currentLocation = player.currentLocation?.name ?? new NetString("(unknown location)");

                if (CheckCutscene(player, playerId, currentLocation)) continue;

                if (CheckLocationChange(currentLocation, playerId)) continue;

                if (CheckUsingTool(player, playerId, currentLocation)) continue;


            }
        }

        private static bool CheckUsingTool(Farmer player, NetLong playerId, NetString currentLocation)
        {
            if (player.UsingTool)
            {
                LastActions[playerId] = new PlayerLastActivity
                {
                    Activity = player.CurrentTool?.Name.ToLower() ?? "N/A",
                    When = Game1.timeOfDay,
                    LocationName = currentLocation,
                    Hidden = player.hidden.Value
                };
                return true;
            }

            return false;
        }

        private static bool CheckLocationChange(NetString currentLocation, NetLong playerId)
        {
            if (currentLocation != LastActions[playerId].LocationName)
            {
                LastActions[playerId] = new PlayerLastActivity
                {
                    Activity = "warped",
                    LocationName = currentLocation,
                    When = Game1.timeOfDay,
                    Hidden = false
                };
                return true;
            }

            return false;
        }

        private bool CheckCutscene(Farmer player, NetLong playerId, NetString currentLocation)
        {
            if (player.hidden != LastActions[playerId].Hidden && !(player.isRidingHorse()))
            {
                if (ConfigHelper.GetOptions().ShowCutsceneInfoInChatBox)
                {
                    var verbed = player.hidden ? "entered a" : "finished the";
                    _helper.SelfInfoMessage($"{player.Name} has {verbed} cutscene.");
                }

                LastActions[playerId] = new PlayerLastActivity
                {
                    Activity = "event",
                    When = Game1.timeOfDay,
                    LocationName = currentLocation,
                    Hidden = player.hidden?.Value ?? false
                };

                return true;
            }

            return false;
        }

        public static PlayerLastActivity GetLastActionForPlayer(long playerId)
        {
            return LastActions.GetOrCreateDefault(playerId);
        }
    }
}
