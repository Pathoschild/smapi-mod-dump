/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
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

namespace AeroCore.Backport
{
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0060 // Remove unused parameter
    [ModInit]
    internal class GameStateQuery
    {
        public delegate bool QueryDelegate(string[] condition_split);

        protected static Dictionary<string, QueryDelegate> _queryTypeLookup;

        protected static double _PickedValue;

        protected static Random _SeededRandom;

        protected static Farmer _CurrentTargetFarmer;

        protected static Item _CurrentTargetItem;

        protected static GameLocation _CurrentTargetGameLocation;

        public static void Init()
        {
            if (_queryTypeLookup == null)
            {
                _queryTypeLookup = new Dictionary<string, QueryDelegate>();
                MethodInfo[] array = (from method_info in typeof(GameStateQuery).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                      where method_info.Name.StartsWith("query_")
                                      select method_info).ToArray();
                foreach (MethodInfo methodInfo in array)
                {
                    QueryDelegate query_delegate = (QueryDelegate)Delegate.CreateDelegate(typeof(QueryDelegate), methodInfo);
                    RegisterQueryType(methodInfo.Name["query_".Length..], query_delegate);
                }
                ModEntry.monitor.Log(String.Format("SetupQueryTypes() registered '{0}' methods", _queryTypeLookup.Count), LogLevel.Trace);
            }
        }

        public static void PickRandomValue(Random r)
        {
            if (r == null)
            {
                r = Game1.random;
            }
            _PickedValue = r.NextDouble();
        }

        public static Random GetRandom()
        {
            if (_SeededRandom != null)
            {
                return _SeededRandom;
            }
            return Game1.random;
        }

        public static void RegisterQueryType(string query_name, QueryDelegate query_delegate)
        {
            _queryTypeLookup[query_name] = query_delegate;
        }

        public static string RemoveCondition(string condition_query, string condition_name, out string removed_condition)
        {
            if (condition_query == null)
            {
                removed_condition = null;
                return condition_query;
            }
            List<string> list = new(condition_query.Split(','));
            char[] separator = new char[1] { ' ' };
            for (int i = 0; i < list.Count; i++)
            {
                string text = list[i];
                string[] array = text.Split(separator, 2);
                if (array[0].StartsWith("!"))
                {
                    array[0] = array[0][1..];
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
                _SeededRandom = seeded_random;
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
                            array2[0] = array2[0][1..];
                        }
                        if (_CheckCondition(array2, target_farmer, target_item, game_location) != flag)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            finally
            {
                _SeededRandom = null;
            }
        }

        protected static bool _CheckCondition(string[] condition_split, Farmer target_farmer = null, Item target_item = null, GameLocation game_location = null)
        {
            _CurrentTargetFarmer = target_farmer;
            _CurrentTargetItem = target_item;
            _CurrentTargetGameLocation = game_location;
            try
            {
                if (_queryTypeLookup.TryGetValue(condition_split[0], out var value))
                {
                    try
                    {
                        return value(condition_split);
                    }
                    catch (Exception ex)
                    {
                        ModEntry.monitor.Log("ERROR parsing condition: " + string.Join(" ", condition_split) + "\n" + ex.ToString(), LogLevel.Trace);
                        return false;
                    }
                }
                ModEntry.monitor.Log(String.Format("ERROR: Invalid condition: {0}", condition_split[0]), LogLevel.Trace);
                return false;
            }
            finally
            {
                _CurrentTargetFarmer = null;
                _CurrentTargetItem = null;
                _CurrentTargetGameLocation = null;
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
                if (_CurrentTargetGameLocation == null)
                {
                    if (_CurrentTargetFarmer != null)
                    {
                        return _CurrentTargetFarmer.currentLocation;
                    }
                    return Game1.currentLocation;
                }
                return _CurrentTargetGameLocation;
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
                    return check(_GetTargetPlayer());
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
            if (_CurrentTargetFarmer == null)
            {
                return Game1.player;
            }
            return _CurrentTargetFarmer;
        }

        public static bool query_PICKED_VALUE(string[] condition_split)
        {
            int num = int.Parse(condition_split[1]);
            int num2 = int.Parse(condition_split[2]) + 1;
            int num3 = int.Parse(condition_split[3]);
            return (int)(num + _PickedValue * (num2 - num)) == num3;
        }
        public static bool query_PICKED_VALUE_CHANCE(string[] condition_split)
        {
            return _PickedValue <= (double)float.Parse(condition_split[1]);
        }

        public static bool query_PICKED_VALUE_SUMMER_RAIN_CHANCE(string[] condition_split)
        {
            return _PickedValue <= (double)(float.Parse(condition_split[1]) + Game1.dayOfMonth * float.Parse(condition_split[2]));
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
            return GetRandom().NextDouble() < (double)float.Parse(condition_split[1]);
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
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.FarmingLevel >= target_value);
        }

        public static bool query_PLAYER_MINING_LEVEL(string[] condition_split)
        {
            int target_value = int.Parse(condition_split[2]);
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.MiningLevel >= target_value);
        }

