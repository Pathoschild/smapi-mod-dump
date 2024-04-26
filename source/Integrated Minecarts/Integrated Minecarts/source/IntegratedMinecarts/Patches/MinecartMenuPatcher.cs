/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jibblestein/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley.GameData.Minecarts;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection;
using IntegratedMinecarts;
using System.Diagnostics;
using HarmonyLib;


namespace IntegratedMinecarts.Patches
{
    internal class MinecartMenuPatcher
    {
        private static IMonitor? Monitor;
        // call this method from your Entry class
        public static void Patch(ModEntry mod)
        {
            Monitor = mod.Monitor;

            try
            {
                mod.Harmony!.Patch(
                   original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.ShowPagedResponses)),
                   prefix: new HarmonyMethod(typeof(MinecartMenuPatcher), nameof(ShowPagedResponses_Prefix))
                    );

            }
            catch (Exception ex)
            {
                Monitor.Log($"An error occurred while registering a harmony patch for the GameLocation.\n{ex}", LogLevel.Warn);
            }
        }

        internal static bool ShowPagedResponses_Prefix(GameLocation __instance, string prompt, List<KeyValuePair<string, string>> responses, Action<string> on_response, bool auto_select_single_choice, bool addCancel, ref int itemsPerPage, MethodBase __originalMethod )
        {
            ModEntry mod = ModEntry.Instance;
            StackTrace stackTrace = new StackTrace();
            MethodBase callingmethod = stackTrace.GetFrame(2).GetMethod();
            Monitor?.Log($"Method Calling ShowPagedResponse is: {callingmethod.Name}", LogLevel.Trace);
            try
            {
                if (callingmethod.Name == "ShowMineCartMenu")
                {
                    itemsPerPage = mod.Config.DestinationsPerPage;
                    Monitor?.Log($"Changed Items Per Page to {itemsPerPage}", LogLevel.Trace);
                }
                return true;
            }
            catch (Exception ex)
            {
                Monitor?.Log($"An error occurred when applying patch to ShowPagedResponses. Running original", LogLevel.Warn);
                Monitor?.Log($"Details:\n{ex}", LogLevel.Warn);
                return true;
            }
        }
    }
}
