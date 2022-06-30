/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-online-splitscreen
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using System.Reflection;
using Microsoft.Xna.Framework.Input;

namespace Online_Splitscreen
{
    /// <summary>The mod entry point.</summary>

    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            ObjectPatches.Initialize(this.Monitor);

            harmony.Patch(
               original: AccessTools.Method(typeof(Game1), nameof(Game1.IsLocalCoopJoinable)),
               prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.IsLocalCoopJoinable_Prefix))
            );

        }

    }

    public class ObjectPatches
    {
        private static IMonitor Monitor;
        // call this method from your Entry class
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static bool IsLocalCoopJoinable_Prefix(InputState __instance, ref bool __result)
        {
            try
            {
                __result = true;
                if (GameRunner.instance.gameInstances.Count >= GameRunner.instance.GetMaxSimultaneousPlayers())
                {
                    __result = false;
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(IsLocalCoopJoinable_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
