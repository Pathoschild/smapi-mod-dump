/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/PrimmR/Turbo
**
*************************************************/

using System.Reflection;
using Force.DeepCloner;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace Turbo
{
    /// <summary>Handles patching methods relating to Stardew's update routine</summary>
    internal class UpdatePatcher
    {
        private static IMonitor Mntr;

        internal static void Initialise(IMonitor monitor)
        {
            Mntr = monitor;
        }

        /// <summary>Finaliser patch for Stardew's Game1.Instance_Update method</summary>
        internal static bool UpdateGame(Game1 __instance, MethodInfo __originalMethod)
        {
            // Increment global ticks
            try
            {
                ModEntry.elapsedTicks += checked((long)(ModEntry.SPF * ModEntry.speed));
            }
            catch (OverflowException)
            {
                ModEntry.elapsedTicks += (long)(ModEntry.SPF * ModEntry.speed);
                ModEntry.nextFrame = 0;
            }

            if (!Context.IsWorldReady)
            {
                return true;
            }

            try
            {
                // If update not going to be called, call update input
                if (!(ModEntry.elapsedTicks / ModEntry.SPF >= ModEntry.nextFrame))
                {
                    GameTime time = new GameTime(new TimeSpan(ModEntry.elapsedTicks), new TimeSpan(ModEntry.SPF));
                    GameTime[] parameters = new[] { time };

                    AccessTools.Method(typeof(Game1), "UpdateControlInput").Invoke(__instance, parameters);
                    Mntr.LogOnce("Called UpdateControlInput", LogLevel.Trace);
                }

                // Call update command necessary number of times
                while (ModEntry.elapsedTicks / ModEntry.SPF >= ModEntry.nextFrame)
                {
                    ModEntry.nextFrame++;

                    GameTime time = new GameTime(new TimeSpan(ModEntry.elapsedTicks), new TimeSpan(ModEntry.SPF));
                    GameTime[] parameters = new[] { time };

                    AccessTools.Method(typeof(Game1), "Update").Invoke(__instance, parameters);
                    Mntr.LogOnce("Called Update", LogLevel.Trace);
                }
                return false;
            }
            catch (Exception ex)
            {
                Mntr.Log($"Failed in {nameof(UpdateGame)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        /// <summary>Prefix patch for Stardew's Game1.UpdateGameClock method</summary>
        internal static bool UpdateClock(ref GameTime time)
        {
            try
            {
                if (ModEntry.Config.clockMode == 0)
                    return true;

                GameTime timeC = time.ShallowClone();
                TimeSpan fixedTime = new TimeSpan();

                if (ModEntry.Config.clockMode == 1)
                {
                    ModEntry.clockTicks += (timeC.ElapsedGameTime / ModEntry.speed).Ticks; // Could replace with SPF to trade performance for compatibility 
                    fixedTime = new TimeSpan(ModEntry.clockTicks);
                    ModEntry.clockTicks %= TimeSpan.TicksPerMillisecond;
                }
                timeC.ElapsedGameTime = fixedTime;
                time = timeC;
            }
            catch (Exception ex)
            {
                Mntr.Log($"Failed in {nameof(UpdateClock)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }
    }
}
