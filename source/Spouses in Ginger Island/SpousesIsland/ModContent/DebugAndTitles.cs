/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Linq;

namespace SpousesIsland
{
    public class Debugging
    {
        internal static void Chance(string arg1, string[] arg2)
        {
            var IsDebug = arg2?.Contains<string>("debug") ?? false;

            ModEntry.Mon.Log($"{ModEntry.RandomizedInt}", LogLevel.Info);

            if (IsDebug)
            {
                if (!Context.IsWorldReady)
                {
                    ModEntry.Mon.Log(ModEntry.Help.Translation.Get("CLI.nosaveloaded"), LogLevel.Error);
                    return;
                }

                ModEntry.Mon.Log(ModEntry.Help.Translation.Get("CLI.Day0") + $": {ModEntry.PreviousDayRandom}", LogLevel.Info);
            }
        }

        internal static void GeneralInfo(string arg1, string[] arg2)
        {
            ModEntry.Mon.Log($"\nIslandToday {ModEntry.IslandToday}\nIsFromTicket {ModEntry.IsFromTicket}\nChance {ModEntry.RandomizedInt}, PrevRandom {ModEntry.PreviousDayRandom}\nBoatFixed {ModEntry.BoatFixed}\nChildren {ModEntry.Children.Count}", LogLevel.Info);
        }

        internal static void GetStatus(string arg1, string[] arg2)
        {
            string stats = null;

            foreach(var pair in ModEntry.Status)
            {
                string spouses = null;
                var data = pair.Value;

                foreach (var spouse in data.Who)
                {
                    spouses += spouse + "   ";
                }

                stats += data.Name + "\n      " + $" DayVisit = {data.DayVisit}, WeekVisit = {data.WeekVisit.Item1} {data.WeekVisit.Item2}, Who = {spouses}\n\n";
            }

            ModEntry.Mon.Log(stats, LogLevel.Info);

        }
    }

    public class Titles
    {
        internal static string SpouseT()
        {
            var SpousesGrlTitle = "SDV";
            return SpousesGrlTitle;
        }
        internal static string SVET()
        {
            var sve = "SVE";
            return sve;
        }
        internal static string Debug()
        {
            var db = "Debug";
            return db;
        }
    }
}