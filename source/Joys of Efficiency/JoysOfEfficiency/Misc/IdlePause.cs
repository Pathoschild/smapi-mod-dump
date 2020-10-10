/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using JoysOfEfficiency.Core;
using JoysOfEfficiency.Huds;
using JoysOfEfficiency.Utils;
using StardewValley;

namespace JoysOfEfficiency.Misc
{
    internal class IdlePause
    {
        private static Config Conf => InstanceHolder.Config;
        private static double TimeoutCounter { get; set; }
        private static bool Paused { get; set; }
        private static int LastTimeOfDay { get; set; }

        private static readonly Logger Logger = new Logger("IdlePause");

        public static void OnTickUpdate()
        {
            if (Conf.PauseWhenIdle)
            {
                if (Util.IsPlayerIdle())
                {
                    TimeoutCounter += Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
                    if (TimeoutCounter > Conf.IdleTimeout * 1000)
                    {
                        if (!Paused)
                        {
                            Logger.Log("Paused game");
                            Paused = true;
                        }

                        Game1.timeOfDay = LastTimeOfDay;
                    }
                }
                else
                {
                    if (Paused)
                    {
                        Paused = false;
                        Logger.Log("Resumed game");
                    }

                    TimeoutCounter = 0;
                    LastTimeOfDay = Game1.timeOfDay;
                }
            }
            else
            {
                Paused = false;
            }
        }

        public static void OnDataLoaded()
        {
            LastTimeOfDay = Game1.timeOfDay;
        }

        public static void DrawHud()
        {
            if (Paused)
            {
                PausedHud.DrawPausedHud();
            }
        }
    }
}
