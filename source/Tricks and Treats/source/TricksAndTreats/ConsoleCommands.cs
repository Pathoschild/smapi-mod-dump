/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cl4r3/Halloween-Mod-Jam-2023
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using static TricksAndTreats.Globals;

namespace TricksAndTreats
{
    internal static class ConsoleCommands
    {
        static ICommandHelper CommandHelper;

        internal static void Register(IMod ModInstance)
        {
            CommandHelper = Helper.ConsoleCommands;

            CommandHelper.Add(
                name: ModPrefix + "show_score",
                documentation: Helper.Translation.Get("commands.show_score"),
                callback: ConsoleShowScore);
            CommandHelper.Add(
                name: ModPrefix + "check_score",
                documentation: Helper.Translation.Get("commands.show_score"),
                callback: ConsoleShowScore);
            CommandHelper.Add(
                name: ModPrefix + "get_score",
                documentation: Helper.Translation.Get("commands.show_score"),
                callback: ConsoleShowScore);
            CommandHelper.Add(
                name: ModPrefix + "set_score",
                documentation: Helper.Translation.Get("commands.set_score"),
                callback: ConsoleSetScore);
            CommandHelper.Add(
                name: ModPrefix + "check_costume",
                documentation: Helper.Translation.Get("commands.check_costume"),
                callback: static (string command, string[] args) => Costumes.CheckForCostume(true));
        }

        private static void ConsoleShowScore(string command, string[] args)
        {
            if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 27)
            {
                int min_score = 0;
                switch(Config.ScoreCalcMethod)
                {
                    case "none":
                        Log.Info($"({Helper.Translation.Get("commands.no_min")})");
                        break;
                    case "minval":
                        min_score = Config.CustomMinVal;
                        break;
                    case "minmult":
                        min_score = (int)Math.Round(NPCData.Keys.Count * Config.CustomMinMult);
                        break;
                }
                if (min_score < 1)
                    Log.Info($"({Helper.Translation.Get("commands.no_min")})");
                else
                    Log.Info($"({Helper.Translation.Get("commands.min_score")}: {min_score})");
                Log.Info($"{Helper.Translation.Get("commands.current_score")}: {Game1.player.modData[ScoreKey]}");
            }
            else
                Log.Info($"{Helper.Translation.Get("commands.not_halloween")}");
        }

        private static void ConsoleSetScore(string command, string[] args)
        {
            if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 27)
            {
                if (int.TryParse(args[1], out int n))
                {
                    Game1.player.modData[ScoreKey] = args[1];
                    Log.Info($"{Helper.Translation.Get("commands.current_score")}: {args[1]}");
                }
                else
                    Log.Info($"{Helper.Translation.Get("commands.not_int")}");
            }
            else
                Log.Info($"{Helper.Translation.Get("commands.not_halloween")}");
        }
    }
}
