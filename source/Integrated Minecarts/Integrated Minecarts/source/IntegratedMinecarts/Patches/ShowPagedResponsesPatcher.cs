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
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
//Currently Unused
namespace IntegratedMinecarts.Patches
{
    internal class ShowPagedResponsesPatcher
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
                   transpiler: new HarmonyMethod(typeof(ShowPagedResponsesPatcher), nameof(ShowPagedResponses_Transpiler))
                    );
            }
            catch (Exception ex)
            {
                Monitor.Log($"An error occurred while registering a harmony patch for ShowPagedResponses.\n{ex}", LogLevel.Warn);
            }
        }
        private static IEnumerable<CodeInstruction> ShowPagedResponses_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var method = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.ShowPagedResponses));

            var patchedmethod = AccessTools.Method(typeof(ShowPagedResponsesPatcher), nameof(CreatePagedResponses));
            Monitor.Log($"Starting ShowPagedResponses_Transpiler ", LogLevel.Warn);

            foreach ( var instr in instructions )
            {
                Monitor.Log($"transpiler start line ", LogLevel.Warn);
                if (instr.opcode == OpCodes.Callvirt && instr.operand is MethodInfo minfo && (minfo == method))
                    yield return new CodeInstruction(instr)
                    {
                        opcode = OpCodes.Call,
                        operand = patchedmethod

                    };
                else

                    yield return instr;
            }
 
        }
        public void CreatePagedResponses(int page=-1)
        {
        }
    }
}
