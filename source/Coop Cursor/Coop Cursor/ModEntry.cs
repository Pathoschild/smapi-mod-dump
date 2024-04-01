/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/coop-cursor
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
        public static IModHelper _Helper;
        public static Mod _Mod;
        public static IMonitor _Monitor;

        private static ModConfig Config;
        public static MouseHook Hook = new MouseHook();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            _Helper = helper;
            _Mod = this;
            _Monitor = Monitor;

            Config = Helper.ReadConfig<ModConfig>();
            if (!Config.enabled)
                return;

            var harmony = new Harmony(ModManifest.UniqueID);

            //Ignore mouse inputs when playing on a gamepad
            if (!Config.keyboardPlayer) {
                harmony.Patch(
                   original: AccessTools.Method(typeof(InputState), nameof(InputState.SetMousePosition)),
                   prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.SetMousePosition_Prefix))
                );

                harmony.Patch(
                   original: AccessTools.Method(typeof(InputState), nameof(InputState.GetMouseState)),
                   prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.GetMouseState_Prefix))
                );
            }

            if (Config.keyboardPlayer)
                Hook.Initialize();

            harmony.Patch(
               original: AccessTools.Method(typeof(InputState), nameof(InputState.UpdateStates)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.UpdateStates_Prefix))
            );



        }

        private static FieldInfo _simulatedMousePosition = typeof(InputState).GetField("_simulatedMousePosition", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo _currentGamepadState = typeof(InputState).GetField("_currentGamepadState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo _currentMouseState = typeof(InputState).GetField("_currentMouseState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo _currentKeyboardState = typeof(InputState).GetField("_currentKeyboardState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool SetMousePosition_Prefix(InputState __instance, int x, int y)
        {
            try {
                _simulatedMousePosition.SetValue(__instance, new Point(x, y));

                return false; // don't run original logic
            } catch (Exception ex) {
                _Monitor.Log($"Failed in {nameof(SetMousePosition_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool GetMouseState_Prefix(InputState __instance, ref MouseState __result)
        {
            try {
                Point position = (Point)_simulatedMousePosition.GetValue(__instance);
                __result = new MouseState(position.X, position.Y, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);

                return false; // don't run original logic
            } catch (Exception ex) {
                _Monitor.Log($"Failed in {nameof(GetMouseState_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool UpdateStates_Prefix(InputState __instance)
        {
            if (Config.keyboardPlayer) {
                _currentGamepadState.SetValue(__instance, default(GamePadState));
                _currentKeyboardState.SetValue(__instance, Keyboard.GetState());
                _currentMouseState.SetValue(__instance, Hook.getState());
            } else {
                _currentGamepadState.SetValue(__instance, GamePad.GetState(Game1.playerOneIndex));
            }

            return false;
        }
    }
}