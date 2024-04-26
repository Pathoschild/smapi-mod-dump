/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jibblestein/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Characters;

namespace IntegratedMinecarts.Patches
{
    internal class checkActionPatcher
    {
        private static IMonitor? Monitor;
        // call this method from your Entry class
        public static void Patch(ModEntry mod)
        {
            Monitor = mod.Monitor;

            try
            {
                mod.Harmony!.Patch(
                   original: AccessTools.Method(typeof(StardewValley.Locations.Town), nameof(StardewValley.Locations.Town.checkAction)),
                   transpiler: new HarmonyMethod(typeof(checkActionPatcher), nameof(Town_checkAction_Transpiler))
                    );
                mod.Harmony!.Patch(
                original: AccessTools.Method(typeof(StardewValley.Locations.Mountain), nameof(StardewValley.Locations.Mountain.checkAction)),
                transpiler: new HarmonyMethod(typeof(checkActionPatcher), nameof(Mountain_checkAction_Transpiler))
    );
            }
            catch (Exception ex)
            {
                Monitor.Log($"An error occurred while registering a harmony patch for the checkAction.\n{ex}", LogLevel.Warn);
            }
        }
        private static IEnumerable<CodeInstruction> Town_checkAction_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var foundMinecartMenuMethod1 = false;
            var startIndex1 = -1;
            var endIndex1 = -1;
            var foundMinecartMenuMethod2 = false;
            var startIndex2 = -1;
            var endIndex2 = -1;

            var codes = new List<CodeInstruction>(instructions);
            //find first ShowMineCartMenu
            for (int i = 0; i < codes.Count; i++)
            {
                if (!foundMinecartMenuMethod1 && codes[i].opcode == OpCodes.Ldstr && codes[i].operand as string == "Default")
                {
                    Monitor.Log($"Found Default1 at Code {i}", LogLevel.Trace);
                    startIndex1 = i - 12;
                    endIndex1 = i + 4;
                    startIndex2 = endIndex1 + 1;
                    foundMinecartMenuMethod1 = true;
                    break;
                }
            }
            //Find second ShowMineCartMenu
            for (int i = startIndex2; i < codes.Count; i++)
            {
                if (!foundMinecartMenuMethod2 && codes[i].opcode == OpCodes.Ldstr && codes[i].operand as string == "Default")
                {
                    Monitor.Log($"Found Default2 at Code {i}", LogLevel.Trace);
                    startIndex2 = i - 12;
                    endIndex2 = i + 4;
                    foundMinecartMenuMethod2 = true;
                    break;
                }
            }
            //If we have found both code ranges
            if (startIndex1 > -1 && endIndex1 > -1 && startIndex2 > -1 && endIndex2 > -1)
            {
                //Remove first ShowMineCartMenus
                for (int i = startIndex1; i <= endIndex1; i++)
                {
                    codes[i].opcode = OpCodes.Nop;
                    codes[i].operand = null;
                }
                //Remove second ShowMineCartMenus
                for (int i = startIndex2; i <= endIndex2; i++)
                {
                    codes[i].opcode = OpCodes.Nop;
                    codes[i].operand = null;
                }
                Monitor.Log($"Successfully Noped codes for Town", LogLevel.Trace);
            }
            return codes.AsEnumerable();
        }
        private static IEnumerable<CodeInstruction> Mountain_checkAction_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var foundMinecartMenuMethod = false;
            var startIndex = -1;
            var endIndex = -1;
            var codes = new List<CodeInstruction>(instructions);

            //find ShowMineCartMenu
            for (int i = 0; i < codes.Count; i++)
            {
                if (!foundMinecartMenuMethod && codes[i].opcode == OpCodes.Ldstr && codes[i].operand as string == "Default")
                {
                    Monitor.Log($"Found Default at Code {i}", LogLevel.Trace);
                    startIndex = i - 1; // exclude current 'ret'
                    endIndex = i + 4;
                    foundMinecartMenuMethod = true;
                    break;
                }
            }
            //If we have found the code range
            if (startIndex > -1 && endIndex > -1)
            {
                //Remove first ShowMineCartMenus
                for (int i = startIndex; i <= endIndex; i++)
                {
                    codes[i].opcode = OpCodes.Nop;
                    codes[i].operand = null;
                }
                Monitor.Log($"Successfully Noped codes for Mountain", LogLevel.Trace);
            }
            return codes.AsEnumerable();
        }
    }
}
