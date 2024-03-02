/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Events;
using StardewValley.Locations;
using xTile.Tiles;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public class TheaterInjections
    {
        public const string MOVIE_THEATER_ITEM = "Progressive Movie Theater";
        private const string MOVIE_THEATER_MAIL = "ccMovieTheater";
        private const string ABANDONED_JOJA_MART = "AbandonedJojaMart";
        private const string MOVIE_THEATER = "MovieTheater";
        private const int CC_EVENT_ID = 191393;
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;

        private static Point GetMissingBundleTile(GameLocation location) => location is MovieTheater ? new Point(17, 8) : new Point(8, 8);

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
        }

        // public static FarmEvent pickFarmEvent()
        public static void PickFarmEvent_BreakJojaDoor_Postfix(ref FarmEvent __result)
        {
            try
            {
                if (Game1.weddingToday || __result != null || !_archipelago.HasReceivedItem(MOVIE_THEATER_ITEM))
                {
                    return;
                }

                if ((Game1.isRaining || Game1.isLightning || Game1.IsWinter) && !Game1.MasterPlayer.mailReceived.Contains("abandonedJojaMartAccessible"))
                {
                    __result = new WorldChangeEvent(12);
                    return;
                }

                if (_archipelago.GetReceivedItemCount(MOVIE_THEATER_ITEM) < 2)
                {
                    return;
                }

                if (!Game1.player.mailReceived.Contains("ccMovieTheater%&NL&%") && !Game1.player.mailReceived.Contains("ccMovieTheater"))
                {
                    __result = new WorldChangeEvent(11);
                    Game1.player.mailReceived.Add("ccMovieTheater");
                    return;
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PickFarmEvent_BreakJojaDoor_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public virtual void MakeMapModifications(bool force = false)
        public static bool MakeMapModifications_PlaceMissingBundleNote_Prefix(GameLocation __instance, bool force)
        {
            try
            {
                if (__instance.Name != ABANDONED_JOJA_MART && __instance.Name != MOVIE_THEATER)
                {
                    return true; // run original logic
                }

                var abandonedJojaMart = Game1.getLocationFromName(ABANDONED_JOJA_MART);
                var junimoNotePoint = GetMissingBundleTile(__instance);

                if (Game1.MasterPlayer.hasOrWillReceiveMail("apccMovieTheater"))
                {
                    __instance.removeTile(junimoNotePoint.X, junimoNotePoint.Y, "Buildings");
                    return false; // don't run original logic
                }

                if (__instance.map.TileSheets.Count < 3)
                {
                    var abandonedJojaIndoorTileSheet = abandonedJojaMart.map.GetTileSheet("indoor");

                    // aaa is to make it get sorted to index 0, because the dumbass CC assumes the first tilesheet is the correct one
                    var indoorTileSheet = new TileSheet("aaa" + abandonedJojaIndoorTileSheet.Id, __instance.map, abandonedJojaIndoorTileSheet.ImageSource, abandonedJojaIndoorTileSheet.SheetSize, abandonedJojaIndoorTileSheet.TileSize);
                    __instance.map.AddTileSheet(indoorTileSheet);
                }

                var junimoNoteTileFrames = CommunityCenter.getJunimoNoteTileFrames(0, __instance.map);
                var layerId = "Buildings";
                __instance.map.GetLayer(layerId).Tiles[junimoNotePoint.X, junimoNotePoint.Y] = new AnimatedTile(__instance.map.GetLayer(layerId), junimoNoteTileFrames, 70L);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MakeMapModifications_PlaceMissingBundleNote_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public bool checkTileIndexAction(int tileIndex)
        public static bool CheckTileIndexAction_InteractWithMissingBundleNote_Prefix(GameLocation __instance, int tileIndex, ref bool __result)
        {
            try
            {
                if (__instance.Name != ABANDONED_JOJA_MART && __instance.Name != MOVIE_THEATER)
                {
                    return true; // run original logic
                }

                switch (tileIndex)
                {
                    // I think these are... bundle animation sprites... yeah wtf
                    case 1799:
                    case 1824:
                    case 1825:
                    case 1826:
                    case 1827:
                    case 1828:
                    case 1829:
                    case 1830:
                    case 1831:
                    case 1832:
                    case 1833:
                        // Game1.activeClickableMenu = (IClickableMenu) new JunimoNoteMenu(6, (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).bundlesDict())
                        ((AbandonedJojaMart)(Game1.getLocationFromName("AbandonedJojaMart"))).checkBundle();
                        __result = true;
                        return false; // don't run original logic
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckTileIndexAction_InteractWithMissingBundleNote_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // private void doRestoreAreaCutscene()
        public static bool DoRestoreAreaCutscene_InteractWithMissingBundleNote_Prefix(AbandonedJojaMart __instance)
        {
            try
            {
                // Game1.player.freezePause = 1000;
                var junimoNotePoint = GetMissingBundleTile(__instance);
                DelayedAction.removeTileAfterDelay(junimoNotePoint.X, junimoNotePoint.Y, 100, Game1.currentLocation, "Buildings");
                // Game1.getLocationFromName(nameof(AbandonedJojaMart)).startEvent(new Event(Game1.content.Load<Dictionary<string, string>>("Data\\Events\\AbandonedJojaMart")["missingBundleComplete"], 192393));
                
                Game1.addMailForTomorrow("apccMovieTheater", true, true);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(DoRestoreAreaCutscene_InteractWithMissingBundleNote_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static bool hasSeenCCCeremonyCutscene;
        private static bool hasPamHouseUpgrade;
        private static bool hasShortcuts;

        // public override void MakeMapModifications(bool force = false)
        public static bool MakeMapModifications_JojamartAndTheater_Prefix(Town __instance, bool force)
        {
            try
            {
                if (_archipelago.GetReceivedItemCount(MOVIE_THEATER_ITEM) >= 2)
                {
                    var rectangle = new Microsoft.Xna.Framework.Rectangle(84, 41, 27, 15);
                    __instance.ApplyMapOverride("Town-Theater", rectangle, rectangle);
                    return true; // run original logic
                }

                if (_archipelago.GetReceivedItemCount(MOVIE_THEATER_ITEM) >= 1)
                {
                    // private void showDestroyedJoja()
                    var showDestroyedJojaMethod = _modHelper.Reflection.GetMethod(__instance, "showDestroyedJoja");
                    showDestroyedJojaMethod.Invoke();
                    if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("abandonedJojaMartAccessible"))
                    {
                        __instance.crackOpenAbandonedJojaMartDoor();
                    }
                    if (!Game1.player.mailReceived.Contains(string.Join("ap",ABANDONED_JOJA_MART)))
                    {
                        Game1.player.mailReceived.Add("apAbandonedJojaMart");
                    }
                }
                else
                {
                    hasSeenCCCeremonyCutscene = Utility.HasAnyPlayerSeenEvent(CC_EVENT_ID);
                    if (hasSeenCCCeremonyCutscene)
                    {
                        Game1.player.eventsSeen.Remove(CC_EVENT_ID);
                    }
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MakeMapModifications_JojamartAndTheater_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public override void MakeMapModifications(bool force = false)
        public static void MakeMapModifications_JojamartAndTheater_Postfix(Town __instance, bool force)
        {
            try
            {
                if (hasSeenCCCeremonyCutscene && !Game1.player­.eventsSeen.Contains(CC_EVENT_ID))
                {
                    Game1.player.eventsSeen.Add(CC_EVENT_ID);
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MakeMapModifications_JojamartAndTheater_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static void UpdateScheduleForEveryone()
        {
            foreach (var character in Utility.getAllCharacters())
            {
                if (character.isVillager())
                {
                    character.Schedule = character.getSchedule(Game1.dayOfMonth);
                }
            }
        }

        // private bool changeScheduleForLocationAccessibility(ref string locationName, ref int tileX, ref int tileY, ref int facingDirection)
        public static bool ChangeScheduleForLocationAccessibility_JojamartAndTheater_Prefix(NPC __instance, ref string locationName, ref int tileX, ref int tileY, ref int facingDirection, ref bool __result)
        {
            try
            {
                if (locationName is "Railroad" or "CommunityCenter")
                {
                    return true; // run original logic
                }

                if (locationName != "JojaMart" || !_archipelago.HasReceivedItem(MOVIE_THEATER_ITEM))
                {
                    // no fallback
                    __result = false;
                    return false; // don't run original logic
                }

                if (!__instance.hasMasterScheduleEntry(locationName + "_Replacement"))
                {
                    // Fallback on the default schedule
                    __result = true;
                    return false; // don't run original logic
                }

                string[] strArray = __instance.getMasterScheduleEntry(locationName + "_Replacement").Split(' ');
                locationName = strArray[0];
                tileX = Convert.ToInt32(strArray[1]);
                tileY = Convert.ToInt32(strArray[2]);
                facingDirection = Convert.ToInt32(strArray[3]);

                // no fallback
                __result = false;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ChangeScheduleForLocationAccessibility_JojamartAndTheater_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        //// public Dictionary<int, SchedulePathDescription> parseMasterSchedule(string rawData)
        //public static bool ParseMasterSchedule_JojamartAndTheater_Prefix(NPC __instance, string rawData, ref Dictionary<int, SchedulePathDescription> __result)
        //{
        //    try
        //    {
        //        if (__instance.Name != "Shane")
        //        {
        //            return true; // run original logic
        //        }


        //        string[] strArray1 = rawData.Split('/');
        //        Dictionary<int, SchedulePathDescription> masterSchedule = new Dictionary<int, SchedulePathDescription>();
        //        int index1 = 0;
        //        if (strArray1[0].Contains("GOTO"))
        //        {
        //            string currentSeason = strArray1[0].Split(' ')[1];
        //            if (currentSeason.ToLower().Equals("season"))
        //            {
        //                currentSeason = Game1.currentSeason;
        //            }

        //            try
        //            {
        //                strArray1 = __instance.getMasterScheduleRawData()[currentSeason].Split('/');
        //            }
        //            catch (Exception ex)
        //            {
        //                __result = __instance.parseMasterSchedule(__instance.getMasterScheduleEntry("spring"));
        //                return false; // don't run original logic
        //            }
        //        }

        //        if (strArray1[0].Contains("NOT"))
        //        {
        //            string[] strArray2 = strArray1[0].Split(' ');
        //            if (strArray2[1].ToLower() == "friendship")
        //            {
        //                int index2 = 2;
        //                bool flag = false;
        //                for (; index2 < strArray2.Length; index2 += 2)
        //                {
        //                    string name = strArray2[index2];
        //                    int result = 0;
        //                    if (int.TryParse(strArray2[index2 + 1], out result))
        //                    {
        //                        foreach (Farmer allFarmer in Game1.getAllFarmers())
        //                        {
        //                            if (allFarmer.getFriendshipHeartLevelForNPC(name) >= result)
        //                            {
        //                                flag = true;
        //                                break;
        //                            }
        //                        }
        //                    }

        //                    if (flag)
        //                    {
        //                        break;
        //                    }
        //                }

        //                if (flag)
        //                {
        //                    __result = __instance.parseMasterSchedule(__instance.getMasterScheduleEntry("spring"));
        //                    return false; // don't run original logic
        //                }

        //                ++index1;
        //            }
        //        }
        //        else if (strArray1[0].Contains("MAIL"))
        //        {
        //            string id = strArray1[0].Split(' ')[1];
        //            if (Game1.MasterPlayer.mailReceived.Contains(id) || NetWorldState.checkAnywhereForWorldStateID(id))
        //            {
        //                index1 += 2;
        //            }
        //            else
        //            {
        //                ++index1;
        //            }
        //        }

        //        if (strArray1[index1].Contains("GOTO"))
        //        {
        //            string currentSeason = strArray1[index1].Split(' ')[1];
        //            if (currentSeason.ToLower().Equals("season"))
        //            {
        //                currentSeason = Game1.currentSeason;
        //            }
        //            else if (currentSeason.ToLower().Equals("no_schedule"))
        //            {
        //                __instance.followSchedule = false;
        //                __result = (Dictionary<int, SchedulePathDescription>)null;
        //                return false; // don't run original logic
        //            }

        //            __result = __instance.parseMasterSchedule(__instance.getMasterScheduleEntry(currentSeason));
        //            return false; // don't run original logic
        //        }

        //        // Point point1 = __instance.isMarried() ? new Point(0, 23) : new Point((int)__instance.defaultPosition.X / 64, (int)__instance.defaultPosition.Y / 64);
        //        string startingLocation = __instance.isMarried() ? "BusStop" : (string)(NetFieldBase<string, NetString>)__instance.defaultMap;
        //        int val2 = 610;
        //        string targetLocationName = __instance.DefaultMap;
        //        int x = 1; // (int)((double)__instance.defaultPosition.X / 64.0);
        //        int y = 1; // (int)((double)__instance.defaultPosition.Y / 64.0);
        //        bool flag1 = false;
        //        for (int index3 = index1; index3 < strArray1.Length && strArray1.Length > 1; ++index3)
        //        {
        //            int index4 = 0;
        //            string[] strArray3 = strArray1[index3].Split(' ');
        //            bool flag2 = false;
        //            string str1 = strArray3[index4];
        //            if (str1.Length > 0 && strArray3[index4][0] == 'a')
        //            {
        //                flag2 = true;
        //                str1 = str1.Substring(1);
        //            }

        //            int num1 = Convert.ToInt32(str1);
        //            int index5 = index4 + 1;
        //            string locationName = strArray3[index5];
        //            string endBehavior = (string)null;
        //            string endMessage = (string)null;
        //            int result1 = 0;
        //            int result2 = 0;
        //            int result3 = 2;
        //            int index6;
        //            if (locationName == "bed")
        //            {
        //                if (__instance.isMarried())
        //                {
        //                    locationName = "BusStop";
        //                    result1 = -1;
        //                    result2 = 23;
        //                    result3 = 3;
        //                }
        //                else
        //                {
        //                    string str2 = (string)null;
        //                    if (__instance.hasMasterScheduleEntry("default"))
        //                    {
        //                        str2 = __instance.getMasterScheduleEntry("default");
        //                    }
        //                    else if (__instance.hasMasterScheduleEntry("spring"))
        //                    {
        //                        str2 = __instance.getMasterScheduleEntry("spring");
        //                    }

        //                    if (str2 != null)
        //                    {
        //                        try
        //                        {
        //                            string[] strArray4 = str2.Split('/');
        //                            string[] strArray5 = strArray4[strArray4.Length - 1].Split(' ');
        //                            locationName = strArray5[1];
        //                            if (strArray5.Length > 3)
        //                            {
        //                                if (int.TryParse(strArray5[2], out result1))
        //                                {
        //                                    if (int.TryParse(strArray5[3], out result2))
        //                                    {
        //                                        goto label_51;
        //                                    }
        //                                }

        //                                str2 = (string)null;
        //                            }
        //                            else
        //                            {
        //                                str2 = (string)null;
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            str2 = (string)null;
        //                        }
        //                    }

        //                    label_51:
        //                    if (str2 == null)
        //                    {
        //                        locationName = targetLocationName;
        //                        result1 = x;
        //                        result2 = y;
        //                    }
        //                }

        //                index6 = index5 + 1;
        //                Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions");
        //                string str3 = __instance.name.Value.ToLower() + "_sleep";
        //                string key = str3;
        //                if (dictionary.ContainsKey(key))
        //                {
        //                    endBehavior = str3;
        //                }
        //            }
        //            else
        //            {
        //                if (int.TryParse(locationName, out int _))
        //                {
        //                    locationName = startingLocation;
        //                    --index5;
        //                }

        //                int index7 = index5 + 1;
        //                result1 = Convert.ToInt32(strArray3[index7]);
        //                int index8 = index7 + 1;
        //                result2 = Convert.ToInt32(strArray3[index8]);
        //                index6 = index8 + 1;
        //                try
        //                {
        //                    if (strArray3.Length > index6)
        //                    {
        //                        if (int.TryParse(strArray3[index6], out result3))
        //                        {
        //                            ++index6;
        //                        }
        //                        else
        //                        {
        //                            result3 = 2;
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    result3 = 2;
        //                }
        //            }

        //            //if (__instance.changeScheduleForLocationAccessibility(ref locationName, ref result1, ref result2, ref result3))
        //            //{
        //            //    __result = __instance.getMasterScheduleRawData().ContainsKey("default")
        //            //        ? __instance.parseMasterSchedule(__instance.getMasterScheduleEntry("default"))
        //            //        : __instance.parseMasterSchedule(__instance.getMasterScheduleEntry("spring"));
        //            //    return false; // don't run original logic
        //            //}

        //            if (index6 < strArray3.Length)
        //            {
        //                if (strArray3[index6].Length > 0 && strArray3[index6][0] == '"')
        //                {
        //                    endMessage = strArray1[index3].Substring(strArray1[index3].IndexOf('"'));
        //                }
        //                else
        //                {
        //                    endBehavior = strArray3[index6];
        //                    int index9 = index6 + 1;
        //                    if (index9 < strArray3.Length && strArray3[index9].Length > 0 && strArray3[index9][0] == '"')
        //                    {
        //                        endMessage = strArray1[index3].Substring(strArray1[index3].IndexOf('"')).Replace("\"", "");
        //                    }
        //                }
        //            }

        //            if (num1 == 0)
        //            {
        //                flag1 = true;
        //                targetLocationName = locationName;
        //                x = result1;
        //                y = result2;
        //                startingLocation = locationName;
        //                // point1.X = result1;
        //                // point1.Y = result2;
        //                // __instance.previousEndPoint = new Point(result1, result2);
        //            }
        //            else
        //            {
        //                //SchedulePathDescription scheduleLocation = __instance.pathfindToNextScheduleLocation(startingLocation, point1.X, point1.Y, locationName, result1, result2, result3, endBehavior, endMessage);
        //                //if (flag2)
        //                //{
        //                //    int num2 = 0;
        //                //    Point? nullable = new Point?();
        //                //    foreach (Point point2 in scheduleLocation.route)
        //                //    {
        //                //        if (!nullable.HasValue)
        //                //        {
        //                //            nullable = new Point?(point2);
        //                //        }
        //                //        else
        //                //        {
        //                //            if (Math.Abs(nullable.Value.X - point2.X) + Math.Abs(nullable.Value.Y - point2.Y) == 1)
        //                //            {
        //                //                num2 += 64;
        //                //            }

        //                //            nullable = new Point?(point2);
        //                //        }
        //                //    }

        //                //    int num3 = (int)Math.Round((double)(num2 / 2) / 420.0) * 10;
        //                //    num1 = Math.Max(Utility.ConvertMinutesToTime(Utility.ConvertTimeToMinutes(num1) - num3), val2);
        //                //}

        //                //masterSchedule.Add(num1, scheduleLocation);
        //                //point1.X = result1;
        //                //point1.Y = result2;
        //                //startingLocation = locationName;
        //                //val2 = num1;
        //            }
        //        }

        //        if (Game1.IsMasterGame & flag1)
        //        {
        //            Game1.warpCharacter(__instance, targetLocationName, new Point(x, y));
        //        }

        //        /*if (__instance._lastLoadedScheduleKey != null && Game1.IsMasterGame)
        //        {
        //            __instance.dayScheduleName.Value = __instance._lastLoadedScheduleKey;
        //        }*/

        //        __result = masterSchedule;
        //        return false; // don't run original logic


        //        return false; // don't run original logic
        //    }
        //    catch (Exception ex)
        //    {
        //        _monitor.Log($"Failed in {nameof(ParseMasterSchedule_JojamartAndTheater_Prefix)}:\n{ex}", LogLevel.Error);
        //        return true; // run original logic
        //    }
        //}
    }
}
