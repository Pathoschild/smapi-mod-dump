/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;
using static FarmVisitors.ModEntry;

namespace FarmVisitors
{
    internal static class Debugging
    {
        /*private static void ForceVisit(string command, string[] arg2)
        {
            var farmHouse = Utility.getHomeOfFarmer(Game1.MasterPlayer);

            if (Context.IsWorldReady)
            {
                if (Game1.MasterPlayer.currentLocation == farmHouse)
                {
                    if (arg2 is null)
                    {
                        ModEntry.ChooseRandom();
                    }
                    else if (NPCNames.Contains(arg2?[0]))
                    {
                        ModEntry.VisitorName = arg2[0];
                        ModEntry.Log($"VisitorName= {VisitorName}", LogLevel.Trace);

                        if (!TodaysVisitors.Contains(VisitorName))
                        {
                            //save values
                            NPC visit = Game1.getCharacterFromName(VisitorName);

                            //add them to farmhouse
                            Actions.AddToFarmHouse(visit, farmHouse, false);
                            ModEntry.SetFromCommand(visit);
                        }
                        else if (arg2[1] is "force")
                        {
                            //save values
                            NPC visit = Game1.getCharacterFromName(VisitorName);
                            VisitorData = new TempNPC(visit);

                            //add them to farmhouse
                            Actions.AddToFarmHouse(visit, farmHouse, false);
                            
                        }
                        else
                        {
                            VisitorName = null;
                            ModEntry.Log($"{VisitorName} has already visited the Farm today!", LogLevel.Trace);
                        }
                    }
                    else
                    {
                        ModEntry.Log(ModEntry.TL.Get("error.InvalidValue"), LogLevel.Error);
                    }
                }
                else
                {
                    ModEntry.Log(ModEntry.TL.Get("error.NotInFarmhouse"), LogLevel.Error);
                }
            }
            else
            {
                ModEntry.Log(ModEntry.TL.Get("error.WorldNotReady"), LogLevel.Error);
            }
        }*/

        public static void Print(string command, string[] arg2)
        {
            if (!Context.IsWorldReady)
            {
                ModEntry.Log(ModEntry.TL.Get("error.WorldNotReady"), LogLevel.Error);
            }
            else
            {
                Func<string, bool> InArg2 = word => arg2.Any(s => s.ToLower().Equals(word));

                if (arg2 == null || !(arg2.Any()))
                {
                    ModEntry.Log("Please input an option (Avaiable: animal, blacklist, crop, furniture, info, inlaws, visits).", LogLevel.Warn);
                }

                if (InArg2("info"))
                {
                    string cc = currentCustom?.Count.ToString() ?? "none";
                    string f = VisitorData?.Facing.ToString() ?? "none";
                    string pv = VisitorData?.CurrentPreVisit?.Count.ToString() ?? "none";
                    string n = VisitorData?.Name ?? "none";
                    string am = VisitorData?.AnimationMessage ?? "none";

                    ModEntry.Log($"\ncurrentCustom count = {cc}; \nVisitorData: \n   Name = {n},\n   Facing = {f}, \n  AnimationMessage = {am}, \n  Dialogues pre-visit: {pv}", LogLevel.Trace);
                }

                if (InArg2("inlaw") || InArg2("inlaws"))
                {

                    string result = "\n";

                    foreach (var pair in InLaws)
                    {
                        string pairvalue = "";
                        foreach (string name in pair.Value)
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
                        ModEntry.Log("No in-laws found. (Searched all NPCs with friendship)", LogLevel.Warn);
                    }
                    else
                    {
                        ModEntry.Log(result, LogLevel.Info);
                    }

                }

                if (InArg2("animal") || InArg2("animals"))
                {
                    string print = "\n";
                    foreach(var name in ModEntry.Animals)
                    {
                        print += $"{name} \n";
                    }
                }

                if (InArg2("crop") || InArg2("crops"))
                {
                    string print = "\n";
                    foreach (var type in ModEntry.Crops)
                    {
                        print += $"{type.Value} \n";
                    }
                }

                if (InArg2("visits") || InArg2("v"))
                {
                    string print = "\n";
                    foreach (var name in ModEntry.TodaysVisitors)
                    {
                        print += $"{name} \n";
                    }
                }

                if (InArg2("blacklist") || InArg2("bl"))
                {
                    string print = "\n";
                    foreach (var name in ModEntry.BlacklistParsed)
                    {
                        print += $"{name} \n";
                    }
                }

                if (InArg2("furniture") || InArg2("f"))
                {
                    string print = "\n";
                    foreach (var name in ModEntry.FurnitureList)
                    {
                        print += $"{name} \n";
                    }
                }
            }
        }
    }
}