        public static bool query_PLAYER_FORAGING_LEVEL(string[] condition_split)
        {
            int target_value = int.Parse(condition_split[2]);
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.ForagingLevel >= target_value);
        }

        public static bool query_PLAYER_FISHING_LEVEL(string[] condition_split)
        {
            int target_value = int.Parse(condition_split[2]);
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.FishingLevel >= target_value);
        }

        public static bool query_PLAYER_COMBAT_LEVEL(string[] condition_split)
        {
            int target_value = int.Parse(condition_split[2]);
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.CombatLevel >= target_value);
        }

        public static bool query_PLAYER_HAS_CRAFTING_RECIPE(string[] condition_split)
        {
            return WithPlayer(condition_split[1], delegate (Farmer target_farmer)
            {
                string key = string.Join(" ", condition_split, 2, condition_split.Length - 2);
                return target_farmer.craftingRecipes.ContainsKey(key);
            });
        }

        public static bool query_PLAYER_HAS_COOKING_RECIPE(string[] condition_split)
        {
            return WithPlayer(condition_split[1], delegate (Farmer target_farmer)
            {
                string key = string.Join(" ", condition_split, 2, condition_split.Length - 2);
                return target_farmer.cookingRecipes.ContainsKey(key);
            });
        }

        public static bool query_PLAYER_HEARTS(string[] condition_split)
        {
            return WithPlayer(condition_split[1], delegate (Farmer target_farmer)
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
            return WithPlayer(condition_split[1], delegate (Farmer target_farmer)
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
            if (_CurrentTargetItem == null)
            {
                return false;
            }
            if (_CurrentTargetItem.Stack >= int.Parse(condition_split[1]))
            {
                return true;
            }
            return false;
        }

        public static bool query_ITEM_QUALITY(string[] condition_split)
        {
            if (_CurrentTargetItem == null)
            {
                return false;
            }
            if (_CurrentTargetItem is not StardewValley.Object)
            {
                return false;
            }
            if ((_CurrentTargetItem as StardewValley.Object).Quality >= int.Parse(condition_split[1]))
            {
                return true;
            }
            return false;
        }

        public static bool query_ITEM_ID(string[] condition_split)
        {
            if (_CurrentTargetItem == null)
            {
                return false;
            }
            return _CurrentTargetItem.ParentSheetIndex.ToString() == condition_split[1];
        }

        public static bool query_ITEM_HAS_TAG(string[] condition_split)
        {
            if (_CurrentTargetItem == null)
            {
                return false;
            }
            for (int i = 1; i < condition_split.Length; i++)
            {
                if (!_CurrentTargetItem.HasContextTag(condition_split[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool query_PLAYER_HAS_PROFESSION(string[] condition_split)
        {
            _ = int.TryParse(condition_split[2], out int profession_id);
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.professions.Contains(profession_id));
        }

        public static bool query_PLAYER_HAS_ACHIEVEMENT(string[] condition_split)
        {
            int achievement_id = int.Parse(condition_split[2]);
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.achievements.Contains(achievement_id));
        }

        public static bool query_PLAYER_LOCATION_NAME(string[] condition_split)
        {
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.currentLocation.Name == condition_split[2]);
        }

        public static bool query_PLAYER_LOCATION_CONTEXT(string[] condition_split)
        {
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.currentLocation.GetLocationContext().ToString() == condition_split[2]);
        }

        public static bool query_PLAYER_LOCATION_UNIQUE_NAME(string[] condition_split)
        {
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.currentLocation.NameOrUniqueName == condition_split[2]);
        }

        public static bool query_PLAYER_HAS_MET(string[] condition_split)
        {
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.friendshipData.ContainsKey(condition_split[2]));
        }

        public static bool query_PLAYER_IS_UNMET(string[] condition_split)
        {
            return WithPlayer(condition_split[1], (Farmer target_farmer) => !target_farmer.friendshipData.ContainsKey(condition_split[2]));
        }

        public static bool query_PLAYER_IS_DATING(string[] condition_split)
        {
            return WithPlayer(condition_split[1], delegate (Farmer target_farmer)
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
            return WithPlayer(condition_split[1], delegate (Farmer target_farmer)
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
            return WithPlayer(condition_split[1], delegate (Farmer target_farmer)
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
            return WithPlayer(condition_split[1], delegate (Farmer target_farmer)
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
            return WithPlayer(condition_split[1], delegate (Farmer target_farmer)
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
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.getChildrenCount() >= minimum_children);
        }

        public static bool query_PLAYER_FARMHOUSE_UPGRADE(string[] condition_split)
        {
            int minimum_upgrade_level = int.Parse(condition_split[2]);
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.HouseUpgradeLevel >= minimum_upgrade_level);
        }

        public static bool query_PLAYER_HAS_SECRET_NOTE(string[] condition_split)
        {
            int seen_note = int.Parse(condition_split[2]);
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.secretNotesSeen.Contains(seen_note));
        }

        public static bool query_PLAYER_HAS_PET(string[] condition_split)
        {
            return WithPlayer(condition_split[1], (Farmer target_farmer) => (!target_farmer.hasPet()));
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
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.hasPet() && target_farmer.catPerson == target_pet);
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
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.IsMale == target_gender);
        }

        public static bool query_PLAYER_MONEY_EARNED(string[] condition_split)
        {
            int target_amount = int.Parse(condition_split[2]);
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.totalMoneyEarned >= target_amount);
        }

        public static bool query_PLAYER_CURRENT_MONEY(string[] condition_split)
        {
            int target_amount = int.Parse(condition_split[2]);
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.Money >= target_amount);
        }

        public static bool query_PLAYER_MOD_DATA(string[] condition_split)
        {
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.modData.ContainsKey(condition_split[2]) && target_farmer.modData[condition_split[2]] == condition_split[3]);
        }

        public static bool query_PLAYER_HAS_READ_LETTER(string[] condition_split)
        {
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.mailReceived.Contains(string.Join(" ", condition_split.Skip(2).ToArray())));
        }

        public static bool query_PLAYER_HAS_FLAG(string[] condition_split)
        {
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.hasOrWillReceiveMail(string.Join(" ", condition_split.Skip(2).ToArray())));
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

            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.eventsSeen.Contains(value));
        }

        public static bool query_PLAYER_HAS_ITEM(string[] condition_split)
        {
            if (int.TryParse(condition_split[2], out int value) is false)
            {
                return false;
            }

            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.hasItemInInventory(value, 1));
        }

        public static bool query_PLAYER_HAS_ITEM_NAMED(string[] condition_split)
        {
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.hasItemInInventoryNamed(string.Join(" ", condition_split.Skip(2).ToArray())));
        }

        public static bool query_PLAYER_HAS_CAUGHT_FISH(string[] condition_split)
        {
            if (int.TryParse(condition_split[2], out int value) is false)
            {
                return false;
            }

            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.fishCaught.ContainsKey(value));
        }

        public static bool query_PLAYER_HAS_CONVERSATION_TOPIC(string[] condition_split)
        {
            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.activeDialogueEvents.ContainsKey(condition_split[2]));
        }

        public static bool query_PLAYER_HAS_DIALOGUE_ANSWER(string[] condition_split)
        {
            if (int.TryParse(condition_split[2], out int value) is false)
            {
                return false;
            }

            return WithPlayer(condition_split[1], (Farmer target_farmer) => target_farmer.DialogueQuestionsAnswered.Contains(value));
        }

        public static bool query_WEATHER(string[] condition_split)
        {
            GameLocation location = GetLocation(condition_split[1]);

            var weatherContext = Game1.netWorldState.Value.GetWeatherForLocation(location.GetLocationContext());
            return condition_split[2].ToUpper() switch
            {
                "FESTIVAL" => Game1.isFestival(),
                "RAIN" or "RAINY" => weatherContext.isRaining.Value || weatherContext.isLightning.Value,
                "SNOW" or "SNOWY" => weatherContext.isSnowing.Value,
                "STORM" or "STORMY" => weatherContext.isLightning.Value,
                "WIND" or "WINDY" => weatherContext.isDebrisWeather.Value,
                _ => weatherContext.isRaining.Value is false && weatherContext.isLightning.Value is false && weatherContext.isSnowing.Value is false,
            };
        }

        public static bool query_LOCATION_SEASON(string[] condition_split)
        {
            return Game1.GetSeasonForLocation(GetLocation(condition_split[1])) == condition_split[2];
        }

        public static bool query_LOCATION_IS_MINES(string[] condition_split)
        {
            return GetLocation(condition_split[1]) is MineShaft;
        }

        public static bool query_LOCATION_IS_SKULL_CAVE(string[] condition_split)
        {
            GameLocation location = GetLocation(condition_split[1]);
            if (location is MineShaft && (location as MineShaft).mineLevel >= 121)
            {
                return (location as MineShaft).mineLevel != 77377;
            }
            return false;
        }

        public static bool query_LOCATION_CONTEXT(string[] condition_split)
        {
            return GetLocation(condition_split[1]).GetLocationContext().ToString() == condition_split[2];
        }

        public static bool query_LOCATION_ACCESSIBLE(string[] condition_split)
        {
            return Game1.isLocationAccessible(condition_split[1]);
        }

        public static bool query_FARM_NAME(string[] condition_split)
        {
            return _GetTargetPlayer().farmName.Value == string.Join(" ", condition_split, 2, condition_split.Length - 2);
        }

        public static bool query_FARM_TYPE(string[] condition_split)
        {
            return Game1.GetFarmTypeID() == condition_split[1];
        }

        public static bool query_FARM_CAVE(string[] condition_split)
        {
            if (Game1.MasterPlayer.caveChoice.Value == 1)
            {
                return condition_split[1] == "Bats";
            }
            if (Game1.MasterPlayer.caveChoice.Value == 2)
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

        public static bool query_TRUE(string[] condition_split)
        {
            return true;
        }

        public static bool query_FALSE(string[] condition_split)
        {
            return false;
        }
    }
}
