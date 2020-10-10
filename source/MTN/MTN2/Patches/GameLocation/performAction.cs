/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using Harmony;
using MTN2.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.GameLocationPatches {
    /// <summary>
    /// REASON FOR PATCHING: To adjust the entry warp for custom green house
    /// maps.
    /// 
    /// Patches the method GameLocation.performAction to adjust for the Action call
    /// to warp into the greenhouse when a custom greenhouse map is used.
    /// </summary>
    public class performActionPatch {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="CustomManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public performActionPatch(ICustomManager customManager) {
            performActionPatch.customManager = customManager;
        }

        /// <summary>
        /// Transpiles the CLI to inject calls to <see cref="CustomManager.GreenHouseEntryX"/> and <see cref="CustomManager.GreenHouseEntryY"/>
        /// instead of using hardcoded numerical values for the warps. Enables custom greenhouse maps to be much more flexible while still
        /// being immersive.
        /// </summary>
        /// <param name="instructions">Code Instructions (in CLI)</param>
        /// <param name="iL">Intermediate Assembly Language Generator</param>
        /// <returns></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator iL) {
            var codes = new List<CodeInstruction>(instructions);
            for (int x = 0; x < codes.Count; x++) {
                if (codes[x].opcode == OpCodes.Ldstr && (string) codes[x].operand == "Greenhouse") {
                    codes.Insert(x + 1, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(performActionPatch), "customManager")));
                    codes[x + 2] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(CustomManager), "GreenHouseEntryX").GetGetMethod());
                    codes.Insert(x + 3, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(performActionPatch), "customManager")));
                    codes[x + 4] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(CustomManager), "GreenHouseEntryY").GetGetMethod());
                }

                if (performActionPatch.CheckForCall(codes[x], "get_player") && x + 1 < codes.Count && performActionPatch.CheckForVirtcall(codes[x + 1], "getMailboxPosition")) {
                    codes[x] = new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(performActionPatch), "customManager"));
                    codes[x + 1] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(CustomManager), "MailboxPosition").GetGetMethod());
                }
            }
            return codes.AsEnumerable();
        }

        private static bool CheckForCall(CodeInstruction code, string name) {
            if (code.opcode == OpCodes.Call) {
                MethodInfo methodInfo = code.operand as MethodInfo;
                if (((methodInfo != null) ? methodInfo.Name : null) == name) return true;
            }
            return false;
        }

        private static bool CheckForVirtcall(CodeInstruction code, string name) {
            if (code.opcode == OpCodes.Callvirt) {
                MethodInfo methodInfo = code.operand as MethodInfo;
                return (methodInfo?.Name) == name;
            }
            return false;
        }
    }
}
