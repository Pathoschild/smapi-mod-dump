/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Archery.Framework.Utilities.Backport
{
    // TODO: When updated to SDV v1.6, this class should be deleted (except where needed) in favor of using StardewValley.GameStateQuery
    public class GameStateQuery
    {
        public delegate bool QueryDelegate(string[] condition_split);

        protected static Dictionary<string, QueryDelegate> _queryTypeLookup;

        protected static double _PickedValue;

        protected static Random _SeededRandom;

        protected static Farmer _CurrentTargetFarmer;

        protected static Item _CurrentTargetItem;

        protected static GameLocation _CurrentTargetGameLocation;

        public static void SetupQueryTypes()
        {
            if (GameStateQuery._queryTypeLookup == null)
            {
                GameStateQuery._queryTypeLookup = new Dictionary<string, QueryDelegate>();
                MethodInfo[] array = (from method_info in typeof(GameStateQuery).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                      where method_info.Name.StartsWith("query_")
                                      select method_info).ToArray();
                foreach (MethodInfo methodInfo in array)
                {
                    QueryDelegate query_delegate = (QueryDelegate)Delegate.CreateDelegate(typeof(QueryDelegate), methodInfo);
                    GameStateQuery.RegisterQueryType(methodInfo.Name.Substring("query_".Length), query_delegate);
                }
                Archery.monitor.Log(String.Format("SetupQueryTypes() registered '{0}' methods", GameStateQuery._queryTypeLookup.Count), LogLevel.Trace);
            }
        }

        public static void PickRandomValue(Random r)
        {
            if (r == null)
            {
                r = Game1.random;
            }
            GameStateQuery._PickedValue = r.NextDouble();
        }

        public static Random GetRandom()
        {
            if (GameStateQuery._SeededRandom != null)
            {
                return GameStateQuery._SeededRandom;
            }
            return Game1.random;
        }

        public static void RegisterQueryType(string query_name, QueryDelegate query_delegate)
        {
            GameStateQuery._queryTypeLookup[query_name] = query_delegate;
        }

        public static string RemoveCondition(string condition_query, string condition_name, out string removed_condition)
        {
            if (condition_query == null)
            {
                removed_condition = null;
                return condition_query;
            }
            List<string> list = new List<string>(condition_query.Split(','));
            char[] separator = new char[1] { ' ' };
            for (int i = 0; i < list.Count; i++)
            {
                string text = list[i];
                string[] array = text.Split(separator, 2);
                if (array[0].StartsWith("!"))
                {
                    array[0] = array[0].Substring(1);
                }
                if (array[0] == condition_name)
                {
                    removed_condition = text;
                    list.RemoveAt(i);
                    return string.Join(" ", list);
                }
            }
            removed_condition = null;
            return condition_query;
        }

        public static bool CheckConditions(string condition_query, Random seeded_random = null, Farmer target_farmer = null, Item target_item = null, GameLocation game_location = null)
        {
            try
            {
                GameStateQuery._SeededRandom = seeded_random;
                if (condition_query == null)
                {
                    return true;
                }
                string[] array = condition_query.Split(',');
                foreach (string text in array)
                {
                    if (text.Length != 0)
                    {
                        bool flag = true;
                        string[] array2 = text.Trim().Split(' ');
                        if (array2[0].StartsWith("!"))
                        {
                            flag = false;
                            array2[0] = array2[0].Substring(1);
                        }
                        if (GameStateQuery._CheckCondition(array2, target_farmer, target_item, game_location) != flag)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            finally
            {
                GameStateQuery._SeededRandom = null;
            }
        }

        protected static bool _CheckCondition(string[] condition_split, Farmer target_farmer = null, Item target_item = null, GameLocation game_location = null)
        {
            GameStateQuery._CurrentTargetFarmer = target_farmer;
            GameStateQuery._CurrentTargetItem = target_item;
            GameStateQuery._CurrentTargetGameLocation = game_location;
            try
            {
                if (GameStateQuery._queryTypeLookup.TryGetValue(condition_split[0], out var value))
                {
                    try
                    {
                        return value(condition_split);
                    }
                    catch (Exception ex)
                    {
                        Archery.monitor.Log("ERROR parsing condition: " + string.Join(" ", condition_split) + "\n" + ex.ToString(), LogLevel.Trace);
                        return false;
                    }
                }
                Archery.monitor.Log(String.Format("ERROR: Invalid condition: {0}", condition_split[0]), LogLevel.Trace);
                return false;
            }
            finally
            {
                GameStateQuery._CurrentTargetFarmer = null;
                GameStateQuery._CurrentTargetItem = null;
                GameStateQuery._CurrentTargetGameLocation = null;
            }
        }

        public static GameLocation GetLocation(string location_name)
        {
            if (location_name == "Here")
            {
                return Game1.currentLocation;
            }
            if (location_name == "Target")
            {
                if (GameStateQuery._CurrentTargetGameLocation == null)
                {
                    if (GameStateQuery._CurrentTargetFarmer != null)
                    {
                        return GameStateQuery._CurrentTargetFarmer.currentLocation;
                    }
                    return Game1.currentLocation;
                }
                return GameStateQuery._CurrentTargetGameLocation;
            }
            return Game1.getLocationFromName(location_name);
        }

        public static bool WithPlayer(string target, Func<Farmer, bool> check)
        {
            switch (target)
            {
                case "Any":
                    foreach (Farmer allFarmer in Game1.getAllFarmers())
                    {
                        if (check(allFarmer))
                        {
                            return true;
                        }
                    }
                    return false;
                case "All":
                    foreach (Farmer allFarmer2 in Game1.getAllFarmers())
                    {
                        if (!check(allFarmer2))
                        {
                            return false;
                        }
                    }
                    return true;
                case "Current":
                    return check(Game1.player);
                case "Target":
                    return check(GameStateQuery._GetTargetPlayer());
                case "Host":
                    return check(Game1.MasterPlayer);
                default:
                    {
                        if (long.TryParse(target, out var result))
                        {
                            return check(Game1.getFarmerMaybeOffline(result));
                        }
                        return false;
                    }
            }
        }

        protected static Farmer _GetTargetPlayer()
        {
            if (GameStateQuery._CurrentTargetFarmer == null)
            {
                return Game1.player;
            }
            return GameStateQuery._CurrentTargetFarmer;
        }

        public static bool query_PICKED_VALUE(string[] condition_split)
        {
            int num = int.Parse(condition_split[1]);
            int num2 = int.Parse(condition_split[2]) + 1;
            int num3 = int.Parse(condition_split[3]);
            return (int)((double)num + GameStateQuery._PickedValue * (double)(num2 - num)) == num3;
        }

        public static bool query_PICKED_VALUE_CHANCE(string[] condition_split)
        {
            return GameStateQuery._PickedValue <= (double)float.Parse(condition_split[1]);
        }

        public static bool query_PICKED_VALUE_SUMMER_RAIN_CHANCE(string[] condition_split)
        {
            return GameStateQuery._PickedValue <= (double)(float.Parse(condition_split[1]) + (float)Game1.dayOfMonth * float.Parse(condition_split[2]));
        }

        public static bool query_PICKED_VALUE_DAYS(string[] condition_split)
        {
            int minValue = int.Parse(condition_split[1]);
            int maxValue = int.Parse(condition_split[2]) + 1;
            int num = 0;
            if (condition_split.Length > 4)
            {
                num = int.Parse(condition_split[4]);
            }
            return new Random((int)Game1.stats.DaysPlayed + num).Next(minValue, maxValue) == int.Parse(condition_split[3]);
        }

        public static bool query_PICKED_VALUE_TICK(string[] condition_split)
        {
            int minValue = int.Parse(condition_split[1]);
            int maxValue = int.Parse(condition_split[2]) + 1;
            int num = 0;
            if (condition_split.Length > 4)
            {
                num = int.Parse(condition_split[4]);
            }
            return new Random(Game1.ticks + num).Next(minValue, maxValue) == int.Parse(condition_split[3]);
        }

        public static bool query_SEASON(string[] condition_split)
        {
            return Game1.currentSeason == condition_split[1];
        }

        public static bool query_YEAR(string[] condition_split)
        {
            return Game1.year >= int.Parse(condition_split[1]);
        }

        public static bool query_RANDOM(string[] condition_split)
        {
            return GameStateQuery.GetRandom().NextDouble() < (double)float.Parse(condition_split[1]);
        }

        public static bool query_TIME(string[] condition_split)
        {
            int num = int.Parse(condition_split[1]);
            int num2 = int.Parse(condition_split[2]);
            if (num >= 0 && Game1.timeOfDay < num)
            {
                return false;
            }
            if (num2 >= 0 && Game1.timeOfDay > num2)
            {
                return false;
            }
            return true;
        }

        public static bool query_DAY_OF_WEEK(string[] condition_split)
        {
            return Game1.dayOfMonth % 7 == int.Parse(condition_split[1]);
        }

        public static bool query_DAY_OF_MONTH(string[] condition_split)
        {
            return Game1.dayOfMonth == int.Parse(condition_split[1]);
        }

        public static bool query_DAYS_PLAYED(string[] condition_split)
        {
            return Game1.stats.DaysPlayed >= int.Parse(condition_split[1]);
        }

        public static bool query_PLAYER_FARMING_LEVEL(string[] condition_split)
        {
            int target_value = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.FarmingLevel >= target_value);
        }

        public static bool query_PLAYER_MINING_LEVEL(string[] condition_split)
        {
            int target_value = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.MiningLevel >= target_value);
        }

        public static bool query_PLAYER_FORAGING_LEVEL(string[] condition_split)
        {
            int target_value = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.ForagingLevel >= target_value);
        }

        public static bool query_PLAYER_FISHING_LEVEL(string[] condition_split)
        {
            int target_value = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.FishingLevel >= target_value);
        }

        public static bool query_PLAYER_COMBAT_LEVEL(string[] condition_split)
        {
            int target_value = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.CombatLevel >= target_value);
        }

        public static bool query_PLAYER_HAS_CRAFTING_RECIPE(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], delegate (Farmer target_farmer)
            {
                string key = string.Join(" ", condition_split, 2, condition_split.Length - 2);
                return target_farmer.craftingRecipes.ContainsKey(key);
            });
        }

        public static bool query_PLAYER_HAS_COOKING_RECIPE(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], delegate (Farmer target_farmer)
            {
                string key = string.Join(" ", condition_split, 2, condition_split.Length - 2);
                return target_farmer.cookingRecipes.ContainsKey(key);
            });
        }

        public static bool query_PLAYER_HEARTS(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], delegate (Farmer target_farmer)
            {
                if (condition_split[2] == "Any")
                {
                    return target_farmer.hasAFriendWithHeartLevel(int.Parse(condition_split[3]), datablesOnly: false);
                }
                return (condition_split[2] == "AnyDateable") ? target_farmer.hasAFriendWithHeartLevel(int.Parse(condition_split[3]), datablesOnly: true) : (target_farmer.getFriendshipHeartLevelForNPC(condition_split[2]) >= int.Parse(condition_split[3]));
            });
        }

        public static bool query_PLAYER_HAS_ALL_ACHIEVEMENTS(string[] condition_split)
        {
            Dictionary<int, string> achievement_data = Game1.content.Load<Dictionary<int, string>>("Data\\Achievements");
            return GameStateQuery.WithPlayer(condition_split[1], delegate (Farmer target_farmer)
            {
                foreach (int key in achievement_data.Keys)
                {
                    if (!target_farmer.achievements.Contains(key))
                    {
                        return false;
                    }
                }
                return true;
            });
        }

        public static bool query_ITEM_STACK(string[] condition_split)
        {
            if (GameStateQuery._CurrentTargetItem == null)
            {
                return false;
            }
            if (GameStateQuery._CurrentTargetItem.Stack >= int.Parse(condition_split[1]))
            {
                return true;
            }
            return false;
        }

        public static bool query_ITEM_QUALITY(string[] condition_split)
        {
            if (GameStateQuery._CurrentTargetItem == null)
            {
                return false;
            }
            if (!(GameStateQuery._CurrentTargetItem is StardewValley.Object))
            {
                return false;
            }
            if ((GameStateQuery._CurrentTargetItem as StardewValley.Object).Quality >= int.Parse(condition_split[1]))
            {
                return true;
            }
            return false;
        }

        public static bool query_ITEM_ID(string[] condition_split)
        {
            if (GameStateQuery._CurrentTargetItem == null)
            {
                return false;
            }
            return GameStateQuery._CurrentTargetItem.ParentSheetIndex.ToString() == condition_split[1];
        }

        public static bool query_ITEM_HAS_TAG(string[] condition_split)
        {
            if (GameStateQuery._CurrentTargetItem == null)
            {
                return false;
            }
            for (int i = 1; i < condition_split.Length; i++)
            {
                if (!GameStateQuery._CurrentTargetItem.HasContextTag(condition_split[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool query_PLAYER_HAS_PROFESSION(string[] condition_split)
        {
            int profession_id = 0;
            int.TryParse(condition_split[2], out profession_id);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.professions.Contains(profession_id));
        }

        public static bool query_PLAYER_HAS_ACHIEVEMENT(string[] condition_split)
        {
            int achievement_id = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.achievements.Contains(achievement_id));
        }

        public static bool query_PLAYER_LOCATION_NAME(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => (target_farmer.currentLocation.Name == condition_split[2]) ? true : false);
        }

        public static bool query_PLAYER_LOCATION_CONTEXT(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => (target_farmer.currentLocation.GetLocationContext().ToString() == condition_split[2]) ? true : false);
        }

        public static bool query_PLAYER_LOCATION_UNIQUE_NAME(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => (target_farmer.currentLocation.NameOrUniqueName == condition_split[2]) ? true : false);
        }

        public static bool query_PLAYER_HAS_MET(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.friendshipData.ContainsKey(condition_split[2]));
        }

        public static bool query_PLAYER_IS_UNMET(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => (!target_farmer.friendshipData.ContainsKey(condition_split[2])) ? true : false);
        }

        public static bool query_PLAYER_IS_DATING(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], delegate (Farmer target_farmer)
            {
                if (condition_split[2] == "Any")
                {
                    foreach (Friendship value in target_farmer.friendshipData.Values)
                    {
                        if (value.IsDating())
                        {
                            return true;
                        }
                    }
                }
                return target_farmer.friendshipData.ContainsKey(condition_split[2]) && target_farmer.friendshipData[condition_split[2]].IsDating();
            });
        }

        public static bool query_PLAYER_IS_MARRIED(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], delegate (Farmer target_farmer)
            {
                if (condition_split[2] == "Any")
                {
                    return target_farmer.isMarried();
                }
                if (condition_split[2] == "Player")
                {
                    return target_farmer.team.GetSpouse(target_farmer.UniqueMultiplayerID).HasValue;
                }
                return target_farmer.friendshipData.ContainsKey(condition_split[2]) && target_farmer.friendshipData[condition_split[2]].IsMarried();
            });
        }

        public static bool query_PLAYER_IS_ROOMMATE(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], delegate (Farmer target_farmer)
            {
                if (condition_split[2] == "Any")
                {
                    return target_farmer.hasRoommate();
                }
                if (condition_split[2] == "Player")
                {
                    return false;
                }
                return target_farmer.friendshipData.ContainsKey(condition_split[2]) && target_farmer.friendshipData[condition_split[2]].IsRoommate();
            });
        }

        public static bool query_PLAYER_IS_ENGAGED(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], delegate (Farmer target_farmer)
            {
                if (condition_split[2] == "Any")
                {
                    return target_farmer.isEngaged();
                }
                if (condition_split[2] == "Player")
                {
                    return target_farmer.team.IsEngaged(target_farmer.UniqueMultiplayerID);
                }
                return target_farmer.friendshipData.ContainsKey(condition_split[2]) && target_farmer.friendshipData[condition_split[2]].IsEngaged();
            });
        }

        public static bool query_IS_COMMUNITY_CENTER_COMPLETE(string[] condition_split)
        {
            if (Game1.MasterPlayer.hasCompletedCommunityCenter())
            {
                return !Game1.MasterPlayer.mailReceived.Contains("JojaMember");
            }
            return false;
        }

        public static bool query_IS_JOJA_MART_COMPLETE(string[] condition_split)
        {
            if (Game1.MasterPlayer.hasCompletedCommunityCenter())
            {
                return Game1.MasterPlayer.mailReceived.Contains("JojaMember");
            }
            return false;
        }

        public static bool query_PLAYER_IS_DIVORCED(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], delegate (Farmer target_farmer)
            {
                if (condition_split[2] == "Any")
                {
                    return target_farmer.isDivorced();
                }
                return target_farmer.friendshipData.ContainsKey(condition_split[2]) && target_farmer.friendshipData[condition_split[2]].IsDivorced();
            });
        }

        public static bool query_PLAYER_HAS_CHILDREN(string[] condition_split)
        {
            int minimum_children = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.getChildrenCount() >= minimum_children);
        }

        public static bool query_PLAYER_FARMHOUSE_UPGRADE(string[] condition_split)
        {
            int minimum_upgrade_level = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.HouseUpgradeLevel >= minimum_upgrade_level);
        }

        public static bool query_PLAYER_HAS_SECRET_NOTE(string[] condition_split)
        {
            int seen_note = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.secretNotesSeen.Contains(seen_note));
        }

        public static bool query_PLAYER_HAS_PET(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => (!target_farmer.hasPet()) ? true : false);
        }

        public static bool query_IS_PASSIVE_FESTIVAL_TODAY(string[] condition_split)
        {
            // Not implemented with this backport
            return false;
        }

        public static bool query_IS_PASSIVE_FESTIVAL_OPEN(string[] condition_split)
        {
            // Not implemented with this backport
            return false;
        }

        public static bool query_IS_FESTIVAL_DAY(string[] condition_split)
        {
            int totalDays = Game1.Date.TotalDays;
            int num = 0;
            if (condition_split.Length > 1)
            {
                num = int.Parse(condition_split[1]);
            }
            int num2 = (totalDays + num) % 112;
            int num3 = num2 / 28;
            int num4 = num2 % 28;
            return num3 switch
            {
                0 => Utility.isFestivalDay(num4 + 1, "spring"),
                1 => Utility.isFestivalDay(num4 + 1, "summer"),
                2 => Utility.isFestivalDay(num4 + 1, "fall"),
                3 => Utility.isFestivalDay(num4 + 1, "winter"),
                _ => false,
            };
        }

        public static bool query_PLAYER_PREFERRED_PET(string[] condition_split)
        {
            bool target_pet = false;
            if (condition_split[2] == "Cat")
            {
                target_pet = true;
            }
            else if (condition_split[2] == "Dog")
            {
                target_pet = false;
            }
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.hasPet() && target_farmer.catPerson == target_pet);
        }

        public static bool query_PLAYER_GENDER(string[] condition_split)
        {
            bool target_gender = false;
            if (condition_split[2] == "Male")
            {
                target_gender = true;
            }
            else if (condition_split[2] == "Female")
            {
                target_gender = false;
            }
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.IsMale == target_gender);
        }

        public static bool query_PLAYER_MONEY_EARNED(string[] condition_split)
        {
            int target_amount = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.totalMoneyEarned >= target_amount);
        }

        public static bool query_PLAYER_CURRENT_MONEY(string[] condition_split)
        {
            int target_amount = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.Money >= target_amount);
        }

        public static bool query_PLAYER_MOD_DATA(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.modData.ContainsKey(condition_split[2]) && target_farmer.modData[condition_split[2]] == condition_split[3]);
        }

        public static bool query_PLAYER_HAS_READ_LETTER(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.mailReceived.Contains(string.Join(" ", condition_split.Skip(2).ToArray())));
        }

        public static bool query_PLAYER_HAS_FLAG(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.hasOrWillReceiveMail(string.Join(" ", condition_split.Skip(2).ToArray())));
        }

        public static bool query_MINE_LOWEST_LEVEL_REACHED(string[] condition_split)
        {
            int num = int.Parse(condition_split[1]);
            return MineShaft.lowestLevelReached >= num;
        }

        public static bool query_PLAYER_HAS_SEEN_EVENT(string[] condition_split)
        {
            if (int.TryParse(condition_split[2], out int value) is false)
            {
                return false;
            }

            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.eventsSeen.Contains(value));
        }

        public static bool query_PLAYER_HAS_ITEM(string[] condition_split)
        {
            if (int.TryParse(condition_split[2], out int value) is false)
            {
                return false;
            }

            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.hasItemInInventory(value, 1));
        }

        public static bool query_PLAYER_HAS_ITEM_NAMED(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.hasItemInInventoryNamed(string.Join(" ", condition_split.Skip(2).ToArray())));
        }

        public static bool query_PLAYER_HAS_CAUGHT_FISH(string[] condition_split)
        {
            if (int.TryParse(condition_split[2], out int value) is false)
            {
                return false;
            }

            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.fishCaught.ContainsKey(value));
        }

        public static bool query_PLAYER_HAS_CONVERSATION_TOPIC(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.activeDialogueEvents.ContainsKey(condition_split[2]));
        }

        public static bool query_PLAYER_HAS_DIALOGUE_ANSWER(string[] condition_split)
        {
            if (int.TryParse(condition_split[2], out int value) is false)
            {
                return false;
            }

            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.DialogueQuestionsAnswered.Contains(value));
        }

        public static bool query_WEATHER(string[] condition_split)
        {
            GameLocation location = GameStateQuery.GetLocation(condition_split[1]);

            var weatherContext = Game1.netWorldState.Value.GetWeatherForLocation(location.GetLocationContext());
            switch (condition_split[2].ToUpper())
            {
                case "FESTIVAL":
                    return Game1.isFestival();
                case "RAIN":
                case "RAINY":
                    return weatherContext.isRaining.Value || weatherContext.isLightning.Value;
                case "SNOW":
                case "SNOWY":
                    return weatherContext.isSnowing.Value;
                case "STORM":
                case "STORMY":
                    return weatherContext.isLightning.Value;
                case "WIND":
                case "WINDY":
                    return weatherContext.isDebrisWeather.Value;
                default:
                    return weatherContext.isRaining.Value is false && weatherContext.isLightning.Value is false && weatherContext.isSnowing.Value is false;
            }
        }

        public static bool query_LOCATION_SEASON(string[] condition_split)
        {
            return Game1.GetSeasonForLocation(GameStateQuery.GetLocation(condition_split[1])) == condition_split[2];
        }

        public static bool query_LOCATION_IS_MINES(string[] condition_split)
        {
            return GameStateQuery.GetLocation(condition_split[1]) is MineShaft;
        }

        public static bool query_LOCATION_IS_SKULL_CAVE(string[] condition_split)
        {
            GameLocation location = GameStateQuery.GetLocation(condition_split[1]);
            if (location is MineShaft && (location as MineShaft).mineLevel >= 121)
            {
                return (location as MineShaft).mineLevel != 77377;
            }
            return false;
        }

        public static bool query_LOCATION_CONTEXT(string[] condition_split)
        {
            return GameStateQuery.GetLocation(condition_split[1]).GetLocationContext().ToString() == condition_split[2];
        }

        public static bool query_LOCATION_ACCESSIBLE(string[] condition_split)
        {
            return Game1.isLocationAccessible(condition_split[1]);
        }

        public static bool query_FARM_NAME(string[] condition_split)
        {
            return (string)GameStateQuery._GetTargetPlayer().farmName == string.Join(" ", condition_split, 2, condition_split.Length - 2);
        }

        public static bool query_FARM_TYPE(string[] condition_split)
        {
            return Game1.GetFarmTypeID() == condition_split[1];
        }

        public static bool query_FARM_CAVE(string[] condition_split)
        {
            if ((int)Game1.MasterPlayer.caveChoice == 1)
            {
                return condition_split[1] == "Bats";
            }
            if ((int)Game1.MasterPlayer.caveChoice == 2)
            {
                return condition_split[1] == "Mushrooms";
            }
            return condition_split[1] == "None";
        }

        public static bool query_IS_CUSTOM_FARM_TYPE(string[] condition_split)
        {
            return Game1.whichFarm == 7;
        }

        public static bool query_IS_HOST(string[] condition_split)
        {
            return Game1.IsMasterGame;
        }

        public static bool query_WORLD_STATE(string[] condition_split)
        {
            return NetWorldState.checkAnywhereForWorldStateID(condition_split[1]);
        }

        public static bool query_CAN_BUILD_CABIN(string[] condition_split)
        {
            int numberBuildingsConstructed = Game1.getFarm().buildings.Count(b => b.indoors is not null && b.indoors.Value is Cabin);
            if (Game1.IsMasterGame)
            {
                return numberBuildingsConstructed < Game1.CurrentPlayerLimit - 1;
            }
            return false;
        }

        public static bool query_CAN_BUILD_FOR_CABINS(string[] condition_split)
        {
            int numberBuildingsConstructed = Game1.getFarm().buildings.Count(b => b.indoors is not null && b.indoors.Value is Cabin);
            int buildingCountByName = Game1.getFarm().buildings.Count(b => b.textureName().Contains(condition_split[1]));

            return buildingCountByName < numberBuildingsConstructed + 1;
        }

        // TODO: When updated to SDV v1.6, add this query to the native GameStateQuery
        public static bool query_IS_PLAYER_HOLDING_ITEM(string[] condition_split)
        {
            if (int.TryParse(condition_split[2], out int itemId) is false)
            {
                return false;
            }

            int requiredStack = 1;
            if (condition_split.Length > 3 && int.TryParse(condition_split[3], out requiredStack) is false)
            {
                requiredStack = 1;
            }

            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.ActiveObject is Item item && item.ParentSheetIndex == itemId && item.Stack >= requiredStack);
        }

        // TODO: When updated to SDV v1.6, add this query to the native GameStateQuery
        public static bool query_IS_PLAYER_HOLDING_ANYTHING(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.ActiveObject is not null);
        }

        // TODO: When updated to SDV v1.6, add this query to the native GameStateQuery
        public static bool query_IS_PLAYER_HOLDING_TOOL(string[] condition_split)
        {
            return GameStateQuery.WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.CurrentTool is not null);
        }

        public static bool query_TRUE(string[] condition_split)
        {
            return true;
        }

        public static bool query_FALSE(string[] condition_split)
        {
            return false;
        }

        public static bool query_SEASON_DAY(string[] condition_split)
        {
            for (int i = 1; i < condition_split.Length; i += 2)
            {
                string season = condition_split[i];
                int day = int.Parse(condition_split[i + 1]);

                if (string.Equals(Game1.currentSeason, season, StringComparison.OrdinalIgnoreCase) && Game1.dayOfMonth == day)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool query_FOUND_ALL_LOST_BOOKS(string[] condition_split)
        {
            return Game1.netWorldState.Value.LostBooksFound.Value >= 21;
        }

        public static bool query_IS_ISLAND_NORTH_BRIDGE_FIXED(string[] condition_split)
        {
            return ((IslandNorth)Game1.getLocationFromName("IslandNorth"))?.bridgeFixed.Value ?? false;
        }

        public static bool query_IS_MULTIPLAYER(string[] condition_split)
        {
            return Game1.IsMultiplayer;
        }

        public static bool query_IS_VISITING_ISLAND(string[] condition_split)
        {
            string npcName = condition_split[1];
            if (npcName != null)
            {
                return Game1.IsVisitingIslandToday(npcName);
            }
            return false;
        }
        public static bool query_MUSEUM_DONATIONS(string[] condition_split)
        {
            if (condition_split.Length < 2 || !int.TryParse(condition_split[1], out var minCount))
            {
                Archery.monitor.Log(condition_split + "must specify the minimum number required");
                return false;
            }

            bool filtered = condition_split.Length > 2;
            int count = 0;
            foreach (var itemId in Game1.netWorldState.Value.MuseumPieces.Values)
            {
                if (filtered)
                {
                    string objectType = Game1.objectInformation.ContainsKey(itemId) ? Game1.objectInformation[itemId].Split('/')[3] : String.Empty;
                    for (int i = 2; i < condition_split.Length; i++)
                    {
                        if (objectType == condition_split[i])
                        {
                            count++;
                            break;
                        }
                    }
                }
                else
                {
                    count++;
                }
            }

            return count >= minCount;
        }

        public static bool query_WORLD_STATE_FIELD(string[] condition_split)
        {
            string name = condition_split[1];
            string expectedValue = condition_split[2];
            PropertyInfo property = typeof(NetWorldState).GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            if ((object)property == null)
            {
                return false;
            }
            object actualValue = property.GetValue(Game1.netWorldState.Value, null);
            if (actualValue != null)
            {
                if (actualValue is bool)
                {
                    bool actual3 = (bool)actualValue;
                    if (bool.TryParse(expectedValue, out var expectedBool))
                    {
                        return actual3 == expectedBool;
                    }
                    return false;
                }
                if (actualValue is int)
                {
                    int actual2 = (int)actualValue;
                    if (int.TryParse(expectedValue, out var expectedInt))
                    {
                        return actual2 == expectedInt;
                    }
                    return false;
                }
                string actual = actualValue as string;
                if (actual != null)
                {
                    return string.Equals(actual, expectedValue, StringComparison.OrdinalIgnoreCase);
                }
                return string.Equals(actualValue.ToString(), expectedValue, StringComparison.OrdinalIgnoreCase);
            }
            return string.Equals(expectedValue, "null", StringComparison.OrdinalIgnoreCase);
        }

        public static bool query_WORLD_STATE_ID(string[] condition_split)
        {
            return NetWorldState.checkAnywhereForWorldStateID(condition_split[1]);
        }

        public static bool query_PLAYER_LUCK_LEVEL(string[] condition_split)
        {
            string playerKey = condition_split[1];
            int level = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(playerKey, (Farmer target) => target.LuckLevel >= level);
        }

        public static bool query_PLAYER_HAS_TOWN_KEY(string[] condition_split)
        {
            string playerKey = condition_split[1];
            return GameStateQuery.WithPlayer(playerKey, (Farmer target) => target.HasTownKey);
        }

        public static bool query_PLAYER_HAS_TRASH_CAN_LEVEL(string[] condition_split)
        {
            string playerKey = condition_split[1];
            int requiredLevel = int.Parse(condition_split[2]);
            return GameStateQuery.WithPlayer(playerKey, (Farmer target) => target.trashCanLevel == requiredLevel);
        }

        public static bool query_PLAYER_SHIPPED_BASIC_ITEM(string[] condition_split)
        {
            string playerKey = condition_split[1];
            int itemId = int.Parse(condition_split[2]);
            string rawCount = condition_split[3];
            int count = 1;
            if (rawCount != null && !int.TryParse(rawCount, out count))
            {
                Archery.monitor.Log(condition_split + " can't parse '" + rawCount + "' as an integer count");
                return false;
            }

            int value;
            return GameStateQuery.WithPlayer(playerKey, (Farmer target) => target.basicShipped.TryGetValue(itemId, out value) && value >= count);
        }

        public static bool query_PLAYER_SPECIAL_ORDER_ACTIVE(string[] condition_split)
        {
            string playerKey = condition_split[1];
            string orderId = condition_split[2];
            return GameStateQuery.WithPlayer(playerKey, (Farmer target) => target.team.SpecialOrderActive(orderId));
        }

        public static bool query_PLAYER_SPECIAL_ORDER_RULE_ACTIVE(string[] condition_split)
        {
            string playerKey = condition_split[1];
            string ruleId = condition_split[2];
            return GameStateQuery.WithPlayer(playerKey, (Farmer target) => target.team.SpecialOrderRuleActive(ruleId));
        }

        public static bool query_PLAYER_STAT(string[] condition_split)
        {
            string playerKey = condition_split[1];
            string name = condition_split[2];
            string rawMinValue = condition_split[3];
            if (string.IsNullOrEmpty(playerKey) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(rawMinValue))
            {
                Archery.monitor.Log(condition_split + "must specify three query (player key, stat name, and minimum value)");
                return false;
            }
            if (!uint.TryParse(rawMinValue, out var minValue))
            {
                Archery.monitor.Log(condition_split + "invalid minimum value '" + rawMinValue + "', must be an integer");
                return false;
            }

            return GameStateQuery.WithPlayer(playerKey, delegate (Farmer target)
            {
                Stats stats = target.stats;
                if (stats.stat_dictionary.TryGetValue(name, out var value))
                {
                    return value >= minValue;
                }
                object obj = stats.GetType().GetField(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)?.GetValue(stats);
                if (obj is uint)
                {
                    uint num = (uint)obj;
                    return num >= minValue;
                }
                return false;
            });
        }

        // Unable to backport the following queries due to additional requirements of SDV v1.6
        /* 
            query_SYNCED_CHOICE
            query_SYNCED_RANDOM
            query_SYNCED_SUMMER_RAIN_RANDOM
            query_ITEM_TYPE
        */
    }
}
