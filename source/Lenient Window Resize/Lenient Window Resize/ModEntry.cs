/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-lenient-window-resize
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
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using StardewValley.Menus;
using System.Linq;
using System.Reflection.Emit;
using Lenient_Window_Resize.API;

namespace Lenient_Window_Resize
{
    public class ModEntry : Mod
    {
        public static Mod Mod;
        public static ModConfig Config;
        public static IMonitor _Monitor;
        public static IModHelper _Helper;

        public override void Entry(IModHelper helper)
        {
            Mod = this;
            _Monitor = Monitor;
            _Helper = Helper;
            Config = helper.ReadConfig<ModConfig>();

            GenericModConfigMenuHandler.Initialize(this);

            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(Game1), nameof(Game1.SetWindowSize)),
               transpiler: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.SetWindowSize_Transpiler))
            );
        }

        public static IEnumerable<CodeInstruction> SetWindowSize_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions) {
                if (instruction.LoadsConstant(1280))
                    instruction.operand = Config.MinW;

                if (instruction.LoadsConstant(720))
                    instruction.operand = Config.MinH;

                yield return instruction;
            }
        }
    }
}
