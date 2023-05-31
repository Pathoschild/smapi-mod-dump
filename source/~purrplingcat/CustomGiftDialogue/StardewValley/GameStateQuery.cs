/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomGiftDialogue;
using StardewModdingAPI;
using StardewValley.Delegates;
using StardewValley.Locations;
using StardewValley.Network;

namespace StardewValley
{
    /// <summary>Resolves game state queries like <c>SEASON spring</c> in data assets.</summary>
    public static class GameStateQuery
    {
        /// <summary>The helper methods which simplify implementing custom game state query resolvers.</summary>
        public static class Helpers
        {
            /// <summary>Get the location matching a given name.</summary>
            /// <param name="locationName">The location to check. This can be <c>Here</c> (current location), <c>Target</c> (contextual location), or a location name.</param>
            /// <param name="contextualLocation">The location for which the query is being checked.</param>
            public static GameLocation GetLocation(string locationName, GameLocation contextualLocation)
            {
                if (string.Equals(locationName, "Here", StringComparison.OrdinalIgnoreCase))
                {
                    return Game1.currentLocation;
                }
                if (string.Equals(locationName, "Target", StringComparison.OrdinalIgnoreCase))
                {
                    return contextualLocation ?? Game1.currentLocation;
                }
                return Game1.getLocationFromName(locationName);
            }

            /// <summary>Get the location matching a given name, or throw an exception if it's not found.</summary>
            /// <param name="locationName">The location to check. This can be <c>Here</c> (current location), <c>Target</c> (contextual location), or a location name.</param>
            /// <param name="contextualLocation">The location for which the query is being checked.</param>
            public static GameLocation RequireLocation(string locationName, GameLocation contextualLocation)
            {
                return GetLocation(locationName, contextualLocation) ?? throw new KeyNotFoundException("Required location '" + locationName + "' not found.");
            }

