/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using FarmVisitors.Datamodels;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using static FarmVisitors.ModEntry;

namespace FarmVisitors.Visit;

internal static class Content
{
    /// <summary>
    /// Removes potential children NPCs (ie lives in house, is neither spouse nor roommate)
    /// </summary>
    internal static void FixChildrenInfo()
    {
        var hasC2N = Help.ModRegistry.Get("Loe2run.ChildToNPC") != null;
        // ReSharper disable once InconsistentNaming
        var hasLNPCs = Help.ModRegistry.Get("Candidus42.LittleNPCs") != null;

        if (!hasC2N && !hasLNPCs) return;
        
        //check all npcs in farmhouse, since C2N and LNPCs add them at first timechange (iirc)
        foreach (var chara in Utility.getHomeOfFarmer(Game1.player).characters)
        {
            //if npc isnt married to farmer, assuming it's a child.
            if (chara.isMarried() || chara.isRoommate()) continue;
            try
            {
                NameAndLevel.Remove(chara.Name);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    private static void ParseBlacklist()
    {
        Log("Getting raw blacklist.");
        BlacklistRaw = Config.Blacklist;
        if (BlacklistRaw is null)
        {
            Log("No characters in blacklist.");
        }

        var charsToRemove = new[] { "-", ",", ".", ";", "\"", "\'", "/" };
        foreach (var c in charsToRemove)
        {
            BlacklistRaw = BlacklistRaw?.Replace(c, string.Empty);
        }
        if (Config.Verbose)
        {
            Log($"Raw blacklist: \n {BlacklistRaw} \nWill be parsed to list now.", LogLevel.Debug);
        }
        BlacklistParsed = BlacklistRaw?.Split(' ').ToList();
    }

    internal static void CleanTemp()
    {
        CounterToday = 0;
        SetNoVisitor();
        TodaysVisitors?.Clear();

        if (MaxTimeStay != (Config.Duration - 1))
        {
            MaxTimeStay = (Config.Duration - 1);
            Log($"MaxTimeStay = {MaxTimeStay}; Config.Duration = {Config.Duration};");
        }

        Animals?.Clear();
        Crops?.Clear();
        GreenhouseCrops?.Clear();
    }

    internal static void ClearValues()
    {
        InLaws.Clear();
        NameAndLevel?.Clear();
        RepeatedByLV?.Clear();
        TodaysVisitors?.Clear();
        //FurnitureList?.Clear();
        SchedulesParsed?.Clear();
        BlacklistRaw = null;
        BlacklistParsed?.Clear();

        //data stored
        InlawDialogue?.Clear();
        RetiringDialogue?.Clear();
        
        if (!string.IsNullOrWhiteSpace(Config.Blacklist))
        {
            ParseBlacklist();
        }

        CounterToday = 0;
        SetNoVisitor();
    }

    internal static void ReloadCustomschedules()
    {
        Log("Began reloading custom schedules.");

        SchedulesParsed?.Clear();

        var schedules = Help.GameContent.Load<Dictionary<string, ScheduleData>>("mistyspring.farmhousevisits/Schedules");

        if (schedules.Any())
        {
            foreach (var pair in schedules)
            {
                Log($"Checking {pair.Key}'s schedule...");
                var isPatchValid = Data.IsScheduleValid(pair);

                if (!isPatchValid)
                {
                    Log($"{pair.Key} schedule won't be added.");
                }
                else
                {
                    SchedulesParsed?.Add(pair.Key, pair.Value); //NRE
                }
            }
        }

        HasCustomSchedules = SchedulesParsed?.Any() ?? false;

        Log("Finished reloading custom schedules.");
    }

    //below methods: REQUIRED, dont touch UNLESS it's for bug-fixing
    internal static void ChooseRandom()
    {
        Log("Getting random...");
        var visitorName = Game1.random.ChooseFrom(RepeatedByLV);
        Log($"VisitorName= {visitorName}");

        var visit = Game1.getCharacterFromName(visitorName);

        if (!Values.IsFree(visit)) return;

        //visit.IsInvisible = true;
        //save values
        VContext = new Models.VisitData(visit);
        Visitor = DupeNPC.Duplicate(visit);

        HasAnyVisitors = true;


        if (Config.NeedsConfirmation)
        {
            Confirmation.AskToEnter();
        }
        else
        {
            //add them to farmhouse (last to avoid issues)
            Actions.AddToFarmHouse(Visitor, PlayerHome, false);

            if (Config.Verbose)
            {
                Log($"\nHasAnyVisitors set to true.\n{Visitor.Name} will begin visiting player.\nTimeOfArrival = {VContext.TimeOfArrival};\nControllerTime = {VContext.ControllerTime};", LogLevel.Debug);
            }
        }
    }

    /// <summary>
    /// Gets visitor data, considering blacklist and marriage/divorce
    /// </summary>
    internal static void GetAllVisitors()
    {
        if (!IsConfigValid)
        {
            return;
        }

        var logLV = Config.Verbose ? LogLevel.Debug : LogLevel.Trace;

        Log("Began obtaining all visitors.", logLV);
        if (!string.IsNullOrWhiteSpace(Config.Blacklist))
            ParseBlacklist();
        
        MaxTimeStay = (Config.Duration - 1);
        Log($"MaxTimeStay = {MaxTimeStay}; Config.Duration = {Config.Duration};", logLV);

        PlayerHome = Utility.getHomeOfFarmer(Game1.player);

        //get all friended excluding children and spouses/divorced
        foreach (var pair in Game1.player.friendshipData.Pairs)
        {

            Log($"Name: {pair.Key}");

            var hearts = pair.Value.Points / 250;
            var isDivorced = pair.Value.IsDivorced();
            var isMarried = pair.Value.IsMarried() || pair.Value.IsRoommate();

            if (!isMarried && hearts > 0)
            {
                if (BlacklistParsed != null)
                {
                    if (BlacklistParsed.Contains(pair.Key))
                    {
                        Log($"{pair.Key} is in the blacklist.", LogLevel.Debug);
                    }
                    else
                    {
                        if (Utility.fuzzyCharacterSearch(pair.Key) != null)
                            NameAndLevel.Add(pair.Key, hearts);
                        else if (_firstLoadedDay)
                            Log($"Couldn't find character {pair.Key} in world. They won't be included.");
                    }
                }
                else if (isDivorced)
                {
                    Log($"{pair} is Divorced.");
                }
                else
                {
                    if (pair.Key.Equals("Dwarf"))
                    {
                        if (!Game1.player.canUnderstandDwarves)
                            Log("Player can't understand dwarves yet!");

                        else
                            NameAndLevel.Add(pair.Key, hearts);
                    }
                    else
                        NameAndLevel.Add(pair.Key, hearts);
                }
            }
            else
            {
                if (isMarried)
                {
                    MarriedNPCs.Add(pair.Key);
                    Log($"Adding {pair.Key} to married list...");
                }
                if (isDivorced)
                {
                    Log($"{pair.Key} is Divorced. They won't visit the player", logLV);
                }
            }
        }

        #region log data
        var call = "\n Name   | Hearts\n--------------------";
        foreach (var pair in NameAndLevel)
        {
            var fixedstr = pair.Key + "               ".Remove(0, pair.Key.Length);
            
            call += $"\n   {fixedstr} {pair.Value}";

            var tempdict = Enumerable.Repeat(pair.Key, pair.Value).ToList();
            RepeatedByLV.AddRange(tempdict);
        }
       
        Log(call, logLV);
        #endregion

        Log("Finished obtaining all visitors.");

        ReloadCustomschedules();
    }
}