/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace SpousesIsland.ModContent
{
    public static class Debugging
    {
        internal static void Chance(string arg1, string[] arg2)
        {
            var isDebug = arg2?.Contains("debug") ?? false;

            ModEntry.Mon.Log($"{ModEntry.RandomizedInt}", LogLevel.Info);

            if (!isDebug) return;
            
            if (!Context.IsWorldReady)
            {
                ModEntry.Mon.Log(ModEntry.Help.Translation.Get("CLI.nosaveloaded"), LogLevel.Error);
                return;
            }

            ModEntry.Mon.Log(ModEntry.Help.Translation.Get("CLI.Day0") + $": {ModEntry.PreviousDayRandom}", LogLevel.Info);
        }

        internal static void GeneralInfo(string arg1, string[] arg2)
        {
            ModEntry.Mon.Log($"\nIslandToday {ModEntry.IslandToday}\nIsFromTicket {ModEntry.IsFromTicket}\nChance {ModEntry.RandomizedInt}, PrevRandom {ModEntry.PreviousDayRandom}\nBoatFixed {ModEntry.BoatFixed}\nChildren {ModEntry.Children.Count}", LogLevel.Info);
        }

        internal static void GetStatus(string arg1, string[] arg2)
        {
            string spouses = null;

            foreach (var spouse in ModEntry.Status.Who)
            {
                spouses += spouse + "  ";
            }

            var stats = $" DayVisit = {ModEntry.Status.DayVisit}, WeekVisit = {ModEntry.Status.WeekVisit.Item1} {ModEntry.Status.WeekVisit.Item2}, Who = {spouses}\n\n";
            ModEntry.Mon.Log(stats, LogLevel.Info);

        }

        internal static void Print(string arg1, string[] arg2)
        {
            if(!arg2.Any())
            {
                ModEntry.Mon.Log("Please choose at least one NPC.",LogLevel.Warn);
            }
            else
            {
                var all= "\n";
                foreach(var name in arg2)
                {
                    NPC chara = Game1.getCharacterFromName(name);
                    if(chara == null)
                    {
                        all += $"{name} not found.\n";
                        continue;
                    }

                    var schedule = "\n";
                    foreach(var point in chara.Schedule)
                    {
                        var newline = $"{point.Key}: \n";
                        foreach(var coord in point.Value.route)
                        {
                            var linecoords = $"({coord.X}, {coord.Y}),";
                            newline += linecoords;
                        }
                        schedule += "\n";
                        schedule += newline;
                    }
                    all += $"{chara.Name}: {schedule}\n";
                }
                ModEntry.Mon.Log(all,LogLevel.Info);
            }
        }
    }
}