            /// <summary>Get whether a check applies to the given player or players.</summary>
            /// <param name="contextualPlayer">The player for which the query is being checked.</param>
            /// <param name="playerKey">The players to check. This can be <c>Any</c> (at least one player matches), <c>All</c> (every player matches), <c>Current</c> (the current player), <c>Target</c> (the contextual player), <c>Host</c> (the main player), or a player ID.</param>
            /// <param name="check">The check to perform.</param>
            public static bool WithPlayer(Farmer contextualPlayer, string playerKey, Func<Farmer, bool> check)
            {
                if (string.Equals(playerKey, "Any", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Farmer farmer2 in Game1.getAllFarmers())
                    {
                        if (check(farmer2))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                if (string.Equals(playerKey, "All", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Farmer farmer in Game1.getAllFarmers())
                    {
                        if (!check(farmer))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                if (string.Equals(playerKey, "Current", StringComparison.OrdinalIgnoreCase))
                {
                    return check(Game1.player);
                }
                if (string.Equals(playerKey, "Target", StringComparison.OrdinalIgnoreCase))
                {
                    return check(contextualPlayer);
                }
                if (string.Equals(playerKey, "Host", StringComparison.OrdinalIgnoreCase))
                {
                    return check(Game1.MasterPlayer);
                }
                if (long.TryParse(playerKey, out var parsedId))
                {
                    return check(Game1.getFarmerMaybeOffline(parsedId));
                }
                return false;
            }

            /// <summary>Log an error indicating that a query couldn't be parsed.</summary>
            /// <param name="query">The game state query split by space, including the query key.</param>
            /// <param name="reason">The human-readable reason why the query is invalid.</param>
            /// <param name="exception">The underlying exception, if applicable.</param>
            /// <returns>Returns false.</returns>
            public static bool ErrorResult(string[] query, string reason, Exception exception = null)
            {
                CustomGiftDialogueMod.ModMonitor.Log($"Failed parsing condition '{string.Join(" ", query)}': {reason}.\n{exception}", LogLevel.Error);
                return false;
            }
        }

        /// <summary>The resolvers for vanilla game state queries. Most code should call <see cref="M:StardewValley.GameStateQuery.CheckConditions(System.String,StardewValley.GameLocation,StardewValley.Farmer,StardewValley.Item,System.Random,System.Collections.Generic.HashSet{System.String})" /> instead of using these directly.</summary>
        public static class DefaultResolvers
        {
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_SEASON_DAY(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                for (int i = 1; i < query.Length; i += 2)
                {
                    if (!ArgUtility.TryGet(query, i, out var season, out var error, allowBlank: false) || !ArgUtility.TryGetInt(query, i + 1, out var day, out error))
                    {
                        return Helpers.ErrorResult(query, error);
                    }
                    if (string.Equals(Game1.currentSeason, season, StringComparison.OrdinalIgnoreCase) && Game1.dayOfMonth == day)
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_DAY_OF_MONTH(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                for (int i = 1; i < query.Length; i++)
                {
                    string rawDay = ArgUtility.Get(query, i);
                    if (int.TryParse(rawDay, out var dayNumber))
                    {
                        if (Game1.dayOfMonth == dayNumber)
                        {
                            return true;
                        }
                        continue;
                    }
                    if (string.Equals(rawDay, "even", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Game1.dayOfMonth % 2 == 0)
                        {
                            return true;
                        }
                        continue;
                    }
                    if (string.Equals(rawDay, "odd", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Game1.dayOfMonth % 2 == 1)
                        {
                            return true;
                        }
                        continue;
                    }
                    return Helpers.ErrorResult(query, "'" + rawDay + "' isn't a valid day of month");
                }
                return false;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_DAY_OF_WEEK(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                for (int i = 1; i < query.Length; i++)
                {
                    string rawDay = ArgUtility.Get(query, i);
                    if (!TryGetDayOfWeekFor(rawDay, out var dayOfWeek))
                    {
                        return Helpers.ErrorResult(query, "'" + rawDay + "' isn't a valid day of week");
                    }
                    if (Game1.Date.DayOfWeek == dayOfWeek)
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_DAYS_PLAYED(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return Game1.stats.DaysPlayed >= int.Parse(query[1]);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_IS_FESTIVAL_DAY(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                int totalDays = Game1.Date.TotalDays;
                int dateOffset = 0;
                if (query.Length > 1)
                {
                    dateOffset = int.Parse(query[1]);
                }
                int num = (totalDays + dateOffset) % 112;
                int month = num / 28;
                int day = num % 28;
                return month switch
                {
                    0 => Utility.isFestivalDay(day + 1, "spring"),
                    1 => Utility.isFestivalDay(day + 1, "summer"),
                    2 => Utility.isFestivalDay(day + 1, "fall"),
                    3 => Utility.isFestivalDay(day + 1, "winter"),
                    _ => false,
                };
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_SEASON(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                foreach (string season in query)
                {
                    if (string.Equals(Game1.currentSeason, season, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_YEAR(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return Game1.year >= int.Parse(query[1]);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_TIME(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                int min = int.Parse(query[1]);
                int max = int.Parse(query[2]);
                if (min >= 0 && Game1.timeOfDay < min)
                {
                    return false;
                }
                if (max >= 0 && Game1.timeOfDay > max)
                {
                    return false;
                }
                return true;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_FARM_CAVE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string type = query[1];
                return Game1.MasterPlayer.caveChoice.Value switch
                {
                    1 => string.Equals(type, "Bats", StringComparison.OrdinalIgnoreCase),
                    2 => string.Equals(type, "Mushrooms", StringComparison.OrdinalIgnoreCase),
                    _ => string.Equals(type, "None", StringComparison.OrdinalIgnoreCase),
                };
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_FARM_NAME(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string name = string.Join(" ", query, 2, query.Length - 2);
                return string.Equals(player.farmName.Value, name, StringComparison.OrdinalIgnoreCase);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_FARM_TYPE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string farmType = query[1];
                if (!string.Equals(Game1.GetFarmTypeID(), farmType, StringComparison.OrdinalIgnoreCase))
                {
                    return string.Equals(GetFarmTypeKey(), farmType, StringComparison.OrdinalIgnoreCase);
                }
                return true;
            }

            /// <summary>Get the human-readable identifier for the current farm type. For a custom farm type, this is equivalent to <see cref="M:StardewValley.Game1.GetFarmTypeID" />.</summary>
            private static string GetFarmTypeKey()
            {
                return Game1.whichFarm switch
                {
                    0 => "Standard",
                    1 => "Riverland",
                    2 => "Forest",
                    3 => "Hilltop",
                    4 => "Wilderness",
                    5 => "FourCorners",
                    6 => "Beach",
                    _ => Game1.GetFarmTypeID(),
                };
            }

            /// <summary>Get whether all the Lost Books for the library have been found.</summary>
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_FOUND_ALL_LOST_BOOKS(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return Game1.netWorldState.Value.LostBooksFound.Value >= 21;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_IS_COMMUNITY_CENTER_COMPLETE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                if (Game1.MasterPlayer.hasCompletedCommunityCenter())
                {
                    return !Game1.MasterPlayer.mailReceived.Contains("JojaMember");
                }
                return false;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_IS_CUSTOM_FARM_TYPE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return Game1.whichFarm == 7;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_IS_HOST(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return Game1.IsMasterGame;
            }

            /// <summary>Get whether the <see cref="T:StardewValley.Locations.IslandNorth" /> bridge is fixed.</summary>
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_IS_ISLAND_NORTH_BRIDGE_FIXED(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return ((IslandNorth)Game1.getLocationFromName("IslandNorth"))?.bridgeFixed.Value ?? false;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_IS_JOJA_MART_COMPLETE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                if (Game1.MasterPlayer.hasCompletedCommunityCenter())
                {
                    return Game1.MasterPlayer.mailReceived.Contains("JojaMember");
                }
                return false;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_IS_MULTIPLAYER(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return Game1.IsMultiplayer;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_IS_VISITING_ISLAND(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string npcName = ArgUtility.Get(query, 1);
                if (npcName != null)
                {
                    return Game1.IsVisitingIslandToday(npcName);
                }
                return false;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_LOCATION_ACCESSIBLE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return Game1.isLocationAccessible(query[1]);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_LOCATION_CONTEXT(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                location = Helpers.RequireLocation(query[1], location);
                string contextId = query[2];
                return string.Equals(location.GetLocationContext().ToString(), contextId, StringComparison.OrdinalIgnoreCase);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_LOCATION_IS_MINES(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                location = Helpers.GetLocation(query[1], location);
                return location is MineShaft;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_LOCATION_IS_SKULL_CAVE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                location = Helpers.GetLocation(query[1], location);
                if (location is MineShaft shaft && shaft.mineLevel >= 121)
                {
                    return shaft.mineLevel != 77377;
                }
                return false;
            }

            public static string GetSeasonKeyForLocation(GameLocation location)
            {
                return location?.GetSeasonForLocation() ?? Game1.currentSeason;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_LOCATION_SEASON(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                location = Helpers.GetLocation(query[1], location);
                for (int i = 2; i < query.Length; i++)
                {
                    if (string.Equals(GetSeasonKeyForLocation(location), query[i], StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }

            private static string GetWeatherName(GameLocation location)
            {
                var weather = Game1.netWorldState.Value.GetWeatherForLocation(location.GetLocationContext());

                if (weather.isLightning.Value)
                {
                    return "Storm";
                }
                else if (weather.isRaining.Value)
                {
                    return "Rain";
                }
                else if (weather.isSnowing.Value)
                {
                    return "Snow";
                }
                else if (weather.isDebrisWeather.Value)
                {
                    return "Wind";
                }
                else
                {
                    return "Sun";
                }
            }


            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_WEATHER(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                location = Helpers.RequireLocation(query[1], location);
                string weatherId = query[2];
                return string.Equals(GetWeatherName(location), weatherId, StringComparison.OrdinalIgnoreCase);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_WORLD_STATE_FIELD(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string name = query[1];
                string expectedValue = query[2];
                PropertyInfo property = typeof(NetWorldState).GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if ((object)property == null)
                {
                    return false;
                }
                object actualValue = property.GetValue(Game1.netWorldState.Value, null);
                if (actualValue != null)
                {
                    if (!(actualValue is bool actual3))
                    {
                        if (!(actualValue is int actual2))
                        {
                            if (actualValue is string actual)
                            {
                                return string.Equals(actual, expectedValue, StringComparison.OrdinalIgnoreCase);
                            }
                            return string.Equals(actualValue.ToString(), expectedValue, StringComparison.OrdinalIgnoreCase);
                        }
                        if (int.TryParse(expectedValue, out var expectedInt))
                        {
                            return actual2 == expectedInt;
                        }
                        return false;
                    }
                    if (bool.TryParse(expectedValue, out var expectedBool))
                    {
                        return actual3 == expectedBool;
                    }
                    return false;
                }
                return string.Equals(expectedValue, "null", StringComparison.OrdinalIgnoreCase);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_WORLD_STATE_ID(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return NetWorldState.checkAnywhereForWorldStateID(query[1]);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_MINE_LOWEST_LEVEL_REACHED(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                int targetDepth = int.Parse(query[1]);
                return MineShaft.lowestLevelReached >= targetDepth;
            }

            /// <summary>Get whether a player has the given combat level, including any buffs which increase it.</summary>
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_COMBAT_LEVEL(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int level = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.CombatLevel >= level);
            }

            /// <summary>Get whether a player has the given farming level, including any buffs which increase it.</summary>
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_FARMING_LEVEL(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int level = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.FarmingLevel >= level);
            }

            /// <summary>Get whether a player has the given fishing level, including any buffs which increase it.</summary>
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_FISHING_LEVEL(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int level = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.FishingLevel >= level);
            }

            /// <summary>Get whether a player has the given foraging level, including any buffs which increase it.</summary>
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_FORAGING_LEVEL(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int level = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.ForagingLevel >= level);
            }

            /// <summary>Get whether a player has the given luck level, including any buffs which increase it.</summary>
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_LUCK_LEVEL(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int level = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.LuckLevel >= level);
            }

            /// <summary>Get whether a player has the given mining level, including any buffs which increase it.</summary>
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_MINING_LEVEL(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int level = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.MiningLevel >= level);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_CURRENT_MONEY(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int targetAmount = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.Money >= targetAmount);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_FARMHOUSE_UPGRADE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int minimumUpgradeLevel = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.HouseUpgradeLevel >= minimumUpgradeLevel);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_GENDER(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string genderName = query[2];
                bool isMale = string.Equals(genderName, "Male", StringComparison.OrdinalIgnoreCase);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.IsMale == isMale);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_ACHIEVEMENT(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int achievementId = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.achievements.Contains(achievementId));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_ALL_ACHIEVEMENTS(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                Dictionary<int, string> achievementData = Game1.content.Load<Dictionary<int, string>>("Data\\Achievements");
                return Helpers.WithPlayer(player, playerKey, delegate (Farmer target)
                {
                    foreach (int current in achievementData.Keys)
                    {
                        if (!target.achievements.Contains(current))
                        {
                            return false;
                        }
                    }
                    return true;
                });
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_CONVERSATION_TOPIC(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string topic = query[2];
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.activeDialogueEvents.ContainsKey(topic));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_CRAFTING_RECIPE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string recipeName = string.Join(" ", query, 2, query.Length - 2);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.craftingRecipes.ContainsKey(recipeName));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_COOKING_RECIPE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string recipeName = string.Join(" ", query, 2, query.Length - 2);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.cookingRecipes.ContainsKey(recipeName));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_DIALOGUE_ANSWER(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int responseId = Convert.ToInt32(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.DialogueQuestionsAnswered.Contains(responseId));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_FLAG(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string flag = string.Join(" ", query.Skip(2));
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.hasOrWillReceiveMail(flag));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_ITEM(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int itemId = Convert.ToInt32(query[2]);
                int minCount = ArgUtility.GetInt(query, 3, 1);
                return Helpers.WithPlayer(player, playerKey, delegate (Farmer target)
                {
                    switch (itemId)
                    {
                        case 73:
                            return Game1.netWorldState.Value.GoldenWalnuts.Value >= minCount;
                        case 858:
                            return Game1.player.QiGems >= minCount;
                        default:
                            return Game1.player.hasItemInInventory(itemId, minCount);
                    }
                });
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_PROFESSION(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int.TryParse(query[2], out var professionId);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.professions.Contains(professionId));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_READ_LETTER(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string letterId = string.Join(" ", query.Skip(2));
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.mailReceived.Contains(letterId));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_SECRET_NOTE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int seenNote = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.secretNotesSeen.Contains(seenNote));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_SEEN_EVENT(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int eventId = Convert.ToInt32(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.eventsSeen.Contains(eventId));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_TOWN_KEY(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = ArgUtility.Get(query, 1);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.HasTownKey);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_TRASH_CAN_LEVEL(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = ArgUtility.Get(query, 1);
                int requiredLevel = ArgUtility.GetInt(query, 2);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.trashCanLevel == requiredLevel);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_LOCATION_CONTEXT(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string name = query[2];
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => string.Equals(target.currentLocation.GetLocationContext().ToString(), name, StringComparison.OrdinalIgnoreCase));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_LOCATION_NAME(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string name = query[2];
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => string.Equals(target.currentLocation.Name, name, StringComparison.OrdinalIgnoreCase));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_LOCATION_UNIQUE_NAME(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string name = query[2];
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => string.Equals(target.currentLocation.NameOrUniqueName, name, StringComparison.OrdinalIgnoreCase));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_MOD_DATA(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string key = query[2];
                string value = query[3];
                string value2;
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.modData.TryGetValue(key, out value2) && string.Equals(value2, value, StringComparison.OrdinalIgnoreCase));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_MONEY_EARNED(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int targetAmount = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.totalMoneyEarned >= targetAmount);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_SPECIAL_ORDER_ACTIVE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = ArgUtility.Get(query, 1);
                string orderId = ArgUtility.Get(query, 2);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.team.SpecialOrderActive(orderId));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_SPECIAL_ORDER_RULE_ACTIVE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = ArgUtility.Get(query, 1);
                string ruleId = ArgUtility.Get(query, 2);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.team.SpecialOrderRuleActive(ruleId));
            }

            /// <summary>Get whether the given player has a minimum value for a <see cref="P:StardewValley.Game1.stats" /> field returned by <see cref="M:StardewValley.Stats.getStat(System.String)" />.</summary>
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_STAT(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = ArgUtility.Get(query, 1);
                string name = ArgUtility.Get(query, 2);
                string rawMinValue = ArgUtility.Get(query, 3);
                if (string.IsNullOrEmpty(playerKey) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(rawMinValue))
                {
                    return Helpers.ErrorResult(query, "must specify three query (player key, stat name, and minimum value)");
                }
                if (!uint.TryParse(rawMinValue, out var minValue))
                {
                    return Helpers.ErrorResult(query, "invalid minimum value '" + rawMinValue + "', must be an integer");
                }
                return Helpers.WithPlayer(player, playerKey, delegate (Farmer target)
                {
                    Stats stats = target.stats;
                    if (stats.stat_dictionary.TryGetValue(name, out var value))
                    {
                        return value >= minValue;
                    }
                    return stats.GetType().GetField(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)?.GetValue(stats) is uint num && num >= minValue;
                });
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_CHILDREN(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                int minChildren = int.Parse(query[2]);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.getChildrenCount() >= minChildren);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_PET(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.hasPet());
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HEARTS(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string npcName = query[2];
                int minHearts = int.Parse(query[3]);
                return Helpers.WithPlayer(player, playerKey, delegate (Farmer target)
                {
                    if (string.Equals(npcName, "Any", StringComparison.OrdinalIgnoreCase))
                    {
                        return target.hasAFriendWithHeartLevel(minHearts, datablesOnly: false);
                    }
                    return string.Equals(npcName, "AnyDateable", StringComparison.OrdinalIgnoreCase) ? target.hasAFriendWithHeartLevel(minHearts, datablesOnly: true) : (target.getFriendshipHeartLevelForNPC(npcName) >= minHearts);
                });
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_HAS_MET(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string name = query[2];
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.friendshipData.ContainsKey(name));
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_IS_DATING(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string npcName = query[2];
                return Helpers.WithPlayer(player, playerKey, delegate (Farmer target)
                {
                    Friendship value;
                    if (string.Equals(npcName, "Any", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (Friendship value2 in target.friendshipData.Values)
                        {
                            if (value2.IsDating())
                            {
                                return true;
                            }
                        }
                    }
                    else if (target.friendshipData.TryGetValue(query[2], out value))
                    {
                        return value.IsDating();
                    }
                    return false;
                });
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_IS_ENGAGED(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string npcName = query[2];
                return Helpers.WithPlayer(player, playerKey, delegate (Farmer target)
                {
                    if (string.Equals(npcName, "Any", StringComparison.OrdinalIgnoreCase))
                    {
                        return target.isEngaged();
                    }
                    if (string.Equals(npcName, "Player", StringComparison.OrdinalIgnoreCase))
                    {
                        return target.team.IsEngaged(target.UniqueMultiplayerID);
                    }
                    Friendship value;
                    return target.friendshipData.TryGetValue(query[2], out value) && value.IsEngaged();
                });
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_IS_MARRIED(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string npcName = query[2];
                return Helpers.WithPlayer(player, playerKey, delegate (Farmer target)
                {
                    if (string.Equals(npcName, "Any", StringComparison.OrdinalIgnoreCase))
                    {
                        return target.isMarried();
                    }
                    if (string.Equals(npcName, "Player", StringComparison.OrdinalIgnoreCase))
                    {
                        return target.team.GetSpouse(target.UniqueMultiplayerID).HasValue;
                    }
                    Friendship value;
                    return target.friendshipData.TryGetValue(query[2], out value) && value.IsMarried();
                });
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_IS_DIVORCED(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string npcName = query[2];
                return Helpers.WithPlayer(player, playerKey, delegate (Farmer target)
                {
                    if (string.Equals(npcName, "Any", StringComparison.OrdinalIgnoreCase))
                    {
                        return target.isDivorced();
                    }
                    Friendship value;
                    return target.friendshipData.TryGetValue(query[2], out value) && value.IsDivorced();
                });
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_IS_ROOMMATE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string npcName = query[2];
                return Helpers.WithPlayer(player, playerKey, delegate (Farmer target)
                {
                    if (string.Equals(npcName, "Any", StringComparison.OrdinalIgnoreCase))
                    {
                        return target.hasRoommate();
                    }
                    if (string.Equals(npcName, "Player", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                    Friendship value;
                    return target.friendshipData.TryGetValue(query[2], out value) && value.IsRoommate();
                });
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_PLAYER_PREFERRED_PET(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                string playerKey = query[1];
                string petId = query[2];
                bool catPerson = string.Equals(petId, "Cat", StringComparison.OrdinalIgnoreCase);
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.catPerson == catPerson);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_RANDOM(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return RandomImpl(random, query, 1);
            }

            /// <summary>The common implementation for most <c>RANDOM</c> game state queries.</summary>
            /// <param name="random">The random instance to use.</param>
            /// <param name="query">The condition arguments received by the query.</param>
            /// <param name="skipArguments">The number of arguments to skip. The next argument should be the chance value, followed by an optional <c>@addDailyLuck</c> argument.</param>
            public static bool RandomImpl(Random random, string[] query, int skipArguments)
            {
                if (!ArgUtility.TryGetFloat(query, skipArguments, out var chance, out var error))
                {
                    return Helpers.ErrorResult(query, error);
                }
                bool addDailyLuck = false;
                for (int i = skipArguments + 1; i < query.Length; i++)
                {
                    if (string.Equals(query[i], "@addDailyLuck", StringComparison.OrdinalIgnoreCase))
                    {
                        addDailyLuck = true;
                    }
                }
                if (addDailyLuck)
                {
                    chance += (float)Game1.player.DailyLuck;
                }
                return random.NextDouble() < (double)chance;
            }

            /// <summary>Get whether the target item has all of the given context tags.</summary>
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_ITEM_HAS_TAG(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                if (item == null)
                {
                    return false;
                }
                for (int i = 1; i < query.Length; i++)
                {
                    if (!item.HasContextTag(query[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>Get whether the target item has a min stack size (ignoring other stacks in the inventory).</summary>
            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_ITEM_STACK(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return item?.Stack >= int.Parse(query[1]);
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_TRUE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return true;
            }

            /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
            public static bool query_FALSE(string[] query, GameLocation location, Farmer player, Item item, Random random)
            {
                return false;
            }
        }

        /// <summary>The cached metadata for a raw game state query.</summary>
        public readonly struct ParsedGameStateQuery
        {
            /// <summary>Whether the result should be negated.</summary>
            public readonly bool Negated;

            /// <summary>The game state query split by space, including the query key.</summary>
            public readonly string[] Query;

            /// <summary>The resolver which handles the game state query.</summary>
            public readonly GameStateQueryDelegate Resolver;

            /// <summary>An error indicating why the query is invalid, if applicable.</summary>
            public readonly string Error;

            /// <summary>Construct an instance.</summary>
            /// <param name="negated">Whether the result should be negated.</param>
            /// <param name="query">The game state query split by space, including the query key.</param>
            /// <param name="resolver">The resolver which handles the game state query.</param>
            public ParsedGameStateQuery(bool negated, string[] query, GameStateQueryDelegate resolver, string error)
            {
                Negated = negated;
                Query = query;
                Resolver = resolver;
                Error = error;
            }
        }

        /// <summary>The prefix for the default resolver methods on this class.</summary>
        private const string QueryPrefix = "query_";

        /// <summary>The supported game state queries and their resolvers.</summary>
        private static readonly Dictionary<string, GameStateQueryDelegate> QueryTypeLookup;

        /// <summary>The <see cref="F:StardewValley.Game1.ticks" /> value when the cache should be reset.</summary>
        private static int NextClearCacheTick;

        /// <summary>The cache of parsed game state queries.</summary>
        private static readonly Dictionary<string, ParsedGameStateQuery[]> ParseCache;

        /// <summary>The query keys which check the season, like <c>LOCATION_SEASON</c> or <c>SEASON</c>.</summary>
        public static HashSet<string> SeasonQueryKeys;

        /// <summary>The query keys which are ignored when catching fish with the Magic Bait equipped.</summary>
        public static HashSet<string> MagicBaitIgnoreQueryKeys;

        /// <summary>Register the default game state queries, defined as <see cref="T:StardewValley.GameStateQuery.DefaultResolvers" /> methods whose name is prefixed with <see cref="F:StardewValley.GameStateQuery.QueryPrefix" />.</summary>
        static GameStateQuery()
        {
            ParseCache = new Dictionary<string, ParsedGameStateQuery[]>();
            SeasonQueryKeys = new HashSet<string> { "LOCATION_SEASON", "SEASON" };
            MagicBaitIgnoreQueryKeys = new HashSet<string> { "DAY_OF_MONTH", "DAY_OF_WEEK", "DAYS_PLAYED", "LOCATION_SEASON", "SEASON", "SEASON_DAY", "WEATHER", "TIME" };
            QueryTypeLookup = new Dictionary<string, GameStateQueryDelegate>(StringComparer.OrdinalIgnoreCase);
            MethodInfo[] methods = typeof(DefaultResolvers).GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                if (method.Name.StartsWith("query_"))
                {
                    GameStateQueryDelegate queryDelegate = (GameStateQueryDelegate)Delegate.CreateDelegate(typeof(GameStateQueryDelegate), method);
                    RegisterQueryType(method.Name.Substring("query_".Length), queryDelegate);
                }
            }
        }

        /// <summary>Update the game state query tracking.</summary>
        internal static void Update()
        {
            if (Game1.ticks >= NextClearCacheTick)
            {
                if (ParseCache.Count > 50)
                {
                    ParseCache.Clear();
                }
                NextClearCacheTick = Game1.ticks + 3600;
            }
        }

        /// <summary>Register a game state query resolver.</summary>
        /// <param name="queryKey">The game state query key, like <c>SEASON</c>. This should only contain alphanumeric, underscore, and dot characters. For custom queries, this should be prefixed with your mod ID like <c>Example.ModId_QueryName</c>.</param>
        /// <param name="queryDelegate">The resolver which returns whether a given query matches in the current context.</param>
        public static void RegisterQueryType(string queryKey, GameStateQueryDelegate queryDelegate)
        {
            QueryTypeLookup[queryKey] = queryDelegate;
        }

        /// <summary>Get whether a set of game state queries matches in the current context.</summary>
        /// <param name="queryString">The game state queries to check as a comma-delimited string.</param>
        /// <param name="location">The location for which to check the query, or <c>null</c> to use the current location.</param>
        /// <param name="player">The player for which to check the query, or <c>null</c> to use the current player.</param>
        /// <param name="item">The item for which to check the query, or <c>null</c> if not applicable.</param>
        /// <param name="random">The RNG to use for randomization, or <c>null</c> to use <see cref="F:StardewValley.Game1.random" />.</param>
        /// <param name="ignoreQueryKeys">The query keys to ignore when checking conditions (like <c>LOCATION_SEASON</c>), or <c>null</c> to check all of them.</param>
        /// <returns>Returns whether the query matches.</returns>
        public static bool CheckConditions(string queryString, GameLocation location = null, Farmer player = null, Item item = null, Random random = null, HashSet<string> ignoreQueryKeys = null)
        {
            if (queryString != null && (queryString == null || queryString.Length != 0) && !(queryString == "TRUE"))
            {
                if (queryString == "FALSE")
                {
                    return false;
                }
                ParsedGameStateQuery[] parsed = Parse(queryString);
                if (parsed.Length == 0)
                {
                    return true;
                }
                if (parsed[0].Error != null)
                {
                    return Helpers.ErrorResult(parsed[0].Query, parsed[0].Error);
                }
                player = player ?? Game1.player;
                location = location ?? player?.currentLocation ?? Game1.currentLocation;
                random = random ?? Game1.random;
                ParsedGameStateQuery[] array = parsed;
                for (int i = 0; i < array.Length; i++)
                {
                    ParsedGameStateQuery query = array[i];
                    if (ignoreQueryKeys != null && ignoreQueryKeys.Contains(query.Query[0]))
                    {
                        continue;
                    }
                    try
                    {
                        if (query.Resolver(query.Query, location, player, item, random) == query.Negated)
                        {
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        return Helpers.ErrorResult(query.Query, "unhandled exception", e);
                    }
                }
                return true;
            }
            return true;
        }

        private static DayOfWeek GetDayOfWeekFor(int dayOfMonth)
        {
            return (DayOfWeek)(dayOfMonth % 7);
        }

        private static bool TryGetDayOfWeekFor(string day, out DayOfWeek dayOfWeek)
        {
            if (int.TryParse(day, out var numeric))
            {
                dayOfWeek = GetDayOfWeekFor(numeric);
                return true;
            }
            switch (day?.ToLower())
            {
                case "mon":
                case "monday":
                    dayOfWeek = DayOfWeek.Monday;
                    return true;
                case "tue":
                case "tuesday":
                    dayOfWeek = DayOfWeek.Tuesday;
                    return true;
                case "wed":
                case "wednesday":
                    dayOfWeek = DayOfWeek.Wednesday;
                    return true;
                case "thu":
                case "thursday":
                    dayOfWeek = DayOfWeek.Thursday;
                    return true;
                case "fri":
                case "friday":
                    dayOfWeek = DayOfWeek.Friday;
                    return true;
                case "sat":
                case "saturday":
                    dayOfWeek = DayOfWeek.Saturday;
                    return true;
                case "sun":
                case "sunday":
                    dayOfWeek = DayOfWeek.Sunday;
                    return true;
                default:
                    dayOfWeek = DayOfWeek.Sunday;
                    return false;
            }
        }

        /// <summary>Parse a raw query string into its component query data.</summary>
        /// <param name="queryString">The query string to parse.</param>
        /// <returns>Returns the parsed game state queries. This value is cached, so it should not be modified. If any part of the query string is invalid, this returns a single value containing the invalid query with the error property set.</returns>
        public static ParsedGameStateQuery[] Parse(string queryString)
        {
            if (!ParseCache.TryGetValue(queryString, out var parsed))
            {
                string[] rawQueries = queryString.Split(',', StringSplitOptions.RemoveEmptyEntries);
                parsed = new ParsedGameStateQuery[rawQueries.Length];
                for (int i = 0; i < rawQueries.Length; i++)
                {
                    string[] query = ArgUtility.SplitBySpace(rawQueries[i]);
                    bool negated = query[0].StartsWith('!');
                    if (negated)
                    {
                        query[0] = query[0].Substring(1);
                    }
                    if (!QueryTypeLookup.TryGetValue(query[0], out var resolver))
                    {
                        if (parsed.Length > 1)
                        {
                            parsed = new ParsedGameStateQuery[1];
                        }
                        parsed[0] = new ParsedGameStateQuery(negated: false, query, null, "'" + query[0] + "' isn't a known query");
                        break;
                    }
                    parsed[i] = new ParsedGameStateQuery(negated, query, resolver, null);
                }
                ParseCache[queryString] = parsed;
            }
            return parsed;
        }
    }
}