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
using System.Linq;
using FarmVisitors.Datamodels;
using FarmVisitors.Models;
using FarmVisitors.Visit;
using StardewModdingAPI;
using StardewValley;
using static FarmVisitors.ModEntry;

namespace FarmVisitors;

internal static class Debugging
{
    internal static void Reload(string command, string[] arg2) => Content.GetAllVisitors();
    
    public static void ForceVisit(string command, string[] arg2)
    {
        var farmHouse = Utility.getHomeOfFarmer(Game1.player);

        if (!Context.IsWorldReady)
        {
            Log(TL.Get("error.WorldNotReady"), LogLevel.Error);
            return;
        }

        if (!Game1.player.currentLocation.Equals(farmHouse))
        {
            Log(TL.Get("error.NotInFarmhouse"), LogLevel.Error);
            return;
        }

        try
        {
            var hasArgs = arg2?.Any() ?? false;
            var name = arg2.Length >= 1 ? arg2[0] : null;
            var force = arg2.Length >= 2 ? bool.Parse(arg2[1]) : false;
            var npc = name != null ? Utility.fuzzyCharacterSearch(name) : null;

            if (!hasArgs)
            {
                Content.ChooseRandom();
            }
            else if (npc != null)
            {
                Log($"VisitorName= {npc.Name}");

                if (!TodaysVisitors.Contains(name) || force)
                {
                    //save values
                    VContext = new VisitData(npc);
                    Visitor = DupeNPC.Duplicate(npc);
                    HasAnyVisitors = true;
                    //add them to farmhouse
                    Actions.AddToFarmHouse(Visitor, farmHouse, false);
                }
                else
                {
                    Log($"{npc.displayName} has already visited the Farm today!");
                }
            }
            else
            {
                Log(TL.Get("error.InvalidValue"), LogLevel.Error);
            }
        }
        catch (Exception)
        {
            //ignore
        }
    }

    public static void Print(string command, string[] arg2)
    {
        if (!Context.IsWorldReady)
        {
            Log(TL.Get("error.WorldNotReady"), LogLevel.Error);
            return;
        }
        var hasArgs = arg2?.Any() ?? false;

        if (!hasArgs)
        {
            Log("Please input an option (Avaiable: animal, blacklist, crop, furniture, info, inlaws, visits).", LogLevel.Warn);
            return;
        }

        var print = "\n";
        foreach (var arg in arg2)
        {
            var toLower = arg.ToLower();
            switch (toLower)
            {
                case "inlaw":
                case "inlaws":
                    {
                        print += "\n In-Laws \n--------------------";
                        var result = "\n";
                        foreach (var pair in InLaws)
                        {
                            var pairvalue = "";
                            foreach (var name in pair.Value)
                            {
                                if (pair.Value[^1].Equals(name))
                                {
                                    pairvalue += $"{name}.";
                                }
                                else
                                {
                                    pairvalue += $"{name}, ";
                                }
                            }

                            result += $"\n{pair.Key}: {pairvalue}";
                        }

                        if (result.Equals("\n"))
                        {
                            Log("No in-laws found. (Searched all NPCs with friendship)", LogLevel.Warn);
                        }
                        else
                        {
                            print += result;
                        }

                        break;
                    }
                case "animal":
                case "animals":
                    {
                        print += "\n Animals \n--------------------";
                        foreach (var name in Animals)
                        {
                            print += $"{name} \n";
                        }

                        break;
                    }
                case "crop":
                case "crops":
                    {
                        print += "\n Crops \n--------------------";
                        foreach (var type in Crops)
                        {
                            print += $"{type} \n";
                        }

                        break;
                    }
                case "visits":
                case "v":
                    {
                        print += "\n Visits \n--------------------";
                        foreach (var name in TodaysVisitors)
                        {
                            print += $"{name} \n";
                        }

                        break;
                    }
                case "blacklist":
                case "bl":
                    {
                        print += "\n Blacklist \n--------------------";
                        foreach (var name in BlacklistParsed)
                        {
                            print += $"{name} \n";
                        }

                        break;
                    }
            }
        }
        Log(print, LogLevel.Info);
    }
}
