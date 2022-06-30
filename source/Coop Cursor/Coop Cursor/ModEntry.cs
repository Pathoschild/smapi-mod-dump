/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-coop-cursor
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

namespace Coop_Cursor
{
    /// <summary>The mod entry point.</summary>

    public class ModEntry : Mod
    {
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            if( !this.Config.enabled ) return;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            ObjectPatches.Initialize(this.Monitor);

            harmony.Patch(
               original: AccessTools.Method(typeof(InputState), nameof(InputState.SetMousePosition)),
               prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.SetMousePosition_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(InputState), nameof(InputState.GetMouseState)),
               prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.GetMouseState_Prefix))
            );

        }

    }

    public class ObjectPatches
    {
        private static IMonitor Monitor;
        private static FieldInfo _simulatedMousePosition = typeof(InputState).GetField("_simulatedMousePosition", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static bool SetMousePosition_Prefix(InputState __instance, int x, int y)
        {
            try
            {
                _simulatedMousePosition.SetValue(__instance, new Point(x, y));

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(SetMousePosition_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool GetMouseState_Prefix(InputState __instance, ref MouseState __result)
        {
            try
            {
                Point position = (Point)_simulatedMousePosition.GetValue(__instance);
                __result = new MouseState(position.X, position.Y, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(GetMouseState_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}